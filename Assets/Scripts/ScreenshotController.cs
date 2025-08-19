using PassthroughCameraSamples;
using System.Collections;
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
        [SerializeField] private WebCamTextureManager _webCamTextureManager;

        [Header("Debug")]
        [SerializeField] private Texture2D _debugTexture;

        private void Awake()
        {
            if (!_screenshotCamera)
                _screenshotCamera = Camera.main;
            if (!_webCamTextureManager)
                _webCamTextureManager = FindAnyObjectByType<WebCamTextureManager>();
            if (!_screenshotPresenter)
                _screenshotPresenter = FindAnyObjectByType<ScreenshotPresenter>();
        }

        private void Start()
        {
            _btnConfirmView.onClick.AddListener(ConfirmView);
        }

        private void OnDestroy()
        {
            _btnConfirmView.onClick.RemoveListener(ConfirmView);
        }

        //public Texture2D CaptureScreenshot()
        //{
        //    // Save the original culling mask
        //    int originalMask = _screenshotCamera.cullingMask;

        //    // Exclude the UI layer (layer 5)
        //    _screenshotCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

        //    _screenshotCamera.targetTexture = _renderTexture;

        //    // Render the camera's view to the RenderTexture
        //    RenderTexture.active = _renderTexture;
        //    _screenshotCamera.Render();

        //    // Create a Texture2D to store the screenshot
        //    Texture2D screenshot = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGB24, false);
        //    screenshot.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
        //    screenshot.Apply();

        //    // Reset the camera's target texture and RenderTexture
        //    _screenshotCamera.targetTexture = null;
        //    RenderTexture.active = null;

        //    // Restore the original culling mask
        //    _screenshotCamera.cullingMask = originalMask;

        //    return screenshot;
        //}

        //[ContextMenu("Capture MR Frame")]
        //public void Capture() => StartCoroutine(CaptureCo());

        //IEnumerator CaptureCo()
        //{
        //    yield return new WaitForEndOfFrame();

        //    int w = Screen.width, h = Screen.height;
        //    var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        //    tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        //    tex.Apply();
        //    _screenshotPresenter.SetScreenshot(tex);

        //    Destroy(tex);
        //}

        public Texture2D MakeCameraSnapshot()
        {
            var webCamTexture = _webCamTextureManager.WebCamTexture;
            if (webCamTexture == null || !webCamTexture.isPlaying)
                return null;

            var cameraSnapshot = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
            
            // Copy the last available image from WebCamTexture to a separate object
            Color32[] m_pixelsBuffer = new Color32[webCamTexture.width * webCamTexture.height];
            _ = _webCamTextureManager.WebCamTexture.GetPixels32(m_pixelsBuffer);
            cameraSnapshot.SetPixels32(m_pixelsBuffer);
            cameraSnapshot.Apply();

            return cameraSnapshot;
        }

        private void ConfirmView()
        {
            if (Application.isEditor)
            {
                _screenshotPresenter.SetScreenshot(_debugTexture);
                return;
            }
            Texture2D screenshot = MakeCameraSnapshot();
            Texture2D resized = Utility.ResizeTexture(screenshot, 1024);
            _screenshotPresenter.SetScreenshot(resized);
        }
    } 
}
