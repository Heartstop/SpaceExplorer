using Godot;

public class Player : RigidBody2D
{
	public float RotSpeed => 2;
	public float ThrustSpeed => 100;

	private AnimatedSprite _thrustAnimatedSprite;

	public override void _Ready()
	{
		_thrustAnimatedSprite = GetNode<AnimatedSprite>("ThrustAnimation");
		_thrustAnimatedSprite.Play();
	}

	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("player_up"))
			_thrustAnimatedSprite.Show();

		if(Input.IsActionJustReleased("player_up"))
			_thrustAnimatedSprite.Hide();
	}

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
