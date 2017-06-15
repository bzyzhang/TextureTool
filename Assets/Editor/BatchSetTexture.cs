
using System.Collections.Generic;
using UnityEditor;
public class BatchSetTexture
{
    [MenuItem("TextureTool/批量设置纹理")]
    static void BatchSet()
    {
        SetTextures();
    }

    static void SetTextures()
    {
        string[] assets = AssetDatabase.FindAssets("t:Texture");

        AssetDatabase.StartAssetEditing();

        foreach (string guid in assets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
                continue;
            if (importer.textureType == TextureImporterType.Advanced)
            {
                if (importer.textureFormat == TextureImporterFormat.ETC2_RGB4)
                {
                    importer.textureFormat = TextureImporterFormat.ASTC_RGB_8x8;
                    //AssetDatabase.ImportAsset(assetPath);
                    importer.SaveAndReimport();
                }
                else if (importer.textureFormat == TextureImporterFormat.ETC2_RGBA8)
                {
                    importer.textureFormat = TextureImporterFormat.ASTC_RGBA_8x8;
                    //AssetDatabase.ImportAsset(assetPath);
                    importer.SaveAndReimport();
                }
                else
                {
                    UnityEngine.Debug.Log(assetPath);
                }
            }
        }

        AssetDatabase.StopAssetEditing();

        EditorUtility.DisplayDialog("设置完成", "设置完成", "确定");
    }
}
