using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace Potential
{
    public interface GameObject : ICloneable
    {
        //TODO: should return new object so world not altered between frames.
        //then replace old world with new
        Utilities.ErrorCodes Update(GameTime gameTime, World world = null, GameState state = null);
        Utilities.ErrorCodes Draw(SpriteBatch batch);
    }
}