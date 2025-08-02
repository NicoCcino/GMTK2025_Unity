using UnityEditor;
using UnityEngine;
using System.IO;

public class BlockPreviewGenerator
{
    [MenuItem("Tools/Generate Preview for Selected Prefab")]
    public static void GeneratePreview()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is GameObject go)
            {
                string path = $"Assets/Resources/Sprites/BlockPreviews/{go.name}.png";
                Texture2D tex = AssetPreview.GetAssetPreview(go);

                if (tex != null)
                {
                    byte[] bytes = tex.EncodeToPNG();
                    File.WriteAllBytes(path, bytes);
                    Debug.Log($"Preview generated for {go.name} at {path}");

                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning("Failed to generate preview.");
                }
            }
        }
    }
}
