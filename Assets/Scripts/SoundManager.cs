using UnityEngine;

namespace LegalAliens
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _soundSource;
        [SerializeField] private AudioClip _gameOverSound;

        private void Awake()
        {
            if (!_soundSource) _soundSource = GetComponent<AudioSource>();
        }

        public void PlayAudioClip(AudioClip clip)
        {
            _soundSource.clip = clip;
            _soundSource.Play();
        }

        [ContextMenu("Play Game Over Sound")]
        public void PlayGameOverSound()
        {
            PlayAudioClip(_gameOverSound);
        }
    } 
}
