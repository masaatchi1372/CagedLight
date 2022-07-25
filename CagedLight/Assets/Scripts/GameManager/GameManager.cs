using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletoneMonoBehaviour<GameManager>
{
    public int enemyCount = 0;
    public int tryCount = 10;
    private int levelNumber;

    private void loadLevel(int levelNo)
    {
        levelNumber = levelNo;
    }

    void Update()
    {
        // for resetting all player prefs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }

        // if we ran out tries and enemies are alive, we lost this level
        if (tryCount <= 0 && enemyCount > 0)
        {
            Debug.Log("you Lose");
        }
    }

}
