using System.Collections.Generic;
namespace Potential
{
    public class World
    {
        private List<Field> IntrinsicFields { get; set; }
        private List<Particle> Particles { get; set; }
        World()
        {
            IntrinsicFields = new List<Field>();
            Particles = new List<Particle>();
        }
        public void AddField(Field f) {
            IntrinsicFields.Add(f);
        }
        public void AddParticle(Field f)
        {
            IntrinsicFields.Add(f);
        }
    }
}