using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Potential
{
    public class Tracer : GameObject
    {
        private Texture2D ParticleTexture { get; set; } = null;
        public Tracer(Texture2D texture)
        {
            ParticleTexture = texture;
        }
        public Utilities.ErrorCodes Update(GameTime time, World world, GameState state)
        {
            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            return Utilities.ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            return new Tracer(ParticleTexture);
        }
    }
}