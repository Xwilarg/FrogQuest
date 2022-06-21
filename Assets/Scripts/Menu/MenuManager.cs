using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Button _exitButton;

        private void Start()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                _exitButton.gameObject.SetActive(false);
            }
        }

        public void Play()
        {
            SceneManager.LoadScene("MenuRuns");
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
