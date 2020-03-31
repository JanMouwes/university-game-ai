using GameAI.behaviour;
using GameAI.entity;
using Microsoft.Xna.Framework;

namespace GameAI.GoalBehaviour.Atomic
{
    public class TraverseEdge<TOwner> : Goal<TOwner> where TOwner : MovingEntity
    {
        private readonly Vector2 target;
        private readonly float nearRange;
        public float DecelerateDistance { get; set; } = 3f;

        /// <param name="owner">Goal's owner</param>
        /// <param name="target">Where to move to</param>
        /// <param name="nearRange">Which distance it will consider 'close enough'. 3 pixels by default</param>
        public TraverseEdge(TOwner owner, Vector2 target, float nearRange = 3f) : base(owner)
        {
            this.target = target;
            this.nearRange = nearRange;
        }

        public override void Activate()
        {
            base.Activate();

            this.Owner.Steering = new ArriveBehaviour(this.Owner, this.target, this.DecelerateDistance);
        }

        public override GoalStatus Process(GameTime gameTime)
        {
            bool isNear = Vector2.DistanceSquared(this.Owner.Pos, this.target) < this.nearRange * this.nearRange;

            if (isNear) { this.Status = GoalStatus.Completed; }

            // TODO: Check for stuck-ness

            return this.Status;
        }
    }
}