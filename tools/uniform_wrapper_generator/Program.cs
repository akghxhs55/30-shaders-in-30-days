using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShadersIn30Days.tools.uniform_wrapper_generator;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run -- <path-to-gdshader>");
            return;
        }
        
        string path = args[0];
        string source = File.ReadAllText(path);

        string cleaned = Regex.Replace(source, @"//.*", "");

        string[] statements = cleaned.Split(';', '}');
        var uniformStatements = statements
            .Select(s => s.Trim())
            .Where(s => s.StartsWith("uniform"));

        foreach (string statement in uniformStatements)
        {
            var uniformInfo = ParseUniformStatement(statement);
            string propertyCode = GenerateProperty(uniformInfo);
            Console.WriteLine(propertyCode + Environment.NewLine);
        }
    }

    private static readonly Regex UniformPattern = new(
        @"^(?<type>\S+)\s+(?<name>\S+?)(?:\s*:\s*(?<hint>.+?))?(?:\s*=\s*(?<init>.+))?$",
        RegexOptions.Singleline
    );

    private static UniformInfo ParseUniformStatement(string statement)
    {
        string body = statement["uniform".Length..].Trim();
    
        var match = UniformPattern.Match(body);
        if (!match.Success)
            throw new FormatException($"Failed to parse uniform: '{statement}'");
    
        return new UniformInfo(
            Type: match.Groups["type"].Value,
            Name: match.Groups["name"].Value,
            Hint: match.Groups["hint"].Success ? match.Groups["hint"].Value.Trim() : null,
            Initializer: match.Groups["init"].Success ? match.Groups["init"].Value.Trim() : null
        );
    }

    private static string GenerateProperty(UniformInfo uniform)
    {
        string propertyName = ToPascalCase(uniform.Name);
    
        return uniform switch
        {
            { Type: "int", Hint: { } h } when h.StartsWith("hint_enum")
                => GenerateEnumProperty(uniform, propertyName),
            
            { Type: "int" or "float", Hint: { } h } when h.StartsWith("hint_range")
                => GenerateRangeProperty(uniform, propertyName),
            
            { Type: "vec3" or "vec4", Hint: { } h } when h.StartsWith("source_color")
                => GenerateDefaultProperty(uniform, propertyName, "Color"),
            
            _ => GenerateDefaultProperty(uniform, propertyName, uniform.Type switch
            {
                "int" => "int",
                "float" => "float",
                "vec2" => "Vector2",
                "vec3" => "Vector3",
                "vec4" => "Vector4",
                "sampler2D" => "Texture2D",
                _ => uniform.Type
            })
        };
    }
    
    private static string GenerateEnumProperty(UniformInfo uniform, string propertyName)
    {
        string enumTypeName = propertyName + "Mode";
        string[] members = ExtractEnumValues(uniform.Hint!);
        string[] identifiers = members.Select(ToValidEnumIdentifier).ToArray();
    
        int defaultIdx = uniform.Initializer != null ? int.Parse(uniform.Initializer) : 0;
    
        return $$"""
                 public enum {{enumTypeName}} { {{string.Join(", ", identifiers)}} }

                 [Export]
                 public {{enumTypeName}} {{propertyName}}
                 {
                     get => _{{ToCamelCase(propertyName)}};
                     set
                     {
                         _{{ToCamelCase(propertyName)}} = value;
                         UpdateShaderParam("{{uniform.Name}}", (int)value);
                         NotifyPropertyListChanged();
                     }
                 }
                 private {{enumTypeName}} _{{ToCamelCase(propertyName)}} = {{enumTypeName}}.{{identifiers[defaultIdx]}};
                 """;
    }
    
    private static string GenerateRangeProperty(UniformInfo uniform, string propertyName)
    {
        string csharpType = uniform.Type == "int" ? "int" : "float";
    
        int start = uniform.Hint!.IndexOf('(') + 1;
        int end = uniform.Hint.LastIndexOf(')');
        string rangeArgs = uniform.Hint[start..end].Trim();
    
        string defaultValue = uniform.Initializer ?? (csharpType == "int" ? "0" : "0.0f");
        if (csharpType == "float" && !defaultValue.EndsWith('f'))
            defaultValue += "f";
    
        return $$"""
                 [Export(PropertyHint.Range, "{{rangeArgs}}")]
                 public {{csharpType}} {{propertyName}}
                 {
                     get => _{{ToCamelCase(propertyName)}};
                     set
                     {
                         _{{ToCamelCase(propertyName)}} = value;
                         UpdateShaderParam("{{uniform.Name}}", value);
                     }
                 }
                 private {{csharpType}} _{{ToCamelCase(propertyName)}} = {{defaultValue}};
                 """;
    }
    
    private static string GenerateDefaultProperty(UniformInfo uniform, string propertyName, string typeName)
    {
        return $$"""
                  [Export]
                  public {{typeName}} {{propertyName}}
                  {
                      get => _{{ToCamelCase(propertyName)}};
                      set
                      {
                          _{{ToCamelCase(propertyName)}} = value;
                          UpdateShaderParam("{{uniform.Name}}", value);
                      }
                  }
                  private {{typeName}} _{{ToCamelCase(propertyName)}} = {{ConvertInitializer(uniform.Initializer, typeName)}};
                  """;
    }
    
    private static string[] ExtractEnumValues(string hint)
    {
        int start = hint.IndexOf('(') + 1;
        int end = hint.LastIndexOf(')');
        string inner = hint[start..end];
    
        return Regex.Matches(inner, "\"([^\"]+)\"")
            .Select(m => m.Groups[1].Value)
            .ToArray();
    }

    private static string ToValidEnumIdentifier(string displayName)
    {
        return string.Concat(
            displayName.Split(' ', '-')
                .Select(word => char.ToUpper(word[0]) + word[1..])
        );
    }
    
    private static string ConvertInitializer(string? initializer, string csharpType)
    {
        if (initializer == null) return "default";
    
        return csharpType switch
        {
            "float" => initializer.EndsWith('f') ? initializer : initializer + "f",
            "Vector2" => ConvertVectorInitializer(initializer, "Vector2"),
            "Vector3" => ConvertVectorInitializer(initializer, "Vector3"),
            "Vector4" => ConvertVectorInitializer(initializer, "Vector4"),
            "Color" => ConvertVectorInitializer(initializer, "Color"),
            _ => initializer
        };
    }

    private static string ConvertVectorInitializer(string initializer, string typeName)
    {
        int start = initializer.IndexOf('(');
        string inner = initializer[(start + 1)..initializer.LastIndexOf(')')];
        var args = inner.Split(',')
            .Select(a => a.Trim())
            .Select(a => a.EndsWith('f') ? a : a + "f");
        return $"new {typeName}({string.Join(", ", args)})";
    }

    private static string ToPascalCase(string text)
    {
        return string.Concat(text.Split('_').Select(part => char.ToUpper(part[0]) + part[1..]));
    }
    
    private static string ToCamelCase(string text)
    {
        string pascal = ToPascalCase(text);
        return char.ToLower(pascal[0]) + pascal[1..];
    }

    private record UniformInfo(
        string Type,
        string Name,
        string? Hint,
        string? Initializer
    );
}
