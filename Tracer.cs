using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private Vector3[] PreviousLocations { get; set; } = null;

        public Tracer(Texture2D texture, Vector3 start, int length = 10, float spacing = 5, float size = 10, Color? color = null)
        {
            ParticleTexture = texture;
            Length = length;
            Spacing = spacing;
            Size = size;
            PreviousLocations = new Vector3[Length];
            PreviousLocations[0] = start;
            if (color.HasValue)
                ParticleColor = color.Value;
            Scale = new Vector2(Size / ParticleTexture.Width, Size / ParticleTexture.Height);
        }
        public Utilities.ErrorCodes Update(GameTime time, World world, GameState state)
        {
            //TODO: track

            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            foreach (var e in PreviousLocations)
            {
                batch.Draw(ParticleTexture, new Vector2(e.X, e.Y),
                    origin: Origin, scale: Scale, color: ParticleColor);
            }
            return Utilities.ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            return new Tracer(ParticleTexture, PreviousLocations[0], Length, Spacing, Size, ParticleColor);
        }
    }
}