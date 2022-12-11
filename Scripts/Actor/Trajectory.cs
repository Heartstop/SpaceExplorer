using System;
using System.Collections.Immutable;
using System.Linq;
using Godot;

namespace SpaceExplorer.Scripts.Actor;

public class Trajectory : Node2D
{
	[Export]
	public int Points = 10;

	[Export]
	public Color Color = Colors.Gold;

	[Export]
	public float Width = 5f;

	[Export]
	public float StepSize = 10;

	[Export]
	public bool Render { get; set; } = true;
	
	[Export]
	public NodePath TrajectoryTarget { get; set; }

	private int _updatesPerFrame = 100;
	private Vector2[] _pathPoints = { };
	private Vector2 _lastVelocity;
	private int _pointIndex;
	// Reused to optimize performance
	private Vector2[] _renderPoints = { };

	public override void _PhysicsProcess(float delta)
	{
		var gravityFields = GetTree().GetNodesInGroup("gravity_field").Cast<Area2D>().ToImmutableArray();

		// Thread safety
		var p = Points;
		if (_pathPoints.Length != p)
		{
			_pointIndex = 0;
			_pathPoints = new Vector2[p];
		}

		if (_pointIndex == 0)
		{
			_pathPoints[0] = GlobalPosition;
			_lastVelocity = GetNode<RigidBody2D>(TrajectoryTarget).LinearVelocity;
			_pointIndex++;
		}

		var lastGlobalPosition = _pathPoints[_pointIndex - 1];
		var end = Math.Min(_pathPoints.Length, _pointIndex + _updatesPerFrame);
		for (; _pointIndex < end; _pointIndex++)
		{
			var acceleration = Physics.CalculateGravity(gravityFields, lastGlobalPosition);
			var timeStep = Mathf.Min(StepSize / acceleration.Length(), 1f);
			_lastVelocity += timeStep * acceleration;
			lastGlobalPosition += timeStep * _lastVelocity;
			_pathPoints[_pointIndex] = lastGlobalPosition;
		}

		if (_pointIndex == _pathPoints.Length)
			_pointIndex = 0;

		var physicsPerformance = Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess) * Engine.IterationsPerSecond;
		if (physicsPerformance > 1)
		{
			_updatesPerFrame = Math.Max(1, _updatesPerFrame - 1);
		}
		else if (physicsPerformance < 0.9)
		{
			_updatesPerFrame = Math.Min(p, _updatesPerFrame + 1);
		}
		Update();
	}

	public override void _Draw()
	{
		if(!Render)
			return;

		if (_renderPoints.Length != _pathPoints.Length)
			_renderPoints = new Vector2[_pathPoints.Length];

		// First pos should always be Vector2.Zero
		for (var i = 1; i < _renderPoints.Length; i++)
		{
			_renderPoints[i] = ToLocal(_pathPoints[i]);
		}
		DrawPolyline(
			points: _renderPoints,
			color: Color,
			width: Width,
			antialiased: true);
	}
}
