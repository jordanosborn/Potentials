using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Potential
{
    using Utilities;

    public interface IField : IGameObject
    {
        float Value<T>(T particle) where T : Particle;
        float Value(Vector3 position);
    }
    internal enum CombineBy
    {
        ADDITION,
        SUBTRACTION,
        MULTIPLICATION,
        DIVISION,
        EXPONENTIATION
    }
    public class UniqueField : IField
    {
        public UniqueField()
        {

        }

        public float Value<T>(T particle) where T : Particle
        {

            return 0.0f;
        }
        public float Value(Vector3 position)
        {

            return 0.0f;
        }
        public ErrorCodes Update(GameTime time, World world = null, GameState state = null, object args = null)
        {
            return ErrorCodes.SUCCESS;
        }

        public ErrorCodes Draw(SpriteBatch batch)
        {
            return ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            return new UniqueField();
        }
    }

    public class CombinedField : IField
    {
        private Func<float, IField, Particle, float> CombinatorParticle { get; set; }
        private Func<float, IField, Vector3, float> CombinatorPosition { get; set; }
        private readonly IField[] F;
        private readonly CombineBy C;
        CombinedField(IField f, IField g, CombineBy c)
        {
            CombinatorParticle = CombineFunction<Particle>(c);
            CombinatorPosition = CombineFunction(c);
            F = new IField[2];
            F[0] = f;
            F[1] = g;
            C = c;
        }

        private static Func<float, IField, T, float> CombineFunction<T>(CombineBy c) where T : Particle
        {
            Func<float, IField, T, float> f;
            switch (c)
            {
                case CombineBy.ADDITION:
                    f = (x, y, p) => x + y.Value(p);
                    break;
                case CombineBy.SUBTRACTION:
                    f = (x, y, p) => x - y.Value(p);
                    break;
                case CombineBy.MULTIPLICATION:
                    f = (x, y, p) => x * y.Value(p);
                    break;
                case CombineBy.DIVISION:
                    f = (x, y, p) => x / y.Value(p);
                    break;
                case CombineBy.EXPONENTIATION:
                    f = (x, y, p) => (float)Math.Pow(x, y.Value(p));
                    break;
                default:
                    f = (x, y, p) => 0.0f;
                    break;
            }
            return f;
        }
        private static Func<float, IField, Vector3, float> CombineFunction(CombineBy c)
        {
            Func<float, IField, Vector3, float> f;
            switch (c)
            {
                case CombineBy.ADDITION:
                    f = (x, y, p) => x + y.Value(p);
                    break;
                case CombineBy.SUBTRACTION:
                    f = (x, y, p) => x - y.Value(p);
                    break;
                case CombineBy.MULTIPLICATION:
                    f = (x, y, p) => x * y.Value(p);
                    break;
                case CombineBy.DIVISION:
                    f = (x, y, p) => x / y.Value(p);
                    break;
                case CombineBy.EXPONENTIATION:
                    f = (x, y, p) => (float)Math.Pow(x, y.Value(p));
                    break;
                default:
                    f = (x, y, p) => 0.0f;
                    break;
            }
            return f;
        }
        CombinedField(IField[] f, CombineBy c)
        {
            CombinatorParticle = CombineFunction<Particle>(c);
            CombinatorPosition = CombineFunction(c);
            F = f;
        }

        public float Value<T>(T particle) where T : Particle
        {
            var sum = 0.0f;
            foreach (var x in F)
            {
                sum = CombinatorParticle(sum, x, particle);
            }
            return sum;
        }
        public float Value(Vector3 position)
        {
            var sum = 0.0f;
            foreach (var x in F)
            {
                sum = CombinatorPosition(sum, x, position);
            }
            return sum;
        }
        public ErrorCodes Update(GameTime time, World world = null, GameState state = null, object args = null)
        {
            return ErrorCodes.SUCCESS;
        }
        public ErrorCodes Draw(SpriteBatch batch)
        {
            return ErrorCodes.SUCCESS;
        }
        public object Clone()
        {
            return new CombinedField(F, C);
        }
    }
}