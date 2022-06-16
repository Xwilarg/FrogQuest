using System.Collections;
using UnityEngine;

namespace TouhouPrideGameJam4.Sound
{
    public class BGMManager : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _intro, _loop;

        private AudioSource _source;

        private void Start()
        {
            _source = GetComponent<AudioSource>();
            _source.clip = _intro;
            _source.Play();
            StartCoroutine(WaitIntroAndLoop());
        }

        private IEnumerator WaitIntroAndLoop()
        {
            yield return new WaitUntil(() => !_source.isPlaying);
            _source.clip = _loop;
            _source.loop = true;
            _source.Play();
        }
    }
}
