namespace Potential
{
    public interface GameObject
    {
        Utilities.ErrorCodes Update(World world);
        Utilities.ErrorCodes Draw(object window);
    }
}