using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace PatiAdventure.Tests
{
    public sealed class FollowCameraTests
    {
        private readonly List<GameObject> objects = new List<GameObject>();
        private Transform target;
        private FollowCamera follow;
        private Vector3 Focus => target.position + Vector3.up * 1.1f;

        [SetUp] public void Setup()
        {
            Time.timeScale = 1;
            var actor = new GameObject("Camera test target");objects.Add(actor);
            target = actor.transform;target.position = new Vector3(200, 10, 200);
            var cameraObject = new GameObject("Camera test camera");objects.Add(cameraObject);
            cameraObject.AddComponent<Camera>().enabled = false;
            follow = cameraObject.AddComponent<FollowCamera>();follow.target = target;
        }

        [UnityTearDown] public IEnumerator Cleanup()
        {
            Time.timeScale = 1;
            foreach (var obj in objects) if (obj) Object.Destroy(obj);
            objects.Clear();yield return null;
        }

        private GameObject Wall()
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);objects.Add(wall);
            wall.transform.position = target.position + new Vector3(0, 3, -2);
            wall.transform.localScale = new Vector3(8, 8, .4f);
            Physics.SyncTransforms();return wall;
        }

        [Test] public void SnapStopsBeforeAnObstructionImmediately()
        {
            Wall();follow.Snap();
            Assert.Greater(follow.transform.position.z, target.position.z - 1.7f,
                "Snap must be safe before the next LateUpdate, including after travel or respawn.");
            Assert.Less(Vector3.Distance(follow.transform.position, Focus), 2);
        }

        [UnityTest] public IEnumerator PausedDragDoesNotJumpOnResume()
        {
            follow.Snap();var rotation = follow.transform.rotation;
            Time.timeScale = 0;follow.TouchLook = new Vector2(75, 50);yield return null;
            Assert.AreEqual(Vector2.zero, follow.TouchLook);
            Assert.Less(Quaternion.Angle(rotation, follow.transform.rotation), .001f);
            Time.timeScale = 1;yield return null;
            Assert.Less(Quaternion.Angle(rotation, follow.transform.rotation), .001f,
                "A paused drag must not be replayed on the first unpaused frame.");
        }

        [UnityTest] public IEnumerator CameraPullsOutGraduallyAfterObstacleDisappears()
        {
            var wall = Wall();follow.Snap();
            float blockedDistance = Vector3.Distance(follow.transform.position, Focus);
            Object.Destroy(wall);yield return null;
            float firstDistance = Vector3.Distance(follow.transform.position, Focus);
            Assert.GreaterOrEqual(firstDistance, blockedDistance);
            Assert.Less(firstDistance, follow.distance - 1,
                "Removing a wall should not snap the view straight to the full distance.");
            yield return new WaitForSeconds(2);
            Assert.AreEqual(follow.distance, Vector3.Distance(follow.transform.position, Focus), .05f);
        }

        [Test] public void SnapIgnoresPlayerChildrenAndTriggers()
        {
            var child = GameObject.CreatePrimitive(PrimitiveType.Cube);objects.Add(child);
            child.transform.SetParent(target, false);child.transform.localPosition = new Vector3(0, 2, -2);
            child.transform.localScale = new Vector3(8, 8, .4f);
            var trigger = Wall();trigger.GetComponent<Collider>().isTrigger = true;
            Physics.SyncTransforms();follow.Snap();
            Assert.AreEqual(follow.distance, Vector3.Distance(follow.transform.position, Focus), .001f,
                "The player's own shapes and portal triggers must not push the view inward.");
        }
    }
}
