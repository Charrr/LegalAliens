using System.Collections;
using UnityEngine;

public class PermissionRequester : MonoBehaviour
{
    public Renderer targetRenderer;          // Assign a quad's Renderer, or
    public UnityEngine.UI.RawImage uiImage;  // ...or a UI RawImage

    private WebCamTexture _cam;

    IEnumerator Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Ask for permissions (Meta’s camera permission + Android camera)
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.CAMERA"))
            UnityEngine.Android.Permission.RequestUserPermission("android.permission.CAMERA");
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("horizonos.permission.HEADSET_CAMERA"))
            UnityEngine.Android.Permission.RequestUserPermission("horizonos.permission.HEADSET_CAMERA");
#endif
        // Wait one frame so permission UI can process
        yield return null;

        // Pick the passthrough device (Meta maps them as WebCamTexture devices)
        var devices = WebCamTexture.devices;
        if (devices.Length == 0) { Debug.LogError("No passthrough cameras found."); yield break; }

        // Use first device (or choose left/right by name/index)
        _cam = new WebCamTexture(devices[0].name, 1280, 960, 30); // Quest 3 max is typically 1280x960
        _cam.Play();

        // Bind to material/RawImage
        if (targetRenderer) targetRenderer.material.mainTexture = _cam;
        if (uiImage) uiImage.texture = _cam;
    }

    void OnDisable()
    {
        if (_cam != null) { _cam.Stop(); Destroy(_cam); }
    }
}