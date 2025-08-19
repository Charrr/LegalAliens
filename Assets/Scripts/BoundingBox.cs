using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegalAliens
{
    public class BoundingBox : MonoBehaviour
    {
        [SerializeField] private Image _img;
        [SerializeField] private Button _btn;
        [SerializeField] private TMP_Text _txt;
        [SerializeField] private RectTransform _rt;

        public DwaniAIResponse.Detection Detection;

        public event Action<DwaniAIResponse.Detection> OnBoundingBoxSelected;

        private void Awake()
        {
            if (!_img) _img = GetComponentInChildren<Image>();
            if (!_btn) _btn = GetComponentInChildren<Button>();
            if (!_txt) _txt = GetComponentInChildren<TMP_Text>();
            if (!_rt) _rt = _img.GetComponent<RectTransform>();
        }

        public void ConfigureBoundingBox(DwaniAIResponse.Detection detection)
        {
            Detection = detection;
            name = "BoundingBox: " + detection.label;
            Color col = UnityEngine.Random.ColorHSV(0f, 1f, 0.7f, 0.9f, 0.7f, 0.9f, 0.3f, 0.3f); // Random color
            _txt.color = col;
            _txt.text = $"{detection.label} ({detection.confidence * 100:F2}%)";
            _txt.rectTransform.anchoredPosition = new Vector2(detection.box[0], -detection.box[1] - 20); // Position text above the box
            _rt.anchoredPosition = new Vector2(detection.box[0], -detection.box[1]);
            _rt.sizeDelta = new Vector2(detection.box[2] - detection.box[0], detection.box[3] - detection.box[1]);
            _img.color = col; // Match the bounding box color to the text color
            _btn.onClick.AddListener(SelectButton);
        }

        private void SelectButton()
        {
            OnBoundingBoxSelected?.Invoke(Detection);
            FindAnyObjectByType<ScreenshotPresenter>(FindObjectsInactive.Include).ConfirmSelectObject(Detection);
        }
    }
}
