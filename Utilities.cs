using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Potential
{
    public static class Utilities
    {
        public enum ErrorCodes
        {
            SUCCESS,
            FAILURE
        }
    }
    class SmartFramerate
    {
        double currentFrametimes;
        double weight;
        int numerator;
        public Vector2 Position { get; set; }

        public double Framerate
        {
            get
            {
                return (numerator / currentFrametimes);
            }
        }

        public SmartFramerate(int oldFrameWeight, (int, int) position)
        {
            numerator = oldFrameWeight;
            weight = (double)oldFrameWeight / ((double)oldFrameWeight - 1d);
            Position = new Vector2(position.Item1, position.Item2);
        }

        public void Update(GameTime time)
        {
            var timeSinceLastFrame = time.ElapsedGameTime.TotalSeconds;
            currentFrametimes = currentFrametimes / weight;
            currentFrametimes += timeSinceLastFrame;
        }
    }
    public static class Math
    {
        public static Vector3 Derivative(Field f, Vector3 position, Vector3 orientation, float h = 0.001f)
        {
            orientation.Normalize();
            var dq = orientation * h;
            var df = (-f.Value(position + 2 * dq) + 8 * f.Value(position + dq) - 8 * f.Value(position - dq) + f.Value(position - 2 * dq)) / (12 * h);
            return df * orientation;
        }
        public static Vector3 Derivative3(Field f, Vector3 position, float h = 0.001f)
        {
            var dx = new Vector3(h, 0, 0);
            var dy = new Vector3(0, h, 0);
            var dz = new Vector3(0, 0, h);
            var dfx = Derivative(f, position, dx, h);
            var dfy = Derivative(f, position, dy, h);
            var dfz = Derivative(f, position, dz, h);
            return dfx + dfy + dfz;
        }
    }
}
static class Extensions
{
    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }
}