using UnityEngine;

public class Test_Happy : MonoBehaviour
{
    public Character_Controller character;

    void OnEnable()
    {
        if (character != null)
            character.Become_Happy();
    }
}
