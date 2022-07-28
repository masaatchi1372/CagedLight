using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletoneMonoBehaviour<GameManager>
{
    public Transform levelParent;
    public List<LevelSO> levelsList;
    private int tryCount = 0;
    private LevelSO currentLevel;
    private GameState gameState;
    private int levelNo;
    private int tryAllowed;
    private int enemyCount;

    private void Start()
    {
        // the first gameState is notPlaying
        gameState = GameState.notPlaying;

        // loading player prefs
        if (PlayerPrefs.HasKey("currentLevel"))
        {
            levelNo = levelsList[PlayerPrefs.GetInt("currentLevel") - 1].levelNo;
        }
        else // there's no currentLevel in PlayerPrefs which means it's the first time user opened the game
        {
            // we initiate PlayerPrefs and set current level to 1
            PlayerPrefs.SetInt("currentLevel", 1);
            levelNo = levelsList[0].levelNo;
        }

        Debug.Log($"Level No: {levelNo}");

        // we start listening for OnEnemyDie event
        EventManager.EnemyDie += EventManagerOnEnemyDie;

        // we start listening for line events
        EventManager.LineDie += EventManagerOnLineDie;
        EventManager.DrawLine += EventManagerOnDrawLine;
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

                if (Input.GetKeyDown(KeyCode.A))
                {
                    gameState = GameState.loading;
                }
                break;
            case GameState.loading:
                LoadLevel(levelNo);
                gameState = GameState.playing;
                break;
            case GameState.playing:
                IsGameFinished();
                break;
            case GameState.lost:
                Flush();
                gameState = GameState.loading;
                break;
            case GameState.won:
                Flush();
                gameState = GameState.loading;
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

    private void LoadLevel(int levelNo)
    {
        levelNo = levelsList[levelNo - 1].levelNo;
        tryAllowed = levelsList[levelNo - 1].tryAllowed;
        enemyCount = levelsList[levelNo - 1].enemyCount;
        tryCount = 0;

        Debug.Log($"-> Level No:{levelNo}, TA:{tryAllowed}, EN:{enemyCount}, TC:{tryCount}");

        Instantiate(levelsList[levelNo - 1].environmentPrefab, Vector3.zero, Quaternion.identity, levelParent);
        Instantiate(levelsList[levelNo - 1].levelPrefab, Vector3.zero, Quaternion.identity, levelParent);
    }

    private bool IsGameFinished()
    {
        if (enemyCount < 1)
        {
            gameState = GameState.won; // we eliminated all enemies

            // we should go to the next level
            levelNo++;

            // saving levelNo in PlayerPrefs
            PlayerPrefs.SetInt("currentLevel", levelNo);

            return true;
        }

        if (tryAllowed != -1 && tryCount >= tryAllowed)
        {
            gameState = GameState.lost; // we lost and can't cast any more spells, tryCount is zero
            return true;
        }

        return false;
    }

    public void Flush()
    {
        for (int i = 0; i < levelParent.transform.childCount; i++)
        {
            Destroy(levelParent.transform.GetChild(i).gameObject);
        }
    }

    private void EventManagerOnEnemyDie()
    {        
        enemyCount--;
        Debug.Log($"enemyCount decreased:{enemyCount}");
    }

    public void EventManagerOnLineDie()
    {
        // Debug.Log("tryCount Decreased");
        // tryCount--;
    }

    public void EventManagerOnDrawLine()
    {
        tryCount++;
        Debug.Log($"tryCount Increased:{tryCount}");
    }

    public bool CanDrawLine()
    {
        if (tryAllowed == -1 || tryCount < tryAllowed || gameState != GameState.playing)
        {
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        EventManager.EnemyDie -= EventManagerOnEnemyDie;
        EventManager.LineDie -= EventManagerOnLineDie;
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
