using System.Collections.Generic;
using GameAI.Entity.GoalBehaviour.Atomic;
using Microsoft.Xna.Framework;

namespace GameAI.Entity.GoalBehaviour.Composite
{
    public class FollowPath<TOwner> : GoalComposite<TOwner> where TOwner : MovingEntity
    {
        public FollowPath(TOwner owner, IEnumerable<Vector2> path) : base(owner)
        {
            foreach (Vector2 target in path) { AddWaypoint(target); }
        }

        public void AddWaypoint(Vector2 waypoint)
        {
            AddSubgoal(new TraverseEdge<TOwner>(this.Owner, waypoint));
        }
    }
}