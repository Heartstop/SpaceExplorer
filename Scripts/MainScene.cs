using System.Runtime.InteropServices;
using Godot;

namespace SpaceExplorer.Scripts;

public class MainScene : Node
{
	AudioStreamPlayer _musicPlayer = null!;
	public override void _Ready()
	{
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		_musicPlayer.Play();
	}

}
