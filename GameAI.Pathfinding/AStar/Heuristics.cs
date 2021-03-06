using System;
using Graph;
using Microsoft.Xna.Framework;

namespace GameAI.Pathfinding.AStar
{
    public static class Heuristics
    {
        public static double None(Vertex<Vector2> from, Vertex<Vector2> to)
        {
            return 0;
        }

        public static double Manhattan(Vertex<Vector2> from, Vertex<Vector2> to)
        {
            return Math.Abs(to.Value.X - @from.Value.X) + Math.Abs(to.Value.Y - @from.Value.Y);
        }

        public static double Euclidean(Vertex<Vector2> from, Vertex<Vector2> to)
        {
            return Vector2.Distance(to.Value, from.Value);
        }
    }
}