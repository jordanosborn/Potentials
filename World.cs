using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Potential
{
    public class World : GameObject
    {
        private List<Field> IntrinsicFields { get; set; }
        private List<Particle> Particles { get; set; }
        public World()
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

        public Utilities.ErrorCodes Update(GameTime time, World world = null, GameState state = null)
        {
            world = (world == null) ? this : world;
            foreach (var p in Particles)
            {
                p.Update(time, world, state);
            }
            foreach (var f in IntrinsicFields)
            {
                f.Update(time, world, state);
            }
            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            batch.Begin();
            foreach (var p in Particles)
            {
                p.Draw(batch);
            }
            foreach (var f in IntrinsicFields)
            {
                f.Draw(batch);
            }
            batch.End();
            return Utilities.ErrorCodes.SUCCESS;
        }
    }
}