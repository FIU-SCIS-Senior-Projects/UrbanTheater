using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using NUnit.Framework;
using System;

namespace UnityTest
{
    public class StartMenuUnitTest 
    {
        [Test]
        public void SceneZeroExists()
        {
            Assert.That(SceneManager.GetSceneAt(0).name == "StartMenu");
        }

        [Test]
        public void CanvasMenuTest()
        {
            GameObject menu = null;
            // Are we in the right scene?
            try
            {
                if (SceneManager.GetSceneAt(0).name == "StartMenu")
                {
                    menu = GameObject.Find("Menu");
                    if (menu.activeInHierarchy && menu.transform.parent.gameObject.name == "Canvas")
                    {
                        Assert.Pass();
                    }
                    else
                    {
                        Assert.Fail("Can't find Menu gameObject");
                    }
                }
            }
            catch(AssertionException e)
            {
                Assert.Fail("Wrong Scene or " + e.Message);
            }
        }

        [Test]
        public void BeginButtonTest()
        {
            try
            {
                if (SceneManager.GetSceneAt(0).name == "StartMenu")
                {
                    GameObject go = GameObject.Find("BeginBtn");
                    Button b = go.GetComponent<Button>();
                    //b.SendMessageUpwards("LoadScene", "SimulationScene");
                    b.onClick.Invoke();
                }
            }
            catch (AssertionException e)
            {
                Assert.Fail("Button name error or Button missing or: " + e.Message);
            }
            Assert.Pass();
        }

		//slider Unit test
		[Test]
		public void SliderText_match_Test ()
		{
			GameObject sl = GameObject.Find ("Budget Slider");
			Slider s = sl.GetComponent <UnityEngine.UI.Slider>();
			string sliderValue = s.value.ToString (); 
			Assert.That(sliderValue.Contains("20"));
		}

        //[Test]
        //public void BeginButtonFunctionTest()
        //{
        //    try
        //    {
        //        SceneManager.LoadScene("SimulationScene");
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.Fail(e.Message);
        //    }
        //    Assert.That(SceneManager.sceneCountInBuildSettings == 2);
        //}
        
    }
}
