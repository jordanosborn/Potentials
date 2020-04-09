using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase(8, 9, 20, 37)]
        [TestCase(7, 2, 5, 14)]
        public void Add(int x, int y, int z, int answer)
        {
            Assert.AreEqual(x + y + z, answer);
        }
    }
}