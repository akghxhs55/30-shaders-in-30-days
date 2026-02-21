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
    
    [ExportGroup("Animation Capture")]
    [Export(PropertyHint.Range, "0.1,5.0,")]
    public float Duration { get; set; } = 2.0f;

    [Export(PropertyHint.Range, "10,30,")]
    public int Fps { get; set; } = 15;

    [ExportToolButton("Capture Animation", Icon = "Animation")]
    private Callable CaptureAnimationButton => Callable.From(StartAnimationCapture);

    private bool _capturing = false;
    private float _elapsed = 0.0f;
    private int _frameCount = 0;
    private float _frameInterval = 0.0f;
    private float _timeSinceLastFrame = 0.0f;

    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint() || !_capturing) return;

        _elapsed += (float)delta;
        _timeSinceLastFrame += (float)delta;

        if (_timeSinceLastFrame >= _frameInterval)
        {
            _timeSinceLastFrame -= _frameInterval;
            CaptureFrame();
        }

        if (_elapsed >= Duration)
        {
            _capturing = false;
            GD.Print($"Animation capture complete: {_frameCount} frames saved");
        }
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
    
    private void StartAnimationCapture()
    {
        var viewport = GetSubViewport();
        if (viewport == null)
        {
            GD.PrintErr("SubViewport not found!");
            return;
        }

        _capturing = true;
        _elapsed = 0.0f;
        _frameCount = 0;
        _frameInterval = 1.0f / Fps;
        _timeSinceLastFrame = _frameInterval; // 첫 프레임 즉시 캡처

        GD.Print($"Starting animation capture: {Duration}s at {Fps}fps");
    }

    private void CaptureFrame()
    {
        var viewport = GetSubViewport();
        if (viewport == null) return;

        string baseName = FileName.Replace(".png", "");
        string framePath = OutputDirectory.TrimEnd('/') + $"/{baseName}_{_frameCount:D4}.png";

        var image = viewport.GetTexture().GetImage();
        var err = image.SavePng(framePath);
        if (err == Error.Ok)
            _frameCount++;
        else
            GD.PrintErr($"Failed to save frame {_frameCount}: {err}");
    }
    
    private SubViewport? GetSubViewport() => 
        GetNodeOrNull<SubViewportContainer>("SubViewportContainer")?
            .GetNodeOrNull<SubViewport>("SubViewport");
}