using System;
using NUnit.Framework;
using Mzayad.Models;

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
        public void GetLevel_Level_5_XP_125()
        {
            var results = new LevelService().GetLevel(5);
            Assert.AreEqual(125, results.XpRequired);
        }
        
        [Test]
        public void GetLevel_Level_5_Tokens_25()
        {
            var results = new LevelService().GetLevel(5);
            Assert.AreEqual(25, results.TokensAwarded);
        }

        [Test]
        public void GetLevel_Level_10_XP_500()
        {
            var results = new LevelService().GetLevel(10);
            Assert.AreEqual(500, results.XpRequired);
        }

        [Test]
        public void GetLevel_Level_10_Tokens_50()
        {
            var results = new LevelService().GetLevel(10);
            Assert.AreEqual(50, results.TokensAwarded);
        }
    }
}
