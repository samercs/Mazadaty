using NUnit.Framework;

namespace Mzayad.Services.Tests
{
    [TestFixture]
    public class LevelServiceTest
    {
        [Test]
        public void GetLevel_LessThanOne_ReturnLevelZero()
        {
            var results = new LevelService().GetLevel(0);
            Assert.AreEqual(0, results.XpRequired);
        }

        [Test]
        public void GetLevel_Level_5_XP()
        {
            var results = new LevelService().GetLevel(5);
            Assert.AreEqual(1000, results.XpRequired);
        }
        
        [Test]
        public void GetLevel_Level_5_Tokens()
        {
            var results = new LevelService().GetLevel(5);
            Assert.AreEqual(25, results.TokensAwarded);
        }

        [Test]
        public void GetLevel_Level_10_XP()
        {
            var results = new LevelService().GetLevel(10);
            Assert.AreEqual(4500, results.XpRequired);
        }

        [Test]
        public void GetLevel_Level_10_Tokens()
        {
            var results = new LevelService().GetLevel(10);
            Assert.AreEqual(50, results.TokensAwarded);
        }
    }
}
