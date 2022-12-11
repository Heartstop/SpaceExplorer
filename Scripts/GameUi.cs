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
	public bool DisableInput { get; set; } = false;

	[Export] float MarginTop = 20f;
	[Export] float MarginRight = 20f;
	[Export] float MarginBottom = 20f;
	[Export] float MarginLeft = 20f;

	ProgressBar _healthProgressBar = null!;
	ProgressBar _fuelProgressBar = null!;
	Control _worldIconsContainer = null!;
	Control _upgradeMenuContainer = null!;
	Label _timeScale = null!;
	MessageDialog _messageDialog = null!;
	Panel _resourcePanel = null!;
	Control _container = null!;
	
	[Signal]
	delegate void TimeScaleChanged();

	private Dictionary<ulong, ZoomedOutIconRef> _zoomedOutIconRefs = new Dictionary<ulong, ZoomedOutIconRef>();
 
	public override void _Ready()
	{
		_healthProgressBar = GetNode<ProgressBar>("Container/HealthVBoxContainer/HealthProgressBar");
		_fuelProgressBar = GetNode<ProgressBar>("Container/FuelVBoxContainer/FuelProgressBar");
		_timeScale = GetNode<Label>("Container/TimeScaleLabel");
		_worldIconsContainer = GetNode<Control>("WorldIconsContainer");
		_upgradeMenuContainer = GetNode<Control>("UpgradeMenuContainer");
		_messageDialog = GetNode<MessageDialog>("MessageDialog");
		_resourcePanel = GetNode<Panel>("ResourcePanel");
		_container = GetNode<Control>("Container");

		_upgradeMenuContainer.Connect("gui_input", this, nameof(OnUpgradeMenuContainerInput));
		Connect(nameof(TimeScaleChanged), this, nameof(OnTimeScaleChange));

		foreach(var node in GetTree().GetNodesInGroup("ZoomedOutContainer")){
			EvalIconRef(node);
		}
	}

	public void ShowMessage(string message, Action? callback = null, bool keepUiHidden = false)
	{
		_resourcePanel.Visible = false;
		_upgradeMenuContainer.Visible = false;
		_container.Visible = false;

		Action modifiedCallback = () => {
			if(!keepUiHidden)
			{
				_resourcePanel.Visible = true;
				_container.Visible = true;
			}
			if(callback is not null)
				callback();
		};

		_messageDialog.ShowMessage(message, modifiedCallback);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(_messageDialog.Visible || DisableInput)
			return;

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

	private void OnTimeScaleChange() => _timeScale.Text = $"{Engine.TimeScale:F1}x Speed";

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

			containerRef.LocalRef.Visible = (
				AlwaysShowIcons
				|| canvasPosition.x <= xMin 
				|| canvasPosition.x >= xMax
				|| canvasPosition.y <= yMin
				|| canvasPosition.y >= yMax
			) && !containerRef.GameRef.AlwaysHidden;

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
