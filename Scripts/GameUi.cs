using Godot;

public class GameUi : CanvasLayer
{
    ProgressBar _healthProgressBar = null!;
    ProgressBar _fuelProgressBar = null!;

    public override void _Ready()
    {
        _healthProgressBar = GetNode<ProgressBar>("Container/HealthVBoxContainer/HealthProgressBar");
        _fuelProgressBar = GetNode<ProgressBar>("Container/FuelVBoxContainer/FuelProgressBar");
    }

    public void SetHealthBarValue(double value) => _healthProgressBar.Value = value;
    public void SetMaxHealthBarValue(double max) => _healthProgressBar.MaxValue = max;
    public void SetFuelBarValue(double value) => _fuelProgressBar.Value = value;
    public void SetMaxFuelBarValue(double max) => _fuelProgressBar.MaxValue = max;


}
