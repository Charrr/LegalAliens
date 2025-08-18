using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegalAliens
{
    public class ObjectSelector : MonoBehaviour
    {
        [SerializeField] private Texture2D _testImage;
        [SerializeField] private DwaniAIResponse.DetectionData _detectionData;
        private DwaniAIManager _dwaniAIManager;

        private void Awake()
        {
            if (!_dwaniAIManager)
            {
                _dwaniAIManager = FindAnyObjectByType<DwaniAIManager>();
            }
        }

        private void Start()
        {
            _dwaniAIManager.OnReceiveResponse += ProcessDwaniResponse;
        }

        private void OnDestroy()
        {
            _dwaniAIManager.OnReceiveResponse -= ProcessDwaniResponse;
        }

        private void ProcessDwaniResponse(string json)
        {
            _detectionData = JsonUtility.FromJson<DwaniAIResponse.DetectionData>(json);
        }
    } 
}
