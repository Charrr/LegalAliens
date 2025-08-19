using System.Collections;
using UnityEngine;

public class Character_Controller : MonoBehaviour
{
    [Header("References")]
    public Animator Character_Animator;
    public VoiceOver_script VoiceOver; 

    [Header("Floating Offset")]
    public Vector3 Offset = new Vector3(0, -0.5f, 2f); // adjust in inspector
    public GameObject CameraTarget; // drag your AR camera object here
    public Vector3 LocalRotationEuler = Vector3.zero; // additional local rotation

    [Header("Dialogue")]
    public string[] Happy_phrases;
    public string[] Sad_phrases;
    public string[] Question_phrases;
    public float Speaking_delay = 3f;

    private string Words_to_Speak;

    void Update()
    {
        // Keep floating in front of the target object
        if (CameraTarget != null)
        {
            Transform cam = CameraTarget.transform;

            // Position offset relative to camera
            transform.position = cam.position 
                               + cam.forward * Offset.z 
                               + cam.up * Offset.y
                               + cam.right * Offset.x;

            // Face the camera
            Quaternion lookRot = Quaternion.LookRotation(transform.position - cam.position);

            // Apply additional local rotation
            transform.rotation = lookRot * Quaternion.Euler(LocalRotationEuler);
        }
    }

    // --- HAPPY STATE ---
    [ContextMenu("Become Happy")]
    public void Become_Happy()
    {
        StopAllCoroutines();
        StartCoroutine(HappyRoutine());
    }

    private IEnumerator HappyRoutine()
    {
        Character_Animator.SetBool("Happy", true);

        Words_to_Speak = Happy_phrases[Random.Range(0, Happy_phrases.Length)];
        VoiceOver.Speak_Now(Words_to_Speak);

        yield return new WaitForSeconds(Speaking_delay);

        Character_Animator.SetBool("Happy", false);
    }

    // --- SAD STATE ---
    [ContextMenu("Become Sad")]
    public void Become_Sad()
    {
        StopAllCoroutines();
        StartCoroutine(SadRoutine());
    }

    private IEnumerator SadRoutine()
    {
        Character_Animator.SetBool("Sad", true);

        Words_to_Speak = Sad_phrases[Random.Range(0, Sad_phrases.Length)];
        VoiceOver.Speak_Now(Words_to_Speak);

        yield return new WaitForSeconds(Speaking_delay);

        Character_Animator.SetBool("Sad", false);
    }

    // --- SPEAKING STATE ---
    public void Ask_a_question(int index)
    {
        StopAllCoroutines();
        StartCoroutine(TalkRoutine(index));
    }

    private IEnumerator TalkRoutine(int index)
    {
        Character_Animator.SetBool("Talk", true);

        if (index >= 0 && index < Question_phrases.Length)
        {
            Words_to_Speak = Question_phrases[index];
            VoiceOver.Speak_Now(Words_to_Speak);
        }
        else
        {
            Debug.LogWarning("Ask_a_question index out of range!");
        }

        yield return new WaitForSeconds(Speaking_delay);

        Character_Animator.SetBool("Talk", false);
    }
}
