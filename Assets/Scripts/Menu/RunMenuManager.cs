using UnityEngine;
using UnityEngine.SceneManagement;

namespace TouhouPrideGameJam4.Menu
{
    public class RunMenuManager : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
