using UnityEngine;
using UnityEngine.UI;

namespace LegalAliens
{
    public class MainCanvasPresenter : MonoBehaviour
    {
        [SerializeField] private RawImage _imgSelectedObject;
        [SerializeField] private RectTransform _pnlLoadingQuiz, _pnlQuiz;
        [SerializeField] private OpenAIManager _openAIManager;

        private bool _isLoadingQuiz;
        public bool IsLoadingQuiz
        {
            get => _isLoadingQuiz;
            set
            {
                _isLoadingQuiz = value;
                _pnlLoadingQuiz.gameObject.SetActive(value);
                _pnlQuiz.gameObject.SetActive(!value);
            }
        }

        private void Awake()
        {
            if (!_openAIManager) _openAIManager = FindAnyObjectByType<OpenAIManager>();
        }

        private void Start()
        {
            _openAIManager.OnRequestImagePrompt += UpdateViewOnLoadingQuiz;
            _openAIManager.OnReceiveQuizJson += UpadingViewOnFinishLoading;
        }

        private void OnDestroy()
        {
            _openAIManager.OnRequestImagePrompt -= UpdateViewOnLoadingQuiz;
            _openAIManager.OnReceiveQuizJson -= UpadingViewOnFinishLoading;
        }

        public void SetSelectedObjectImage(Texture tex)
        {
            _imgSelectedObject.texture = tex;
            _imgSelectedObject.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
        }

        private void UpdateViewOnLoadingQuiz()
        {
            IsLoadingQuiz = true;
        }

        private void UpadingViewOnFinishLoading(string _)
        {
            IsLoadingQuiz = false;
        }
    }
}