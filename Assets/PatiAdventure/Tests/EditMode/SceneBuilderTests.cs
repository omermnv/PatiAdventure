using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
namespace PatiAdventure.Tests
{
    public sealed class SceneBuilderTests
    {
        [Test] public void GeneratorUsesUniquePathsAndPreservesActiveScene()
        {
            var active=SceneManager.GetActiveScene();var roots=active.GetRootGameObjects().Length;
            if(string.IsNullOrEmpty(active.path))
            {
                if(!File.Exists("ValidationResults/GeneratedScene.txt"))Assert.Ignore("Save the current untitled scene before testing additive generation.");
                EditorSceneManager.SaveScene(active,AssetDatabase.GenerateUniqueAssetPath("Assets/PatiAdventure/Generated/TestActive.unity"));
            }
            PatiAdventure.Editor.SunnyForestBuilder.Create();string first=AssetDatabase.GetAssetPath(Selection.activeObject);string content=File.ReadAllText(first);
            PatiAdventure.Editor.SunnyForestBuilder.Create();string second=AssetDatabase.GetAssetPath(Selection.activeObject);
            Assert.AreNotEqual(first,second);Assert.AreEqual(content,File.ReadAllText(first));Assert.AreEqual(active.handle,SceneManager.GetActiveScene().handle);Assert.AreEqual(roots,active.GetRootGameObjects().Length);
        }
    }
}
