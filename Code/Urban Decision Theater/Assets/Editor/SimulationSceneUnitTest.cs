using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using UnityEngine.SceneManagement;

namespace UnityTest
{
    // Unity has an error 
    // The return of the currently active scene build index is -1
    // Make sure to run this in the SimulationScene only!
    public class SimulationSceneUnitTest
    {
        // Group test: Test that the GO is active and the scale is the correct scale.
        [Test]
        public void UrbanModelTest()
        {
            try
            {
                GameObject go = GameObject.Find("Miami");
                Vector3 scale = go.transform.lossyScale;
                Assert.True(go.activeInHierarchy, "Not active");
                Assert.True(scale.x == 1 && scale.y == 1 && scale.z == 1, "Scale should be Vector3(1,1,1)");
            }
            catch (AssertionException e)
            {
                Assert.Fail(e.Message);
            }
        }

        // Group test: Test that the GO is active and the water is at the initial starting point
        [Test]
        public void WaterTest()
        {
            try
            {
                GameObject go = GameObject.Find("Water");
                float y = go.transform.position.y;            
                Assert.True(go.activeInHierarchy, "Not active");
                Assert.True(y == 3, "Initial Starting of Water should be y = 3");
            }
            catch(AssertionException e)
            {
                Assert.Fail(e.Message);
            }
            
        }

        // Group test: Test that the GO is active and the terrain is at the fixed point
        [Test]
        public void TerrainTest()
        {
            try
            {
                GameObject go = GameObject.Find("Miami_Terrain");
                Vector3 pos = go.transform.position;
                Assert.True(go.activeInHierarchy, "Not active");
                Assert.True(pos.x == 0 && pos.y == 0 && pos.z == 0, "Position should be Vector3(0,0,0)");
            }            
            catch (AssertionException e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}

