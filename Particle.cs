using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Potential
{
    public class Particle : GameObject
    {
        public Texture2D Texture { get; set; } = null;
        public float Radius { get; private set; }
        public Vector2 Scale { get; set; }
        public float Charge { get; private set; }
        public float Mass { get; private set; }
        public bool IsFixed { get; private set; }
        public Vector3 Position { get; private set; }
        public float Rotation { get; private set; }
        public Vector3 Velocity { get; private set; }
        public float AngularVelocity { get; private set; } = 0.0f;
        public Particle((float, float, float) position, (float, float, float) velocity, float radius = 10.0f, float mass = 0, float charge = 0, float rotation = 0.0f, float angular_velocity = 0.0f, bool isfixed = false)
        {
            var (x, y, z) = position;
            var (vx, vy, vz) = velocity;
            Position = new Vector3(x, y, z);
            Velocity = new Vector3(vx, vy, vz);
            Radius = radius;
            Mass = mass;
            Charge = charge;
            IsFixed = isfixed;
            AngularVelocity = angular_velocity;
            Rotation = rotation;
        }
        public Particle(Vector3 position, Vector3 velocity, float radius = 10.0f, float mass = 0, float charge = 0, float rotation = 0.0f, float angular_velocity = 0.0f, bool isfixed = false)
        {
            Position = position;
            Velocity = velocity;
            Radius = radius;
            Mass = mass;
            Charge = charge;
            IsFixed = isfixed;
            AngularVelocity = angular_velocity;
            Rotation = rotation;
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            if (Texture == null)
                return Utilities.ErrorCodes.FAILURE;
            var origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            batch.Draw(Texture, new Vector2(Position.X, Position.Y), scale: Scale, color: Color.White, rotation: Rotation, origin: origin);
            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Update(GameTime time, World world, GameState state)
        {
            Rotation += (float)time.ElapsedGameTime.TotalSeconds * AngularVelocity;
            if (IsFixed)
            {
                return Utilities.ErrorCodes.SUCCESS;
            }
            return Utilities.ErrorCodes.SUCCESS;
        }
    }
}