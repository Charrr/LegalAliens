using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

namespace LegalAliens
{
    public static class OpenAIResponseParser
    {
        [Serializable]
        private class Root
        {
            public string output_text;       // convenience field (if present)
            public OutputItem[] output;      // fallback path
        }

        [Serializable]
        private class OutputItem
        {
            public string type;              // e.g., "message"
            public string role;              // e.g., "assistant"
            public ContentItem[] content;    // array of content blocks
        }

        [Serializable]
        private class ContentItem
        {
            public string type;              // e.g., "output_text" or "text"
            public string text;              // the actual text
        }

        /// <summary>
        /// Extract the assistant's text from a Responses API JSON payload.
        /// Returns null or empty string if nothing could be found.
        /// </summary>
        public static string ExtractText(string json)
        {
            if (string.IsNullOrEmpty(json)) return string.Empty;

            Root root = null;
            try
            {
                root = JsonConvert.DeserializeObject<Root>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to parse JSON with JsonUtility: " + e.Message);
                return string.Empty;
            }

            // 1) Fast path: "output_text" (added by the Responses API for convenience)
            if (root != null && !string.IsNullOrEmpty(root.output_text))
                return root.output_text.Trim();

            // 2) Fallback path: walk "output" → items of type "message" → content entries
            if (root?.output != null && root.output.Length > 0)
            {
                var sb = new StringBuilder();
                foreach (var item in root.output)
                {
                    if (item == null) continue;
                    if (!string.Equals(item.type, "message", StringComparison.OrdinalIgnoreCase)) continue;
                    if (item.content == null) continue;

                    foreach (var c in item.content)
                    {
                        if (c == null || string.IsNullOrEmpty(c.text)) continue;
                        // OpenAI may use "output_text" (preferred) or sometimes plain "text"
                        if (string.Equals(c.type, "output_text", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(c.type, "text", StringComparison.OrdinalIgnoreCase))
                        {
                            if (sb.Length > 0) sb.Append("\n");
                            sb.Append(c.text.Trim());
                        }
                    }
                }
                return sb.ToString();
            }

            return string.Empty;
        }
    }

}