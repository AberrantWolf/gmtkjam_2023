using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Tiles : TileMap
{

	[Export]
	private PackedScene scareCrowScene = ResourceLoader.Load<PackedScene>("res://scenes/Enemies/ScareCrow.tscn");
	
	[Export]
	private PackedScene farmerScene = ResourceLoader.Load<PackedScene>("res://scenes/Enemies/Farmer.tscn");

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

	public List<TileMapPattern> MountainPatterns;

	public TileArray TilesArray { get; set; }
	public override void _EnterTree()
	{
		base._EnterTree();

		TilesArray = new TileArray(mapWidth, mapHeight);
		MountainPatterns = Enumerable.Range(0, this.TileSet.GetPatternsCount()).Select(i => this.TileSet.GetPattern(i)).ToList();

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

		var toPaintEmpty = new Godot.Collections.Array<Vector2I>();

		for (int x = 0; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				var noiseVal = noise.GetNoise2D(x, y);
				var currentCell = TilesArray.Tiles[x, y];
				if(currentCell.CurrentType == TileTypes.Blank && noiseVal > mountainThreshold && x < mapWidth - 3 && y < mapHeight - 3)
				{
					this.SetPattern(0, new Vector2I(x, y), MountainPatterns[new Random().Next(0, MountainPatterns.Count - 1)]);
					currentCell.ConvertTo(TileTypes.Mountains);	
					TilesArray.Tiles[x + 1, y].ConvertTo(TileTypes.Mountains);	
					TilesArray.Tiles[x, y + 1].ConvertTo(TileTypes.Mountains);	
					TilesArray.Tiles[x + 1, y + 1].ConvertTo(TileTypes.Mountains);	
				}
			}
		}

		this.SetCellsTerrainConnect(0, toPaintEmpty, 0, (int)TerrainTypes.Empty);
	}

	private void AddForest()
	{
		var noise = GetNoise(forestFrequency, forestAmplitude);

		var toPaintForest = new Godot.Collections.Array<Vector2I>();
		for (int x = 0; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				var noiseVal = noise.GetNoise2D(x, y);
				var currentCell = TilesArray.Tiles[x, y];
				if (currentCell.CurrentType == TileTypes.Blank && noiseVal > forestThreshold)
				{
					toPaintForest.Add(new Vector2I(x, y));
					currentCell.ConvertTo(TileTypes.Forest);
				}
			}
		}

		this.SetCellsTerrainConnect(0, toPaintForest, 0, (int)TerrainTypes.Forest);
	}

	private void AddWater()
	{
		var noise = GetNoise(waterFrequency, waterAmplitude);
		var toPaintWater = new Godot.Collections.Array<Vector2I>();
		for (int x = 0; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				var noiseVal = noise.GetNoise2D(x, y);
				var currentCell = TilesArray.Tiles[x, y];
				if (currentCell.CurrentType == TileTypes.Blank && noiseVal > waterThreshold)
				{
					toPaintWater.Add(new Vector2I(x, y));
					currentCell.ConvertTo(TileTypes.Water);
				}
			}
		}
		this.SetCellsTerrainConnect(0, toPaintWater, 0, (int)TerrainTypes.Water);
	}

	private void AddFields()
	{
		var noise = GetNoise(fieldFrequency, fieldAmplitude);
		var toPaintFields = new Godot.Collections.Array<Vector2I>();
		var toPaintFarmhouse = new Godot.Collections.Array<Vector2I>();
		for (int x = 0; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				var noiseVal = noise.GetNoise2D(x, y);
				var currentCell = TilesArray.Tiles[x, y];
				if (currentCell.CurrentType == TileTypes.Blank && noiseVal > fieldThreshold)
				{
					if (noiseVal > farmhouseThreshold && !currentCell.SurroundingTiles.Any(t => t.CurrentType == TileTypes.Farmhouse))
					{
						toPaintFarmhouse.Add(new Vector2I(x, y));
						currentCell.ConvertTo(TileTypes.Farmhouse);
						if(Helpers.SpawnChance(0.1)) {
							var mob = farmerScene.Instantiate() as Farmer;
							mob.GlobalPosition = new Vector2(x*16,y*16);
							AddChild(mob);
						}
					}
					else
					{
						toPaintFields.Add(new Vector2I(x, y));
						currentCell.ConvertTo(TileTypes.Fields);
						if(Helpers.SpawnChance(0.01)) {
							var mob = scareCrowScene.Instantiate() as ScareCrow;
							mob.GlobalPosition = new Vector2(x*16,y*16);
							AddChild(mob);
						}
					}
					
				}
			}
		}
		this.SetCellsTerrainConnect(0, toPaintFarmhouse, 0, (int)TerrainTypes.Farmhouse);
		this.SetCellsTerrainConnect(0, toPaintFields, 0, (int)TerrainTypes.Field);
	}

	public bool runSim { get; set; } = false;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!runSim) return;
		var growableCells = TilesArray.GrowableCells;

		for (int i = 8; i > 0; i--)
		{
			var priorityCells = growableCells.Where(t => t.SurroundingTiles.Where(m => m.CanSpread).Count() == i).ToList();
			if (priorityCells.Count > 0)
			{
				var random = new Random().Next(0, priorityCells.Count);
				var randomCell = priorityCells[random];
				randomCell.ConvertTo(TileTypes.Fields);
				i = 0;
			}
		}
	}

	public TileItem GetTile(Vector2 global)
	{
		Vector2I coords = LocalToMap(ToLocal(global));
		return TilesArray.Tiles[coords.X, coords.Y];
	}

	// @mika work on this thing too
	public void SetTiles(Vector2I topLeft, Vector2I botRight, TileTypes type)
	{
		List<Vector2I> tileList = new List<Vector2I>();

		if (type == TileTypes.Mountains)
		{
			throw new NotImplementedException("Cant Square Select mountains in...");
		}

		var toPaintEmpty = Enumerable.Range(topLeft.X, botRight.X).Select(x=> {
			return Enumerable.Range(topLeft.Y, botRight.Y).Select(y=> {

				TilesArray.Tiles[x,y].ConvertTo(type);

				return new Vector2I(x,y);
			}).ToList();
		}).ToList();

		var arr = new Godot.Collections.Array<Vector2I>(toPaintEmpty.SelectMany(x=>x).AsEnumerable());

		SetCellsTerrainConnect(0, arr, 0, (int)TerrainTypes.Empty);
	}
}

