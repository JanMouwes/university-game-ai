using System.Collections.Generic;
using System.Linq;
using Graph;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GameAI.Util
{
    public delegate IEnumerable<(int, float)> NeighbourGenerator(int x, int y, int width, int height, float xDistance,
        float yDistance);

    public static class GraphGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Current x-coordinate</param>
        /// <param name="y">Current y-coordinate</param>
        /// <param name="width">Total width of the square graph</param>
        /// <param name="height">Total width of the square graph</param>
        /// <param name="xDistance">Distance to the neighbouring nodes on the x-axis</param>
        /// <param name="yDistance">Distance to the neighbouring nodes on the x-axis</param>
        /// <returns>Index and distance to this index</returns>
        public static IEnumerable<(int, float)> AdjacentIndices(int x, int y, int width, int height, float xDistance,
            float yDistance)
        {
            int currentIndex = (x + (y * width));

            if (x > 0)
            {
                yield return (currentIndex - 1, xDistance); /* Left */
            }

            if (x + 1 < width)
            {
                yield return (currentIndex + 1, xDistance); /* Right */
            }

            if (y > 0)
            {
                yield return (currentIndex - width, yDistance); /* Top */
            }

            if (y + 1 < height)
            {
                yield return (currentIndex + width, yDistance); /* Bottom */
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Current x-coordinate</param>
        /// <param name="y">Current y-coordinate</param>
        /// <param name="width">Total width of the square graph</param>
        /// <param name="height">Total width of the square graph</param>
        /// <param name="xDistance">Distance to the neighbouring nodes on the x-axis</param>
        /// <param name="yDistance">Distance to the neighbouring nodes on the x-axis</param>
        /// <returns>Index and distance to this index</returns>
        public static IEnumerable<(int, float)> DiagonalIndices(int x, int y, int width, int height, float xDistance,
            float yDistance)
        {
            float distance = new Vector2(xDistance, yDistance).Length();

            int currentIndex = (x + (y * width));

            bool isLeftEdge = x == 0;
            bool isRightEdge = x == width - 1;
            bool isTopEdge = y == 0;
            bool isBottomEdge = y == height - 1;

            if (!isLeftEdge)
            {
                if (!isTopEdge)
                {
                    yield return (currentIndex - (width + 1), distance); /* LeftTop */
                }

                if (!isBottomEdge)
                {
                    yield return (currentIndex + (width - 1), distance); /* LeftBottom */
                }
            }

            if (!isRightEdge)
            {
                if (!isTopEdge)
                {
                    yield return (currentIndex - (width - 1), distance); /* RightTop */
                }

                if (!isBottomEdge)
                {
                    yield return (currentIndex + (width + 1), distance); /* RightBottom */
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Current x-coordinate</param>
        /// <param name="y">Current y-coordinate</param>
        /// <param name="width">Total width of the square graph</param>
        /// <param name="height">Total width of the square graph</param>
        /// <param name="xDistance">Distance to the neighbouring nodes on the x-axis</param>
        /// <param name="yDistance">Distance to the neighbouring nodes on the x-axis</param>
        /// <returns>Index and distance to this index</returns>
        public static IEnumerable<(int, float)> AxisAndDiagonalIndices(int x, int y, int width, int height,
            float xDistance, float yDistance)
        {
            foreach ((int, float) axisIndex in AdjacentIndices(x, y, width, height, xDistance, yDistance))
            {
                yield return axisIndex;
            }

            foreach ((int, float) axisIndex in DiagonalIndices(x, y, width, height, xDistance, yDistance))
            {
                yield return axisIndex;
            }
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

        public static Graph<Vector2> CreateGridGraph_Base(int width, int height, NeighbourGenerator neighbourGetter)
        {
            Graph<Vector2> graph = new Graph<Vector2>();

            int index = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vertex<Vector2> vertex = graph.GetVertex(index + 1);
                    vertex.Value = new Vector2(x, y);

                    foreach ((int neighbourIndex, float cost) in neighbourGetter(x, y, width, height, 1, 1))
                    {
                        graph.AddEdge(index + 1, neighbourIndex, cost);
                    }

                    index++;
                }
            }

            return graph;
        }

        public static Graph<Vector2> GenerateGraph(float totalWidth, float totalHeight,
            int xAxisVertexCount, int yAxisVertexCount,
            float xOffset, float yOffset,
            NeighbourGenerator neighbourGenerator)
        {
            float vectorXDistance = totalWidth / (xAxisVertexCount - 1);
            float vectorYDistance = totalHeight / (yAxisVertexCount - 1);

            Graph<Vector2> returnGraph = new Graph<Vector2>();

            int index = 0;

            for (int y = 0; y < yAxisVertexCount; y++)
            {
                for (int x = 0; x < xAxisVertexCount; x++)
                {
                    Vector2 vector = new Vector2
                    {
                        X = xOffset + vectorXDistance * x,
                        Y = yOffset + vectorYDistance * y
                    };

                    Vertex<Vector2> vertex = returnGraph.GetVertex(index);
                    vertex.Value = vector;

                    foreach ((int neighbourIndex, float cost) in neighbourGenerator(x, y, xAxisVertexCount,
                        yAxisVertexCount, vectorXDistance, vectorYDistance))
                    {
                        returnGraph.AddEdge(index, neighbourIndex, cost);
                    }

                    index++;
                }
            }

            return returnGraph;
        }

        public static Graph<Vector2> GenerateGraph((float, float) dimensions,
            (int, int) vertexCounts,
            (float, float) offset = default,
            NeighbourGenerator neighbourGenerator = null)
        {
            neighbourGenerator = neighbourGenerator ?? AdjacentIndices;

            return GenerateGraph(dimensions.Item1, dimensions.Item2,
                vertexCounts.Item1, vertexCounts.Item2,
                offset.Item1, offset.Item2,
                neighbourGenerator);
        }

        public static Graph<Vector2> GenerateGraphWithPadding((float, float) dimensions,
            (int, int) vertexCounts,
            (float, float) padding,
            NeighbourGenerator neighbourGenerator = null)
        {
            neighbourGenerator = neighbourGenerator ?? AdjacentIndices;

            return GenerateGraph(dimensions.Item1 - padding.Item1 * 2, dimensions.Item2 - padding.Item2 * 2,
                vertexCounts.Item1, vertexCounts.Item2,
                padding.Item1, padding.Item2,
                neighbourGenerator);
        }

        public static Graph<Vector2> GenerateGraphWithObstacles((float, float) dimensions,
            (int, int) vertexCounts,
            (float, float) padding, 
            World world, 
            NeighbourGenerator neighbourGenerator = null)
        {
            neighbourGenerator = neighbourGenerator ?? AdjacentIndices;

            float vectorXDistance = (dimensions.Item1 - padding.Item1 * 2) / (vertexCounts.Item1 - 1);
            float vectorYDistance = (dimensions.Item2 - padding.Item2 * 2) / (vertexCounts.Item2 - 1);

            Graph<Vector2> returnGraph = new Graph<Vector2>();

            int index = 0;

            for (int y = 0; y < vertexCounts.Item2; y++)
            {
                for (int x = 0; x < vertexCounts.Item1; x++)
                {
                    Vector2 vector = new Vector2
                    {
                        X = padding.Item1 + vectorXDistance * x,
                        Y = padding.Item2 + vectorYDistance * y
                    };

                    bool cont = true;
                    foreach (BaseGameEntity o in world.obstacles)
                    {
                        CircleF notAllowedZone = new CircleF(o.Pos, o.Scale + (padding.Item1 + padding.Item2) / 2);
                        if (notAllowedZone.Contains(vector)) cont = false;
                    }

                    if (cont)
                    {
                        Vertex<Vector2> vertex = returnGraph.GetVertex(index);
                        vertex.Value = vector;

                        foreach ((int neighbourIndex, float cost) in neighbourGenerator(x, y, vertexCounts.Item1,
                            vertexCounts.Item2, vectorXDistance, vectorYDistance))
                        {
                            returnGraph.AddEdge(index, neighbourIndex, cost);
                        }
                    }
                    else
                    {
                        Vertex<Vector2> vertex = returnGraph.GetVertex(index);
                        returnGraph.RemoveVertex(index);
                    }
                    index++;
                }
            }
            return returnGraph;
        }
    }
}