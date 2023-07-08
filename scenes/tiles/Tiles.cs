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
	private int mapWidth = 20;

	[Export]
	private int mapHeight = 20;

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
		//AddFarmhouses();
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
				if(noiseVal > mountainThreshold)
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
				if(noiseVal > forestThreshold && currentCell != Mountains)
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
				if(noiseVal > waterThreshold && currentCell != Mountains)
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
				if(noiseVal > fieldThreshold && currentCell != Mountains && currentCell != Forest && currentCell != Water)
				{
					if(noiseVal > farmhouseThreshold && !this.GetSurroundingCells(new Vector2I(x, y)).Any(c => this.GetCellAtlasCoords(0, c) == Farmhouse))
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TileMap me = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var coords = new Vector2I(new Random().Next(0,mapWidth), new Random().Next(0,mapHeight));
		if(this.GetCellAtlasCoords(0, coords) == Empty && this.GetSurroundingCells(coords).Any(c => this.GetCellAtlasCoords(0, c) == Fields)) 
		{
			this.SetCell(0, coords, sourceId: 0, atlasCoords: Fields);
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
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Tiles[x, y] = new TileItem(this);
			}
		}
	}
	public class TileItem
	{
		public TileItem(TileArray thisParent)
		{
			parent = thisParent;	
		}
		private TileArray parent;
		public Vector2I Coords { get; set; }
		public TileTypes Type { get; set; }

		public bool CanGrow
		{
			get
			{
				var surroundingTiles = (from x in Enumerable.Range(Coords.X - 1, Coords.X + 1)
										from y in Enumerable.Range(Coords.Y - 1, Coords.Y + 1)
										where x >= 0 && y >= 0 && (x != Coords.X | y != Coords.Y) 
										select parent.Tiles[x,y]).ToList();

				if(surroundingTiles.Any(t => t.Type == TileTypes.Fields) && Type == TileTypes.Empty)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}

