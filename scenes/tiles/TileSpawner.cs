using Godot;

public class TileSpawner
{
	public static TileItem CreateGrassTile(
		TileArray parent,
		Vector2I coords
	)
	{
		return new TileItem(
			parent,
			coords,
			0,
			5,
			true,
			true,
			true,
			100
		);
	}
}