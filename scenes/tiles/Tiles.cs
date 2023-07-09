using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Tiles : TileMap
{
  [Export]
  private float mountainFrequency = 0.02f;

  [Export]
  private float mountainAmplitude = 1f;

  [Export]
  private float mountainThreshold = 0.3f;

  [Export]
  private float forestFrequency = 0.02f;

  [Export]
  private float forestAmplitude = 1f;

  [Export]
  private float forestThreshold = 0.2f;

  [Export]
  private float waterFrequency = 0.02f;

  [Export]
  private float waterAmplitude = 1f;

  [Export]
  private float waterThreshold = 0.35f;

  [Export]
  private float fieldFrequency = 0.1f;

  [Export]
  private float fieldAmplitude = 1f;

  [Export]
  private float fieldThreshold = 0.2f;

  [Export]
  private float farmhouseFrequency = 0.01f;

  [Export]
  private float farmhouseAmplitude = 1f;

  [Export]
  private float farmhouseThreshold = 0.42f;

  [Export]
  private int mapWidth = 160;

  [Export]
  private int mapHeight = 90;

  [Export]
  public Vector2I Empty = new Vector2I(0, 0);
  [Export]
  public Vector2I Mountains = new Vector2I(2, 1);
  [Export]
  public Vector2I Forest = new Vector2I(0, 1);
  [Export]
  public Vector2I Water = new Vector2I(1, 1);
  [Export]
  public Vector2I Fields = new Vector2I(1, 0);
  [Export]
  public Vector2I Farmhouse = new Vector2I(2, 0);

  public TileArray TilesArray { get; set; }
  public override void _EnterTree()
  {
	base._EnterTree();

	TilesArray = new TileArray(mapWidth, mapHeight);

	AddMountains();
	AddForest();
	AddWater();
	AddFields();
	TilesArray.InstantiateTiles();
  }

  private FastNoiseLite GetNoise(float frequency, float amplitude)
  {
	var noise = new FastNoiseLite();
	noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
	noise.Frequency = frequency;
	noise.DomainWarpAmplitude = amplitude;
	noise.Seed = new Random().Next();
	return noise;
  }

  private void AddMountains()
  {
	var noise = GetNoise(mountainFrequency, mountainAmplitude);

	for (int x = 0; x < mapWidth; x++)
	{
	  for (int y = 0; y < mapHeight; y++)
	  {
		var noiseVal = noise.GetNoise2D(x, y);
		if (noiseVal > mountainThreshold)
		{
		  this.SetCell(0, new Vector2I(x, y), sourceId: 0, atlasCoords: Mountains);
		  TilesArray.Tiles[x, y].Type = TileTypes.Mountains;
		  TilesArray.Tiles[x, y].Coords = new Vector2I(x, y);
		}
		else
		{
		  this.SetCell(0, new Vector2I(x, y), sourceId: 0, atlasCoords: Empty);
		  TilesArray.Tiles[x, y].Type = TileTypes.Empty;
		  TilesArray.Tiles[x, y].Coords = new Vector2I(x, y);
		}
	  }
	}
  }

  private void AddForest()
  {
	var noise = GetNoise(forestFrequency, forestAmplitude);

	for (int x = 0; x < mapWidth; x++)
	{
	  for (int y = 0; y < mapHeight; y++)
	  {
		var noiseVal = noise.GetNoise2D(x, y);
		var currentCell = this.GetCellAtlasCoords(0, new Vector2I(x, y));
		if (noiseVal > forestThreshold && currentCell != Mountains)
		{
		  this.SetCell(0, new Vector2I(x, y), sourceId: 0, atlasCoords: Forest);
		  TilesArray.Tiles[x, y].Type = TileTypes.Forest;
		  TilesArray.Tiles[x, y].Coords = new Vector2I(x, y);
		}
	  }
	}
  }

  private void AddWater()
  {
	var noise = GetNoise(waterFrequency, waterAmplitude);

	for (int x = 0; x < mapWidth; x++)
	{
	  for (int y = 0; y < mapHeight; y++)
	  {
		var noiseVal = noise.GetNoise2D(x, y);
		var currentCell = this.GetCellAtlasCoords(0, new Vector2I(x, y));
		if (noiseVal > waterThreshold && currentCell != Mountains)
		{
		  this.SetCell(0, new Vector2I(x, y), sourceId: 0, atlasCoords: Water);
		  TilesArray.Tiles[x, y].Type = TileTypes.Water;
		  TilesArray.Tiles[x, y].Coords = new Vector2I(x, y);
		}
	  }
	}
  }

  private void AddFields()
  {
	var noise = GetNoise(fieldFrequency, fieldAmplitude);

	for (int x = 0; x < mapWidth; x++)
	{
	  for (int y = 0; y < mapHeight; y++)
	  {
		var noiseVal = noise.GetNoise2D(x, y);
		var currentCell = this.GetCellAtlasCoords(0, new Vector2I(x, y));
		if (noiseVal > fieldThreshold && currentCell != Mountains && currentCell != Forest && currentCell != Water)
		{
		  if (noiseVal > farmhouseThreshold && !this.GetSurroundingCells(new Vector2I(x, y)).Any(c => this.GetCellAtlasCoords(0, c) == Farmhouse))
		  {
			this.SetCell(0, new Vector2I(x, y), sourceId: 0, atlasCoords: Farmhouse);
			TilesArray.Tiles[x, y].Type = TileTypes.Farmhouse;
			TilesArray.Tiles[x, y].Coords = new Vector2I(x, y);
		  }
		  else
		  {
			this.SetCell(0, new Vector2I(x, y), sourceId: 0, atlasCoords: Fields);
			TilesArray.Tiles[x, y].Type = TileTypes.Fields;
			TilesArray.Tiles[x, y].Coords = new Vector2I(x, y);
		  }
		}
	  }
	}
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
	var growableCells = TilesArray.GrowableCells;

	for (int i = 8; i > 0; i--)
	{
	  var priorityCells = growableCells.Where(t => t.SurroundingTiles.Where(m => m.Type == TileTypes.Fields).Count() == i).ToList();
	  if (priorityCells.Count > 0)
	  {
		try
		{
		  var random = new Random().Next(0, priorityCells.Count);
		  var randomCell = priorityCells[random];
		  this.SetCell(0, randomCell.Coords, sourceId: 0, atlasCoords: Fields);
		  TilesArray.Tiles[randomCell.Coords.X, randomCell.Coords.Y].Type = TileTypes.Fields;
		  TilesArray.Tiles[randomCell.Coords.X, randomCell.Coords.Y].InstantiateTile();
		  TilesArray.RemoveFromGrowableCells(randomCell);
		  TilesArray.AddToGrowableCells(randomCell.SurroundingTiles.Where(t => t.Type == TileTypes.Empty));
		  i = 0;
		}
		catch (Exception ex)
		{
		  GD.PrintErr(ex.Message);
		}
	  }
	}

  }
}


public enum TileTypes
{
  Empty,
  Mountains,
  Forest,
  Water,
  Fields,
  Farmhouse
}


public class TileArray
{
  public TileItem[,] Tiles { get; set; }

  public TileArray(int width, int height)
  {
	Tiles = new TileItem[width, height];
	for (int x = 0; x < width; x++)
	{
	  for (int y = 0; y < height; y++)
	  {
		Tiles[x, y] = new TileItem(this);
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
  public class TileItem
  {
	public TileItem(TileArray thisParent)
	{
	  parent = thisParent;
	}

	public void InstantiateTile()
	{
	  SurroundingTiles = (from x in Enumerable.Range(Coords.X - 1, 3)
						  from y in Enumerable.Range(Coords.Y - 1, 3)
						  where x >= 0 && y >= 0 && x < parent.Tiles.GetLength(0) && y < parent.Tiles.GetLength(1) && (x != Coords.X || y != Coords.Y)
						  select parent.Tiles[x, y]).ToList();

	  if (SurroundingTiles.Any(t => t.Type == TileTypes.Fields) && Type == TileTypes.Empty)
	  {
		CanGrow = true;
	  }
	  else
	  {
		CanGrow = false;
	  }
	}
	private TileArray parent;
	public Vector2I Coords { get; set; }
	public TileTypes Type { get; set; }

	public int Tier { get; set; }

	private List<TileItem> surroundingTiles;

	public List<TileItem> SurroundingTiles
	{
	  get
	  {
		return surroundingTiles;
	  }
	  set
	  {
		surroundingTiles = value;
	  }
	}
	private bool canGrow;
	public bool CanGrow
	{
	  get
	  {
		return canGrow;
	  }
	  set
	  {
		canGrow = value;
	  }
	}
  }
}

