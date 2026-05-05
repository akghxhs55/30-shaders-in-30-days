use regex::Regex;
use std::{env, fs};
use std::sync::LazyLock;

fn main() {
    let args: Vec<String> = env::args().collect();

    if args.len() < 2 {
        eprintln!("Usage: uniform_wrapper_generator <path-to-gdshader>");
        std::process::exit(1);
    }

    let source = fs::read_to_string(&args[1]).unwrap_or_else(|e| {
        eprintln!("Failed to read file: {e}");
        std::process::exit(1);
    });

    let cleaned = Regex::new(r"//.*").unwrap().replace_all(&source, "");

    cleaned
        .split([';', '}'])
        .map(str::trim)
        .filter(|s| s.starts_with("uniform"))
        .for_each(|statement| {
            match parse_uniform_statement(statement).and_then(|u| generate_property(&u)) {
                Ok(code) => println!("{code}\n"),
                Err(e) => eprintln!("Error: {e}"),
            }
        });
}

fn parse_uniform_statement(statement: &str) -> Result<UniformInfo, String> {
    let body = statement
        .strip_prefix("uniform")
        .ok_or_else(|| format!("Failed to parse uniform: {statement}"))?
        .trim();

    static UNIFORM_PATTERN: LazyLock<Regex> = LazyLock::new(|| {
        Regex::new(
            r"(?s)^(?<type>\S+)\s+(?<name>\S+?)(?:\s*:\s*(?<hint>.+?))?(?:\s*=\s*(?<init>.+))?$",
        )
        .expect("Invalid regex pattern")
    });

    UNIFORM_PATTERN
        .captures(body)
        .ok_or_else(|| format!("Failed to parse uniform: {statement}"))
        .map(|captures| UniformInfo {
            uniform_type: captures["type"].to_string(),
            name: captures["name"].to_string(),
            hint: captures.name("hint").map(|h| h.as_str().trim().to_string()),
            initializer: captures.name("init").map(|i| i.as_str().trim().to_string()),
        })
}

fn generate_property(uniform: &UniformInfo) -> Result<String, String> {
    let property_name = to_pascal_case(&uniform.name);

    match (uniform.uniform_type.as_str(), uniform.hint.as_deref()) {
        ("int", Some(hint)) if hint.starts_with("hint_enum") => {
            generate_enum_property(uniform, &property_name)
        }

        ("int" | "float", Some(hint)) if hint.starts_with("hint_range") => {
            generate_range_property(uniform, &property_name)
        }

        ("vec3" | "vec4", Some(hint)) if hint.starts_with("source_color") => {
            Ok(generate_default_property(uniform, &property_name, "Color"))
        }

        _ => {
            let type_name = match uniform.uniform_type.as_str() {
                "int" => "int",
                "float" => "float",
                "vec2" => "Vector2",
                "vec3" => "Vector3",
                "vec4" => "Vector4",
                "sampler2D" => "Texture2D",
                other => other,
            };

            Ok(generate_default_property(
                uniform,
                &property_name,
                type_name,
            ))
        }
    }
}

fn generate_enum_property(uniform: &UniformInfo, property_name: &str) -> Result<String, String> {
    let hint = uniform
        .hint
        .as_deref()
        .ok_or_else(|| "Failed to get hint".to_string())?;

    let enum_type_name = format!("{property_name}Mode");
    let members = extract_enum_values(hint)?;
    let identifiers = members
        .iter()
        .map(|m| to_valid_enum_identifier(m))
        .collect::<Vec<_>>();

    let default_index = uniform
        .initializer
        .as_deref()
        .map(|s| s.parse::<usize>())
        .transpose()
        .map_err(|e| format!("Invalid initializer: {e}"))?
        .unwrap_or(0);
    let default_identifier = identifiers
        .get(default_index)
        .or_else(|| identifiers.first())
        .ok_or_else(|| "Enum members are empty".to_string())?;

    Ok(format!(
r#"public enum {enum_type_name} {{ {identifiers} }}

[Export]
public {enum_type_name} {property_name}
{{
    get => _{camel_name};
    set
    {{
        _{camel_name} = value;
        UpdateShaderParam("{name}", (int)value);
        NotifyPropertyListChanged();
    }}
}}
private {enum_type_name} _{camel_name} = {enum_type_name}.{default_identifier};"#,
        identifiers = identifiers.join(", "),
        camel_name = to_camel_case(property_name),
        name = uniform.name,
    ))
}

fn generate_range_property(uniform: &UniformInfo, property_name: &str) -> Result<String, String> {
    let type_name = if uniform.uniform_type.as_str() == "int" {
        "int"
    } else {
        "float"
    };

    let hint = uniform
        .hint
        .as_deref()
        .ok_or_else(|| "Failed to get hint".to_string())?;

    let start = hint
        .find('(')
        .ok_or_else(|| format!("Failed to find parenthesis: {hint}"))?;
    let end = hint
        .rfind(')')
        .ok_or_else(|| format!("Failed to find parenthesis: {hint}"))?;
    let range_args = hint[start + 1..end].trim();

    let default_value = match type_name {
        "int" => uniform.initializer.as_deref().unwrap_or("0").to_string(),
        "float" => match uniform.initializer.as_deref() {
            Some(v) if v.ends_with('f') => v.to_string(),
            Some(v) => format!("{v}f"),
            None => "0.0f".to_string(),
        },
        _ => "default".to_string(),
    };

    Ok(format!(
r#"[Export(PropertyHint.Range, "{range_args}")]
public {type_name} {property_name}
{{
    get => _{camel_name};
    set
    {{
        _{camel_name} = value;
        UpdateShaderParam("{name}", value);
    }}
}}
private {type_name} _{camel_name} = {default_value};"#,
        camel_name = to_camel_case(property_name),
        name = uniform.name,
    ))
}

fn generate_default_property(
    uniform: &UniformInfo,
    property_name: &str,
    type_name: &str,
) -> String {
    format!(
r#"[Export]
public {type_name} {property_name}
{{
    get => _{camel_name};
    set
    {{
        _{camel_name} = value;
        UpdateShaderParam("{name}", value);
    }}
}}
private {type_name} _{camel_name} = {initializer};"#,
        camel_name = to_camel_case(property_name),
        name = uniform.name,
        initializer = convert_initializer(uniform.initializer.as_deref(), type_name),
    )
}

fn extract_enum_values(hint: &str) -> Result<Vec<String>, String> {
    let start = hint
        .find('(')
        .ok_or_else(|| format!("Failed to find parenthesis: {hint}"))?;
    let end = hint
        .rfind(')')
        .ok_or_else(|| format!("Failed to find parenthesis: {hint}"))?;

    let inner = &hint[start + 1..end];

    static ENUM_SPLIT_PATTERN: LazyLock<Regex> =
        LazyLock::new(|| Regex::new(r#""([^"]+)""#).expect("Invalid regex pattern"));

    Ok(ENUM_SPLIT_PATTERN
        .captures_iter(inner)
        .filter_map(|captures| captures.get(1))
        .map(|m| m.as_str().to_string())
        .collect())
}

fn to_valid_enum_identifier(display_name: &str) -> String {
    display_name
        .split([' ', '-'])
        .map(|word| {
            let mut chars = word.chars();
            match chars.next() {
                Some(first) => first.to_uppercase().collect::<String>() + chars.as_str(),
                None => String::new(),
            }
        })
        .collect()
}

fn convert_initializer(initializer: Option<&str>, type_name: &str) -> String {
    let Some(initializer) = initializer else {
        return "default".to_string();
    };

    match type_name {
        "float" => {
            if initializer.ends_with('f') {
                initializer.to_string()
            } else {
                format!("{initializer}f")
            }
        }

        "Vector2" | "Vector3" | "Vector4" | "Color" => {
            convert_vector_initializer(initializer, type_name)
        }

        _ => initializer.to_string(),
    }
}

fn convert_vector_initializer(initializer: &str, type_name: &str) -> String {
    let Some(start) = initializer.find('(') else {
        return initializer.to_string();
    };
    let Some(end) = initializer.rfind(')') else {
        return initializer.to_string();
    };

    let inner = &initializer[start + 1..end];
    let args = inner
        .split(',')
        .map(|word| {
            let word = word.trim();
            if word.ends_with('f') {
                word.to_string()
            } else {
                format!("{word}f")
            }
        })
        .collect::<Vec<_>>();

    format!("new {type_name}({})", args.join(", "))
}

fn to_pascal_case(value: &str) -> String {
    value
        .split('_')
        .map(|part| {
            let mut chars = part.chars();
            match chars.next() {
                Some(first) => first.to_uppercase().collect::<String>() + chars.as_str(),
                None => String::new(),
            }
        })
        .collect()
}

fn to_camel_case(value: &str) -> String {
    let pascal_case = to_pascal_case(value);
    let mut chars = pascal_case.chars();
    match chars.next() {
        Some(first) => first.to_lowercase().collect::<String>() + chars.as_str(),
        None => pascal_case,
    }
}

#[derive(Debug)]
struct UniformInfo {
    uniform_type: String,
    name: String,
    hint: Option<String>,
    initializer: Option<String>,
}
