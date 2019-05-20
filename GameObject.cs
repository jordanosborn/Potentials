using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Potential
{
    public interface GameObject
    {
        Utilities.ErrorCodes Update(World world, GameState state = null);
        Utilities.ErrorCodes Draw(SpriteBatch batch);
    }
}