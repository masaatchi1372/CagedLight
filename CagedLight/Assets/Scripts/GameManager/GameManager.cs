using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletoneMonoBehaviour<GameManager>
{
    public Transform levelParent;
    public List<LevelSO> levelsList;
    public float LevelChangeSpeed = 2f;
    private GameState gameState;
    private LevelSO currentLevel;
    private int levelNo;
    private int tryAllowed;
    private int enemyCount;
    private int gameRestartedCount = 0;
    private bool isLevelLoaded = false;
    private Queue<GameObject> trash = new Queue<GameObject>();

    private void Start()
    {
        //  for testing purposes
        PlayerPrefs.DeleteAll();

        // the first gameState is notPlaying
        gameState = GameState.notPlaying;

        // loading player prefs
        if (PlayerPrefs.HasKey("currentLevel"))
        {
            if (PlayerPrefs.GetInt("currentLevel") <= levelsList.Count)
            {
                levelNo = levelsList[PlayerPrefs.GetInt("currentLevel") - 1].levelNo;
            }
            else
            {
                PlayerPrefs.SetInt("currentLevel", 1);
                levelNo = 1;
            }
        }
        else // there's no currentLevel in PlayerPrefs which means it's the first time user opened the game
        {
            // we initiate PlayerPrefs and set current level to 1
            PlayerPrefs.SetInt("currentLevel", 1);
            levelNo = levelsList[0].levelNo;
        }

        // Debug.Log($"Level No: {levelNo}");

        // we start listening for events
        EventManager.EnemyDie += EventManagerOnEnemyDie;
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

                // if (Input.GetKeyDown(KeyCode.A))
                // {
                gameState = GameState.loading;
                // }
                break;
            case GameState.loading:
                if (!isLevelLoaded)
                {
                    putTheLevelInTrash(); // the former level should be thrown into trash (we'll remove it in future)
                    LoadLevel2(levelNo);
                    isLevelLoaded = true;
                }

                // new level should move towards the center of the screen
                var targetPosition = Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, 0.5f, 0));
                targetPosition.z = 0;
                var step = LevelChangeSpeed * Time.deltaTime; // calculate distance to move
                levelParent.position = Vector3.MoveTowards(levelParent.position, targetPosition, step);
                // check if we reached our distination
                if (Vector3.Distance(levelParent.position, targetPosition) < 0.001f)
                {
                    EmptyTrash();
                    isLevelLoaded = false;

                    // putting things back to normal
                    levelParent.position = Vector3.zero;
                    allChildrenToZero();


                    gameState = GameState.playing;
                    EventManager.OnLevelLoaded(levelsList[levelNo - 1]);
                }
                break;
            case GameState.playing:
                if (IsGameFinished())
                {
                    EventManager.OnLevelFinished();
                }
                break;
            case GameState.lost:
                break;
            case GameState.won:
                gameState = GameState.loading;
                break;
            case GameState.paused:
                break;
            case GameState.restart:
                Flush();
                LoadLevel(levelNo);
                gameState = GameState.playing;
                EventManager.OnLevelLoaded(levelsList[levelNo - 1]);
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

    public void StateToRestart()
    {
        if (gameState == GameState.playing)
        {
            gameState = GameState.restart;
        }
    }

    public bool IsPlaying()
    {
        if (gameState == GameState.playing)
        {
            return true;
        }
        return false;
    }

    private void allChildrenToZero()
    {
        for (int i = 0; i < levelParent.transform.childCount; i++)
        {
            levelParent.transform.GetChild(i).position = Vector3.zero;
        }
    }

    private void putTheLevelInTrash()
    {
        for (int i = 0; i < levelParent.transform.childCount; i++)
        {
            trash.Enqueue(levelParent.transform.GetChild(i).gameObject);
        }
    }

    private void EmptyTrash()
    {
        var size = trash.Count;
        for (int i = 0; i < size; i++)
        {
            var tmp = trash.Dequeue();
            Destroy(tmp);
        }
    }

    private void LoadLevel2(int levelNo)
    {
        levelNo = levelsList[levelNo - 1].levelNo;
        tryAllowed = levelsList[levelNo - 1].tryAllowed;
        enemyCount = levelsList[levelNo - 1].enemyCount;

        Instantiate(levelsList[levelNo - 1].environmentPrefab, Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 0.5f, 0)) + 2 * Vector3.forward, Quaternion.identity, levelParent);
        Instantiate(levelsList[levelNo - 1].levelPrefab, Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 0.5f, 0)) + 2 * Vector3.forward, Quaternion.identity, levelParent);
    }

    private void LoadLevel(int levelNo)
    {
        levelNo = levelsList[levelNo - 1].levelNo;
        tryAllowed = levelsList[levelNo - 1].tryAllowed;
        enemyCount = levelsList[levelNo - 1].enemyCount;

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
    }

    private void OnDestroy()
    {
        EventManager.EnemyDie -= EventManagerOnEnemyDie;
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
