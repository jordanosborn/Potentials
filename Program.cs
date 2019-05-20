using System;

namespace Potential
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var gameBehaviour = Microsoft.Xna.Framework.GameRunBehavior.Synchronous;
            using (var game = new PotentialGame())
                game.Run(gameBehaviour);
        }
    }
}
