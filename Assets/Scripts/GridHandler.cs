using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int tileNumber;
    private float _tileSize;
    private float _gridSize;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 tileBoundsSize = tilePrefab.GetComponent<BoxCollider2D>().size;
        _tileSize = tileBoundsSize.x;
        _gridSize = _tileSize * tileNumber;
        
        InitializeGrid();
    }

    // create a n x n grid using maximum screen space
    void InitializeGrid()
    {
        for (int i = 0; i < tileNumber; i++)
        {
            for (int j = 0; j < tileNumber; j++)
            {
                // calculate the position of the tile and create
                float xPos = (i - (tileNumber - 1) * 0.5f) * _tileSize;
                float yPos = (j - (tileNumber - 1) * 0.5f) * _tileSize;
                GameObject tile = Instantiate(tilePrefab, new Vector3(xPos, yPos), Quaternion.identity);
                
                tile.name = "Tile " + i + "-" + j;
    
                // set tile's parent as grid to create hierarchy
                tile.transform.parent = transform; 
            }
        }
        
        // the height of the window seen by the camera
        float height = Camera.main.orthographicSize * 2;
        // the width of the window with aspect ratio
        float width = height * Screen.width/ Screen.height;
        float maxSize = height < width ? height : width;
        
        // rescale whole grid using maximum screen space
        float scale = maxSize / _gridSize;
        transform.localScale = new Vector3(scale,scale, 1f);
    }
}
