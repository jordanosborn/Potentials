using Microsoft.Xna.Framework;
namespace Potential
{

    public interface Field : GameObject
    {
        float ValueAt(Vector3 position);
        float ValueFor(Particle p);
        Vector3 ForceAt(Vector3 position);
        Vector3 ForceFor(Particle p);
    }
}