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
                boundingBox.OnBoundingBoxSelected += (det) =>
                {
                    var croppedTexture = Utility.CropByBoundingBox(_imgScreenshot.texture as Texture2D, det.box[0], det.box[1], det.box[2], det.box[3]);
                    _gameManager.CurrentState = GameState.InQuiz;
                    //_quizManager.SetCurrentQuizImage(croppedTexture);
                    Debug.Log($"Selected: {det.label} with confidence {det.confidence * 100:F2}%");
                    // Handle selection logic here, e.g., highlight the object, show more info, etc.
                };
            }
        }
    } 
}
