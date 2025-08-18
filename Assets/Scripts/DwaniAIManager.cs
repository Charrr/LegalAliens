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

        public event Action OnSendRequest;
        public event Action<string> OnReceiveResponse;

        [ContextMenu("Send Request")]
        public void SendRequest(Texture2D image)
        {
            StartCoroutine(SendImageCoroutine(image));
        }

        private IEnumerator SendImageCoroutine(Texture2D image)
        {
            if (image == null)
            {
                Debug.LogError("No image assigned!");
                yield break;
            }

            Texture2D readableImage = Utility.EnsureReadable(image);
            // Encode to JPG
            byte[] jpgBytes = readableImage.EncodeToJPG();

            // Create form
            WWWForm form = new WWWForm();
            form.AddBinaryData("image_file", jpgBytes, image.name, "image/jpeg");
            OnSendRequest?.Invoke();

            using (UnityWebRequest www = UnityWebRequest.Post(_serverUrl, form))
            {
                www.SetRequestHeader("accept", "application/json");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + www.error);
                }
                else
                {
                    Debug.Log("Response: " + www.downloadHandler.text);
                    OnReceiveResponse?.Invoke(www.downloadHandler.text);
                }
            }
        }
    }
}
