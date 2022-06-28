using System.Collections;
using TouhouPrideGameJam4.Dialog;
using TouhouPrideGameJam4.Game.Persistency;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TouhouPrideGameJam4.Test
{
    public class VNTests
    {
        [UnityTest]
        public IEnumerator AreStoriesValid()
        {
            var go = new GameObject("PersistencyManager", typeof(PersistencyManager));
            go.GetComponent<PersistencyManager>().StoryProgress = StoryProgress.Done;
            SceneManager.LoadScene("VNUI", LoadSceneMode.Additive);
            yield return null;
            StoryManager.Instance.ParseAllStories();
        }
    }

}