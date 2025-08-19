using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PageController pageController;
    [SerializeField] GameObject menuRoot;
    [SerializeField] Transform startPage;

    [Header("UI Elements")]
    public TMP_Text Start_button_field;       // text field for Start button
    public string[] Start_Button_text = new string[2]; // array with 2 strings
    public GameObject Questions_object;       // object to deactivate when reopening menu

    private void Start()
    {
        // Make sure menu is visible on start
        ToggleMenu(true);

        // Put string[0] into the Start button text field
        if (Start_button_field != null && Start_Button_text.Length > 0)
        {
            Start_button_field.text = Start_Button_text[0];
        }

        // Open the start page
        OpenStartPage();
    }

    public void OpenStartPage()
    {
        pageController.OpenPage(startPage);
    }

    public void ToggleMenu(bool isOn)
    {
        if (menuRoot != null)
        {
            menuRoot.SetActive(isOn);
        }
    }

    public void Open_menu()
    {
        // Put string[1] into Start button text field
        if (Start_button_field != null && Start_Button_text.Length > 1)
        {
            Start_button_field.text = Start_Button_text[1];
        }

        // If Questions_object is active, deactivate it
        if (Questions_object != null && Questions_object.activeSelf)
        {
            Questions_object.SetActive(false);
        }

        // Open the menu
        ToggleMenu(true);
        OpenStartPage();
    }
}