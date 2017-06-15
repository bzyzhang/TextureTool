using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SearchWithTag : EditorWindow
{
    [MenuItem("TextureTool/标签")]
    static void Init()
    {
        SearchWithTag window = (SearchWithTag)EditorWindow.GetWindow(typeof(SearchWithTag));
        window.position = new Rect(230, 160, 900, 500);
        window.Show();
    }

    List<string> listResult;
    Vector2 scroll;
    string tag = "UI";
    int resultCount = 1;

    void OnGUI()
    {
        GUILayout.Label(position + "");
        GUILayout.Space(3);

        GUILayout.BeginHorizontal();
        GUILayout.Label("TAG:", GUILayout.Width(50), GUILayout.Height(20));
        tag = GUILayout.TextField(tag, GUILayout.Width(150), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("查找"))
        {
            listResult = GetTextures();
        }

        if (listResult != null)
        {
            if (listResult.Count == 0)
            {
                GUILayout.Label("没有图片使用tag：" + tag);
            }
            else
            {
                resultCount = 1;
                GUILayout.Label(string.Format("下面{0}张图片使用了tag{1}", listResult.Count, tag));
                scroll = GUILayout.BeginScrollView(scroll);

                foreach (string s in listResult)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(resultCount + "    " + s, GUILayout.Width(position.width * 0.8f));
                    if (GUILayout.Button("选择", GUILayout.Width(position.width * 0.15f)))
                    {
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(s);
                    }
                    GUILayout.EndHorizontal();
                    ++resultCount;
                }
                GUILayout.EndScrollView();
            }
        }
    }

    List<string> GetTextures()
    {
        string[] assets = AssetDatabase.FindAssets("t:Texture");
        EditorUtility.DisplayProgressBar("查找中", "", 0);
        List<string> result = new List<string>();
        float allCount = assets.Length;
        int index = 0;

        foreach (string guid in assets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
                continue;
            if ((importer.textureType == TextureImporterType.Sprite || (importer.textureType == TextureImporterType.Advanced && importer.spriteImportMode != SpriteImportMode.None)) && importer.spritePackingTag.Equals(tag))
                result.Add(importer.assetPath);

            ++index;
            EditorUtility.DisplayProgressBar("查找中", (System.Math.Round((index / allCount), 2) * 100).ToString() + "%", index / allCount);
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("查找完成", "查找完成", "确定");
        return result;
    }
}