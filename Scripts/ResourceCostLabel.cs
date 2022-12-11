using Godot;
using SpaceExplorer.Scripts.Minable;

public class ResourceCostLabel : HBoxContainer
{
    [Export] NodePath UpgradeMenuNodePath = null!;
    [Export] MinableType MinableType;
    
    private UpgradeMenu _upgradeMenu = null!;
    private Label _label = null!;
    private string _prefix = "";
    public override void _Ready()
    {
        _upgradeMenu = GetNode<UpgradeMenu>(UpgradeMenuNodePath);
        _label = GetNode<Label>("Label");
        _prefix = _label.Text;
    }

    public override void _Process(float delta)
    {
        Visible = _upgradeMenu.SelectedUpgrade.ResourceCost.TryGetValue(MinableType, out var cost);
        if(!Visible)
            return;

        _label.Text = $"{_prefix} ({ResourceInventory.ResourceCounts[MinableType]}/{cost})";
    }
}