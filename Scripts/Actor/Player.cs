using System;
using System.Collections.Immutable;
using System.Linq;
using Godot;

namespace SpaceExplorer.Scripts.Actor;

public class Player : RigidBody2D
{
	const float FUEL_CONSUMPTION_RATE = 4f;
	const float REFUEL_RATE = 8f;
	const float REPAIR_RATE = 8f;

	[Signal] public delegate void HealthChanged(int newHealth);
	[Signal] public delegate void FuelChanged(int newFuel);

	[Export]
	public float MaxHealth = 100;

	[Export]
	public float MaxFuel = 100;

	public bool DisableInput { get; set; } = false;

	public float RotSpeed => 100;
	public float ThrustSpeed => 100;
	public bool IsOnLandingPad { get; private set; } = false;

	// Node refs
	private AnimatedSprite _thrustAnimatedSprite = null!;
	private AudioStreamPlayer2D _rocketAudio = null!;
	private AudioStreamPlayer2D _impactAudio = null!;
	private AudioStreamPlayer2D _refuelAudio = null!;
	private AudioStreamPlayer2D _explosionAudio = null!;
	private Particles2D	_dustParticles = null!;
	private Sprite _shipSprite = null!;

	// Vars
	private Vector2 _velocityLastFrame = new Vector2(0,0); 
	private float _health;
	private float _fuel;

	public override void _Ready()
	{
		_thrustAnimatedSprite = GetNode<AnimatedSprite>("ThrustAnimation");
		_rocketAudio = GetNode<AudioStreamPlayer2D>("RocketAudio");
		_impactAudio = GetNode<AudioStreamPlayer2D>("ImpactAudio");
		_refuelAudio = GetNode<AudioStreamPlayer2D>("RefuelAudio");
		_explosionAudio = GetNode<AudioStreamPlayer2D>("ExplosionNode/ExplosionAudio");
		_dustParticles = GetNode<Particles2D>("ExplosionNode/DustParticles");
		_shipSprite = GetNode<Sprite>("ShipSprite");
		_thrustAnimatedSprite.Play();
		_health = MaxHealth;
		_fuel = MaxFuel;

		var feetHurtBox = GetNode<Area2D>("FeetHurtBox");
		feetHurtBox.Connect("body_entered", this, nameof(OnBodyShapeEnteredFeet));
		feetHurtBox.Connect("body_exited", this, nameof(OnBodyShapeExitedFeet));
		GetNode<Area2D>("HullHurtBox").Connect("body_entered", this, nameof(OnBodyShapeEnteredHull));
		_impactAudio.Connect("finished", this, nameof(OnImpactSoundFinished));
	}

	public override void _Process(float delta)
	{
		ConsumeFuel(delta);
		RefuelingAndRepair(delta);
		EffectsProcess();
	}

	public void EffectsProcess()
	{
		if(DisableInput)
			return;
		
		if (Input.IsActionJustPressed("player_up") && _fuel > 0)
		{
			_thrustAnimatedSprite.Show();
			_rocketAudio.Play();
		}

		if (Input.IsActionJustReleased("player_up") || _fuel <= 0)
		{
			_thrustAnimatedSprite.Hide();
			_rocketAudio.Stop();
		}
	}

	public void RefuelingAndRepair(float delta){
		if(!IsOnLandingPad || (_fuel >= MaxFuel && _health >= MaxHealth ) || Input.IsActionPressed("player_up"))
		{
			if(_refuelAudio.Playing)
				_refuelAudio.Stop();

			return;
		}

		if(!_refuelAudio.Playing)
			_refuelAudio.Play();

		_refuelAudio.PitchScale = (_fuel / MaxFuel) * (_health / MaxHealth) * 0.25f + 0.75f;
		_fuel += delta * REFUEL_RATE;
		_health += delta * REPAIR_RATE;

		_fuel = Math.Min(_fuel, MaxFuel);
		_health = Math.Min(_health, MaxHealth);

		EmitSignal(nameof(FuelChanged), _fuel);
		EmitSignal(nameof(HealthChanged), _health);

	}

	public void ConsumeFuel(float delta) {
		if(DisableInput)
			return;

		var inputStrength = Input.GetActionStrength("player_up");
		_fuel -= inputStrength * delta * FUEL_CONSUMPTION_RATE;
		EmitSignal(nameof(FuelChanged), _fuel);
	}

	public override void _PhysicsProcess(float delta)
	{
		AppliedForce = Vector2.Zero;
		AppliedTorque = 0f;
		ApplyGravity();
		ApplyThrust();
		_velocityLastFrame = LinearVelocity;
	}

	private void ApplyThrust()
	{
		if(DisableInput)
			return;

		if(_fuel > 0)
		{
			var thrust = ThrustSpeed * Vector2.Up * Input.GetActionStrength("player_up");
			AppliedForce += thrust.Rotated(Rotation);
		}

		var torque = Input.GetActionStrength("player_right") - Input.GetActionStrength("player_left");
		AppliedTorque += RotSpeed * torque;
	}

	private void ApplyGravity()
	{
		var gravityFields = GetTree().GetNodesInGroup("gravity_field").Cast<Area2D>().ToImmutableArray();
		var force = Physics.CalculateGravity(gravityFields, GlobalPosition);
		AppliedForce += force * Mass;
	}

	private void OnBodyShapeEnteredFeet(Node node){ 
		if(node is LandingPlatform)
			IsOnLandingPad = true;

		DamageShip(0.1f);
	}

	private void OnBodyShapeExitedFeet(Node node){ 
		if(node is LandingPlatform)
			IsOnLandingPad = false;
	}

	private void OnBodyShapeEnteredHull(Node _) => DamageShip(0.5f);

	private void DamageShip(float modifier = 1f)
	{
		var impact = (float)Math.Round(_velocityLastFrame.Length() * modifier);
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
		_explosionAudio.Play();
		_shipSprite.Hide();
		Mode = RigidBody2D.ModeEnum.Static;
	}
}
