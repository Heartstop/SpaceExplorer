using System;
using Godot;

namespace SpaceExplorer.Scripts.Actor;

public class Player : RigidBody2D
{
	[Signal] public delegate void HealthChanged(int newHealth);

	public float RotSpeed => 2;
	public float ThrustSpeed => 100;

	[Export]
	public int StartHealth = 100;

	// Node refs
	private AnimatedSprite _thrustAnimatedSprite = null!;
	private AudioStreamPlayer2D _rocketAudio = null!;
	private AudioStreamPlayer2D _impactAudio = null!;
	private AudioStreamPlayer2D _explosionAudio = null!;
	private Particles2D	_dustParticles = null!;
	private Particles2D _debrisParticles = null!;
	private Sprite _shipSprite = null!;

	// Vars
	private Vector2 _velocityLastFrame = new Vector2(0,0); 
	private int _health = 100;

	public override void _Ready()
	{
		_thrustAnimatedSprite = GetNode<AnimatedSprite>("ThrustAnimation");
		_rocketAudio = GetNode<AudioStreamPlayer2D>("RocketAudio");
		_impactAudio = GetNode<AudioStreamPlayer2D>("ImpactAudio");
		_explosionAudio = GetNode<AudioStreamPlayer2D>("ExplosionNode/ExplosionAudio");
		_dustParticles = GetNode<Particles2D>("ExplosionNode/DustParticles");
		_debrisParticles = GetNode<Particles2D>("ExplosionNode/DebrisParticles");
		_shipSprite = GetNode<Sprite>("ShipSprite");
		_thrustAnimatedSprite.Play();
		_health = StartHealth;

		Connect("body_entered", this, nameof(_on_Player_body_entered));
		_impactAudio.Connect("finished", this, nameof(OnImpactSoundFinished));
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
		_velocityLastFrame = LinearVelocity;
	}

	private void ApplyThrust(float delta)
	{
		var force = Vector2.Up * Input.GetActionStrength("player_up");
		LinearVelocity += delta * ThrustSpeed * force.Rotated(GlobalRotation);

		var torque = Input.GetActionStrength("player_right") - Input.GetActionStrength("player_left");
		AngularVelocity += delta * RotSpeed * torque;
	}

	private void _on_Player_body_entered(Node body)
	{
		var impact = (int)Math.Round(_velocityLastFrame.Length() * 0.5);
		if(impact < 10)
			return;

		_health -= impact;
		EmitSignal(nameof(HealthChanged), _health);
		if(_health <= 0)
			Explode();
		else
			PlayImpactSound();
	}

	private void PlayImpactSound(){
		var pitchScale = _impactAudio.PitchScale;
		_impactAudio.PitchScale = pitchScale + ((GD.Randf() - 0.5f) * .8f);
		_impactAudio.Play();
	}

	private void OnImpactSoundFinished() => _impactAudio.PitchScale = 1;

	private void Explode()
	{
		_dustParticles.Emitting = true;
		_debrisParticles.Emitting = true;
		_explosionAudio.Play();
		_shipSprite.Hide();
		Mode = RigidBody2D.ModeEnum.Static;
	}
}
