using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Potential.Utilities
{
    //TODO: adjust these
    public static class Constants
    {
        public static float G = 1.0f;
        public static float Epsilon0 = 1.0f;
        public static float c = 50.0f;
        public static float c2 = (float)Math.Pow(c, 2);
        public static float MinClassicalR = 3.0f;
    }

    public enum ErrorCodes
    {
        SUCCESS,
        FAILURE
    }

    class SmartFramerate
    {
        double CurrentFrametimes;
        readonly double Weight;
        readonly int Numerator;
        public Vector2 Position { get; set; }

        public double Framerate
        {
            get => Math.Abs(CurrentFrametimes) > float.Epsilon ? Numerator / CurrentFrametimes : 0;

        }

        public SmartFramerate(int oldFrameWeight, (int, int) position)
        {
            Numerator = oldFrameWeight;
            Weight = oldFrameWeight / (oldFrameWeight - 1d);
            Position = new Vector2(position.Item1, position.Item2);
        }

        public void Update(GameTime time)
        {
            var timeSinceLastFrame = time.ElapsedGameTime.TotalSeconds;
            CurrentFrametimes = CurrentFrametimes / Weight;
            CurrentFrametimes += timeSinceLastFrame;
        }
    }
    public static class MathUtils
    {
        public static Vector3 Derivative(IField f, Vector3 position, Vector3 orientation, float h = 0.001f)
        {
            orientation.Normalize();
            var dq = orientation * h;
            var df = (-f.Value(position + 2 * dq) + 8 * f.Value(position + dq) - 8 * f.Value(position - dq) + f.Value(position - 2 * dq)) / (12 * h);
            return df * orientation;
        }
        public static Vector3 Derivative3(IField f, Vector3 position, float h = 0.001f)
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
internal static class Extensions
{
    public static IList<T> Clone<T>(this IEnumerable<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }
}