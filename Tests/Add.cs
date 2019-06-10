using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(8, 9, 20)]
        public void Add(int x, int y, int z)
        {
            Assert.AreEqual(x + y + z, 37);
        }
    }
}