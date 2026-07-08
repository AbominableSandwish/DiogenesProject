/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Private Data
    protected static GameManager _instance = null;
    private MapManager _map;
    #endregion


    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;
    public static int s_CurrentLevel = 0;
    public static int s_MaxAvailableLevel = 5;


    #region Mono
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }    
    }
    #endregion

    #region Public Method
    public static GameManager Instance { get => _instance; protected set => _instance = value; }
    public static MapManager GetMap()
    {
        return Instance._map;
    }

    public void ExitGame()
    {
        s_CurrentLevel = 0;
    }

    public void SetCurrentLevel(int level)
    {
        s_CurrentLevel = level;
    }

    public static void LoadNextLevel()
    {
        m_SceneLoadOpHandle = Addressables.LoadSceneAsync("LoadingScene", activateOnLoad: true);
    }

    public static void LevelCompleted()
    {
        s_CurrentLevel++;

        // Just to make sure we don't try to go beyond the allowed number of levels.
        s_CurrentLevel = s_CurrentLevel % s_MaxAvailableLevel;

        LoadNextLevel();
    }

    public static void ExitGameplay()
    {
        SceneManager.LoadSceneAsync("MainMenu");

        
    }
    #endregion
}
