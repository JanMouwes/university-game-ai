using GameAI.Entity.Steering.Simple;
using Microsoft.Xna.Framework;

namespace GameAI.Entity.GoalBehaviour.Atomic
{
    public class ChaseTarget : Goal<Ship>
    {
        private readonly Ship target;
        private readonly float nearRange;

        public ChaseTarget(Ship owner, Ship target, float nearRange) : base(owner)
        {
            this.target = target;
            this.nearRange = nearRange;
        }

        public override void Activate()
        {
            this.Owner.Steering = new PursueBehaviour(this.Owner, this.target);

            base.Activate();
        }

        public override void Process(GameTime gameTime)
        {
            if (Vector2.DistanceSquared(this.Owner.Position, this.target.Position) < this.nearRange * this.nearRange) { this.Status = GoalStatus.Completed; }
        }
    }
}