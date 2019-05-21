using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace Potential
{
    public interface GameObject : ICloneable
    {
        Utilities.ErrorCodes Update(GameTime gameTime, World world = null, GameState state = null, object args = null);
        Utilities.ErrorCodes Draw(SpriteBatch batch);
    }
}