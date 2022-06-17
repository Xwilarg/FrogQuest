using System.Collections;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TouhouPrideGameJam4.Test
{
    public class VNTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void VNTestsSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator VNTestsWithEnumeratorPasses()
        {
            SceneManager.LoadScene("VNUI", LoadSceneMode.Additive);
            yield return null;
            StoryManager.Instance.ParseAllStories();
        }
    }

}