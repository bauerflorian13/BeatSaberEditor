﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEditorManager : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private TextMeshProUGUI txtPrecision;
    [SerializeField]
    private TextMeshProUGUI txtBeatTime;

    private double? playingPrecision;

    #endregion

    #region Properties

    public Note.ColorType CurrentColor { get; private set; }
    public int Precision { get; private set; }
    public double CurrentTime { get; private set; }
    public bool Playing { get; private set; }
    private double bpmInSeconds;

    public double CurrentBeatTimeInSeconds
    {
        get
        {
            return MapCreator._Map.BeatLenghtInSeconds * CurrentTime;
        }
    }

    public static MapEditorManager Instance { get; private set; }

    #endregion

    #region Events

    private void Start()
    {
        Instance = this;
        CurrentColor = Note.ColorType.Blue;
        Precision = 1;

        bpmInSeconds = GetBPMInSeconds(MapCreator._Map._beatsPerMinute);
        UpdateBeatTimeText();
    }

    #endregion

    #region Methods

    public void Play()
    {
        Playing = !Playing;
        if (!Playing)
        {
            playingPrecision = null;
            StopCoroutine(PlayCoroutine());
        }
        else
        {
            if (!playingPrecision.HasValue)
                SetPlayingPrecision();
            StartCoroutine(PlayCoroutine());
        }
    }

    public IEnumerator PlayCoroutine()
    {
        while (Playing)
        {
            yield return new WaitForSeconds((float)bpmInSeconds / (float)(playingPrecision.HasValue ? playingPrecision.Value : Precision));
            ChangeTime(true, true);
            if (!playingPrecision.HasValue)
                SetPlayingPrecision();
        }
    }

    public void IncreasePrecision()
    {
        if (Precision < 64)
            Precision++;

        UpdatePrecisionText();
    }

    public void DecreasePrecision()
    {
        if (Precision > 1)
            Precision--;

        UpdatePrecisionText();
    }

    private void SetPlayingPrecision()
    {
        double? nextTime = null;

        if (MapCreator._Map.NoteTimeChunks.Count > 0 && MapCreator._Map.NoteTimeChunks.Count > MapCreator._Map.NoteTimeChunks.IndexOfKey(CurrentTime) + 1)
            nextTime = MapCreator._Map.NoteTimeChunks.Values[MapCreator._Map.NoteTimeChunks.IndexOfKey(CurrentTime) + 1].FirstOrDefault()._time;
        else
            nextTime = null;

        if (nextTime.HasValue && nextTime.Value > CurrentTime)
            playingPrecision = 1d / (nextTime.Value - CurrentTime);
        else
            playingPrecision = null;
    }

    public void ChangeTime(bool forward, bool autoPlay)
    {
        ShowHideNotes(false);

        if (forward)
            CurrentTime += autoPlay && playingPrecision.HasValue ? 1d / playingPrecision.Value : 1d / Precision;
        else
        {
            CurrentTime -= 1d / Precision;
            if (CurrentTime < 0)
                CurrentTime = 0;
        }

        UpdateBeatTimeText();

        if (autoPlay)
            SetPlayingPrecision();
        else
            playingPrecision = null;

        ShowHideNotes(true);
    }

    public void StepTime(bool forward)
    {
        ChangeTime(forward, false);
    }

    public void SwitchColor()
    {
        switch (CurrentColor)
        {
            case Note.ColorType.Red:
                CurrentColor = Note.ColorType.Blue;
                break;
            case Note.ColorType.Blue:
                CurrentColor = Note.ColorType.Red;
                break;
        }
    }

    public void ShowHideNotes(bool show)
    {
        if (!MapCreator._Map.NoteTimeChunks.ContainsKey(CurrentTime))
            return;

        foreach (var note in MapCreator._Map.NoteTimeChunks[CurrentTime])
        {
            note.gameObject.SetActive(show);
        }
    }

    private void UpdatePrecisionText()
    {
        txtPrecision.text = "Precision: 1/" + Precision;
    }

    private void UpdateBeatTimeText()
    {
        txtBeatTime.text = "Beat: " + Math.Round(CurrentTime, 2);
    }

    private double GetBPMInSeconds(int bpm)
    {
        return 60d / bpm;
    }

    #endregion
}
