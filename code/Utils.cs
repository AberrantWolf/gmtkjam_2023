using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Vector2Extension
{
    // Linq Average overload for Vector2
    public static Vector2 Average(this IEnumerable<Vector2>? source)
    {
        if (source is null || !source.Any())
        {
            throw new InvalidOperationException("Cannot compute average for a null or empty set");
        }

        return source.Sum() / source.Count();
    }

    // Linq Sum overload for Vector2
    public static Vector2 Sum(this IEnumerable<Vector2>? source)
    {
        Vector2 accum = new Vector2();
        foreach (var vec in source)
        {
            accum += vec;
        }

        return accum;
    }
}