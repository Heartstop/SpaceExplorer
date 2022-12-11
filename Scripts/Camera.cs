using System;
using Godot;

public class Camera : Camera2D
{
	[Export] public float MinZoom = 0.25f;
	[Export] public float MaxZoom = 3.5f;
	[Signal] public delegate void ZoomChanged(float newZoom);

	public bool DisableInput { get; set; }
	public float CurrentZoom { get; set; } = 1;
	const float SCROLL_WHEEL_ZOOM_RATE = 0.1f;

	public override void _Process(float delta)
	{
		var material = (ShaderMaterial)GetNode<ColorRect>("CanvasLayer/ColorRect").Material;
		material.SetShaderParam("ROTATION", GlobalRotation);

		ZoomProcess(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(DisableInput)
			return;

		var zoom = Zoom.x;

		if(!@event.IsAction("camera_zoom_in") && !@event.IsAction("camera_zoom_out"))
			return;

		if(@event is not InputEventMouse mouseEvent)
			return;

		float amount = (mouseEvent.IsAction("camera_zoom_in") ? 1 : -1) * SCROLL_WHEEL_ZOOM_RATE;
	
		AddRelativeZoom(amount);
	}

	private void ZoomProcess(float delta)
	{
		var zoomPow = (float)Math.Pow(CurrentZoom, 2.8);
		Zoom = new Vector2(zoomPow, zoomPow);

		if(DisableInput)
			return;
			
		var amount = (Input.GetActionStrength("camera_zoom_in") - Input.GetActionStrength("camera_zoom_out")) * delta;
		AddRelativeZoom(amount);
		
	}

	private void AddRelativeZoom(float amount) {
		var newZoom = (float)Math.Max(Math.Min(MaxZoom, CurrentZoom + amount), MinZoom);
		CurrentZoom = newZoom;
		EmitSignal(nameof(ZoomChanged), newZoom);
	}
}
