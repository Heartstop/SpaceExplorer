using System.Collections.Generic;
using Godot;

public class ZoomedOutContainerRef {
    public ZoomedOutContainer NodeRef { get; set; }
    public Sprite ZoomedOutSprite { get; set; }

    public ZoomedOutContainerRef(Sprite zoomedOutSprite, ZoomedOutContainer nodeRef)
    {
        ZoomedOutSprite = zoomedOutSprite;
        NodeRef = nodeRef;
    }
}

public class GameUi : CanvasLayer
{
    ProgressBar _healthProgressBar = null!;
    ProgressBar _fuelProgressBar = null!;
    private Dictionary<ulong, ZoomedOutContainerRef> _zoomedOutContainersRefs = new Dictionary<ulong, ZoomedOutContainerRef>();

    public override void _Ready()
    {
        foreach(var node in GetTree().GetNodesInGroup("ZoomedOutContainer")){
            EvalContainer(node);
        }

        _healthProgressBar = GetNode<ProgressBar>("Container/HealthVBoxContainer/HealthProgressBar");
        _fuelProgressBar = GetNode<ProgressBar>("Container/FuelVBoxContainer/FuelProgressBar");
        
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        foreach(var containerRef in _zoomedOutContainersRefs.Values){
            containerRef.ZoomedOutSprite.Position = containerRef.NodeRef.GetGlobalTransformWithCanvas().origin;
        }
    }

    private void EvalContainer(object node){
        if(node is not ZoomedOutContainer zoomedOutContainer)
            return;

        var sprite = new Sprite();
        sprite.Texture = zoomedOutContainer.Texture;
        _zoomedOutContainersRefs.Add(zoomedOutContainer.GetInstanceId(), new ZoomedOutContainerRef(sprite, zoomedOutContainer));
        AddChild(sprite);
    }

    private void OnNodeAdded(Node node) => EvalContainer(node);
    public void SetHealthBarValue(double value) => _healthProgressBar.Value = value;
    public void SetMaxHealthBarValue(double max) => _healthProgressBar.MaxValue = max;
    public void SetFuelBarValue(double value) => _fuelProgressBar.Value = value;
    public void SetMaxFuelBarValue(double max) => _fuelProgressBar.MaxValue = max;


}
