using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Godot;
using SpaceExplorer.Scripts.Actor;

namespace SpaceExplorer.Scripts;

public class MainScene : Node
{
	AudioStreamPlayer _musicPlayer = null!;
	Player _player = null!;
	GameUi _ui = null!;
	private OptionsMenu _optionsMenu = null!;
	Camera _camera = null!;
	AnimationPlayer _animationPlayer = null!;
	private int Lifes = 5;

	[Export]
	public bool DisableInput { get; set; } = true;
	[Signal]
	public delegate void Restart();
	private int _currentProgressionStep = 0;
	private bool Dying = false;

	public override void _Ready()
	{
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		_player = GetNode<Player>("World/Player");
		_ui = GetNode<GameUi>("GameUi");
		_optionsMenu = GetNode<OptionsMenu>("GameUi/OptionsMenu");
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
		_optionsMenu.Connect(nameof(OptionsMenu.RespawnGame), this, nameof(OnRespawn));
		_ui.GetNode<Label>("Container/LifesLabel").Text = $"{Lifes} Lives";
		Connect(nameof(Restart), this, nameof(OnRestartGame));

		_musicPlayer.Play();
		_animationPlayer.Play("scene_1");
		_player.GetNode<Trajectory>("Trajectory").Render = false;
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
@"We wont be able to go any reasonable distance with your current engine. So first of all you need to mine some [color=aqua]Aluminum[/color] so we can upgrade your rocket power!
Luckily there seems to be some on this little astroid. Go mine 5 [color=aqua]Aluminum[/color], then return to the landing pad and upgrade your rocket.", 
					() => {
						_camera.Current = true;
						DisableInput = false;
						_player.GetNode<Trajectory>("Trajectory").Render = true;
					})
				);
				break;
			}
			case "end": {
				throw new NotImplementedException("Game over");
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
					_ui.ShowMessage(
@"Great job! We can sense a weak sign of [color=lime]Uranium[/color] in this system, but we cannot locate it at the moment. If we had a radio tower we could start locating the [color=lime]Uranium[/color].
We have sent the coordinates of a nearby [color=maroon]Brown planet[/color] containing [color=#e78e3f]Copper[/color] we can use to build a radio tower.", () =>
					_ui.ShowMessage(
@"[color=lime]TIP![/color] Remember you can use time warp to make your journey a lot quicker. But be careful to not accidentally crash into a planet while doing so."
					));
					
				}
				break;
			}
			case 1: {
				if(currentUpgrades.Contains("Radio"))
				{
					Progress(1);
					_currentProgressionStep += 1;
					_ui.ShowMessage(
@"Good job! Now that we have a radio tower we can start locating the uranium needed for our warp drive. Since we are going to at sometime mine radioactive material, you might aswell start crafting [color=lime]Radioactive shielding[/color].
We have sent you the coordinates of 4 different planets nearby that contain [color=#ff580f]Tungsten[/color] and [color=#2d1e39]Lead[/color] to craft the [color=lime]Radioactive shielding[/color].", () => {
	_ui.ShowMessage(
@"[color=red]Warning[/color] The [color=white]White moon[/color] will freeze you if you don't have the [color=aqua]Antifreeze[/color] upgrade, and the [color=ec9102]Orange planet[/color] will burn you if you don't have the [color=teal]Titanium hull[/color] upgrade.
So you should probably craft those upgrades before visiting those celestial bodies.
");
});
				}
				break;
			}
			case 2: {
				if(currentUpgrades.Contains("TitaniumHull") && currentUpgrades.Contains("Antifreeze"))
				{
					Progress(2);
					_currentProgressionStep += 1;
					_ui.ShowMessage(
@"Great! Now you can focus on crafting the [color=teal]Radiation shielding[/color]. We will soon of the location of the [color=lime]Uranium[/color]."
					);
				}
				break;
			}
			case 3: {
				if(currentUpgrades.Contains("RadiationShielding"))
				{
					Progress(3);
					_ui.ShowMessage(
@"Good news! We have found the location of the [color=lime]Uranium[/color]. We have sent you the location of it, go over there and gather it!"
					);
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
				_ui.ShowMessage("You have the warpdrive! Finally we can go home!", () => {
					_animationPlayer.Play("end");
				});
				break;
			}
			default: throw new NotImplementedException();
		}
	}

	public override void _Process(float delta)
	{
		_player.UIOpen = _ui.IsMenuOpen || DisableInput;
		_camera.DisableInput = _ui.IsMenuOpen || DisableInput;
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

	private void OnRestartGame()
	{
		Engine.TimeScale = 1;
		ResourceInventory.SubtractResources(ResourceInventory.ResourceCounts.ToImmutableDictionary());
		var reload = GetTree().ReloadCurrentScene();
	}
	
	private void OnRespawn()
	{
		Lifes -= 1;
		_ui.GetNode<Label>("Container/LifesLabel").Text = $"{Lifes} Lives";
		if (Lifes <= 0)
		{
			_optionsMenu.Visible = false;
			_ui.ShowMessage(
				"You unfortunately ran out of spare ships, try again?",
				() => _ui.ShowMessage("Restart of game is not implemented, reload the game instead.",
					() => throw new NotImplementedException("Restart of game is not implemented, reload the game instead.")));
			return;
		}
		Engine.TimeScale = 1;
		_player.GlobalPosition = GetNode<Node2D>("World/SpawnPoint").GlobalPosition;
		_player.GlobalRotation = 0;
		_player.ResetPhysicsInterpolation();
		_player.EmitSignal(nameof(Player.Respawned));
		Dying = false;
	}
	private void OnPlayerHealthChanged(float health)
	{
		_ui.SetHealthBarValue(health);
		if (health <= 0 && Dying == false)
		{
			Dying = true;
			var timer = GetTree().CreateTimer(3);
			timer.Connect("timeout", this, nameof(OnRespawn));
		}
	}

	private void OnPlayerFuelChanged(float fuel) => _ui.SetFuelBarValue(fuel);
	private void OnCameraZoomChanged(float zoom) => _ui.AlwaysShowIcons = zoom > 1.9f;

}
