using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;
using Potential.Utilities;
using System.Collections.Generic;
namespace Potential
{
    public class Particle : GameObject
    {
        private static UInt64 ParticleCounter = 0;
        public static float RelativisticEnergy(float mass, Vector3 momentum)
        {
            if (mass > 0)
                return (float)(mass * Constants.c2 / System.Math.Sqrt(1.0 - (momentum / mass).LengthSquared() / (Constants.c2))); //TODO: plus potential
            else
                return momentum.Length() * Constants.c;
        }
        public UInt64? ID { get; private set; } = null;
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

        private List<(UInt64, Func<Particle, Particle, Vector3>)> InterParticleForces = new List<(UInt64, Func<Particle, Particle, Vector3>)>();
        public Tracer ParticleTracer { get; set; } = null;
        public Particle(Texture2D texture, Vector3 position, Vector3 velocity, float radius = 10.0f, float mass = 0, float charge = 0, float energy = -1.0f, float rotation = 0.0f, float angular_velocity = 0.0f, bool isfixed = false)
        {
            ID = ParticleCounter;
            ParticleCounter++;
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
            if (velocity != Vector3.Zero)
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

        private Particle(UInt64? id, Texture2D texture, Vector3 position, Vector3 velocity, float radius = 10.0f, float mass = 0, float charge = 0, float energy = -1.0f, float rotation = 0.0f, float angular_velocity = 0.0f, bool isfixed = false)
        {
            ID = id;
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
            if (velocity != Vector3.Zero)
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
            if (ParticleTracer != null && GameState.GetState().Flags.Contains(GameState.UIFlags.TRACERS_ON))
                ParticleTracer.Draw(batch);
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
        public void AddInterParticleForce(UInt64? particle_id, Func<Particle, Particle, Vector3> force)
        {
            if (particle_id.HasValue && particle_id < ParticleCounter)
            {
                InterParticleForces.Add((particle_id.Value, force));
            }
        }
        public void RemoveInterParticleForce(UInt64 position)
        {
            if (InterParticleForces.Count() > (int)position)
            {
                InterParticleForces[(int)position] = (InterParticleForces[(int)position].Item1, null);
            }
        }
        public void AddInterParticleForceSymmetric(Particle p, Func<Particle, Particle, Vector3> force)
        {
            InterParticleForces.Add((p.ID.Value, force));
            p.InterParticleForces.Add((ID.Value, force));
        }
        public void RemoveInterParticleForceSymmetric()
        {
            //TODO: think
        }

        public Vector3 ApplyInterParticleForces(World world)
        {
            var sum = Vector3.Zero;
            foreach (var e in InterParticleForces)
            {
                var p2 = world.Particles.Where((p) => p.ID == e.Item1).ToList();
                if (p2.Count() == 1)
                {
                    sum += e.Item2(this, p2[0]);
                }
            }
            return sum;
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
                    if (r.Length() > Constants.MinClassicalR)
                    {
                        var R3 = (float)System.Math.Pow(r.Length(), 3);
                        var R = r / R3;
                        return ((Mass * p.Mass) * Constants.G - (Charge * p.Charge) / (float)(4 * System.Math.PI * Constants.Epsilon0)) * R;
                    }
                    else
                    {
                        return Vector3.Zero;
                    }

                }
                return Vector3.Zero;
            }).Aggregate((x, y) => x + y);
        }
        public Utilities.ErrorCodes Update(GameTime time, World world, GameState state, object args = null)
        {
            //TODO: add collisions
            Rotation += (float)time.ElapsedGameTime.TotalSeconds * AngularVelocity;
            if (IsFixed)
            {
                return Utilities.ErrorCodes.SUCCESS;
            }
            //TODO: update forces felt by particle from other particles and fields
            Force = Vector3.Zero;
            Force += GravityAndElectrostatic(world);
            Force += ApplyInterParticleForces(world);
            ApplyForce(time);
            if (ParticleTracer != null && GameState.GetState().Flags.Contains(GameState.UIFlags.TRACERS_ON))
                ParticleTracer.Update(time, world, state, this);
            return Utilities.ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            var p = new Particle(ID, Texture, Position, Velocity, Radius, Mass, Charge, Energy, Rotation, AngularVelocity, IsFixed);
            p.ParticleTracer = ParticleTracer;
            p.InterParticleForces = InterParticleForces;
            return p;
        }
    }
}