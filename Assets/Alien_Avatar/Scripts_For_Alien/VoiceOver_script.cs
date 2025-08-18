using UnityEngine;
using Meta.WitAi.TTS.Utilities;

public class VoiceOver_script : MonoBehaviour
{
    private TTSSpeaker speaker;

    private void Awake()
    {
        // Cache the TTSSpeaker component in the scene
        speaker = GameObject.FindObjectOfType<TTSSpeaker>();
        if (speaker == null)
        {
            Debug.LogError("No TTSSpeaker found in the scene!");
        }
    }

    public void Speak_Now(string text)
    {
        if (speaker != null && !string.IsNullOrEmpty(text))
        {
            speaker.Speak(text);
        }
        else
        {
            Debug.LogWarning("Speak_Now called with empty text or missing speaker.");
        }
    }

    public void Stop_Speaking()
    {
        if (speaker != null)
        {
            speaker.Stop();
        }
    }
}
