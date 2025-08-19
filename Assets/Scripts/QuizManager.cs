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
        [SerializeField] private Transform _tfOptionsSpawnParent;
        [SerializeField] private TMP_Text _txtQuestion;
        [SerializeField] private HeartCounter _heartCounter;
        [SerializeField] private OpenAIManager _openAIManager;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private Character_Controller _alienCtrl;

        [Header("Debug")]
        [TextArea(4, 30)] public string DebugJson;

        private QuizData _currentQuizData;
        private int _currentQuestionIndex;

        public string CurrentQuizAnswer => _currentQuizData.QuizQuestions[_currentQuestionIndex].Answer;
        public event Action<bool> OnAnswerChecked;
        public event Action OnFinishQuiz;

        private void Awake()
        {
            if (!_heartCounter) _heartCounter = FindAnyObjectByType<HeartCounter>();
            if (!_openAIManager) _openAIManager = FindAnyObjectByType<OpenAIManager>();
            if (!_gameManager) _gameManager = FindAnyObjectByType<GameManager>();
            if (!_alienCtrl) _alienCtrl = FindAnyObjectByType<Character_Controller>();
        }

        private void Start()
        {
            _openAIManager.OnReceiveQuizJson += ProcessQuizDataJson;
            InitView();
            // TODO: Just for debug, to be refactoed.
            //ParseDebugJson();
            //StartQuiz();
        }

        private void OnDestroy()
        {
            _openAIManager.OnReceiveQuizJson -= ProcessQuizDataJson;
        }

        public void InitView()
        {
            _heartCounter.ResetHearts();
            FindAnyObjectByType<MainCanvasPresenter>().IsLoadingQuiz = true;
        }

        [ContextMenu("Prase Debug Json")]
        private void ParseDebugJson()
        {
            ProcessQuizDataJson(DebugJson);
        }

        private void ProcessQuizDataJson(string json)
        {
            string refinedJson = RefineJsonObject(json);
            Debug.Log($"Refined JSON: {refinedJson}");
            _currentQuizData = JsonConvert.DeserializeObject<QuizData>(refinedJson);
            StartQuiz();

            static string RefineJsonObject(string json)
            {
                if (string.IsNullOrEmpty(json))
                    return string.Empty;

                int start = json.IndexOf('{');
                int end = json.LastIndexOf('}');

                if (start == -1 || end == -1 || end < start)
                    return string.Empty;

                return json.Substring(start, end - start + 1);
            }
        }

        [ContextMenu("Start Quiz")]
        private void StartQuiz()
        {
            _currentQuestionIndex = 0;
            ShowQuizOnPanel(_currentQuizData.QuizQuestions[_currentQuestionIndex]);
            _heartCounter.ResetHearts();
        }

        [ContextMenu("Go to Next Quiz")]
        private void GoToNextQuestion()
        {
            if (_currentQuestionIndex < _currentQuizData.QuizQuestions.Length)
            {
                _currentQuestionIndex++;
            }
            ShowQuizOnPanel(_currentQuizData.QuizQuestions[_currentQuestionIndex]);
        }

        private void ShowQuizOnPanel(QuizQuestion quiz)
        {
            SetQuestionText(quiz.Question);
            CreateOptions(quiz.Options);
        }

        private void SetQuestionText(string text)
        {
            _txtQuestion.text = (_currentQuestionIndex + 1) + ". " + text;
        }

        private void CreateOptions(string[] options)
        {
            foreach (Transform child in _tfOptionsSpawnParent)
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
            Button btn = Instantiate(_btnAnswerOptionPrefab, _tfOptionsSpawnParent);
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

            if (_currentQuestionIndex >= _currentQuizData.QuizQuestions.Length - 1)
            {
                FinishQuiz();
            }
            else
            {
                GoToNextQuestion();
            }
        }

        private void FinishQuiz()
        {
            Debug.Log("Quiz Finished!");
            OnFinishQuiz?.Invoke();
            StartCoroutine(EvaluateAndPresentQuizResult());
        }

        private IEnumerator EvaluateAndPresentQuizResult()
        {
            if (_heartCounter.HeartCount == 0)
            {
                _alienCtrl.gameObject.SetActive(true);
                _alienCtrl.Become_Sad();
                // TODO: Show game over screen or similar.
            }
            else if (_heartCounter.HeartCount == _currentQuizData.QuizQuestions.Length)
            {
                _alienCtrl.gameObject.SetActive(true);
                _alienCtrl.Become_Happy();
                // TODO: Show perfect score screen or similar.
            }
            else
            {
                _alienCtrl.gameObject.SetActive(true);
                _alienCtrl.Become_Happy();
                // TODO: Show score screen or similar.
            }

            // TODO: Animations, sound effects, etc.
            yield return new WaitForSeconds(5f);
            _gameManager.CurrentState = GameState.ConfirmView;
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
