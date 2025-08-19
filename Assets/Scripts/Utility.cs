using UnityEngine;

public static class Utility
{
    // Makes a readable, uncompressed copy if needed (avoids EncodeToJPG errors on compressed textures).
    public static Texture2D EnsureReadable(Texture2D src)
    {
        if (src == null)
        {
            return null;
        }

        try
        {
            // If already readable in a supported format, just return it.
            src.GetPixels32(); // will throw if not readable
            return src;
        }
        catch { /* fall through */ }

        // RenderTexture path to force-decompress/copy into a readable Texture2D
        var rt = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        var prev = RenderTexture.active;
        Graphics.Blit(src, rt);
        RenderTexture.active = rt;

        var copy = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false, true);
        copy.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0);
        copy.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        return copy;
    }

    public static Texture2D ResizeTexture(Texture2D source, int targetWidth)
    {
        int originalWidth = source.width;
        int originalHeight = source.height;
        float aspectRatio = (float)originalHeight / originalWidth;
        int targetHeight = Mathf.RoundToInt(targetWidth * aspectRatio);

        // Create a new texture with the target dimensions
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        // Scale pixels using bilinear filtering
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                float u = (float)x / (float)targetWidth;
                float v = (float)y / (float)targetHeight;
                Color color = source.GetPixelBilinear(u, v);
                result.SetPixel(x, y, color);
            }
        }
        result.Apply();
        return result;
    }

    public static Texture2D CropTexture(Texture2D source, int a, int b, int c, int d)
    {
        // Ensure the texture is readable
        source = EnsureReadable(source);

        int cropWidth = c - a + 1;
        int cropHeight = d - b + 1;

        // Clamp values to source texture bounds
        a = Mathf.Clamp(a, 0, source.width - 1);
        c = Mathf.Clamp(c, 0, source.width - 1);
        b = Mathf.Clamp(b, 0, source.height - 1);
        d = Mathf.Clamp(d, 0, source.height - 1);

        cropWidth = c - a + 1;
        cropHeight = d - b + 1;

        Color[] pixels = source.GetPixels(a, b, cropWidth, cropHeight);
        Texture2D cropped = new Texture2D(cropWidth, cropHeight, source.format, false);
        cropped.SetPixels(pixels);
        cropped.Apply();
        return cropped;
    }

    public static Texture2D CropByBoundingBox(Texture2D source, int x0, int y0, int x1, int y1)
    {
        return CropTexture(source, x0, y0, x1, y1);
    }
}
