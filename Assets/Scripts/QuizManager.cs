using Newtonsoft.Json;
using System;
using System.Collections;
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
        [SerializeField] private HeartCounter _heartCounter;

        [Header("Debug")]
        [TextArea(4, 30)] public string DebugJson;

        private QuizData _currentQuizData;
        private int _currentQuizIndex;

        public string CurrentQuizAnswer => _currentQuizData.QuizQuestions[_currentQuizIndex].Answer;
        public event Action<bool> OnAnswerChecked;
        public event Action OnFinishQuiz;

        private void Awake()
        {
            if (!_heartCounter) _heartCounter = FindAnyObjectByType<HeartCounter>();
        }

        private void Start()
        {
            // TODO: Just for debug, to be refactoed.
            ParseDebugJson();
            StartQuiz();
        }

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
            ShowQuizOnPanel(_currentQuizData.QuizQuestions[_currentQuizIndex]);
        }

        [ContextMenu("Go to Next Quiz")]
        private void GoToNextQuestion()
        {
            if (_currentQuizIndex < _currentQuizData.QuizQuestions.Length)
            {
                _currentQuizIndex++;
            }
            ShowQuizOnPanel(_currentQuizData.QuizQuestions[_currentQuizIndex]);
        }

        private void ShowQuizOnPanel(QuizQuestion quiz)
        {
            SetQuestionText(quiz.Question);
            CreateOptions(quiz.Options);
        }

        private void SetQuestionText(string text)
        {
            _txtQuestion.text = (_currentQuizIndex + 1) + ". " + text;
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
            btn.onClick.AddListener(() => HandleSelectOption(text));
            return btn;
        }

        private void HandleSelectOption(string option)
        {
            bool result = option == CurrentQuizAnswer;
            Debug.Log($"Option selected: {option}. Correct Answer: {CurrentQuizAnswer}.");
            OnAnswerChecked?.Invoke(result);
            if (result == false)
            {
                _heartCounter.LoseOneHeart();
            }
            StartCoroutine(PresentQuizQuestionResult(result));
        }

        private IEnumerator PresentQuizQuestionResult(bool isCorrect)
        {
            // TODO: Animations, sound effects, etc.
            yield return new WaitForSeconds(0f);
            if (_currentQuizIndex >= _currentQuizData.QuizQuestions.Length - 1)
            {
                OnFinishQuiz?.Invoke();
            }
            else
            {
                GoToNextQuestion();
            }
        }
    }

    [Serializable]
    public class QuizData
    {
        public string QuizzedObjectName;
        public QuizQuestion[] QuizQuestions;
    }

    [Serializable]
    public class QuizQuestion
    {
        public string Question;
        public string[] Options;
        public string Answer;
    }
}
