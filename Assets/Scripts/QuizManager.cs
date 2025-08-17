using Newtonsoft.Json;
using System;
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

        [Header("Debug")]
        [TextArea(4, 30)] public string DebugJson;

        private QuizData _currentQuizData;
        private int _currentQuizIndex;

        [ContextMenu("Prase Debug Json")]
        private void ParseDebugJson()
        {
            ParseQuizDataJson(DebugJson);
        }

        private QuizData ParseQuizDataJson(string json)
        {
            _currentQuizData = JsonConvert.DeserializeObject<QuizData>(json);
            return _currentQuizData;
        }

        [ContextMenu("Start Quiz")]
        private void StartQuiz()
        {
            _currentQuizIndex = 0;
            ShowQuizOnPanel(_currentQuizData.Quizzes[_currentQuizIndex]);
        }

        private void ShowQuizOnPanel(Quiz quiz)
        {
            SetQuestionText(quiz.Question);
            CreateOptions(quiz.Options);
        }

        private void SetQuestionText(string text)
        {
            _txtQuestion.text = text;
        }

        private void CreateOptions(string[] options)
        {
            foreach (Transform child in _tfSpawnParent)
            {
                Destroy(child.gameObject);
            }

            foreach (string option in options)
            {
                CreateOptionButton(option);
            }
        }

        private Button CreateOptionButton(string text)
        {
            Button btn = Instantiate(_btnAnswerOptionPrefab, _tfSpawnParent);
            btn.name = "Btn_Option: " + text;
            btn.GetComponentInChildren<TMP_Text>().text = text;
            return btn;
        }
    }

    [Serializable]
    public class QuizData
    {
        public string QuizzedObjectName;
        public Quiz[] Quizzes;
    }

    [Serializable]
    public class Quiz
    {
        public string Question;
        public string[] Options;
        public string Answer;
    }
}
