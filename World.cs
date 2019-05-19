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
        public void AddField(Field f)
        {
            IntrinsicFields.Add(f);
        }
        public void AddParticle(Particle p)
        {
            Particles.Add(p);
        }

        public Particle RemoveParticle(uint id)
        {
            if (id < Particles.Count)
            {
                var particle = Particles[(int)id];
                Particles.RemoveAt((int)id);
                return particle;
            }
            return null;
        }
        public Field RemoveField(uint id)
        {
            if (id < IntrinsicFields.Count)
            {
                var field = IntrinsicFields[(int)id];
                IntrinsicFields.RemoveAt((int)id);
                return field;
            }
            return null;
        }
    }
}