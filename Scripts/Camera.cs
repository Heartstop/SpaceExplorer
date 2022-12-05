using System;
using System.Security.Permissions;
using Godot;

public class Camera : Camera2D
{
	[Export] public float MinZoom = 0.25f;
	[Export] public float MaxZoom = 3.5f;
	[Signal] public delegate void ZoomChanged(float newZoom);
	const float SCROLL_WHEEL_ZOOM_RATE = 0.1f; 

	public override void _Process(float delta)
	{
		var material = (ShaderMaterial)GetNode<ColorRect>("CanvasLayer/ColorRect").Material;
		material.SetShaderParam("ROTATION", GlobalRotation);

		ZoomProcess(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
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
		var amount = (Input.GetActionStrength("camera_zoom_in") - Input.GetActionStrength("camera_zoom_out")) * delta;
		AddRelativeZoom(amount);
	}

	private void AddRelativeZoom(float amount) {
		var newZoom = Math.Max(Math.Min(MaxZoom, Zoom.x + amount), MinZoom);
		Zoom = new Vector2(newZoom, newZoom);
		EmitSignal(nameof(ZoomChanged), newZoom);
	}
}
