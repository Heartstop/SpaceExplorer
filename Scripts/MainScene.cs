using Godot;
using SpaceExplorer.Scripts.Actor;

namespace SpaceExplorer.Scripts;

public class MainScene : Node
{
	AudioStreamPlayer _musicPlayer = null!;
	Player _player = null!;
	GameUi _ui = null!;
	Camera _camera = null!;

	public override void _Ready()
	{
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		_player = GetNode<Player>("World/Player");
		_ui = GetNode<GameUi>("GameUi");
		_camera = GetNode<Camera>("World/Player/PlayerCamera");

		_ui.SetMaxHealthBarValue(_player.MaxHealth);
		_ui.SetHealthBarValue(_player.MaxHealth);
		_ui.SetMaxFuelBarValue(_player.MaxFuel);
		_ui.SetFuelBarValue(_player.MaxFuel);

		_player.Connect(nameof(Player.HealthChanged), this, nameof(OnPlayerHealthChanged));
		_player.Connect(nameof(Player.FuelChanged), this, nameof(OnPlayerFuelChanged));
		_camera.Connect(nameof(Camera.ZoomChanged), this, nameof(OnCameraZoomChanged));

		_musicPlayer.Play();
	}

	private void OnPlayerHealthChanged(int health) => _ui.SetHealthBarValue(health);
	private void OnPlayerFuelChanged(int fuel) => _ui.SetFuelBarValue(fuel);
	private void OnCameraZoomChanged(float zoom) => _ui.ShowIcons = zoom > 4.5f;

}
