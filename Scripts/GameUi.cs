using System;
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
	public bool AlwaysShowIcons { get; set; } = false;
	public bool IsUpgradeMenuOpen { get { return _upgradeMenuContainer.Visible; }}

	[Export] float MarginTop = 20f;
	[Export] float MarginRight = 20f;
	[Export] float MarginBottom = 20f;
	[Export] float MarginLeft = 20f;

	ProgressBar _healthProgressBar = null!;
	ProgressBar _fuelProgressBar = null!;
	Control _worldIconsContainer = null!;
	Control _upgradeMenuContainer = null!;
	private Dictionary<ulong, ZoomedOutIconRef> _zoomedOutIconRefs = new Dictionary<ulong, ZoomedOutIconRef>();
 
	public override void _Ready()
	{
		_healthProgressBar = GetNode<ProgressBar>("Container/HealthVBoxContainer/HealthProgressBar");
		_fuelProgressBar = GetNode<ProgressBar>("Container/FuelVBoxContainer/FuelProgressBar");
		_worldIconsContainer = GetNode<Control>("WorldIconsContainer");
		_upgradeMenuContainer = GetNode<Control>("UpgradeMenuContainer");

		_upgradeMenuContainer.Connect("gui_input", this, nameof(OnUpgradeMenuContainerInput));

		foreach(var node in GetTree().GetNodesInGroup("ZoomedOutContainer")){
			EvalIconRef(node);
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsActionReleased("open_upgrade_menu"))
		{
			_upgradeMenuContainer.Visible = !_upgradeMenuContainer.Visible;
		}
	}

	private void OnUpgradeMenuContainerInput(InputEvent inputEvent){
		if(inputEvent is not InputEventMouse mouseEvent || mouseEvent.ButtonMask != (int)ButtonList.Left)
			return;

		_upgradeMenuContainer.Visible = false;
	}

	public override void _Process(float delta)
	{
		base._Process(delta);
		var viewPortSize = GetViewport().GetVisibleRect().Size;

		foreach(var containerRef in _zoomedOutIconRefs.Values){
			var xMin = MarginLeft;
			var xMax = viewPortSize.x - MarginRight;
			var yMin = MarginTop;
			var yMax = viewPortSize.y - MarginBottom;
			var canvasPosition = containerRef.GameRef.GetGlobalTransformWithCanvas().origin;

			containerRef.LocalRef.Visible = AlwaysShowIcons 
				|| canvasPosition.x <= xMin 
				|| canvasPosition.x >= xMax
				|| canvasPosition.y <= yMin
				|| canvasPosition.y >= yMax;

			containerRef.LocalRef.Position = new Vector2(
				x: Math.Min(xMax, (Math.Max(xMin, canvasPosition.x))),
				y: Math.Min(yMax, (Math.Max(yMin, canvasPosition.y)))
			);
		}
	}

	private void EvalIconRef(object node){
		if(node is not ZoomedOutIcon zoomedOutIcon)
			return;

		var sprite = new Sprite();
		sprite.Texture = zoomedOutIcon.Texture;
		_zoomedOutIconRefs.Add(zoomedOutIcon.GetInstanceId(), new ZoomedOutIconRef(sprite, zoomedOutIcon));
		_worldIconsContainer.AddChild(sprite);
	}

	private void OnNodeAdded(Node node) => EvalIconRef(node);
	public void SetHealthBarValue(double value) => _healthProgressBar.Value = value;
	public void SetMaxHealthBarValue(double max) => _healthProgressBar.MaxValue = max;
	public void SetFuelBarValue(double value) => _fuelProgressBar.Value = value;
	public void SetMaxFuelBarValue(double max) => _fuelProgressBar.MaxValue = max;


}
