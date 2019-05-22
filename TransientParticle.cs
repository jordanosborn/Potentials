using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Potential
{
    public class TransientParticle : Particle
    {
        private float Lifetime { get; } = float.PositiveInfinity;
        private float Age { get; } = 0.0f;
        //TODO: decay products
        public TransientParticle(Texture2D texture, Vector3 position, Vector3 velocity, float radius = 10.0f, float mass = 0, float charge = 0, float energy = -1.0f, float rotation = 0.0f, float angularVelocity = 0.0f, bool isFixed = false, float lifetime = float.PositiveInfinity) :
        base(texture, position, velocity, radius, mass, charge, energy, rotation, angularVelocity, isFixed)
        {

        }
    }
}