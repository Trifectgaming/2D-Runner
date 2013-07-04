using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

public class LevelRecording : MonoBehaviour {

    private List<List<InputRecord>> replayQueue = new List<List<InputRecord>>();
    private XmlSerializer _serializer = new XmlSerializer(typeof(List<List<InputRecord>>));
    public static List<InputRecord> currentQueue;
    public bool reset;

    void Awake()
    {
        //Messenger.Default.Register<InputProcessedMessage>(this, OnInputProcessed);
        Messenger.Default.Register<GameStartMessage>(this, OnGameStart);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
    }

    private void OnGameOver(GameOverMessage obj)
    {
        replayQueue.Add(currentQueue);
        currentQueue = null;
    }

    private void OnGameStart(GameStartMessage obj)
    {
        currentQueue = new List<InputRecord>();
    }

    public static void Enqueue(InputRecord inputRecord)
    {
        if (inputRecord.InputState != InputState.None)
            currentQueue.Add(inputRecord);
    }

    private void Start()
    {
        //TODO: load from player prefs
        OnGameStart(new GameStartMessage());
        LoadLevelRecording();
        //StartCoroutine(SaveLevelRecording());
    }

    private void LoadLevelRecording()
    {
        if (reset)
            PlayerPrefs.DeleteKey("LevelRecording");
        var levelRecording = PlayerPrefs.GetString("LevelRecording", null);
        if (string.IsNullOrEmpty(levelRecording)) return;
        using (var memoryStream = new MemoryStream())
        {
            List<List<InputRecord>> data;
            using (var writer = new StreamWriter(memoryStream)
                                    {
                                        AutoFlush = true
                                    })
            {
                writer.Write(levelRecording);
                memoryStream.Position = 0;
                data = (List<List<InputRecord>>) _serializer.Deserialize(memoryStream);
            }
            replayQueue = data;
        }
    }

    void Update () {
	    
	}

    void OnApplicationQuit()
    {
        //StopCoroutine("SaveLevelRecording");
        //PlayerPrefs.Save();
    }

    private IEnumerator SaveLevelRecording()
    {
        while (true)
        {
            if (currentQueue != null)
                replayQueue.Add(currentQueue);
            using (var memoryStream = new MemoryStream())
            {
                _serializer.Serialize(memoryStream, replayQueue);
                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream))
                {
                    var data = reader.ReadToEnd();
                    PlayerPrefs.SetString("LevelRecording", data);
                }
            }
            Debug.Log("Saved Level Recording");
            yield return new WaitForSeconds(2);
        }
    }
}