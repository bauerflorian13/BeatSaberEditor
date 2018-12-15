﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Canvas canvas;
    public Tile tilePrefab;

    public Dictionary<Vector2Int, Tile> Tiles { get; private set; }

    public static GridGenerator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Tiles = new Dictionary<Vector2Int, Tile>();
        CreateGrid(4);
    }

    private void CreateGrid(int distance)
    {
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var tileRect = tilePrefab.GetComponent<RectTransform>().rect;

        int width = 4;
        int height = 3;

        float totalWidth = tileRect.width * width + distance * (width - 1);
        float distanceFromCenter = (canvasRect.width - totalWidth) * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var newTile = Instantiate(tilePrefab);
                newTile.gameObject.transform.SetParent(gameObject.transform, false);
                newTile.transform.position = new Vector3(
                    tileRect.width * 0.5f + tileRect.width * x + distance * x + 150, 
                    canvasRect.height - tileRect.height * 0.5f - 100 - tileRect.height * y - distance * y, x);
                newTile.SetCoordinate(new Vector2Int(x, height - 1 - y));
                Tiles.Add(new Vector2Int(x, 2 - y), newTile);
            }
        }
    }
}