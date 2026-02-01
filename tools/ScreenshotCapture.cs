using System.Collections.Generic;
using Godot;

namespace ShadersIn30Days.tools;

[Tool]
public partial class ScreenshotCapture : Node
{
    [ExportGroup("Output Settings")]
    [Export(PropertyHint.Dir)]
    public string OutputDirectory { get; set; } = "res://";
    
    [Export]
    public string FileName
    {
        get => _fileName;
        set => _fileName = value.EndsWith(".png") ? value : value + ".png";
    }
    private string _fileName = "output.png";

    private string OutputPath => OutputDirectory.TrimEnd('/') + "/" + FileName;

    [ExportToolButton("Capture", Icon = "SubViewport")]
    private Callable CaptureButton => Callable.From(CaptureScreenshot);

    private SubViewport? GetSubViewport() => 
        GetNodeOrNull<SubViewportContainer>("SubViewportContainer")?
            .GetNodeOrNull<SubViewport>("SubViewport");


    private void CaptureScreenshot()
    {
        var viewport = GetSubViewport();
        if (viewport == null)
        {
            GD.PrintErr("SubViewport not found!");
            return;
        }

        var image = viewport.GetTexture().GetImage();
        var err = image.SavePng(OutputPath);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to save screenshot: {err}");
            return;
        }

        GD.Print($"Screenshot saved to {OutputPath}");
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (GetSubViewport() == null)
        {
            warnings.Add("SubViewport node is missing. Please add a SubViewport as a child.");
        }

        return warnings.ToArray();
    }
}