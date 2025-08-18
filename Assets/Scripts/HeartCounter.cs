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

        public int HeartCount => _heartImages.Count;
        public event Action<int> OnHeartCountChanged;
        public event Action OnLoseAllHeats;

        private void Start()
        {
            ResetHearts();
        }

        [ContextMenu("Initialize Hearts")]
        public void LoseOneHeart()
        {
            Destroy(_heartImages[0].gameObject);
            _heartImages.RemoveAt(0);
            OnHeartCountChanged?.Invoke(HeartCount);
            if (HeartCount == 0)
            {
                OnLoseAllHeats?.Invoke();
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
