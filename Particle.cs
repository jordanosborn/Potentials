using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Potential
{
    public class Particle : GameObject
    {
        public static float MAXSPEED = 10.0f;
        public static float MAXSPEED2 = MAXSPEED * MAXSPEED;
        public static float RelativisticEnergy(float mass, Vector3 momentum)
        {
            if (mass > 0)
                return (float)(mass * MAXSPEED2 / System.Math.Sqrt(1.0 - (momentum / mass).LengthSquared() / (MAXSPEED2))); //TODO: plus potential
            else
                return momentum.Length() * MAXSPEED;
        }
        public Vector2 Origin { get; set; }
        public Texture2D Texture { get; set; } = null;
        public float Radius { get; private set; }
        public Vector2 Scale { get; set; }
        public float Charge { get; private set; }
        public float Mass { get; private set; }
        public bool IsFixed { get; private set; }
        public Vector3 Position { get; private set; }
        public float Rotation { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 Momentum { get; private set; }
        public float Energy { get; private set; }
        public float AngularVelocity { get; private set; } = 0.0f;
        public Vector3 Force { get; private set; }
        public Particle(Texture2D texture, Vector3 position, Vector3 velocity, float radius = 10.0f, float mass = 0, float charge = 0, float energy = -1.0f, float rotation = 0.0f, float angular_velocity = 0.0f, bool isfixed = false)
        {
            Texture = texture;
            Position = position;
            Radius = radius;
            Mass = mass;
            Charge = charge;
            IsFixed = isfixed;
            AngularVelocity = angular_velocity;
            Rotation = rotation;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Scale = new Vector2(Radius / Texture.Width, Radius / Texture.Height);
            Force = new Vector3(0, 0, 0);
            float vel_scale = (Mass > 0) ? velocity.Length() : MAXSPEED;
            velocity.Normalize();
            vel_scale = (vel_scale > MAXSPEED) ? MAXSPEED : vel_scale;
            Velocity = (vel_scale * velocity);
            if (Mass > 0)
            {
                Momentum = Mass * Velocity;
                Energy = RelativisticEnergy(Mass, Momentum);
            }
            else
            {
                Energy = (energy <= 0) ? 1.0f : energy; //TODO: plus potential
                Momentum = (Energy / MAXSPEED2) * Velocity;
            }
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            if (Texture == null)
                return Utilities.ErrorCodes.FAILURE;
            batch.Draw(Texture, new Vector2(Position.X, Position.Y), scale: Scale, color: Color.White, rotation: Rotation, origin: Origin);
            return Utilities.ErrorCodes.SUCCESS;
        }
        private void ApplyForce(GameTime time)
        {
            //TODO: look into other integration schemes
            var dt = (float)time.ElapsedGameTime.TotalSeconds;

            if (Mass > 0)
            {
                Velocity += dt * Force / Mass;
                Momentum = Mass * Velocity;
            }
            else
            {
                Momentum += dt * Force;
                Velocity = (Momentum / Momentum.Length()) * MAXSPEED;
            }
            Energy = RelativisticEnergy(Mass, Momentum);
            Position += dt * Velocity;
        }
        public Utilities.ErrorCodes Update(GameTime time, World world, GameState state)
        {
            Rotation += (float)time.ElapsedGameTime.TotalSeconds * AngularVelocity;
            if (IsFixed)
            {
                return Utilities.ErrorCodes.SUCCESS;
            }
            //TODO: update forces felt by particle from other particles and fields
            ApplyForce(time);
            return Utilities.ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            var p = new Particle(Texture, Position, Velocity, Radius, Mass, Charge, Energy, Rotation, AngularVelocity, IsFixed);
            return p;
        }
    }
}