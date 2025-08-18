using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LegalAliens
{
    public class ScreenshotController : MonoBehaviour
    {
        [SerializeField] private Button _btnConfirmView;
        [SerializeField] private RenderTexture _renderTexture;

        // Assign the main camera or the MR camera in the Inspector
        [SerializeField] private Camera _screenshotCamera;
        [SerializeField] private ScreenshotPresenter _screenshotPresenter;

        private void Awake()
        {
            if (!_screenshotCamera)
                _screenshotCamera = Camera.main;
        }
        private void Start()
        {
            _btnConfirmView.onClick.AddListener(ConfirmView);
        }

        private void OnDestroy()
        {
            _btnConfirmView.onClick.RemoveListener(ConfirmView);
        }

        public Texture2D CaptureScreenshot()
        {
            // Save the original culling mask
            int originalMask = _screenshotCamera.cullingMask;

            // Exclude the UI layer (layer 5)
            _screenshotCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

            _screenshotCamera.targetTexture = _renderTexture;

            // Render the camera's view to the RenderTexture
            RenderTexture.active = _renderTexture;
            _screenshotCamera.Render();

            // Create a Texture2D to store the screenshot
            Texture2D screenshot = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            screenshot.Apply();

            // Reset the camera's target texture and RenderTexture
            _screenshotCamera.targetTexture = null;
            RenderTexture.active = null;

            // Restore the original culling mask
            _screenshotCamera.cullingMask = originalMask;

            return screenshot;
        }

        private void ConfirmView()
        {
            Texture2D screenshot = CaptureScreenshot();
            _screenshotPresenter.SetScreenshot(screenshot);
        }
    } 
}
