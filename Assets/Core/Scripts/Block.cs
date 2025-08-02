using UnityEngine;

public class Block
{
    public string blockName;
    private GameObject _blockPrefab;        // Prefab du bloc (private)
    private Sprite _previewSprite;        // Prefab du preview sprite (private)
    private string _prefabPath;             // Path for lazy loading
    private string _previewSpritePath;      // Path for lazy loading
    public GameObject gameObject;           // L'objet visuel instantié du bloc
    public Vector2Int gridPosition;         // Position du centre (pivot) sur la grille
    public Cell[,] blockMatrix = new Cell[3, 3]; // Matrice 3x3 du bloc
    public Color color = Color.white;       // Couleur du bloc

    // Public property for lazy loading
    public GameObject blockPrefab
    {

        // Get block prefab asset
        get
        {
            if (_blockPrefab == null && !string.IsNullOrEmpty(_prefabPath))
            {
                _blockPrefab = Resources.Load<GameObject>(_prefabPath);
                if (_blockPrefab == null)
                {
                    Debug.LogError($"Failed to load prefab at path: {_prefabPath}");
                }
                else
                {
                    Debug.Log($"Successfully loaded prefab: {_prefabPath}");
                }
            }
            return _blockPrefab;
        }
        set
        {
            _blockPrefab = value;
        }
    }
    public Sprite previewSprite
    {

        // Get block preview sprite asset

        // Lazy-load du sprite preview
        get
        {
            if (_previewSprite == null && !string.IsNullOrEmpty(_previewSpritePath))
            {
                _previewSprite = Resources.Load<Sprite>(_previewSpritePath);
                if (_previewSprite == null)
                    Debug.LogError($"Failed to load preview sprite at path: {_previewSpritePath}");
                else
                    Debug.Log($"Loaded preview sprite: {_previewSpritePath}");
            }
            return _previewSprite;
        }
        set
        {
            _previewSprite = value;
        }
    }

    public Block(string name, GameObject prefab)
    {
        blockName = name;
        _blockPrefab = prefab;
        InitializeMatrix();
    }

    public Block(string name, string prefabPath)
    {
        blockName = name;
        _prefabPath = prefabPath;
        InitializeMatrix();
    }

        public Block(string name, string prefabPath, string previewSpritePath)
    {
        blockName = name;
        _prefabPath = prefabPath;
        _previewSpritePath = previewSpritePath;
        InitializeMatrix();
    }


    public Block(GameObject obj, Vector2Int pos, Color col)
    {
        gameObject = obj;
        gridPosition = pos;
        color = col;
        InitializeMatrix();
    }

    protected virtual void InitializeMatrix()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                blockMatrix[x, y] = new Cell(null, this, new Vector2Int(x, y));
            }
        }
    }

    // Méthode virtuelle que les classes filles peuvent override
    public virtual void Trigger(Vector2Int position, GameObject player)
    {
        Debug.Log($"Block {blockName} triggered at {position}");
    }
}
