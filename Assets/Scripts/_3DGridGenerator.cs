﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class _3DGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Canvas _3DCanvas;
    [SerializeField]
    private GameObject bigLine;
    [SerializeField]
    private GameObject smallLine;
    [SerializeField]
    private TextMeshProUGUI txtBeatLinePrefab;

    private Rect _3DCanvasRect;
    private Rect smallLineRect;

    public readonly int startYPos = -500;
    public readonly int distance = 150;

    public static _3DGridGenerator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        int lastLineYPos;

        _3DCanvasRect = _3DCanvas.GetComponent<RectTransform>().rect;
        smallLineRect = smallLine.GetComponent<RectTransform>().rect;

        GenerateHorizontalLines(startYPos, out lastLineYPos);
        GenerateVerticalLines(startYPos, lastLineYPos + startYPos);
    }

    public float GetBeatPosition(float beat)
    {
        return startYPos + distance * beat * 4;
    }

    public Vector2 GetCoordinatePosition(Vector2Int coordinate, GameObject cube)
    {
        Bounds cubeBounds = cube.GetComponent<MeshFilter>().mesh.bounds;
        float cubeHeight = cubeBounds.size.y * cube.transform.localScale.y;

        return new Vector2(smallLineRect.x + _3DCanvasRect.width / 4 * coordinate.x + _3DCanvasRect.width / 8, 
            (2 - coordinate.y) * (2 - cubeHeight) - 50 * (2 - coordinate.y) - cubeHeight * 0.5f);
    }

    private void GenerateHorizontalLines(int yPos, out int lastLineYPos)
    {
        int beat = 0;
        for (int i = 0; i < MapCreator._Map.AmountOfBeatsInSong() * 4; i++)
        {
            GameObject lineType = i % 4 == 0 ? bigLine : smallLine;
            InstantiateLine(lineType, new Vector2(0, yPos));

            if (lineType.Equals(bigLine))
            {
                InstantiateTextBeatLine(beat.ToString(), new Vector2(smallLineRect.width * 0.5f + 120, yPos));
                beat++;
            }

            yPos += distance;
        }

        lastLineYPos = yPos;
    }

    private void GenerateVerticalLines(int yPos, int length)
    {
        float xPos = smallLineRect.x;
        for (int i = 0; i < 5; i++)
        {
            GameObject verticalLine;
            InstantiateVerticalLine(new Vector2(xPos, yPos), out verticalLine);
            verticalLine.GetComponent<RectTransform>().sizeDelta = new Vector2(length * 0.5f, smallLineRect.height);

            verticalLine.transform.position = new Vector3(verticalLine.transform.position.x, verticalLine.transform.position.y, verticalLine.transform.position.z + length * 0.25f);
            xPos += _3DCanvasRect.width / 4;
        }
    }

    private void InstantiateLine(GameObject lineType, Vector2 position)
    {
        GameObject line = Instantiate(lineType);
        line.transform.position = position;
        line.transform.SetParent(_3DCanvas.transform, false);
    }

    private void InstantiateVerticalLine(Vector2 position, out GameObject line)
    {
        line = Instantiate(smallLine);
        line.transform.Rotate(new Vector3(0, 0, 1), 90);
        line.transform.position = position;
        line.transform.SetParent(_3DCanvas.transform, false);
    }

    private void InstantiateTextBeatLine(string text, Vector2 position)
    {
        TextMeshProUGUI txt = Instantiate(txtBeatLinePrefab);
        txt.transform.position = position;
        txt.transform.SetParent(_3DCanvas.transform, false);
        txt.text = text;
    }
}