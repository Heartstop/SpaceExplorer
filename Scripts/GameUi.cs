using System.Collections.Generic;
using Godot;

public class ZoomedOutIconRef {
    public ZoomedOutIcon GameRef { get; set; }
    public Sprite LocalRef { get; set; }

    public ZoomedOutIconRef(Sprite localRef, ZoomedOutIcon gameRef)
    {
        LocalRef = localRef;
        GameRef = gameRef;
    }
}

public class GameUi : CanvasLayer
{
    ProgressBar _healthProgressBar = null!;
    ProgressBar _fuelProgressBar = null!;
    private Dictionary<ulong, ZoomedOutIconRef> _zoomedOutIconRefs = new Dictionary<ulong, ZoomedOutIconRef>();

    public override void _Ready()
    {
        foreach(var node in GetTree().GetNodesInGroup("ZoomedOutContainer")){
            EvalIconRef(node);
        }

        _healthProgressBar = GetNode<ProgressBar>("Container/HealthVBoxContainer/HealthProgressBar");
        _fuelProgressBar = GetNode<ProgressBar>("Container/FuelVBoxContainer/FuelProgressBar");
        
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        foreach(var containerRef in _zoomedOutIconRefs.Values){
            containerRef.LocalRef.Position = containerRef.GameRef.GetGlobalTransformWithCanvas().origin;
        }
    }

    private void EvalIconRef(object node){
        if(node is not ZoomedOutIcon zoomedOutIcon)
            return;

        var sprite = new Sprite();
        sprite.Texture = zoomedOutIcon.Texture;
        _zoomedOutIconRefs.Add(zoomedOutIcon.GetInstanceId(), new ZoomedOutIconRef(sprite, zoomedOutIcon));
        AddChild(sprite);
    }

    private void OnNodeAdded(Node node) => EvalIconRef(node);
    public void SetHealthBarValue(double value) => _healthProgressBar.Value = value;
    public void SetMaxHealthBarValue(double max) => _healthProgressBar.MaxValue = max;
    public void SetFuelBarValue(double value) => _fuelProgressBar.Value = value;
    public void SetMaxFuelBarValue(double max) => _fuelProgressBar.MaxValue = max;


}
