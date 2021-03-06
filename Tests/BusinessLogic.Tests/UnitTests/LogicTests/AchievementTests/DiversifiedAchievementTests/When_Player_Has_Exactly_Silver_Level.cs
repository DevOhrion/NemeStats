using System.Linq;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.DiversifiedAchievementTests
{
    [TestFixture]
    public class When_Player_Has_Exactly_Silver_Level : Base_DiversifiedAchievementTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.InsertPlayedGames(Achievement.ClassUnderTest.LevelThresholds[AchievementLevel.Silver], this.PlayerId);
            this.InsertPlayedGames(Achievement.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze], OtherPlayerId);
        }

        [Test]
        public void Then_Returns_Silver_Achievement()
        {
            var result = Achievement.ClassUnderTest.IsAwardedForThisPlayer(PlayerId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.LevelAwarded.HasValue);

            Assert.That(result.LevelAwarded.Value, Is.EqualTo(AchievementLevel.Silver));
            foreach (var game in PlayedGames.Where(c => c.PlayerId == PlayerId))
            {
                Assert.IsTrue(result.RelatedEntities.Contains(game.PlayedGame.GameDefinitionId));
            }
        }
    }
}