using UnityEditor;
using UnityEngine;
using System.IO;

public class TransparentPreviewGenerator
{
    [MenuItem("Tools/Generate Transparent Preview for Selected Prefab")]
    public static void GenerateTransparentPreview()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is GameObject go)
            {
                // Create temporary camera
                GameObject camGO = new GameObject("TempCam");
                Camera cam = camGO.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0, 0, 0, 0); // transparent
                cam.orthographic = true;
                cam.orthographicSize = 2f;
                cam.cullingMask = LayerMask.GetMask("Preview"); // Crée un layer spécial Preview

                // Create RenderTexture with alpha channel
                int res = 256;
                RenderTexture rt = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32);
                rt.antiAliasing = 8;
                cam.targetTexture = rt;

                // Instantiate prefab on Preview layer
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(go);

                int previewLayer = LayerMask.NameToLayer("Preview");
                if (previewLayer == -1)
                {
                    Debug.LogError("Le layer 'Preview' n'existe pas. Merci de le créer dans Tags and Layers.");
                    return;
                }
                instance.layer = previewLayer;

                // Position camera to see prefab
                instance.transform.position = Vector3.zero;
                cam.transform.position = new Vector3(0, 1.7f, -10);
                //cam.transform.LookAt(instance.transform);

                // Render
                cam.Render();

                // Read RenderTexture pixels
                RenderTexture.active = rt;
                Texture2D tex = new Texture2D(res, res, TextureFormat.ARGB32, false);
                tex.ReadPixels(new Rect(0, 0, res, res), 0, 0);
                tex.Apply();

                // Cleanup
                RenderTexture.active = null;
                cam.targetTexture = null;
                Object.DestroyImmediate(camGO);
                Object.DestroyImmediate(instance);
                Object.DestroyImmediate(rt);

                // Save PNG
                string path = $"Assets/Resources/Sprites/BlockPreviews/{go.name}_transparent.png";
                File.WriteAllBytes(path, tex.EncodeToPNG());
                AssetDatabase.Refresh();

                Debug.Log($"Generated transparent preview for {go.name} at {path}");
            }
        }
    }
}
