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
    public ReplayRunner runnerPrefab;
    public bool reset;
    public int maxReplays;
    public bool createReplays;

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
        OnGameStart(new GameStartMessage());
        if (createReplays)
        {
            LoadLevelRecording();
            CreateReplay();
            StartCoroutine(SaveLevelRecording());
        }
    }

    private void CreateReplay()
    {
        for (int index = 0; index < replayQueue.Count; index++)
        {
            var replay = replayQueue[index];
            var runner = (ReplayRunner)Instantiate(runnerPrefab, new Vector3(transform.position.x, transform.position.y, 0),
                                     Quaternion.identity);
            runner.name = "ReplayRunner" + index;
            runner.LoadInputQueue(replay);
        }
    }

    private void LoadLevelRecording()
    {
        if (reset)
            PlayerPrefs.DeleteKey(GetLevelKeyName());
        var levelRecording = PlayerPrefs.GetString(GetLevelKeyName(), null);
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
            Debug.Log("Replay Queue contains " + replayQueue.Count + " replays");
        }
    }

    void Update () {
	    
	}

    void OnApplicationQuit()
    {
        if (createReplays)
        {
            StopCoroutine("SaveLevelRecording");
            PlayerPrefs.Save();
        }
    }

    private IEnumerator SaveLevelRecording()
    {
        while (gameObject.activeSelf)
        {
            if (currentQueue != null && !replayQueue.Contains(currentQueue))
            {
                replayQueue.Insert(0, currentQueue);
                if (replayQueue.Count > maxReplays)
                {
                    while (replayQueue.Count > maxReplays)
                    {
                        replayQueue.RemoveAt(replayQueue.Count - 1);
                    }
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                _serializer.Serialize(memoryStream, replayQueue);
                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream))
                {
                    var data = reader.ReadToEnd();
                    PlayerPrefs.SetString(GetLevelKeyName(), data);
                }
            }
            Debug.Log("Saved Level Recording");
            yield return new WaitForSeconds(2);
        }
    }

    private static string GetLevelKeyName()
    {
        return "LevelRecording" + Application.loadedLevelName;
    }
}