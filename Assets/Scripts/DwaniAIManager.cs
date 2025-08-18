using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LegalAliens
{
    // 8/18/2025 AI-Tag
    // This was created with the help of Assistant, a Unity Artificial Intelligence product.
    public class DwaniAIManager : MonoBehaviour
    {
        [SerializeField] private string _serverUrl = "https://api.dwani.ai/detect/";
        [SerializeField] private Texture2D _testImage;

        [ContextMenu("Send Request")]
        private void SendRequest()
        {
            StartCoroutine(SendImageCoroutine());
        }

        private IEnumerator SendImageCoroutine()
        {
            if (_testImage == null)
            {
                Debug.LogError("No image assigned!");
                yield break;
            }

            Texture2D readableImage = Utility.EnsureReadable(_testImage);
            // Encode to JPG
            byte[] jpgBytes = readableImage.EncodeToJPG();

            // Create form
            WWWForm form = new WWWForm();
            form.AddBinaryData("image_file", jpgBytes, "test_image_living_room.jpg", "image/jpeg");

            using (UnityWebRequest www = UnityWebRequest.Post(_serverUrl, form))
            {
                www.SetRequestHeader("accept", "application/json");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + www.error);
                    Debug.LogError(www.downloadHandler.text);
                }
                else
                {
                    Debug.Log("Response: " + www.downloadHandler.text);
                }
            }
        }
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
