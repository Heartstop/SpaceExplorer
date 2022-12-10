using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot;

class UpgradeInfo {
    public string Name { get; set; }
    public string Description { get; set; }
    public HashSet<string> UpgradeDependencies { get; set; }
    public string NodePath { get; set; }

    public UpgradeInfo(string name, string description, HashSet<string> upgradeDependencies, string nodePath)
    {
        Name = name;
        Description = description;
        UpgradeDependencies = upgradeDependencies;
        NodePath = nodePath;
    }
}

public class UpgradeMenu : Panel
{
    private static IDictionary<string, UpgradeInfo> Upgrades = new Dictionary<string, UpgradeInfo>
    {
        { "RocketPower", new UpgradeInfo(
            name: "Rocket Power!",
            nodePath: "VBoxContainer/HBox/RocketPower",
            upgradeDependencies: new HashSet<string>(),
            description: 
@"If we are going to be able to take ourselves anywhere, we will need 
to modify our rockets to produce some more power!")
        },
        { "Radio", new UpgradeInfo(
            name: "Radio tower",
            nodePath: "VBoxContainer/HBox2/Radio",
            upgradeDependencies: new HashSet<string> { "RocketPower" },
            description: 
@"This radio tower will help us find celestial bodies containing the
resources to get us home.")
        },
        {
            "TitaniumHull", new UpgradeInfo(
                name: "Titanium hull",
                nodePath: "VBoxContainer/HBox3/TitaniumHull",
                upgradeDependencies: new HashSet<string> { "Radio" },
                description: 
"A hull made of titanium that is capable of handling high temperature environments.")
        },
        {
            "Antifreeze", new UpgradeInfo( 
                name: "Antifreeze",
                nodePath: "VBoxContainer/HBox3/Antifreeze",
                upgradeDependencies: new HashSet<string> { "Radio" },
                description: 
"Antifreeze additive for our fuel so that it does not freeze in cold environments.")
        },
        {
            "RadiationShielding", new UpgradeInfo(
                name: "Radiation shielding",
                nodePath: "VBoxContainer/HBox4/RadiationShielding",
                upgradeDependencies: new HashSet<string> { "TitaniumHull", "Antifreeze" },
                description: 
"Adds radiation shielding to our hull to protect ourselves against radioactive environments")
        },
        {
            "WarpDrive", new UpgradeInfo(
                name: "Warp drive",
                nodePath: "VBoxContainer/HBox5/WarpDrive",
                upgradeDependencies: new HashSet<string> { "RadiationShielding" },
                description: "A warp drive to take us home!")
        }
    };

    [Signal] public delegate void CraftedUpgradesChanged(List<string> currentUpgrades);

    private RichTextLabel _nameLabel = null!;
    private RichTextLabel _descriptionLabel = null!;
    private Control _currentlyFocusedControl = null!;
    private Dictionary<string, Button> _upgradeButtons = new Dictionary<string, Button>();
    private string _selectedUpgradeKey = "RocketPower";
    private HashSet<string> _craftedUpgrades = new HashSet<string>();
    public override void _Ready()
    {
        _nameLabel = GetNode<RichTextLabel>("InfoVBox/NameLabel");
        _descriptionLabel = GetNode<RichTextLabel>("InfoVBox/DescriptionLabel");
        _currentlyFocusedControl = GetNode<Button>("VBoxContainer/HBox/RocketPower");

        foreach(var upgrade in Upgrades)
            GetNode<Button>(upgrade.Value.NodePath).Connect("gui_input", this, nameof(OnUpgradeButtonInput));

        GetViewport().Connect("gui_focus_changed", this, nameof(OnGuiFocusChanged));
        Connect("visibility_changed", this, nameof(OnVisibilityChanged));

        _currentlyFocusedControl.GrabFocus();
    }

    private void OnUpgradeButtonInput(InputEvent inputEvent)
    {
        if(!inputEvent.IsAction("ui_accept"))
            return;

        var selectedUpgrade = Upgrades[_selectedUpgradeKey];
        var button = GetNode<Button>(selectedUpgrade.NodePath);

        if(button.Disabled)
            return;

        // TODO: Add logic for checking if player has the resources available
        _craftedUpgrades.Add(_selectedUpgradeKey);
        EmitSignal(nameof(CraftedUpgradesChanged), _craftedUpgrades.ToList());        
        SetDisabledState();
    }

    private void SetDisabledState()
    {
        foreach(var upgrade in Upgrades){
            var button = GetNode<Button>(upgrade.Value.NodePath);
            button.Disabled = 
                _craftedUpgrades.Contains(upgrade.Key) 
                || !upgrade.Value.UpgradeDependencies.All(key => _craftedUpgrades.Contains(key));
        }
    }

    private void OnVisibilityChanged()
    {
        if(Visible)
            _currentlyFocusedControl.GrabFocus();
    }


    private void OnGuiFocusChanged(Control control)
    {
        if(control is not UpgradeButton upgradeButton)
            return;

        var upgradeKey = upgradeButton.Name;
        var upgradeInfo = Upgrades[upgradeKey];
        if (upgradeInfo is null)
            throw new System.Exception("Upgrade info is null");

        _nameLabel.Text = upgradeInfo.Name;
        _descriptionLabel.Text = upgradeInfo.Description;
        _currentlyFocusedControl = upgradeButton;
        _selectedUpgradeKey = upgradeKey;
    }

    public override void _Process(float delta)
    {
    }

}
