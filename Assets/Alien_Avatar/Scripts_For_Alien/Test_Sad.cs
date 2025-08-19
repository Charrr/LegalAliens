using UnityEngine;

public class Test_Sad : MonoBehaviour
{
    public Character_Controller character;

    void OnEnable()
    {
        if (character != null)
            character.Become_Sad();
    }
}
