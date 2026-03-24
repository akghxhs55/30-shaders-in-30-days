using Godot;

[GlobalClass, Tool]
public partial class ColorPair : Resource
{
    [Export] 
    public Color Source {
        get => _source;
        set { _source = value; EmitChanged(); }
    }
    private Color _source = Colors.White;

    [Export]
    public Color Target
    {
        get => _target;
        set  { _target = value; EmitChanged(); }
    } 
    private Color _target = Colors.White;
}
