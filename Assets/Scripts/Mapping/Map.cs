﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class Map
{
    #region Fields

    [JsonIgnore]
    public List<Note> _notesObjects;
    public List<JsonNote> _notes;

    public string _version;
    public int _beatsPerMinute;
    public int _beatsPerBar;
    public int _noteJumpSpeed;
    public int _shuffle;
    public double _shufflePeriod = 0.5;

    #endregion

    #region Properties

    [JsonIgnore]
    public SortedList<double, List<Note>> NoteTimeChunks { get; private set; }

    [JsonIgnore]
    public float BeatLenghtInSeconds
    {
        get
        {
            return 60f / _beatsPerMinute;
        }
    }

    #endregion

    #region Constructors

    public Map(string _version, int _beatsPerMinute, int _beatsPerBar, int _noteJumpSpeed, List<Note> _notes)
    {
        NoteTimeChunks = new SortedList<double, List<Note>>();
        this._version = _version;
        this._beatsPerMinute = _beatsPerMinute;
        this._noteJumpSpeed = _noteJumpSpeed;
        this._notesObjects = _notes;
    }

    #endregion

    #region Methods

    public void RemoveNote(Note note)
    {
        _notesObjects.Remove(note);
        NoteTimeChunks[note._time].Remove(note);
        if (NoteTimeChunks[note._time].Count == 0)
            NoteTimeChunks.Remove(note._time);

        GameObject.Destroy(note.arrowCube);
        GameObject.Destroy(note.gameObject);
    }

    public void ClearNotes()
    {
        if (_notes != null)
            _notes.Clear();
        if (_notesObjects != null)
            _notesObjects.Clear();
    }
    
    public Note AddNote(Note notePrefab, GameObject blueCubePrefab, GameObject redCubePrefab, Note.CutDirection cutDirection, Vector2Int coordinate, double time, Note.ColorType color, bool active = false)
    {
        if (!NoteTimeChunks.ContainsKey(time))
            NoteTimeChunks.Add(time, new List<Note>());

        var note = GameObject.Instantiate(notePrefab);
        note.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("2DGrid").transform);
        note.gameObject.transform.Rotate(Vector3.forward, CutDirectionButton.GetAngle(cutDirection).Value);
        note.gameObject.transform.position = GridGenerator.Instance.Tiles[coordinate].gameObject.transform.position;
        note.Set(GetBeatTime(_beatsPerMinute, 0, time), coordinate.x, coordinate.y, color, cutDirection);

        NoteTimeChunks[time].Add(note);
        _notesObjects.Add(note);

        note.gameObject.SetActive(active);

        GameObject arrowCube = color == Note.ColorType.Blue ? GameObject.Instantiate(blueCubePrefab) : GameObject.Instantiate(redCubePrefab);
        Vector2 arrowCubePos = _3DGridGenerator.Instance.GetCoordinatePosition(coordinate, arrowCube);

        arrowCube.transform.position = new Vector3(arrowCubePos.x, (float)_3DGridGenerator.Instance.GetBeatPosition(time), arrowCubePos.y);
        arrowCube.transform.Rotate(Vector3.back, CutDirectionButton.GetAngle((Note.CutDirection)note._cutDirection).Value);
        arrowCube.transform.SetParent(GameObject.FindGameObjectWithTag("3DCanvas").transform, false);

        note.arrowCube = arrowCube;

        return note;
    }

    public double AmountOfBeatsInSong()
    {
        return MusicPlayer.Instance.MusicLengthInSeconds() / BeatLenghtInSeconds;
    }

    public static double GetBeatTime(double bpm, double ms, double _time)
    {
        return GetMSInBeats(bpm, ms) + _time;
    }

    public static double GetMSInBeats(double bpm, double ms)
    {
        return (bpm / 60000) * ms;
    }

    public Note GetNote(double time, int cutDirection)
    {
        foreach (var note in NoteTimeChunks[time])
        {
            if (note._cutDirection == cutDirection)
                return note;
        }

        return null;
    }

    public Note GetNextNote(double currentTime)
    {
        return null;
    }

    #endregion
}