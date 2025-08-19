using UnityEngine;

public class Test_Talk : MonoBehaviour
{
    public Character_Controller character;
    public int QuestionIndex = 0;

    void OnEnable()
    {
        if (character != null)
            character.Ask_a_question(QuestionIndex);
    }
}