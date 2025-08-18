using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegalAliens
{
    public class DwaniAIResponseParser : MonoBehaviour
    {

    }

    public class DwaniAIResponse
    {

        [Serializable]
        public class Detection
        {
            public List<int> box; // The bounding box coordinates
            public float confidence; // Confidence score
            public int class_id; // Class ID
            public string label; // Label of the detected object
        }

        [Serializable]
        public class DetectionData
        {
            public List<Detection> detections; // List of detections
        }
    }
}
