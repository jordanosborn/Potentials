using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Potential
{
    using Utilities;
    public interface IGameObject : ICloneable
    {
        ErrorCodes Update(GameTime gameTime, World world = null, GameState state = null, object args = null);
        ErrorCodes Draw(SpriteBatch batch);
        //Returns shape of objet true if vector inside shape.
        // w
    }
}