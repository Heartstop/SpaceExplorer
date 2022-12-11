using System.Linq;
using Godot;

public class MiningLaser : Node2D
{
	[Export]
	public float Range
	{
		get => _range;
		set
		{
			_detectionArea.Scale = new Vector2(Range, Range);
			_range = value;
		}
	}

	public bool DisableInput { get; set; }

	private RayCast2D _rayCast = null!;
	private Particles2D _hitParticles = null!;
	private Line2D _laser = null!;
	private Area2D _detectionArea = null!;
	private AudioStreamPlayer2D _miningSound = null!;
	private float _range = 200;

	public override void _Ready()
	{
		_rayCast = GetNode<RayCast2D>("RayCast");
		_hitParticles = GetNode<Particles2D>("HitParticles");
		_laser = GetNode<Line2D>("Laser");
		_detectionArea = GetNode<Area2D>("DetectionArea");
		_miningSound = GetNode<AudioStreamPlayer2D>("MiningSound");
	}

	public override void _PhysicsProcess(float delta)
	{
		if(DisableInput)
			return;

		if (!Input.IsActionPressed("mine"))
		{
			_miningSound.Stop();
			SetBeamEnabled(false);
			return;
		}
		
		var target = _detectionArea.GetOverlappingBodies()
			.OfType<Node2D>()
			.Where(body => body.IsInGroup("Minable"))
			.OrderBy(minable => minable.GlobalPosition.DistanceSquaredTo(GlobalPosition))
			.FirstOrDefault();
		if (target == null)
		{
			_miningSound.Stop();
			SetBeamEnabled(false);
			return;
		}

		if(!_miningSound.Playing)
			_miningSound.Play();

		SetBeamEnabled(true);
		_rayCast.CastTo = _rayCast.ToLocal(target.GlobalPosition);
		_rayCast.ForceRaycastUpdate();
		var laserEndGlobal = _rayCast.GetCollisionPoint();
		_laser.Points = new[] { Vector2.Zero, _laser.ToLocal(laserEndGlobal) };
		_hitParticles.GlobalPosition = laserEndGlobal;
		_hitParticles.Rotation = _laser.ToLocal(laserEndGlobal).Angle() + 0.5f * Mathf.Pi;

		var collidingWith = _rayCast.GetCollider();
		if (ReferenceEquals(target, collidingWith))
		{
			target.EmitSignal("Mined", delta);
		}
	}

	private void SetBeamEnabled(bool enabled)
	{
		_laser.Visible = enabled;
		_hitParticles.Emitting = enabled;
	}
}
