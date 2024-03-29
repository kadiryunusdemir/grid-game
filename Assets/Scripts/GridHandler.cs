﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TMP_InputField tileNumber;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private GameObject starterPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TextMeshProUGUI score;
    private int _tileNumber;
    private float _tileSize;
    private float _gridSize;
    // private int _score;
    private Dictionary<Vector2, SpriteRenderer> _tileRenderers;

    public static GridHandler Instance { get; private set; }

    void Awake()
    {
        // use singleton to access GridHandler form TileHandler
        Instance = this;
        // _score = 0;
    }

    public void SetGridNumber()
    {
        // try to get grid number from user
        if (String.IsNullOrEmpty(tileNumber.text) || !int.TryParse(tileNumber.text, out _tileNumber) || _tileNumber < 3)
        {
            errorMessage.gameObject.SetActive(true);
            return;
        }
        
        // start the game
        starterPanel.SetActive(false);
        // score.text = _score.ToString();
        gamePanel.SetActive(true);
        InitializeGrid();
        Timer.Instance.StartTheTimer();
    }
    
    // create a n x n grid using maximum screen space
    void InitializeGrid()
    {
        _tileRenderers = new Dictionary<Vector2, SpriteRenderer>();
        
        Vector2 tileBoundsSize = tilePrefab.GetComponent<BoxCollider2D>().size;
        _tileSize = tileBoundsSize.x;
        _gridSize = _tileSize * _tileNumber;
        
        for (int i = 0; i < _tileNumber; i++)
        {
            for (int j = 0; j < _tileNumber; j++)
            {
                // calculate the position and create the tile
                float xPos = (i - (_tileNumber - 1) * 0.5f) * _tileSize;
                float yPos = (j - (_tileNumber - 1) * 0.5f) * _tileSize;
                GameObject tile = Instantiate(tilePrefab, new Vector3(xPos, yPos), Quaternion.identity);
                
                // give position values to tile's handler
                tile.name = "Tile " + i + "-" + j;

                // give position values to tile's handler
                tile.GetComponent<TileHandler>().position = new Vector2(i, j);
                
                // disable the tile's cross renderer
                SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
                tileRenderer.enabled = false;
    
                // set tile's parent as grid to create hierarchy
                tile.transform.parent = transform; 
                
                // store the position information with tile's renderer
                _tileRenderers.Add(new Vector2(i, j), tileRenderer);
            }
        }
        
        if (Camera.main != null)
        {
            // the height of the window seen by the camera
            float height = Camera.main.orthographicSize * 2;
            // the width of the window with aspect ratio
            float width = height * Screen.width/ Screen.height;
            float maxSize = height < width ? height : width;
        
            // rescale whole grid using maximum screen space
            float scale = maxSize / (_gridSize + _tileSize * 2); // give margin
            transform.localScale = new Vector3(scale,scale, 1f);
        }
    }

    // check and destroy connected tiles if connected tiles number is at least 3
    public void CheckNeighbors(Vector2 newTilePosition)
    {
        // constant-time complexity O(1) for search, no duplicate positions
        HashSet<Vector2> validNeighbors = new HashSet<Vector2>();
        Queue<Vector2> connectedNeighbors = new Queue<Vector2>();
        
        // add newly added position to search queue
        connectedNeighbors.Enqueue(newTilePosition);
    
        while (connectedNeighbors.Count > 0)
        {
            FindConnectedNeighbors(connectedNeighbors.Dequeue(), connectedNeighbors, validNeighbors);
        }
    
        if (validNeighbors.Count >= 3)
        {
            StartCoroutine(DeactivateTiles(validNeighbors));
        }
    }
    
    // wait to disable connected tiles
    IEnumerator DeactivateTiles(HashSet<Vector2> validNeighbors)
    {
        yield return new WaitForSeconds(0.3f);

        // disable all connected tiles
        foreach (var tilePosition in validNeighbors)
        {
            if (_tileRenderers.TryGetValue(tilePosition, out SpriteRenderer tile))
            {
                tile.enabled = false;
            }
        }

        // _score += validNeighbors.Count;
        // score.text = _score.ToString();
    }
    
    // search connected neighbors for searchPosition
    private void FindConnectedNeighbors(Vector2 searchPosition, Queue<Vector2> connectedNeighbors, HashSet<Vector2> validNeighbors)
    {
        // if search position is already search then return
        if (validNeighbors.Contains(searchPosition))
        {
            return;
        }
        
        // add search position to connected neighbor set
        validNeighbors.Add(searchPosition);

        // define the eight possible neighboring positions
        Vector2[] neighbors = new Vector2[]
        {
            new Vector2(searchPosition.x - 1, searchPosition.y - 1),  // Top Left
            new Vector2(searchPosition.x, searchPosition.y - 1),        // Top
            new Vector2(searchPosition.x + 1, searchPosition.y - 1),  // Top Right
            new Vector2(searchPosition.x - 1, searchPosition.y),        // Left
            new Vector2(searchPosition.x + 1, searchPosition.y),        // Right
            new Vector2(searchPosition.x - 1, searchPosition.y + 1),  // Bottom Left
            new Vector2(searchPosition.x, searchPosition.y + 1),        // Bottom
            new Vector2(searchPosition.x + 1, searchPosition.y + 1)   // Bottom Right
        };
    
        // check each neighboring position whether it's enabled
        // and add it to search queue
        foreach (var neighbor in neighbors)
        {
            if (_tileRenderers.TryGetValue(neighbor, out SpriteRenderer neighborTile))
            {
                if (neighborTile.enabled)
                {
                    if (!validNeighbors.Contains(neighbor))
                    {
                        connectedNeighbors.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    public void SetTheScore()
    {
        float activeTiles = 0;

        foreach (var tile in _tileRenderers)
        {
            if (tile.Value.enabled)
            {
                activeTiles++;
            }
        }

        int possibleMaxTiles = 0;

        int xAxis = _tileNumber;
        int xAxisCounter = 0;
        while (xAxis >= 2)
        {
            xAxis -= 2;
            xAxisCounter++;
        }
        if (xAxis == 1)
        {
            xAxisCounter++;
        }
        
        int yAxis = _tileNumber;
        int yAxisCounter = 0;
        while (yAxis >= 3)
        {
            yAxis -= 3;
            yAxisCounter += 2;
        }
        if (yAxis == 2)
        {
            yAxisCounter += 2;
        }

        possibleMaxTiles = xAxisCounter * yAxisCounter;
        
        if (xAxis == 1)
        {
            possibleMaxTiles += yAxisCounter + 1;
        } 
        
        float percentage = activeTiles / possibleMaxTiles;
        score.text = percentage.ToString("%0.00");
    }
}
