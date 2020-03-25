using Microsoft.Xna.Framework.Graphics;
using Graph;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GameAI.Util
{
    public class GraphRenderer
    {
        private Graph<Vector2> graph;

        public bool Enabled { get; set; }

        public GraphRenderer(Graph<Vector2> graph)
        {
            this.graph = graph;
        }

        public void ToggleEnabled() => this.Enabled = !this.Enabled;

        public void Render(SpriteBatch spriteBatch)
        {
            if (!Enabled) { return; }

            foreach (Vertex<Vector2> graphVertex in this.graph.Vertices)
            {
                spriteBatch.DrawPoint(graphVertex.Value, Color.Aqua, 3f);

                foreach (Edge<Vector2> edge in graphVertex.Edges) { spriteBatch.DrawLine(edge.Source.Value, edge.Dest.Value, Color.White); }
            }
        }
    }
}