using System;
using Microsoft.Xna.Framework;
namespace Potential.Force
{
    using Function = Func<Particle, Particle, Vector3>;
    public static class Factory
    {
        public static Function SpringForce(float k, float l0)
        {
            return (p1, p2) =>
            {
                var x = p1.Position - p2.Position;
                var xl = x.Length();
                if (Math.Abs(xl) > float.Epsilon)
                {
                    x.Normalize();
                }
                return -k * (xl - l0) * x;
            };
        }
    }
}