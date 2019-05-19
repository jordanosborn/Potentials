using Microsoft.Xna.Framework;
namespace Potential
{
    public class Particle : GameObject
    {
        public float Charge { get; private set; }
        public float Mass { get; private set; }
        public bool IsFixed { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }
        Particle((float, float, float) position, (float, float, float) velocity, float charge = 0, float mass = 0, bool isfixed = false)
        {
            var (x, y, z) = position;
            var (vx, vy, vz) = velocity;
            Position = new Vector3(x, y, z);
            Velocity = new Vector3(vx, vy, vz);
            Charge = charge;
            Mass = mass;
            IsFixed = isfixed;
        }
        public Utilities.ErrorCodes Draw(object Window)
        {

            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Update(World world)
        {
            return Utilities.ErrorCodes.SUCCESS;
        }
    }
}