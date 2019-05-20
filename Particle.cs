using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Potential
{
    public class TransientParticle : Particle
    {
        private float Lifetime { get; } = float.PositiveInfinity;
        private float Age { get; } = 0.0f;
        //TODO: decay products
        public TransientParticle(Texture2D texture, Vector3 position, Vector3 velocity, float radius = 10.0f, float mass = 0, float charge = 0, float energy = -1.0f, float rotation = 0.0f, float angular_velocity = 0.0f, bool isfixed = false, float lifetime = float.PositiveInfinity) :
        base(texture, position, velocity, radius, mass, charge, energy, rotation, angular_velocity, isfixed)
        {

        }
    }
    public class Particle : GameObject
    {
        public static float MAXSPEED = 10.0f;
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
                Energy = (float)(Mass * MAXSPEED / System.Math.Sqrt(1.0 - Velocity.LengthSquared() / (MAXSPEED * MAXSPEED))); //TODO: plus potential
            }
            else
            {
                Energy = (energy <= 0) ? 1.0f : energy; //TODO: plus potential
            }
            Momentum = (Mass > 0) ? Mass * Velocity : (Energy / (MAXSPEED * MAXSPEED)) * Velocity;
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
                Energy = 0.5f * Mass * Velocity.LengthSquared();// TODO: + potential;
            }
            else
            {
                Momentum += dt * Force;
                Energy = Momentum.Length() * MAXSPEED;
                Velocity = (Momentum / Momentum.Length()) * MAXSPEED;
            }
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