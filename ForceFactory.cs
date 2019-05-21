using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Potential.ForceFactory
{
    using Function = Func<Particle, Particle, Vector3>;
    public static class ForceFactory
    {
        public static Function SpringForce(float k, float l0)
        {
            return (p1, p2) =>
            {
                var x = p1.Position - p2.Position;
                return -k * (x.Length() - l0) * x / x.Length();
            };
        }
    }
}