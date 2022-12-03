using Godot;

public class Player : RigidBody2D
{
	public override void _Ready()
	{
	}

	public override void _Process(float delta)
	{
		ApplyThrust();
	}

	private void ApplyThrust()
	{
		var force = 
			(Vector2.Up * Input.GetActionStrength("player_up"))
			* (Vector2.Left * Input.GetActionStrength("player_left") * 0.15f)
			* (Vector2.Right * Input.GetActionStrength("player_right") * 0.15f)
			* (Vector2.Down * Input.GetActionStrength("player_down") * 0.1f);
		AddCentralForce(force);

		var torque = Input.GetActionStrength("player_rot_right") - Input.GetActionStrength("player_rot_left");
		AddTorque(torque);
	}
}
