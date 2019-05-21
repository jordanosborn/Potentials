using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
namespace Potential
{
    public class Tracer : GameObject
    {
        private readonly Texture2D ParticleTexture;
        private readonly Vector2 Origin;
        private Color ParticleColor { get; set; } = Color.White;
        private readonly float Spacing;
        private readonly int Length;
        private readonly float TracerHeight;
        private readonly float TracerWidth;
        private readonly Vector2 Scale;
        private LinkedList<Vector3> PreviousLocations { get; set; } = null;
        private LinkedList<Vector3> PreviousOrientations { get; set; } = null;

        public Tracer(Texture2D texture, Particle p, int length = 10, float spacing = 20, float tracerwidth = 10, float tracerheight = 5, Color? color = null)
        {
            ParticleTexture = texture;
            Length = length;
            Spacing = spacing;
            TracerWidth = tracerwidth;
            TracerHeight = tracerheight;
            var start = p.Position;
            var orientation = p.Velocity;
            if (orientation.Length() != 0)
                orientation.Normalize();
            PreviousLocations = new LinkedList<Vector3>();
            PreviousLocations.AddLast(start);
            PreviousOrientations = new LinkedList<Vector3>();
            PreviousOrientations.AddLast(orientation);
            if (color.HasValue)
                ParticleColor = color.Value;
            Scale = new Vector2(TracerWidth / ParticleTexture.Width, TracerHeight / ParticleTexture.Height);
            Origin = new Vector2(ParticleTexture.Width / 2, ParticleTexture.Height / 2);
        }
        private Tracer(Texture2D texture, LinkedList<Vector3> start, LinkedList<Vector3> orientations, int length = 10, float spacing = 5, float tracerwidth = 10, float tracerheight = 5, Color? color = null)
        {
            ParticleTexture = texture;
            Length = length;
            Spacing = spacing;
            TracerWidth = tracerwidth;
            TracerHeight = tracerheight;
            PreviousLocations = start;
            PreviousOrientations = orientations;
            if (color.HasValue)
                ParticleColor = color.Value;
            Scale = new Vector2(TracerWidth / ParticleTexture.Width, TracerHeight / ParticleTexture.Height);
            Origin = new Vector2(ParticleTexture.Width / 2, ParticleTexture.Height / 2);
        }
        public Utilities.ErrorCodes Update(GameTime time, World world, GameState state, object particle)
        {
            var p = (Particle)particle;
            var start = p.Position;
            var vel = p.Velocity;
            if (vel.Length() != 0)
            {
                vel.Normalize();
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
            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            //TODO: draw extended length along velocity direction!

            foreach (var (position, orientation) in PreviousLocations.Zip(PreviousOrientations, (p, o) => (p, o)))
            {
                var theta = 0.0f;
                if (orientation != Vector3.Zero)
                {
                    var cos_theta = Vector3.Dot(orientation, Vector3.UnitX) / orientation.Length();
                    theta = (float)System.Math.Acos(cos_theta);
                }
                batch.Draw(ParticleTexture, new Vector2(position.X, position.Y),
                    origin: Origin, scale: new Vector2(Scale.X, Scale.Y), rotation: theta, color: ParticleColor);
            }

            return Utilities.ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            return new Tracer(ParticleTexture, PreviousLocations, PreviousOrientations, Length, Spacing, TracerWidth, TracerHeight, ParticleColor);
        }
    }
}