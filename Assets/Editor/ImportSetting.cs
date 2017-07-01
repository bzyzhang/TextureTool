using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

public class ImportSetting : AssetPostprocessor
{
    static int[] maxSizes = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
    static readonly string DEFAULTS_KEY = "DEFAULTS_DONE";
    static readonly uint DEFAULTS_VERSION = 2;

    public bool IsAssetProcessed
    {
        get
        {
            string key = string.Format("{0}_{1}", DEFAULTS_KEY, DEFAULTS_VERSION);
            return assetImporter.userData.Contains(key);
        }
        set
        {
            string key = string.Format("{0}_{1}", DEFAULTS_KEY, DEFAULTS_VERSION);
            assetImporter.userData = value ? key : string.Empty;
        }
    }

    void OnPreprocessTexture()
    {
        if (IsAssetProcessed)
            return;
        IsAssetProcessed = true;

        if (assetPath.IndexOf("GameResource") < 0 || assetPath.IndexOf("@e") >= 0)
            return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        if (textureImporter == null)
            return;
        textureImporter.textureType = TextureImporterType.Advanced;
        int width = 0;
        int height = 0;
        GetOriginalSize(textureImporter, out width, out height);
        if (!GetIsMultipleOf4(width) || !GetIsMultipleOf4(height))
        {
            Debug.LogError("4---" + assetPath);
        }
        bool IsPowerOfTwo = GetIsPowerOfTwo(width) && GetIsPowerOfTwo(height);
        if (!IsPowerOfTwo)
            textureImporter.npotScale = TextureImporterNPOTScale.None;

        textureImporter.mipmapEnabled = false;
        textureImporter.maxTextureSize = GetMaxSize(Mathf.Max(width,height));

        if (assetPath.EndsWith(".jpg"))
        {
            textureImporter.textureFormat = TextureImporterFormat.ETC2_RGB4;
        }
        else if (assetPath.EndsWith(".png"))
        {
            if (!textureImporter.DoesSourceTextureHaveAlpha())
                textureImporter.grayscaleToAlpha = true;
            textureImporter.alphaIsTransparency = true;
            textureImporter.textureFormat = TextureImporterFormat.ETC2_RGBA8;

            string atlasName = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(assetPath)).Name;
            if (atlasName.Equals("UI"))
            {
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.spritePackingTag = "UI";
            }
        }
        else
        {
            Debug.Log("图片格式---"+assetPath);
        }
    }

    /// <summary>
    /// 获取texture的原始文件尺寸
    /// </summary>
    /// <param name="importer"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    void GetOriginalSize(TextureImporter importer, out int width, out int height)
    {
        object[] args = new object[2] { 0, 0 };
        MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        mi.Invoke(importer, args);
        width = (int)args[0];
        height = (int)args[1];
    }

    bool GetIsMultipleOf4(int f)
    {
        return f % 4 == 0;
    }

    bool GetIsPowerOfTwo(int f)
    {
        return (f & (f - 1)) == 0;
    }

    int GetMaxSize(int longerSize)
    {
        int index = 0;
        for (int i = 0; i < maxSizes.Length; i++)
        {
            if (longerSize <= maxSizes[i])
            {
                index = i;
                break;
            }
        }
        return maxSizes[index];
    }
}