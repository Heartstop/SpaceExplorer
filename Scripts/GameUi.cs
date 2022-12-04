using Godot;

public class GameUi : CanvasLayer
{
    ProgressBar _healthProgressBar = null!;
    public override void _Ready()
    {
        _healthProgressBar = GetNode<ProgressBar>("HFlowContainer/VBoxContainer/HealthProgressBar");
    }

    public void SetHealthBarValue(double value) => _healthProgressBar.Value = value;
    public void SetMaxHealthBarValue(double max) => _healthProgressBar.MaxValue = max;
}
