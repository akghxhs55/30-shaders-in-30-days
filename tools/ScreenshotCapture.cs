using System.Collections.Generic;
using Godot;

namespace ShaderStudy.Tools;

[Tool]
public partial class ScreenshotCapture : Node
{
    [Export] public string OutputFilePath { get; set; } = "res://output.png";

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
        var err = image.SavePng(OutputFilePath);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to save screenshot: {err}");
            return;
        }

        GD.Print($"Screenshot saved to {OutputFilePath}");
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