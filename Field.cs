using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System;
namespace Potential
{

    public interface Field : GameObject
    {
        float Value<T>(T particle) where T : Particle;
        float Value(Vector3 position);
    }
    enum CombineBy
    {
        ADDITION,
        SUBTRACTION,
        MULTIPLICATION,
        DIVISION,
        EXPONENTIATION
    }
    public class UniqueField : Field
    {
        UniqueField()
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
        public Utilities.ErrorCodes Update(GameTime time, World world = null, GameState state = null)
        {
            return Utilities.ErrorCodes.SUCCESS;
        }

        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            return Utilities.ErrorCodes.SUCCESS;
        }
    }

    public class CombinedField : Field
    {
        private Func<float, Field, Particle, float> CombinatorParticle { get; set; }
        private Func<float, Field, Vector3, float> CombinatorPosition { get; set; }
        private readonly Field[] F;
        CombinedField(Field f, Field g, CombineBy c)
        {
            CombinatorParticle = CombinedField.CombineFunction<Particle>(c);
            CombinatorPosition = CombinedField.CombineFunction(c);
            F = new Field[2];
            F[0] = f;
            F[1] = g;
        }

        private static Func<float, Field, T, float> CombineFunction<T>(CombineBy c) where T : Particle
        {
            Func<float, Field, T, float> f = (x, y, p) => 0.0f;
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
                    f = (x, y, p) => (float)System.Math.Pow(x, y.Value(p));
                    break;
            }
            return f;
        }
        private static Func<float, Field, Vector3, float> CombineFunction(CombineBy c)
        {
            Func<float, Field, Vector3, float> f = (x, y, p) => 0.0f;
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
                    f = (x, y, p) => (float)System.Math.Pow(x, y.Value(p));
                    break;
            }
            return f;
        }
        CombinedField(Field[] f, CombineBy c)
        {
            CombinatorParticle = CombinedField.CombineFunction<Particle>(c);
            CombinatorPosition = CombinedField.CombineFunction(c);
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
        public Utilities.ErrorCodes Update(GameTime time, World world = null, GameState state = null)
        {
            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {
            return Utilities.ErrorCodes.SUCCESS;
        }
    }
}