using System;
using System.Collections.Generic;
using Graph;
using Microsoft.Xna.Framework;

namespace GameAI.Pathfinding.Tests
{
    public static class GraphHelper
    {
        public static Graph<Vector2> CreateGraph_3Vertex_Simple()
        {
            Graph<Vector2> graph = new Graph<Vector2>();

            graph.AddVertex(new Vertex<Vector2>(1));
            graph.AddVertex(new Vertex<Vector2>(2));
            graph.AddVertex(new Vertex<Vector2>(3));

            graph.AddEdge(1, 2, 20);
            graph.AddEdge(2, 3, 10);
            graph.AddEdge(1, 3, 40);

            return graph;
        }

        private static IEnumerable<int> AdjacentIndices(int x, int y, int width, int height)
        {
            int currentIndex = (x + (y * width)) + 1;

            if (x > 0) { yield return currentIndex - 1; /* Left */ }

            if (x + 1 < width) { yield return currentIndex + 1; /* Right */ }

            if (y > 0) { yield return currentIndex - width; /* Top */ }

            if (y + 1 < height) { yield return currentIndex + width; /* Bottom */ }
        }

        private static IEnumerable<int> DiagonalIndices(int x, int y, int width, int height)
        {
            int currentIndex = (x + (y * width)) + 1;

            bool isLeftEdge = x   == 0;
            bool isRightEdge = x  == width - 1;
            bool isTopEdge = y    == 0;
            bool isBottomEdge = y == height - 1;

            if (!isLeftEdge)
            {
                if (!isTopEdge) { yield return currentIndex - (width + 1); /* LeftTop */ }

                if (!isBottomEdge) { yield return currentIndex + (width - 1); /* LeftBottom */ }
            }

            if (!isRightEdge)
            {
                if (!isTopEdge) { yield return currentIndex - (width - 1); /* RightTop */ }

                if (!isBottomEdge) { yield return currentIndex + (width + 1); /* RightBottom */ }
            }
        }

        private static IEnumerable<int> AxisAndDiagonalIndices(int x, int y, int width, int height)
        {
            foreach (int axisIndex in AdjacentIndices(x, y, width, height)) { yield return axisIndex; }

            foreach (int axisIndex in DiagonalIndices(x, y, width, height)) { yield return axisIndex; }
        }

        public static Graph<Vector2> CreateGridGraph_AdjacentOnly(int width, int height)
        {
            return CreateGridGraph_Base(width, height, AdjacentIndices);
        }

        public static Graph<Vector2> CreateGridGraph_DiagonalOnly(int width, int height)
        {
            return CreateGridGraph_Base(width, height, DiagonalIndices);
        }

        public static Graph<Vector2> CreateGridGraph_AllNeighbours(int width, int height)
        {
            return CreateGridGraph_Base(width, height, AxisAndDiagonalIndices);
        }

        private static Graph<Vector2> CreateGridGraph_Base(int width, int height, Func<int, int, int, int, IEnumerable<int>> neighbourGetter)
        {
            Graph<Vector2> graph = new Graph<Vector2>();

            int index = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vertex<Vector2> vertex = graph.GetVertex(index + 1);
                    vertex.Value = new Vector2(x, y);

                    foreach (int neighbourIndex in neighbourGetter(x, y, width, height)) { graph.AddEdge(index + 1, neighbourIndex, 1); }

                    index++;
                }
            }

            return graph;
        }
    }
}