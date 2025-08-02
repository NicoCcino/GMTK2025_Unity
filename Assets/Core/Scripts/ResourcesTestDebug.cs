using UnityEngine;

public class ResourcesTestDebug : MonoBehaviour
{
    void Start()
    {
        // Test loading the prefabs directly
        GameObject t5Prefab = Resources.Load<GameObject>("Prefab_Block_T_5");
        GameObject simplePrefab = Resources.Load<GameObject>("Prefab_Block_Simple_1");
        
        Debug.Log($"T5 Prefab loaded: {(t5Prefab != null ? "SUCCESS" : "FAILED")}");
        Debug.Log($"Simple Prefab loaded: {(simplePrefab != null ? "SUCCESS" : "FAILED")}");
        
        if (t5Prefab != null) Debug.Log($"T5 Prefab name: {t5Prefab.name}");
        if (simplePrefab != null) Debug.Log($"Simple Prefab name: {simplePrefab.name}");
    }
}
