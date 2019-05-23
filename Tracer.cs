using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Potential.Utilities;

namespace Potential
{
    public class Tracer : IGameObject
    {
        private readonly int Length;
        private readonly Vector2 Origin;
        private readonly Texture2D ParticleTexture;
        private readonly Vector2 Scale;
        private readonly float Spacing;
        private readonly float TracerHeight;
        private readonly float TracerWidth;

        public Tracer(Texture2D texture, Particle p, int length = 10, float spacing = 20, float tracerWidth = 20,
            float tracerHeight = 5, Color? color = null)
        {
            ParticleTexture = texture;
            Length = length;
            Spacing = spacing;
            TracerWidth = tracerWidth;
            TracerHeight = tracerHeight;
            var start = p.Position;
            var orientation = p.Velocity;
            if (Math.Abs(orientation.Length()) > float.Epsilon) orientation.Normalize();

            PreviousLocations = new LinkedList<Vector3>();
            PreviousLocations.AddLast(start);
            PreviousOrientations = new LinkedList<Vector3>();
            PreviousOrientations.AddLast(orientation);
            if (color.HasValue) ParticleColor = color.Value;

            Scale = new Vector2(TracerWidth / ParticleTexture.Width, TracerHeight / ParticleTexture.Height);
            Origin = new Vector2(ParticleTexture.Width / 2, ParticleTexture.Height / 2);
        }

        private Tracer(Texture2D texture, LinkedList<Vector3> start, LinkedList<Vector3> orientations, int length,
            float spacing, float tracerwidth, float tracerheight, Color? color)
        {
            ParticleTexture = texture;
            Length = length;
            Spacing = spacing;
            TracerWidth = tracerwidth;
            TracerHeight = tracerheight;
            PreviousLocations = start;
            PreviousOrientations = orientations;
            if (color.HasValue) ParticleColor = color.Value;

            Scale = new Vector2(TracerWidth / ParticleTexture.Width, TracerHeight / ParticleTexture.Height);
            Origin = new Vector2(ParticleTexture.Width / 2, ParticleTexture.Height / 2);
        }

        private Color ParticleColor { get; } = Color.White;
        private LinkedList<Vector3> PreviousLocations { get; set; }
        private LinkedList<Vector3> PreviousOrientations { get; set; }

        public ErrorCodes Update(GameTime time, World world, GameState state, object particle)
        {
            var p = (Particle) particle;
            var start = p.Position;
            var vel = p.Velocity;
            if (Math.Abs(vel.Length()) > float.Epsilon) vel.Normalize();
            if (PreviousLocations.Last == null)
            {
                PreviousLocations.AddLast(start);
                var orientation = p.Velocity;
                if (Math.Abs(orientation.Length()) > float.Epsilon) orientation.Normalize();
                PreviousOrientations.AddLast(orientation);
            }

            var difference = start - PreviousLocations.Last.Value;
            if (difference.Length() > Spacing)
            {
                if (PreviousLocations.Count == Length)
                {
                    PreviousLocations.RemoveFirst();
                    PreviousOrientations.RemoveFirst();
                }

                PreviousLocations.AddLast(start);
                PreviousOrientations.AddLast(vel);
            }

            return ErrorCodes.SUCCESS;
        }

        public ErrorCodes Draw(SpriteBatch batch)
        {
            //TODO: draw extended length along velocity direction!

            foreach (var (position, orientation) in PreviousLocations.Zip(PreviousOrientations, (p, o) => (p, o)))
            {
                var theta = 0.0f;
                if (orientation != Vector3.Zero)
                {
                    var cosTheta = Vector3.Dot(orientation, Vector3.UnitX) / orientation.Length();
                    theta = (float) Math.Acos(cosTheta);
                }

                batch.Draw(ParticleTexture, new Vector2(position.X, position.Y),
                    origin: Origin, scale: new Vector2(Scale.X, Scale.Y), rotation: orientation.Y < 0 ? -theta : theta,
                    color: ParticleColor);
            }

            return ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            return new Tracer(ParticleTexture, PreviousLocations, PreviousOrientations, Length, Spacing, TracerWidth,
                TracerHeight, ParticleColor);
        }

        public void Reset()
        {
            PreviousLocations = new LinkedList<Vector3>();
            PreviousOrientations = new LinkedList<Vector3>();
        }
    }
}