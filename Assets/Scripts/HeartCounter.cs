using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LegalAliens
{
    public class HeartCounter : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int _maxHearts = 3;
        [SerializeField] private Image _heartPrefab;

        [Header("Object References")]
        [SerializeField] private List<Image> _heartImages;
        [SerializeField] private SoundManager _soundManager;

        public int HeartCount => _heartImages.Count;
        public event Action<int> OnHeartCountChanged;
        public event Action OnLoseAllHeats;

        private void Awake()
        {
            if (!_soundManager) _soundManager = FindAnyObjectByType<SoundManager>();
        }

        private void Start()
        {
            ResetHearts();
        }

        [ContextMenu("Initialize Hearts")]
        public void LoseOneHeart()
        {
            if (_heartImages.Count == 0)
            {
                Debug.LogWarning("There is no heart left. How could this happen???");
                return;
            }

            Destroy(_heartImages[0].gameObject);
            _heartImages.RemoveAt(0);
            OnHeartCountChanged?.Invoke(HeartCount);
            if (HeartCount == 0)
            {
                OnLoseAllHeats?.Invoke();
                _soundManager.PlayGameOverSound();
                // TODO: VFX SFX etc.
                Debug.Log("YOU DIED!!!!");
            }
        }

        [ContextMenu("Reset Hearts")]
        public void ResetHearts()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            _heartImages = new();
            for (int i = 0; i < _maxHearts; i++)
            {
                Image heartImage = Instantiate(_heartPrefab, transform);
                _heartImages.Add(heartImage);
            }
        }
    } 
}
