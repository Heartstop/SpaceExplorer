using System.Collections.Generic;
using Godot;

class UpgradeInfo {
    public string Name { get; set; }
    public string Description { get; set; }
    public HashSet<string> UpgradeDependencies { get; set; }

    public UpgradeInfo(string name, string description, HashSet<string> upgradeDependencies)
    {
        Name = name;
        Description = description;
        UpgradeDependencies = upgradeDependencies;
    }
}

public class UpgradeMenu : Panel
{
    private static IDictionary<string, UpgradeInfo> Upgrades = new Dictionary<string, UpgradeInfo>
    {
        { "RocketPower", new UpgradeInfo(
            name: "Rocket Power!",
            upgradeDependencies: new HashSet<string>(),
            description: 
@"If we are going to be able to take ourselves anywhere, we will need 
to modify our rockets to produce some more power!")
        },
        { "Radio", new UpgradeInfo(
            name: "Radio tower",
            upgradeDependencies: new HashSet<string> { "RocketPower" },
            description: 
@"This radio tower will help us find celestial bodies containing the
resources to get us home.")
        },
        {
            "TitaniumHull", new UpgradeInfo(
                name: "Titanium hull",
                upgradeDependencies: new HashSet<string> { "Radio" },
                description: 
"A hull made of titanium that is capable of handling high temperature environments.")
        },
        {
            "Antifreeze", new UpgradeInfo( 
                name: "Antifreeze",
                upgradeDependencies: new HashSet<string> { "Radio" },
                description: 
"Antifreeze additive for our fuel so that it does not freeze in cold environments.")
        },
        {
            "RadiationShielding", new UpgradeInfo(
                name: "Radiation shielding",
                upgradeDependencies: new HashSet<string> { "TitaniumHull", "AntiFreeze" },
                description: 
"Adds radiation shielding to our hull to protect ourselves against radioactive environments")
        },
        {
            "WarpDrive", new UpgradeInfo(
                name: "Warp drive",
                upgradeDependencies: new HashSet<string> { "RadiationShielding" },
                description: "A warp drive to take us home!")
        }
    };

    private RichTextLabel _nameLabel = null!;
    private RichTextLabel _descriptionLabel = null!;
    private Control _lastFocusedControl = null!;

    public override void _Ready()
    {
        _nameLabel = GetNode<RichTextLabel>("InfoVBox/NameLabel");
        _descriptionLabel = GetNode<RichTextLabel>("InfoVBox/DescriptionLabel");
        _lastFocusedControl = GetNode<Button>("VBoxContainer/HBox/RocketPower");

        _lastFocusedControl.GrabFocus();
        GetViewport().Connect("gui_focus_changed", this, nameof(OnGuiFocusChanged));
        Connect("visibility_changed", this, nameof(OnVisibilityChanged));
    }

    private void OnVisibilityChanged()
    {
        if(Visible)
            _lastFocusedControl.GrabFocus();
    }


    private void OnGuiFocusChanged(Control control)
    {
        if(control is not UpgradeButton upgradeButton)
            return;

        var upgradeInfo = Upgrades[upgradeButton.Name];
        if (upgradeInfo is null)
            throw new System.Exception("Upgrade info is null");

        _nameLabel.Text = upgradeInfo.Name;
        _descriptionLabel.Text = upgradeInfo.Description;
        _lastFocusedControl = upgradeButton;
    }

    public override void _Process(float delta)
    {
    }

}
