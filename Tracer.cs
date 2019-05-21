using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
namespace Potential
{
    public class Tracer : GameObject
    {
        private readonly Texture2D ParticleTexture;
        private readonly Vector2 Origin;
        private Color ParticleColor { get; set; } = Color.White;
        private readonly float Spacing;
        private readonly int Length;
        private readonly float Size;
        private readonly Vector2 Scale;
        private LinkedList<Vector3> PreviousLocations { get; set; } = null;

        public Tracer(Texture2D texture, Vector3 start, int length = 10, float spacing = 20, float size = 5, Color? color = null)
        {
            ParticleTexture = texture;
            Length = length;
            Spacing = spacing;
            Size = size;
            PreviousLocations = new LinkedList<Vector3>();
            PreviousLocations.AddLast(start);
            if (color.HasValue)
                ParticleColor = color.Value;
            Scale = new Vector2(Size / ParticleTexture.Width, Size / ParticleTexture.Height);
            Origin = new Vector2(ParticleTexture.Width / 2, ParticleTexture.Height / 2);
        }
        private Tracer(Texture2D texture, LinkedList<Vector3> start, int length = 10, float spacing = 5, float size = 10, Color? color = null)
        {
            ParticleTexture = texture;
            Length = length;
            Spacing = spacing;
            Size = size;
            PreviousLocations = start;
            if (color.HasValue)
                ParticleColor = color.Value;
            Scale = new Vector2(Size / ParticleTexture.Width, Size / ParticleTexture.Height);
            Origin = new Vector2(ParticleTexture.Width / 2, ParticleTexture.Height / 2);
        }
        public Utilities.ErrorCodes Update(GameTime time, World world, GameState state, object position)
        {
            var start = (Vector3)position;
            var difference = start - PreviousLocations.Last.Value;
            //TODO: track
            if (difference.Length() > Spacing)
            {
                if (PreviousLocations.Count == Length)
                {
                    PreviousLocations.RemoveFirst();
                }
                PreviousLocations.AddLast(start);
            }
            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            //TODO: draw extended length along velocity direction!

            foreach (var e in PreviousLocations)
            {
                batch.Draw(ParticleTexture, new Vector2(e.X, e.Y),
                    origin: Origin, scale: Scale, color: ParticleColor);
            }
            return Utilities.ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            return new Tracer(ParticleTexture, PreviousLocations, Length, Spacing, Size, ParticleColor);
        }
    }
}