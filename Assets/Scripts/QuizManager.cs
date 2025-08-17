using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegalAliens
{
    public class QuizManager : MonoBehaviour
    {
        [Header("Asset References")]
        [SerializeField] private Button _btnAnswerOptionPrefab;

        [Header("Object References")]
        [SerializeField] private Transform _tfSpawnParent;
        [SerializeField] private TMP_Text _txtQuestion;

        private void SetQuestionText(string text)
        {
            _txtQuestion.text = text;
        }

        private Button CreateOptionButton(string text)
        {
            Button btn = Instantiate(_btnAnswerOptionPrefab, _tfSpawnParent);
            btn.name = "Btn_Option: " + text;
            btn.GetComponentInChildren<Text>().text = text;
            return btn;
        }
    } 
}
