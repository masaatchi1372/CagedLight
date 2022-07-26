using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletoneMonoBehaviour<GameManager>
{
    public List<LevelSO> levelsList;
    private LevelSO currentLevel;
    private GameState gameState;
    private int levelNo;
    private int tryCount;
    private int enemyCount;

    private void Start()
    {
        // the first gameState is notPlaying
        gameState = GameState.notPlaying;

        // loading player prefs
        if (PlayerPrefs.HasKey("currentLevel"))
        {
            currentLevel = levelsList[PlayerPrefs.GetInt("currentLevel") - 1];
        }
        else // there's no currentLevel in PlayerPrefs which means it's the first time user opened the game
        {
            // we initiate PlayerPrefs and set current level to 1
            PlayerPrefs.SetInt("currentLevel", 1);
            currentLevel = levelsList[0];
        }

        // currentLevel.Info();
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.notPlaying:
                // for resetting all player prefs
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerPrefs.DeleteAll();
                }
                break;
            case GameState.loading:
                break;
            case GameState.playing:
                break;
            case GameState.lost:
                break;
            case GameState.won:
                break;
            case GameState.paused:
                break;
            case GameState.restart:
                break;
            default:
                break;
        }

        // if we ran out tries and enemies are alive, we lost this level
        // if (tryCount <= 0 && enemyCount > 0)
        // {
        //     Debug.Log("you Lose");
        // }
    }

    private void loadLevel(int levelNo)
    {
        currentLevel = levelsList[levelNo - 1];
        currentLevel.Info();
    }

    #region Unity Editor
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(levelsList), levelsList))
        {
            return;
        }
    }
#endif
    #endregion

}
