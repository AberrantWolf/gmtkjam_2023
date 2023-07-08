using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class CrowHiveMind : Node
{
    public Vector2 CenterOfMass { get; set; } = new Vector2();
    public List<Crow> AllCrows { get; } = new List<Crow>();

    public static CrowHiveMind Instance;

    public override void _EnterTree()
    {
        base._EnterTree();

        Instance = this;
    }

    public override void _Ready()
    {
        ProcessPriority = 0; // Less than Crow.cs, processes first, which is important!!
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        CenterOfMass = listAverage(AllCrows.Select(crow => crow.Position));
    }

    private Vector2 listAverage(IEnumerable<Vector2> items)
    {
        return new Vector2(items.Select(pos => pos.X).Average(), items.Select(pos => pos.Y).Average());
    }
}
