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
        private AudioSource _attackSource, _footstepsSource, _potionsSource, _spellsSource, _equipSource, _openContainerSource, _miscSource;

        [SerializeField]
        private AudioClip _errorBip, _selectBip;

        public void PlayAttackClip(AudioClip clip) => _attackSource.PlayOneShot(clip);
        public void PlayFootstepsClip(AudioClip clip) => _footstepsSource.PlayOneShot(clip);
        public void PlayPotionsClip(AudioClip clip) => _potionsSource.PlayOneShot(clip);
        public void PlaySpellsClip(AudioClip clip) => _spellsSource.PlayOneShot(clip);
        public void PlayEquipClip(AudioClip clip) => _equipSource.PlayOneShot(clip);
        public void PlayOpenContainerClip(AudioClip clip) => _openContainerSource.PlayOneShot(clip);
        public void PlayMiscClip(AudioClip clip) => _miscSource.PlayOneShot(clip);

        public void PlayError()
        {
            _miscSource.PlayOneShot(_errorBip);
        }

        public void PlaySelectBip()
        {
            _miscSource.PlayOneShot(_selectBip);
        }
    }
}
