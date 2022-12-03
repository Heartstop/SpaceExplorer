using Godot;

public class Player : RigidBody2D
{
	public override void _Ready()
	{
	}

	public override void _Process(float delta)
	{
		ApplyThrust(delta);
	}

	private void ApplyThrust(float delta)
	{
		var force = 
			(Vector2.Up * Input.GetActionStrength("player_up"))
			+ (Vector2.Left * Input.GetActionStrength("player_left") * 0.25f)
			+ (Vector2.Right * Input.GetActionStrength("player_right") * 0.25f)
			+ (Vector2.Down * Input.GetActionStrength("player_down") * 0.25f);
		AddCentralForce(force * delta * 100);

		var torque = Input.GetActionStrength("player_rot_right") - Input.GetActionStrength("player_rot_left");
		AddTorque(torque * delta * 100);
	}
}
