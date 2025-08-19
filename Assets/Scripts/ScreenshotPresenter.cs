using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegalAliens
{
    public class ScreenshotPresenter : MonoBehaviour
    {
        [Header("Asset References")]
        [SerializeField] private TMP_Text _txtLabelPrefab;
        [SerializeField] private BoundingBox _boundingBoxPrefab;

        [Header("Object References")]
        [SerializeField] private RawImage _imgScreenshot;
        [SerializeField] private DwaniAIManager _dwaniAIManager;
        [SerializeField] private QuizManager _quizManager;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private MainCanvasPresenter _mainCanvasPresenter;
        [SerializeField] private OpenAIManager _openAIManager;
        [SerializeField] private ScreenshotController _screenshotController;

        [Header("Response Data")]
        [SerializeField] private DwaniAIResponse.DetectionData _detectionData;

        private void Awake()
        {
            if (!_dwaniAIManager)
            {
                _dwaniAIManager = FindAnyObjectByType<DwaniAIManager>();
            }
            if (!_quizManager)
            {
                _quizManager = FindAnyObjectByType<QuizManager>();
            }
            if (!_gameManager)
            {
                _gameManager = FindAnyObjectByType<GameManager>();
            }
            if (!_mainCanvasPresenter)
            {
                _mainCanvasPresenter = FindAnyObjectByType<MainCanvasPresenter>();
            }
            if (!_openAIManager)
            {
                _openAIManager = FindAnyObjectByType<OpenAIManager>();
            }
            if (!_screenshotController)
            {
                _screenshotController = FindAnyObjectByType<ScreenshotController>();
            }
        }

        private void Start()
        {
            _dwaniAIManager.OnReceiveResponse += ProcessDwaniResponse;
        }

        private void OnDestroy()
        {
            _dwaniAIManager.OnReceiveResponse -= ProcessDwaniResponse;
        }

        public void SetScreenshot(Texture2D screenshot)
        {
            _imgScreenshot.texture = screenshot;
            _imgScreenshot.SetNativeSize(); // Adjust the RawImage size to match the screenshot
            _dwaniAIManager.SendRequest(_imgScreenshot.texture as Texture2D);
        }

        private void ProcessDwaniResponse(string json)
        {
            _detectionData = JsonUtility.FromJson<DwaniAIResponse.DetectionData>(json);
            PresentDetectionData(_detectionData);
        }

        private void PresentDetectionData(DwaniAIResponse.DetectionData data)
        {
            foreach (Transform child in _imgScreenshot.transform)
            {
                Destroy(child.gameObject); // Clear previous detections
            }

            foreach (var detection in data.detections)
            {
                BoundingBox boundingBox = Instantiate(_boundingBoxPrefab, _imgScreenshot.transform);
                boundingBox.ConfigureBoundingBox(detection);
                //boundingBox.OnBoundingBoxSelected += (det) =>
                //{
                //    Texture2D texture = _screenshotController.CurrentSnapshotRisized;
                //    var croppedTexture = Utility.CropByBoundingBox(texture, det.box[0], det.box[1], det.box[2], det.box[3]);
                //    _gameManager.CurrentState = GameState.InQuiz;
                //    _mainCanvasPresenter.SetSelectedObjectImage(croppedTexture);
                //    _openAIManager.PromptGenerateQuizBasedOnImage(croppedTexture);
                //    //_quizManager.SetCurrentQuizImage(croppedTexture);
                //    Debug.Log($"Selected: {det.label} with confidence {det.confidence * 100:F2}%");
                //    // Handle selection logic here, e.g., highlight the object, show more info, etc.
                //};
            }
        }

        public void ConfirmSelectObject(DwaniAIResponse.Detection detection)
        {
            Texture2D texture = _screenshotController.CurrentSnapshotRisized;
            var croppedTexture = Utility.CropByBoundingBox(texture, detection.box[0], detection.box[1], detection.box[2], detection.box[3]);
            _gameManager.CurrentState = GameState.InQuiz;
            _mainCanvasPresenter.SetSelectedObjectImage(croppedTexture);
            _openAIManager.PromptGenerateQuizBasedOnImage(croppedTexture);
        }
    }
}
