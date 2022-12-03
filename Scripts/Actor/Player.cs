using System.Linq;
using Godot;
using SpaceExplorer.Scripts.CelestialBodies;

namespace SpaceExplorer.Scripts.Actor;

public class Player : RigidBody2D
{
	public float RotSpeed => 2;
	public float ThrustSpeed => 100;

	[Export]
	public NodePath[]? GravitationalFields;

	private AnimatedSprite _thrustAnimatedSprite = null!;
	private AudioStreamPlayer2D _rocketAudio = null!;

	public override void _Ready()
	{
		_thrustAnimatedSprite = GetNode<AnimatedSprite>("ThrustAnimation");
		_rocketAudio = GetNode<AudioStreamPlayer2D>("RocketAudio");
		_thrustAnimatedSprite.Play();
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("player_up"))
		{
			_thrustAnimatedSprite.Show();
			_rocketAudio.Play();
		}

		if (Input.IsActionJustReleased("player_up"))
		{
			_thrustAnimatedSprite.Hide();
			_rocketAudio.Stop();
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		ApplyThrust(delta);
		ApplyGravity(delta);
	}

	private void ApplyThrust(float delta)
	{
		var force = Vector2.Up * Input.GetActionStrength("player_up");
		LinearVelocity += delta * ThrustSpeed * force.Rotated(GlobalRotation);

		var torque = Input.GetActionStrength("player_right") - Input.GetActionStrength("player_left");
		AngularVelocity += delta * RotSpeed * torque;
	}

	private void ApplyGravity(float delta)
	{
		if (GravitationalFields is null)
			return;

		var gravityFields = GravitationalFields.Select(GetNode<CelestialBody>);
		var gravitationalForce = Physics.CalculateForce(this, gravityFields);
		LinearVelocity += gravitationalForce;
	}
}
