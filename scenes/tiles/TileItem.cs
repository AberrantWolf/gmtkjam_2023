
using System.Collections.Generic;
using System.Linq;
using Godot;

public class TileItem
{
	public TileArray parent { get; private set; }
	public Vector2I Coords { get; private set; }
	public Vector2I[] StateArray { get; private set; }
	public int Health { get; private set; } = 1;
	public int MaxHealth { get; private set; }
	public bool CanBeAttacked { get; private set; }
	public TileTypes CurrentType { get; private set; } = TileTypes.Blank;
	public bool CanGrow
	{
		get; private set;
	}

	public bool canSpread;
	public bool CanSpread
	{
		get
		{
			return canSpread && Health > 0;
		}
	}


	public TileItem(
	  TileArray parent,
	  Vector2I coords,
	  Vector2I[] stateArray = null,
	  int maxHealth = 0,
	  bool canBeAttacked = false,
	  bool canGrow = false,
	  bool canSpread = false,
	  int regenRate = 0
	) {
		this.parent = parent;
		this.Coords = coords;
		this.MaxHealth = maxHealth;
		this.Health = maxHealth;
		this.CanBeAttacked = canBeAttacked;
		this.canSpread = canSpread;
		this.CanGrow = canGrow;
	}

	public void InstantiateTile()
	{
		SurroundingTiles = GetSurroundingTiles();
		CanGrow = GetCanGrow();
	}

	private List<TileItem> surroundingTiles;
	public List<TileItem> SurroundingTiles
	{
		get
		{
			if (surroundingTiles == null)
			{
				surroundingTiles = GetSurroundingTiles();
			}
			return surroundingTiles;
		}
		set
		{
			surroundingTiles = value;
		}
	}

	public void Attack(int dmg)
	{
		this.Health -= dmg;

		switch(this.CurrentType)
		{
			case TileTypes.Fields:
				if(this.Health <= 0) {
					this.ConvertTo(TileTypes.Empty);
					this.parent.TileMap.SetCellsTerrainConnect(0, new Godot.Collections.Array<Vector2I>(){ this.Coords}, 0, (int)TerrainTypes.Empty);
				}
				break;
			case TileTypes.Empty:
				break;
			case TileTypes.Farmhouse:
				this.ConvertTo(TileTypes.Empty);
				this.parent.TileMap.SetCellsTerrainConnect(0, new Godot.Collections.Array<Vector2I>(){ this.Coords}, 0, (int)TerrainTypes.Empty);
				break;
		}

		// @mika put attack logic here and change the spirite
		// when damaged to stages, show by going down the health tree
		// potentially add a regen function?
	}

	public List<TileItem> GetSurroundingTiles()
	{
		return (from x in Enumerable.Range(Coords.X - 1, 3)
				from y in Enumerable.Range(Coords.Y - 1, 3)
				where x >= 0 && y >= 0 && x < parent.Tiles.GetLength(0) && y < parent.Tiles.GetLength(1) && (x != Coords.X || y != Coords.Y)
				select parent.Tiles[x, y]).ToList();
	}

	private bool GetCanGrow()
	{
		return SurroundingTiles.Any(t => t.CanSpread) && CanGrow;
	}

	public void ConvertTo(TileTypes targetType)
	{
		switch(targetType)
		{
			case TileTypes.Blank:
			case TileTypes.Mountains:
			case TileTypes.Forest:
			case TileTypes.Water:
				this.canSpread = false;
				this.CanGrow = false;
				this.CanBeAttacked = false;
				this.Health = 1;
				this.MaxHealth = 1;
				break;
			case TileTypes.Empty:
				this.canSpread = false;
				this.CanGrow = true;
				this.CanBeAttacked = true;
				this.Health = 100;
				this.MaxHealth = 100;
				break;
			case TileTypes.Fields:
				this.canSpread = true;
				this.CanGrow = false;
				this.CanBeAttacked = true;
				this.Health = 10;
				this.MaxHealth = 10;
				break;
			case TileTypes.Farmhouse:
				this.canSpread = false;
				this.CanGrow = false;
				this.CanBeAttacked = true;
				this.Health = 100;
				this.MaxHealth = 100;
				break;
			default:
				throw new System.Exception("Unhandled Tile");
		}
		this.CurrentType = targetType;
	}
}

