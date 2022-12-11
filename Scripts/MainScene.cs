using System;
using System.Collections.Generic;
using Godot;
using SpaceExplorer.Scripts.Actor;

namespace SpaceExplorer.Scripts;

public class MainScene : Node
{
	AudioStreamPlayer _musicPlayer = null!;
	Player _player = null!;
	GameUi _ui = null!;
	Camera _camera = null!;
	AnimationPlayer _animationPlayer = null!;

	[Export] public bool DisableInput { get; set; } = false;
	private int _currentProgressionStep = 0;

	public override void _Ready()
	{
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		_player = GetNode<Player>("World/Player");
		_ui = GetNode<GameUi>("GameUi");
		_camera = GetNode<Camera>("World/Player/PlayerCamera");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		_ui.SetMaxHealthBarValue(_player.MaxHealth);
		_ui.SetHealthBarValue(_player.MaxHealth);
		_ui.SetMaxFuelBarValue(_player.MaxFuel);
		_ui.SetFuelBarValue(_player.MaxFuel);

		GetNode<UpgradeMenu>("GameUi/UpgradeMenuContainer/UpgradeMenu")
			.Connect(nameof(UpgradeMenu.CraftedUpgradesChanged), this, nameof(OnCraftedUpgradesChanged));
		_player.Connect(nameof(Player.HealthChanged), this, nameof(OnPlayerHealthChanged));
		_player.Connect(nameof(Player.FuelChanged), this, nameof(OnPlayerFuelChanged));
		_camera.Connect(nameof(Camera.ZoomChanged), this, nameof(OnCameraZoomChanged));
		
		_musicPlayer.Play();
		_animationPlayer.Play("scene_1");
	}

	private void OnCraftedUpgradesChanged(List<string> currentUpgrades)
	{
		if (currentUpgrades.Contains("Antifreeze"))
			_player.AntiFreezeUpgrade = true;

		if (currentUpgrades.Contains("TitaniumHull"))
			_player.TitaniumHullUpgrade = true;

		if (currentUpgrades.Contains("RadiationShielding"))
			_player.RadiationShieldUpgrade = true;
		
		switch(_currentProgressionStep)
		{
			case 0: {
				if(currentUpgrades.Contains("RocketPower"))
				{
					Progress(0);
					_currentProgressionStep += 1;
				}
				break;
			}
			case 1: {
				if(currentUpgrades.Contains("Radio"))
				{
					Progress(1);
					_currentProgressionStep += 1;
				}
				break;
			}
			case 2: {
				if(currentUpgrades.Contains("TitaniumHull") && currentUpgrades.Contains("Antifreeze"))
				{
					Progress(2);
					_currentProgressionStep += 1;
				}
				break;
			}
			case 3: {
				if(currentUpgrades.Contains("RadiationShielding"))
				{
					Progress(3);
					_currentProgressionStep += 1;
				}
				break;
			}
			case 4: {
				if(currentUpgrades.Contains("WarpDrive"))
				{
					Progress(4);
					_currentProgressionStep += 1;
				}
				break;
			}
		}
	}

	private void Progress(int step) {
		switch(step) {
			case 0: {
				GetNode<ZoomedOutIcon>("World/P1/ZoomedOutIcon").AlwaysHidden = false;
				_camera.MaxZoom = 3.35f;
				break;
			}
			case 1: {
				GetNode<ZoomedOutIcon>("World/M1/ZoomedOutIcon").AlwaysHidden = false;
				GetNode<ZoomedOutIcon>("World/M2/ZoomedOutIcon").AlwaysHidden = false;
				GetNode<ZoomedOutIcon>("World/P2/ZoomedOutIcon").AlwaysHidden = false;
				GetNode<ZoomedOutIcon>("World/P3/ZoomedOutIcon").AlwaysHidden = false;
				_camera.MaxZoom = 5.5f;
				break;
			}
			case 2: {
				break;
			}
			case 3: {
				GetNode<ZoomedOutIcon>("World/P4/ZoomedOutIcon").AlwaysHidden = false;
				break;
			}
			case 4: {
				// TODO: Game over.
				break;
			}			
			default: throw new NotImplementedException();
		}
	}

	public override void _Process(float delta)
	{
		_player.UIOpen = _ui.IsUpgradeMenuOpen;
		_player.DisableInput = _ui.IsUpgradeMenuOpen || DisableInput;
		_camera.DisableInput = _ui.IsUpgradeMenuOpen || DisableInput;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		HandleTimeScale(@event);
	}

	private void HandleTimeScale(InputEvent @event)
	{
		if (@event.IsActionReleased("time_scale_swap"))
		{
			Engine.TimeScale = Math.Abs(Engine.TimeScale - 1) < 0.1 ? 5 : 1;
		}
		else if (@event.IsActionPressed("time_scale_up", true))
		{
			Engine.TimeScale = Math.Min(Engine.TimeScale + 0.1f, 10f);
		}
		else if (@event.IsActionPressed("time_scale_down", true))
		{
			Engine.TimeScale = Math.Max(Engine.TimeScale - 0.1f, 1f);
		}
		else
		{
			return;
		}
		_ui.EmitSignal("TimeScaleChanged");
	}

	private void OnPlayerHealthChanged(int health) => _ui.SetHealthBarValue(health);
	private void OnPlayerFuelChanged(int fuel) => _ui.SetFuelBarValue(fuel);
	private void OnCameraZoomChanged(float zoom) => _ui.AlwaysShowIcons = zoom > 1.9f;

}
