using Godot;
using SpaceExplorer.Scripts.Actor;

namespace SpaceExplorer.Scripts;

public class MainScene : Node
{
	AudioStreamPlayer _musicPlayer = null!;
	Player _player = null!;
	GameUi _ui = null!;
	public override void _Ready()
	{
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		_player = GetNode<Player>("World/Player");
		_ui = GetNode<GameUi>("GameUi");

		_ui.SetMaxHealthBarValue(_player.StartHealth);
		_player.Connect(nameof(Player.HealthChanged), this, nameof(OnPlayerHealthChanged));
		_musicPlayer.Play();
	}

	public void OnPlayerHealthChanged(int health) => _ui.SetHealthBarValue(health);

}
