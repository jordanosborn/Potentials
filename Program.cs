using System;

namespace Potential
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new PotentialGame())
                game.Run();
        }
    }
}
