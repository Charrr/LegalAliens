using UnityEngine;

namespace LegalAliens
{
    public enum GameState
    {
        None,
        ConfirmView,
        SelectObject,
        InQuiz
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Canvas _quizCanvas, _objectSelectionCanvas, _screenshotCanvas;
        [SerializeField] private Character_Controller _alienCtrl;

        private GameState _currentState = GameState.None;
        public GameState CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                if (value == GameState.ConfirmView)
                {
                    _quizCanvas.gameObject.SetActive(false);
                    _objectSelectionCanvas.gameObject.SetActive(false);
                    _screenshotCanvas.gameObject.SetActive(true);
                    _alienCtrl.gameObject.SetActive(true);
                }
                else if (value == GameState.SelectObject)
                {
                    _quizCanvas.gameObject.SetActive(false);
                    _objectSelectionCanvas.gameObject.SetActive(true);
                    _screenshotCanvas.gameObject.SetActive(false);
                    _alienCtrl.gameObject.SetActive(false);
                }
                else if (value == GameState.InQuiz)
                {
                    _quizCanvas.gameObject.SetActive(true);
                    _objectSelectionCanvas.gameObject.SetActive(false);
                    _screenshotCanvas.gameObject.SetActive(false);
                }
            }
        }

        private void Awake()
        {
            if (!_alienCtrl)
            {
                _alienCtrl = FindAnyObjectByType<Character_Controller>();
            }
        }

        private void Start()
        {
            CurrentState = GameState.ConfirmView;
        }
    }
}


