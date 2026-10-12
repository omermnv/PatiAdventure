using NUnit.Framework;
using UnityEngine;
namespace PatiAdventure.Tests
{
    public sealed class ProgressTests
    {
        [Test] public void FirstForestRequiresPracticeGuideRescueAndBeacon()
        {
            var save=new AdventureSaveData();for(int i=0;i<5;i++)save.stars.Add("star"+i);
            Assert.IsFalse(save.CanFinish(0,0));save.trainingComplete=true;save.guides.Add("Ada");save.friends.Add("Mino");
            Assert.IsFalse(save.CanFinish(0,0));save.beacons.Add("beacon");Assert.IsTrue(save.CanFinish(0,0));
        }
        [Test] public void PickupsCannotBeCountedTwice()
        { var save=new AdventureSaveData();Assert.IsTrue(AdventureSaveData.AddOnce(save.stars,"same"));Assert.IsFalse(AdventureSaveData.AddOnce(save.stars,"same"));Assert.AreEqual(1,save.stars.Count); }
        [Test] public void CheckpointAndCollectiblesSurviveSerialization()
        {
            var save=new AdventureSaveData {level=2,hasCheckpoint=true,checkpoint=new Vector3(1,.3f,22),trainingComplete=true};
            save.stars.Add("star1");save.friends.Add("friend1");save.beacons.Add("beacon1");save.chests.Add("chest1");
            var restored=AdventureSave.Decode(JsonUtility.ToJson(save));Assert.AreEqual(save.level,restored.level);Assert.AreEqual(save.checkpoint,restored.checkpoint);
            CollectionAssert.AreEqual(save.stars,restored.stars);CollectionAssert.AreEqual(save.friends,restored.friends);CollectionAssert.AreEqual(save.chests,restored.chests);
        }
        [Test] public void InvalidSaveFallsBackSafely()
        { Assert.AreEqual(0,AdventureSave.Decode("invalid").level);Assert.AreEqual(0,AdventureSave.Decode("{\"version\":9,\"level\":20}").level); }
        [Test] public void SavedLevelIsClamped()
        { var save=AdventureSave.Decode("{\"version\":2,\"level\":999}");Assert.AreEqual(36,save.level);Assert.NotNull(save.stars); }
        [TestCase(1)] [TestCase(2)] public void LaterStagesRequireAllThreeObjectives(int stage)
        {
            var save=new AdventureSaveData();for(int i=0;i<5;i++)save.stars.Add(i.ToString());
            Assert.IsFalse(save.CanFinish(1,stage));var list=stage==1?save.friends:save.beacons;for(int i=0;i<3;i++)list.Add(i.ToString());Assert.IsTrue(save.CanFinish(1,stage));
        }
        [Test] public void StoryKeepsTwelveBilingualRegions()
        { Assert.AreEqual(12,AdventureStory.TurkishNames.Length);Assert.AreEqual(12,AdventureStory.EnglishNames.Length);Assert.AreEqual(12,AdventureStory.TurkishStory.Length);Assert.AreEqual(12,AdventureStory.EnglishStory.Length); }
    }
}
