using GameAI.world;
using Microsoft.Xna.Framework;

namespace GameAI.Entity.Steering.Complex
{
    public class WallAvoidance : SteeringBehaviour
    {
        private readonly World world;
        private readonly float panicDistance;
        private readonly float strength;

        /// <summary>
        /// Avoids walls from a certain distance
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="world">World</param>
        /// <param name="panicDistance">Distance at which to start avoiding walls</param>
        /// <param name="strength">Strength multiplier</param>
        public WallAvoidance(MovingEntity entity, World world, float panicDistance = 5f, float strength = 3f) : base(entity)
        {
            this.world = world;
            this.panicDistance = panicDistance;
            this.strength = strength;
        }

        public override Vector2 Calculate()
        {
            return SteeringBehaviours.WallAvoidance(this.Entity, this.world, this.panicDistance) * this.strength;
        }
    }
}