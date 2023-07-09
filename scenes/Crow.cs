using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Crow : Area2D
{
    [Export]
    private string groupName = "crows";

    [Export]
    public int Speed = 100;

    [Export]
    public int Separation = 100;

    [Export]
    public int TileSize = 256;

    [Export]
    public int NumRows = 4;

    [Export]
    public int NumCols = 4;

    [Export]
    public Sprite2D CrowSprite;

    [Export]
    public GpuParticles2D[] StartParticles;

    [Export]
    public float MinRangeForCloseness = 100f;

    [Export]
    public float MaxRangeForCloseness = 1000f;

    private Vector2 lastDirection { get; set; } = Vector2.Right;

    private float TurningSpeed = 0.2f;
    private int FramesOfAddedWeight = 0;
    private int CurrentFramesOfAddedWeight = 0;

    private float SeekMouseFactor = 0.01f;

    private float FlockComfortFactor = 0.005f;

    private float RangeForCloseness = 200f;
    private float Conformity = 0.05f;

    private float NeedSpace = 0.05f;
    private float SocialBubbleRadius = 10f;

    private float MinSpeed = 3f;
    private float MaxSpeed = 10f;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
        {
            CurrentFramesOfAddedWeight = this.FramesOfAddedWeight;
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        CrowHiveMind.Instance.AllCrows.Remove(this);
    }

    public override void _Ready()
    {
        base._Ready();

        // Let the hive mind know of our existence
        CrowHiveMind.Instance.AllCrows.Add(this);

        // Set initial behavior values
        var rng = new RandomNumberGenerator();
        this.lastDirection = new Vector2(rng.Randf(), rng.Randf()).Normalized();
        this.MonsterBase = rng.RandfRange((float)0.2, (float)1.1);
        this.FramesOfAddedWeight = rng.RandiRange(50, 250);
        this.TurningSpeed = rng.RandfRange(0.05f, 0.1f);
        this.RangeForCloseness = rng.RandfRange(MinRangeForCloseness, MaxRangeForCloseness);

        // Choose a random sprite to be
        int totalCrowSprites = NumRows * NumCols;
        int mySpriteIndex = (int)(rng.Randi() % totalCrowSprites);
        int row = mySpriteIndex / NumCols;
        int col = mySpriteIndex % NumRows;

        CrowSprite.RegionRect = new Rect2(col * TileSize, row * TileSize, TileSize, TileSize);

        // Play the startup particle effects
        foreach (var emitter in StartParticles)
        {
            emitter.Emitting = true;
        }
    }

    private Vector2 _FollowMouse()
    {
        Vector2 mouseLocation = CrowHiveMind.Instance.MouseLocation;
        Vector2 toMouse = mouseLocation - this.Position;

        // Override mouse goal if ordered to attack somewhere
        if (CurrentFramesOfAddedWeight > 0)
        {
            CurrentFramesOfAddedWeight--;
            toMouse = (CrowHiveMind.Instance.FocusPoint - this.Position).Normalized() * 100;
        }

        return toMouse * SeekMouseFactor;
    }

    private Vector2 _BeSocial()
    {
        var allCrows = CrowHiveMind.Instance.AllCrows;
        Vector2 centerOfMass = CrowHiveMind.Instance.CenterOfMass;

        return (centerOfMass - Position) * FlockComfortFactor;
    }

    private Vector2 _ProtectSocialBounds()
    {
        var allCrows = CrowHiveMind.Instance.AllCrows;
        var squaredComfortRadius = SocialBubbleRadius * SocialBubbleRadius;

        return allCrows.Where(crow => crow != this && crow.Position.DistanceSquaredTo(Position) < squaredComfortRadius).Select(crow => Position - crow.Position).Sum() * NeedSpace;
    }

    private Vector2 _MatchNearbyVelocity()
    {
        var allCrows = CrowHiveMind.Instance.AllCrows;
        var squaredNearbyRadius = RangeForCloseness * RangeForCloseness;

        return allCrows.Where(crow => crow != this && crow.Position.DistanceSquaredTo(Position) < squaredNearbyRadius).Select(crow => crow.lastDirection).Sum() * Conformity;
    }

    private Vector2 _LimitSpeed(Vector2 delta)
    {
        float speed = delta.Length();
        if (speed > MaxSpeed)
        {
            return delta.Normalized() * MaxSpeed;
        }

        if (speed < MinSpeed)
        {
            return delta.Normalized() * MinSpeed;
        }

        return delta;
    }

    public override void _Process(double delta)
    {
        var targetDirection = (
            _FollowMouse()
            + _BeSocial()
            + _ProtectSocialBounds()
            + _MatchNearbyVelocity()
        );

        // TODO: Let them turn faster when they're slower
        Vector2 nextDirection = _LimitSpeed(targetDirection);
        float nextSpeed = nextDirection.Length();

        this.lastDirection = this.lastDirection.Normalized().Slerp(nextDirection.Normalized(), 0.3f) * nextSpeed;
        this.Position += lastDirection;
        this.Rotation = lastDirection.Angle();

        MonsterTimer -= delta;
        if (MonsterTimer < 0)
        {
            MonsterTimer = MonsterBase;
            Monster();
        }
    }

    private double MonsterBase = 10;
    private double MonsterTimer = 10;

    private void Monster()
    {
        try
        {
            var tiles = GetParent().GetNode<Tiles>("TileMap");
            var x = (int)this.Position.X / 16;
            var y = (int)this.Position.Y / 16;

            var tile = tiles.TilesArray.Tiles[x, y];

            if (tile.Type == TileTypes.Fields)
            {
                tiles.TilesArray.Tiles[x, y].Type = TileTypes.Empty;
                tiles.SetCell(0, new Vector2I(x, y), 0, atlasCoords: tiles.Empty);
                //GetParent<World>().AddAdditonalCrow();
            }

        }
        catch { }
        Console.WriteLine("Field Monstered");
    }

}
