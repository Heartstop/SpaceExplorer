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
	private AudioStreamPlayer2D _explosionAudio = null!;
	private Particles2D	_dustParticles = null!;
	private Particles2D _debrisParticles = null!;

	public override void _Ready()
	{
		_thrustAnimatedSprite = GetNode<AnimatedSprite>("ThrustAnimation");
		_rocketAudio = GetNode<AudioStreamPlayer2D>("RocketAudio");
		_explosionAudio = GetNode<AudioStreamPlayer2D>("ExplosionNode/ExplosionAudio");
		_dustParticles = GetNode<Particles2D>("ExplosionNode/DustParticles");
		_debrisParticles = GetNode<Particles2D>("ExplosionNode/DebrisParticles");

		_thrustAnimatedSprite.Play();
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("player_up"))
		{
			_thrustAnimatedSprite.Show();
			_rocketAudio.Play();
			Explode();
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
		var gravitationalForce = delta * Physics.CalculateForce(this, gravityFields);
		LinearVelocity += gravitationalForce;
	}

	private void Explode()
	{
		_dustParticles.Emitting = true;
		_debrisParticles.Emitting = true;
		_explosionAudio.Play();
	}
}
