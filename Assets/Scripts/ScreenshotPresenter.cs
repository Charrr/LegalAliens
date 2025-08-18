using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegalAliens
{
    public class ScreenshotPresenter : MonoBehaviour
    {
        [Header("Asset References")]
        [SerializeField] private TMP_Text _txtLabelPrefab;
        [SerializeField] private Image _imgBoundingBoxPrefab;

        [Header("Object References")]
        [SerializeField] private RawImage _imgScreenshot;
        [SerializeField] private DwaniAIManager _dwaniAIManager;

        [Header("Response Data")]
        [SerializeField] private DwaniAIResponse.DetectionData _detectionData;

        private void Awake()
        {
            if (!_dwaniAIManager)
            {
                _dwaniAIManager = FindAnyObjectByType<DwaniAIManager>();
            }
        }

        private void Start()
        {
            _dwaniAIManager.OnReceiveResponse += ProcessDwaniResponse;
            _dwaniAIManager.SendRequest(_imgScreenshot.texture as Texture2D);
        }

        private void OnDestroy()
        {
            _dwaniAIManager.OnReceiveResponse -= ProcessDwaniResponse;
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
                Color col = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f, 0.3f, 0.3f); // Random color for bounding box
                TMP_Text txtLabel = Instantiate(_txtLabelPrefab, _imgScreenshot.transform);
                txtLabel.color = col;
                txtLabel.text = $"Txt_Label: {detection.label} ({detection.confidence * 100:F2}%)";
                txtLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(detection.box[0], -detection.box[1]);

                Image imgBoundingBox = Instantiate(_imgBoundingBoxPrefab, _imgScreenshot.transform);
                imgBoundingBox.name = "Img_BoundingBox: " + detection.label;
                imgBoundingBox.color = col;
                RectTransform rectTransform = imgBoundingBox.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(detection.box[0], -detection.box[1]);
                rectTransform.sizeDelta = new Vector2(detection.box[2] - detection.box[0], detection.box[3] - detection.box[0]);
            }
        }
    } 
}
