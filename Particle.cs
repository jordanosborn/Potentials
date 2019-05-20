using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Potential
{
    public class Particle : GameObject
    {
        public Texture2D Texture { get; set; } = null;
        public float Charge { get; private set; }
        public float Mass { get; private set; }
        public bool IsFixed { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Particle((float, float, float) position, (float, float, float) velocity, float mass = 0, float charge = 0, bool isfixed = false)
        {
            var (x, y, z) = position;
            var (vx, vy, vz) = velocity;
            Position = new Vector3(x, y, z);
            Velocity = new Vector3(vx, vy, vz);
            Mass = mass;
            Charge = charge;
            IsFixed = isfixed;

        }
        public Particle(Vector3 position, Vector3 velocity, float mass = 0, float charge = 0, bool isfixed = false)
        {
            Position = position;
            Velocity = velocity;
            Charge = charge;
            Mass = mass;
            IsFixed = isfixed;
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            if (Texture == null)
                return Utilities.ErrorCodes.FAILURE;
            batch.Draw(Texture, new Vector2(Position.X, Position.Y));
            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Update(World world)
        {
            return Utilities.ErrorCodes.SUCCESS;
        }
    }
}