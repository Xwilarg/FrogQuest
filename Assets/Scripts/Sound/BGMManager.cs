using System.Collections;
using UnityEngine;

namespace TouhouPrideGameJam4.Sound
{
    public class BGMManager : MonoBehaviour
    {
        public static BGMManager Instance { get; private set; }

        private AudioSource _source;

        private void Awake()
        {
            Instance = this;
            _source = GetComponent<AudioSource>();
        }

        public void SetSong(AudioClip start, AudioClip main)
        {
            if (start != null)
            {
                _source.loop = false;
                _source.clip = start;
                _source.Play();
                StartCoroutine(WaitIntroAndLoop(main));
            }
            else
            {
                _source.loop = true;
                _source.clip = main;
                _source.Play();
            }
        }

        private IEnumerator WaitIntroAndLoop(AudioClip main)
        {
            yield return new WaitUntil(() => !_source.isPlaying);
            _source.clip = main;
            _source.loop = true;
            _source.Play();
        }
    }
}
