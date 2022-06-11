using UnityEngine;

namespace TouhouPrideGameJam4.Sound
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        private AudioSource _source;

        [SerializeField]
        private AudioClip _errorBip, _selectBip;

        public void PlayClip(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }

        public void PlayError()
        {
            _source.PlayOneShot(_errorBip);
        }

        public void PlaySelectBip()
        {
            _source.PlayOneShot(_selectBip);
        }
    }
}
