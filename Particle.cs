using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;
namespace Potential
{
    public class Particle : GameObject
    {
        public static float RelativisticEnergy(float mass, Vector3 momentum)
        {
            if (mass > 0)
                return (float)(mass * Constants.c2 / System.Math.Sqrt(1.0 - (momentum / mass).LengthSquared() / (Constants.c2))); //TODO: plus potential
            else
                return momentum.Length() * Constants.c;
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
            float vel_scale = (Mass > 0) ? velocity.Length() : Constants.c;
            velocity.Normalize();
            vel_scale = (vel_scale > Constants.c) ? Constants.c : vel_scale;
            Velocity = (vel_scale * velocity);
            if (Mass > 0)
            {
                Momentum = Mass * Velocity;
                Energy = RelativisticEnergy(Mass, Momentum);
            }
            else
            {
                Energy = (energy <= 0) ? 1.0f : energy; //TODO: plus potential
                Momentum = (Energy / Constants.c2) * Velocity;
            }
        }

        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            if (Texture == null)
                return Utilities.ErrorCodes.FAILURE;
            batch.Draw(Texture, new Vector2(Position.X, Position.Y), origin: Origin, rotation: Rotation, scale: Scale, color: Color.White);
            return Utilities.ErrorCodes.SUCCESS;
        }
        private void ApplyForce(GameTime time)
        {
            //TODO: look into other integration schemes
            var dt = (float)time.ElapsedGameTime.TotalSeconds;
            Momentum += dt * Force;
            if (Mass > 0)
            {
                Velocity = Momentum / Mass;
            }
            else
            {
                Velocity = (Momentum / Momentum.Length()) * Constants.c;
            }
            Energy = RelativisticEnergy(Mass, Momentum);
            Position += dt * Velocity;
        }
        public Vector3 GravityAndElectrostatic(World world)
        {
            if (MathF.Abs(Mass) < float.Epsilon && MathF.Abs(Charge) < float.Epsilon)
            {
                return Vector3.Zero;
            }
            return world.Particles.Select((p) =>
            {
                if (p.Position != Position)
                {
                    var r = p.Position - Position;
                    var R3 = (float)System.Math.Pow(r.Length(), 3);
                    var R = r / R3;
                    return ((Mass * p.Mass) * Constants.G - (Charge * p.Charge) / (float)(4 * System.Math.PI * Constants.Epsilon0)) * R;
                }
                return Vector3.Zero;
            }).Aggregate((x, y) => x + y);
        }
        public Utilities.ErrorCodes Update(GameTime time, World world, GameState state)
        {
            Rotation += (float)time.ElapsedGameTime.TotalSeconds * AngularVelocity;
            if (IsFixed)
            {
                return Utilities.ErrorCodes.SUCCESS;
            }
            //TODO: update forces felt by particle from other particles and fields
            Force = Vector3.Zero;
            Force += GravityAndElectrostatic(world);
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