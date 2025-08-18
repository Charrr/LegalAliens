using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LegalAliens
{
    public class OpenAIManager : MonoBehaviour
    {
        [SerializeField, TextArea(4, 20)] private string _prompt = "Tell three individual objects you see in this image. Answer in 3 words, one for each object..";
        [SerializeField, TextArea(4, 50)] private string _generateQuizPrompt = 
            "Draft a quiz with 3 questions regarding the object in the image. " +
            "The questions should be very easy. Your response should be a pure json, with objects of the following definition: \r\n" +
            "public class QuizData\r\n    {\r\n        public string QuizzedObjectName;\r\n        public QuizQuestion[] QuizQuestions;\r\n    }\r\n    public class QuizQuestion\r\n    {\r\n        public string Question;\r\n        public string[] Options;\r\n        public string Answer;\r\n    }";

        [SerializeField] private Texture2D _image;
        private string _apiKey;

        private void Awake()
        {
            string keyPath = Path.Combine(Application.dataPath, "Settings", "openai-api-key.txt");
            try
            {
                _apiKey = File.ReadAllText(keyPath).Trim();
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to read API key: {e.Message}");
                _apiKey = "";
            }
        }

        public void PromptGenerateQuizBasedOnImage(Texture2D image)
        {
            StartCoroutine(SendPromptWithImageCoroutine(_generateQuizPrompt, image));
        }

        [ContextMenu("Send Image Prompt")]
        private void SendImagePrompt()
        {
            //StartCoroutine(SendPromptWithImageCoroutine(_prompt, _image));
            PromptGenerateQuizBasedOnImage(_image);
        }

        [ContextMenu("Send Text Prompt")]
        private void SendTextPrompt()
        {
            StartCoroutine(SendPromptWithImageCoroutine(_prompt));
        }

        private IEnumerator SendPromptWithImageCoroutine(string prompt, Texture2D image = null)
        {
            string dataUrl = "";

            if (image != null)
            {
                // Ensure we have a readable, uncompressed copy to avoid EncodeToJPG errors.
                Texture2D readable = EnsureReadable(image);
                if (readable == null)
                {
                    Debug.LogError("Failed to make image readable.");
                    yield break;
                }

                // Convert Texture2D → JPG bytes → base64 (use 85–90 quality to keep payload size reasonable)
                byte[] jpgBytes = readable.EncodeToJPG(90);
                string b64Image = System.Convert.ToBase64String(jpgBytes);
                dataUrl = "data:image/jpeg;base64," + b64Image;
            }

            // Escape any quotes/newlines in the prompt
            string safePrompt = prompt
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");

            // Build JSON for the Responses API (image_url must be a STRING)
            string jsonBody = "{"
                + "\"model\":\"gpt-4o-mini\","
                + "\"input\":[{"
                + "\"role\":\"user\","
                + "\"content\":["
                + "{ \"type\":\"input_text\", \"text\":\"" + safePrompt + "\" }"
                + (image == null ? "" : ",{ \"type\":\"input_image\", \"image_url\":\"" + dataUrl + "\" }")
                + "]"
                + "}]"
                + "}";

            var request = new UnityWebRequest("https://api.openai.com/v1/responses", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error + "\n" + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Full JSON:\n" + request.downloadHandler.text);
                Debug.Log("Extracted output: " + OpenAIResponseParser.ExtractText(request.downloadHandler.text));
            }
        }

        // Makes a readable, uncompressed copy if needed (avoids EncodeToJPG errors on compressed textures).
        private Texture2D EnsureReadable(Texture2D src)
        {
            try
            {
                // If already readable in a supported format, just return it.
                src.GetPixels32(); // will throw if not readable
                return src;
            }
            catch { /* fall through */ }

            // RenderTexture path to force-decompress/copy into a readable Texture2D
            var rt = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            var prev = RenderTexture.active;
            Graphics.Blit(src, rt);
            RenderTexture.active = rt;

            var copy = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false, true);
            copy.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0);
            copy.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            return copy;
        }
    }

}