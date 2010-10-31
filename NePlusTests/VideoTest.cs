using NePlus;
using NePlus.EngineComponents;

using NUnit.Framework;

namespace NePlusTests
{
    [TestFixture]
    public class VideoTest
    {
        [Test]
        public void TestVideo()
        {
            Game1 game = new Game1();
            Engine engine = new Engine(game);

            Assert.AreEqual(engine.Video.Height, 720);
            Assert.AreEqual(engine.Video.Width, 1280);
        }
    }
}
