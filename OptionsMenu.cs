using Godot;

public class OptionsMenu : Control
{
    [Signal] public delegate void Continue();
    [Signal] public delegate void RestartGame();

    [Export] public bool ShowRestartButton { get; set; } = false;
    
    HSlider _musicSlider = null!;
    HSlider _fxSlider = null!;
    Button _restartButton = null!;
    Button _continueButton = null!;
    public override void _Ready()
    {
        _musicSlider = GetNode<HSlider>("PanelContainer/VBoxContainer/HBoxMusic/HSlider");
        _fxSlider = GetNode<HSlider>("PanelContainer/VBoxContainer/HBoxFx/HSlider");
        _continueButton = GetNode<Button>("PanelContainer/VBoxContainer/Button");
        _restartButton = GetNode<Button>("PanelContainer/VBoxContainer/HBoxContainer/RestartButton");

        _restartButton.Visible = ShowRestartButton;
        _continueButton.Connect("gui_input", this, nameof(OnButtonInput));
        _restartButton.Connect("gui_input", this, nameof(OnRestartButtonInput));
        _musicSlider.GrabFocus();
    }

    public void Focus()
    {
        _musicSlider.GrabFocus();
    }

    private void OnRestartButtonInput(InputEvent inputEvent)
    {
        if(!inputEvent.IsActionReleased("ui_accept"))
            return;

        EmitSignal(nameof(RestartGame));
    }

    private void OnButtonInput(InputEvent inputEvent)
    {
        if(!inputEvent.IsActionReleased("ui_accept"))
            return;

        EmitSignal(nameof(Continue));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        AudioServer.SetBusVolumeDb(0, (float)_musicSlider.Value);
        AudioServer.SetBusVolumeDb(1, (float)_fxSlider.Value);
    }

}
