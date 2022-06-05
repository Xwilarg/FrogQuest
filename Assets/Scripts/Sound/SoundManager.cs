using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private AudioClip _errorBip;

        public void PlayClip(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }

        public void PlayError()
        {
            _source.PlayOneShot(_errorBip);
        }
    }
}
