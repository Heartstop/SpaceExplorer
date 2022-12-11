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

	[Export] public bool DisableInput { get; set; } = true;
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
		_animationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));

		
		_musicPlayer.Play();
		_animationPlayer.Play("scene_1");
	}

	private void OnAnimationFinished(string animationName)
	{
		switch(animationName)
		{
			case "scene_1": {
				_ui.ShowMessage(
@"Oh no! It seems like you accidentally took a wrong turn and ended up in a [u][color=fuchsia]wormhole[/color][/u]!
To get home you only need to set course, spin up your [color=aqua]warp drive[/color] and you will be on your way home in no time!"
				, () => {
					_ui.ShowMessage(
@"Your ship is not equipped with a warp drive? No worries! We will provide with you tons of fuel so you can thrust yourself home in the speed of light."
					, () => _animationPlayer.Play("scene_2"), true);
				});
				break;
			}
			case "scene_2": {
				_ui.ShowMessage(
@"Oh... You would be dead by the time that you got back? Well then, you better quickly go back into the wormhole and hope it takes you back before it destabilizes and closes down."
				, () => _animationPlayer.Play("scene_3"), true);
				break;
			}
			case "scene_3": {
				_ui.ShowMessage(
@"It is already gone? I guess you will have to craft a [color=aqua]warp drive[/color] yourself. The main ingredient of a warp drive is highly radioactive [color=lime]Uranium[/color].
We should be able to find some nearby."
				, () => _ui.ShowMessage(
@"We wont be able to go any reasonable distance with your current engine. So first of all you need to mine some [color=aqua]Aluminum[/color] so we can upgrade your rocket power!.
Luckily there seems to be some on this little astroid. Go mine 5 [color=aqua]Aluminum[/color], then return to the landing pad and upgrade your rocket.", 
					() => {
						_camera.Current = true;
						DisableInput = false;
					})
				);
				break;
			}
			default: throw new NotImplementedException();
		}
	}

	private void OnCraftedUpgradesChanged(List<string> currentUpgrades) {
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
		_player.UIOpen = _ui.IsUpgradeMenuOpen || DisableInput;
		_camera.DisableInput = _ui.IsUpgradeMenuOpen || DisableInput;
		_ui.DisableInput = DisableInput;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		HandleTimeScale(@event);
	}

	private void HandleTimeScale(InputEvent @event)
	{
		if(DisableInput)
			return;
			
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
