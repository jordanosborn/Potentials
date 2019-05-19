using Microsoft.Xna.Framework;
namespace Potential
{
    public class Particle
    {
        public float Charge { get; private set; }
        public float Mass { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }
        Particle((float, float, float) position, (float, float, float) velocity, float charge = 0, float mass = 0)
        {
            var (x, y, z) = position;
            var (vx, vy, vz) = velocity;
            Position = new Vector3(x, y, z);
            Velocity = new Vector3(vx, vy, vz);
            Charge = charge;
            Mass = mass;
        }
        public Utilities.ErrorCodes Draw(Object Window, World world)
        {

            return Utilities.ErrorCodes.SUCCESS;
        }
    }
}