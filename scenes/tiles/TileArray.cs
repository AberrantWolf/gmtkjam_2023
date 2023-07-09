
using System.Collections.Generic;
using System.Linq;
using Godot;

public class TileArray
{
	public TileItem[,] Tiles { get; set; }

	public Tiles TileMap;

	public TileArray(int width, int height, Tiles tileMap)
	{
		this.TileMap = tileMap;
		Tiles = new TileItem[width, height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Tiles[x, y] = new TileItem(this, new Vector2I(x, y));
			}
		}
	}



	private List<TileItem> growableCells;
	public List<TileItem> GrowableCells
	{
		get
		{
			return growableCells;
		}
		set
		{
			growableCells = value;
		}
	}

	public void AddToGrowableCells(IEnumerable<TileItem> tiles)
	{
		growableCells.AddRange(tiles);
	}

	public void AddToGrowableCells(TileItem tile)
	{
		growableCells.Add(tile);
	}

	public void RemoveFromGrowableCells(IEnumerable<TileItem> tiles)
	{
		growableCells.RemoveAll(t => tiles.Contains(t));
	}

	public void RemoveFromGrowableCells(TileItem tile)
	{
		growableCells.Remove(tile);
	}

	public void InstantiateTiles()
	{
		foreach (var tile in Tiles)
		{
			tile.InstantiateTile();
		}

		GrowableCells = Tiles.Cast<TileItem>().Where(t => t.CanGrow).ToList();
	}
}
