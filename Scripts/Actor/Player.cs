using System;
using System.Collections.Immutable;
using System.Diagnostics;
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

	public bool DisableInput => UIOpen || Dead || _radioactiveInterferenceEnd > DateTimeOffset.Now;
	public bool Dead = false;
	public bool UIOpen = false;
	private DateTimeOffset? _radioactiveInterferenceEnd = null;

	public float RotSpeed => 10000 / _shipCold;
	public float ThrustSpeed => 10000 / _shipCold;
	public float HeatingSpeed => 0.1f;
	public float HeatDamage => 1f;
	public float ColdDamage => 1f;
	public float CoolingSpeed => 1f;
	public float RadioactiveDamage => 5f;
	private float _shipCold = 1;
	public bool IsOnLandingPad { get; private set; } = false;

	public bool TitaniumHullUpgrade = false;
	public bool AntiFreezeUpgrade = false;
	public bool RadiationShieldUpgrade = false;

	// Node refs
	private AnimatedSprite _thrustAnimatedSprite = null!;
	private AudioStreamPlayer2D _rocketAudio = null!;
	private AudioStreamPlayer2D _impactAudio = null!;
	private AudioStreamPlayer2D _refuelAudio = null!;
	private AudioStreamPlayer2D _explosionAudio = null!;

	private AudioStreamPlayer2D _radioactiveClick = null!;
	
	private Particles2D _dustParticles = null!;
	private Sprite _shipSprite = null!;
	private MiningLaser _miningLaser = null!;

	// Vars
	private Vector2 _velocityLastFrame = new(0, 0);
	private float _health;
	private float _fuel;

	private Random _rngSource = new();

	public override void _Ready()
	{
		_thrustAnimatedSprite = GetNode<AnimatedSprite>("ThrustAnimation");
		_rocketAudio = GetNode<AudioStreamPlayer2D>("RocketAudio");
		_impactAudio = GetNode<AudioStreamPlayer2D>("ImpactAudio");
		_refuelAudio = GetNode<AudioStreamPlayer2D>("RefuelAudio");
		_explosionAudio = GetNode<AudioStreamPlayer2D>("ExplosionNode/ExplosionAudio");
		_radioactiveClick = GetNode<AudioStreamPlayer2D>("DangerFieldSounds/RadioactiveClick");
		_dustParticles = GetNode<Particles2D>("ExplosionNode/DustParticles");
		_shipSprite = GetNode<Sprite>("ShipSprite");
		_miningLaser = GetNode<MiningLaser>("MiningLaser");

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
		DangerProcess(delta);
		ConsumeFuel(delta);
		RefuelingAndRepair(delta);
		EffectsProcess();
		_miningLaser.DisableInput = DisableInput;
	}

	public void DangerProcess(float delta)
	{
		var dangerEffects = GetTree().GetNodesInGroup("danger_field")
			.Cast<DangerField>()
			.Where(field => field.OverlapsBody(this))
			.GroupBy(a => a.FieldType)
			.ToImmutableDictionary(
				g => g.Key,
				g => g.Sum(field => field.GlobalScale.x / field.GlobalPosition.DistanceTo(GlobalPosition)));
		foreach (var dangerEffect in dangerEffects)
		{
			var effect = 100 * delta * dangerEffect.Value;
			switch (dangerEffect.Key)
			{
				case DangerFieldType.Cold:
					_shipCold = Math.Min(AntiFreezeUpgrade ? 2 : 100, _shipCold + effect * CoolingSpeed);
					if (_shipCold > 50)
					{
						_health -= effect * ColdDamage;
						EmitSignal(nameof(HealthChanged), _health);
					}
					break;
				case DangerFieldType.Hot:
					_health -= TitaniumHullUpgrade
						? 0.01f * effect * HeatDamage
						: effect * HeatDamage;
					EmitSignal(nameof(HealthChanged), _health);
					break;
				case DangerFieldType.Radioactive:
					if(_radioactiveInterferenceEnd + TimeSpan.FromSeconds(4) < DateTimeOffset.Now)
						continue;

					_health -= effect * RadioactiveDamage;
					EmitSignal(nameof(HealthChanged), _health);
					_radioactiveInterferenceEnd = DateTimeOffset.Now + TimeSpan.FromSeconds(1);
					_radioactiveClick.Play();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		_shipCold = Math.Max(1f, _shipCold - HeatingSpeed * delta);

		var coldModulate = !AntiFreezeUpgrade && dangerEffects.ContainsKey(DangerFieldType.Cold) ? 25 * dangerEffects[DangerFieldType.Cold] : 1;
		var hotModulate = !TitaniumHullUpgrade && dangerEffects.ContainsKey(DangerFieldType.Hot) ? 25 * dangerEffects[DangerFieldType.Hot] : 1;
		var radioactiveModulate = !RadiationShieldUpgrade && dangerEffects.ContainsKey(DangerFieldType.Radioactive) ? 25 * dangerEffects[DangerFieldType.Radioactive] : 1;
		_shipSprite.Modulate = new Color(
			r: 1 / Math.Max(1, coldModulate * radioactiveModulate),
			g: 1 / Math.Max(1, coldModulate * hotModulate),
			b: 1 / Math.Max(1, hotModulate * radioactiveModulate));
	}

	public void EffectsProcess()
	{
		if (DisableInput)
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

	public void RefuelingAndRepair(float delta)
	{
		if (!IsOnLandingPad || (_fuel >= MaxFuel && _health >= MaxHealth) || Input.IsActionPressed("player_up"))
		{
			if (_refuelAudio.Playing)
				_refuelAudio.Stop();

			return;
		}

		if (!_refuelAudio.Playing)
			_refuelAudio.Play();

		_refuelAudio.PitchScale = (_fuel / MaxFuel) * (_health / MaxHealth) * 0.25f + 0.75f;
		_fuel += delta * REFUEL_RATE;
		_health += delta * REPAIR_RATE;

		_fuel = Math.Min(_fuel, MaxFuel);
		_health = Math.Min(_health, MaxHealth);

		EmitSignal(nameof(FuelChanged), _fuel);
		EmitSignal(nameof(HealthChanged), _health);

	}

	public void ConsumeFuel(float delta)
	{
		if (DisableInput)
			return;

		var inputStrength = Input.GetActionStrength("player_up") + 0.5f * Math.Abs(Input.GetActionStrength("player_right") - Input.GetActionStrength("player_left"));
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
		if (DisableInput || _fuel <= 0)
			return;

		var thrust = ThrustSpeed * Vector2.Up * Input.GetActionStrength("player_up");
		AppliedForce += thrust.Rotated(Rotation);
		
		var torque = Input.GetActionStrength("player_right") - Input.GetActionStrength("player_left");
		AppliedTorque += RotSpeed * torque;
	}

	private void ApplyGravity()
	{
		var gravityFields = GetTree().GetNodesInGroup("gravity_field").Cast<Area2D>().ToImmutableArray();
		var force = Physics.CalculateGravity(gravityFields, GlobalPosition);
		AppliedForce += force * Mass;
	}

	private void OnBodyShapeEnteredFeet(Node node)
	{
		if (node is LandingPlatform)
			IsOnLandingPad = true;

		ShipImpact(0.1f);
	}

	private void OnBodyShapeExitedFeet(Node node)
	{
		if (node is LandingPlatform)
			IsOnLandingPad = false;
	}

	private void OnBodyShapeEnteredHull(Node _) => ShipImpact(0.5f);

	private void ShipImpact(float damageModifier)
	{
		var impact = (float)Math.Round(_velocityLastFrame.Length() * damageModifier);
		if (impact < 10)
			return;

		_health -= impact;
		EmitSignal(nameof(HealthChanged), _health);
		if (_health <= 0)
			Explode();
		else
			PlayImpactSound();
	}

	private void PlayImpactSound()
	{
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
