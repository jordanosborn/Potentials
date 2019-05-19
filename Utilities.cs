using Microsoft.Xna.Framework;
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
