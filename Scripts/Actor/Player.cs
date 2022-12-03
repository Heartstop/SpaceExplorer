using Godot;

public class Player : RigidBody2D
{
	public float RotSpeed => 2;
	public float ThrustSpeed => 100;

	public override void _PhysicsProcess(float delta)
	{
		ApplyThrust(delta);
	}

	private void ApplyThrust(float delta)
	{
		var force = Vector2.Up * Input.GetActionStrength("player_up");
		LinearVelocity += delta * ThrustSpeed * force.Rotated(GlobalRotation);

		var torque = Input.GetActionStrength("player_right") - Input.GetActionStrength("player_left");
		AngularVelocity += delta * RotSpeed * torque;
	}
}
