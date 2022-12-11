using Godot;

public enum DangerFieldType
{
	Cold,
	Hot,
	Radioactive
}

public class DangerField : Area2D
{
	[Export]
	public DangerFieldType FieldType;
}
