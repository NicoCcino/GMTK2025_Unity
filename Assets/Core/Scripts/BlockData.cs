using UnityEngine;

[CreateAssetMenu(fileName = "New BlockData", menuName = "Block/BlockData")]
public class BlockData : ScriptableObject
{
    public string blockName = "New Block";
    public GameObject blockPrefab; // Le prefab du bloc
    public Color color = Color.white;

    [Tooltip("16 éléments pour la forme 3x3 (ligne par ligne)")]
    public bool[] shape = new bool[9];
}