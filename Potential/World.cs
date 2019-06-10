using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Potential.Utilities;

namespace Potential
{
    //TODO: segment world to reduce computational complexity, scale pixels to real world values;
    //TODO: remove particles that have gone too far
    //TODO: use world coords then translate to pixel coords!
    public class World : IGameObject
    {
        public World(Dictionary<string, Texture2D> textures)
        {
            Textures = textures;
            IntrinsicFields = new List<IField>();
            Particles = new List<Particle>();
        }

        public List<IField> IntrinsicFields { get; set; }
        public List<Particle> Particles { get; set; }
        public Dictionary<string, Texture2D> Textures { get; set; }

        public ErrorCodes Update(GameTime time, World world = null, GameState state = null, object args = null)
        {
            world = world ?? this;
            foreach (var p in Particles) p.Update(time, world, state);

            foreach (var f in IntrinsicFields) f.Update(time, world, state);
            return ErrorCodes.SUCCESS;
        }

        public ErrorCodes Draw(SpriteBatch batch)
        {
            foreach (var p in Particles) p.Draw(batch);
            foreach (var f in IntrinsicFields) f.Draw(batch);
            return ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            var world = new World(Textures);
            world.Particles = (List<Particle>) Particles.Clone();
            world.IntrinsicFields = (List<IField>) IntrinsicFields.Clone();
            return world;
        }

        public void AddField(IField f)
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
                var particle = Particles[(int) id];
                Particles.RemoveAt((int) id);
                return particle;
            }

            return null;
        }

        public IField RemoveField(uint id)
        {
            if (id < IntrinsicFields.Count)
            {
                var field = IntrinsicFields[(int) id];
                IntrinsicFields.RemoveAt((int) id);
                return field;
            }

            return null;
        }
    }
}