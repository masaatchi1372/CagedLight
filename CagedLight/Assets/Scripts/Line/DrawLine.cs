using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private GameObject currentLine;
    private List<GameObject> lineGameObjectsList;
    private Queue<GameObject> deletionQueue;
    private Camera mainCam;
    private ObjectPooler objectPooler;
    private int AllowedLines;
    private int drawnLinesCount = -1;

    private void Start()
    {
        // initiating and caching the frequent use variables
        lineGameObjectsList = new List<GameObject>();
        deletionQueue = new Queue<GameObject>();
        objectPooler = ObjectPooler.Instance;
        mainCam = Camera.main;

        // subscribing to Events
        EventManager.LevelFinished += EventManagerOnLevelFinished;
        EventManager.LevelLoaded += EventManagerOnLevelLoaded;
    }

    void Update()
    {
        // on right mouse button all lines would be destroyed
        if (Input.GetMouseButtonDown(1))
        {
            RestartLevel();
        }

        // check for left mouse button or touch
        // we'll start a new line if one hasn't already in the scene and we're in Playing state
        if (Input.GetMouseButtonDown(0) && currentLine == null && GameManager.Instance.IsPlaying())
        {
            // if we reached our limits, we should also put other lines to be destroyed and fire restart level event to summon all obstacles again
            if (lineGameObjectsList.Count >= AllowedLines)
            {
                RestartLevel();
            }

            // Initiating the line
            SpawnLine();            
        }

        // caching currentLine if one exist
        Line currentLineComponent = null;
        if (currentLine != null)
        {
            currentLineComponent = currentLine.GetComponent<Line>();
        }

        // on Drag
        if (Input.GetMouseButton(0))
        {
            // we should set the zero of mouse input to avoid future issues
            Vector3 tempInput = Input.mousePosition;
            tempInput.z = 1f;
            tempInput = mainCam.ScreenToWorldPoint(tempInput);

            // if point distance is enough, we'll add a new point to the currentLine
            if (currentLineComponent != null && currentLineComponent.CanAddNewPoint(tempInput) && currentLineComponent.drawingState)
            {
                currentLineComponent.AddNewPoint(tempInput);
            }
        }

        // on release
        if (Input.GetMouseButtonUp(0) || (currentLineComponent && !currentLineComponent.drawingState))
        {
            if (currentLine != null)
            {
                // drawing is finished and we add the line to the lineList
                lineGameObjectsList.Add(currentLine);

                // we set the lastDrawnTime to current timestamp since game startup
                if (currentLineComponent != null)
                {
                    currentLineComponent.lastDrawnTime = Time.realtimeSinceStartup;
                    currentLineComponent.drawingState = false;
                }
            }

            // we drew a new line
            drawnLinesCount++;

            // the line is created and there's no drawing in process
            currentLine = null;
        }

        // each line should be continue its flow
        if (lineGameObjectsList.Count > 0)
        {
            foreach (GameObject line in lineGameObjectsList)
            {
                if (line != null)
                {
                    Line lineComponent = line.GetComponent<Line>();
                    if (lineComponent == null || lineComponent.inputPositions == null)
                    {
                        continue;
                    }

                    // if there's still one point on the line which user can see (it's in the screen) we should continue the line flow and if not we should destroy it
                    if (lineComponent.inputPositions.Count < 2 || !lineComponent.HasAtLeaseOnePointInScreen())
                    {
                        deletionQueue.Enqueue(line);
                        continue;
                    }

                    // if we should destroy the line
                    if (lineComponent.shouldDestroy)
                    {
                        // check the line points removal speed
                        if (Time.realtimeSinceStartup - lineComponent.lastPointRemovalTime > lineComponent.lineRemovalSpeed)
                        {
                            lineComponent.RemoveFirstPoint();
                        }
                    }
                    else // otherwise will continue the flow
                    {
                        lineComponent.ContinueLineFlow();
                    }
                }
            }
        }

        ClearDeletionQueue();

        // we tried our best but it seems like we failed (we have no active lines and draw count is equal to Allowed count)
        if (lineGameObjectsList.Count == 0 && drawnLinesCount >= AllowedLines)
        {
            RestartLevel();
        }
    }

    // Starting line creation process
    private void SpawnLine()
    {
        // initiating current line with our prefab with no vertex on its lineRenderer
        currentLine = objectPooler.SpawnFromPool("Line", Vector3.zero, Quaternion.identity);
    }

    // removing all lines in the LineGameObjectsList
    private void ClearDeletionQueue()
    {
        for (int i = 0; i < deletionQueue.Count; i++)
        {
            // pool back the object for future use
            GameObject line = deletionQueue.Dequeue();
            objectPooler.PoolObject("Line", line);

            // remove it from the list
            lineGameObjectsList.Remove(line);

            // let the event manager knows we lost a turn
            EventManager.OnLineDie();
        }
    }

    // destroy all line in lineGameObjectsList
    private void DestroyLines()
    {
        // foreach (GameObject line in lineGameObjectsList)
        // {
        //     if (line != null)
        //     {
        //         deletionQueue.Enqueue(line);
        //     }
        // }
        ShouldDestroyLines();
    }

    private void ShouldDestroyLines()
    {
        foreach (GameObject line in lineGameObjectsList)
        {
            if (line != null)
            {
                line.GetComponent<Line>().shouldDestroy = true;
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.LevelFinished -= EventManagerOnLevelFinished;
        EventManager.LevelLoaded -= EventManagerOnLevelLoaded;
    }

    private void RestartLevel()
    {
        GameManager.Instance.StateToRestart();
    }

    private void EventManagerOnLevelFinished()
    {
        DestroyLines();
    }

    private void EventManagerOnLevelLoaded(LevelSO level)
    {
        drawnLinesCount = 0;
        AllowedLines = level.tryAllowed;
        ShouldDestroyLines();
    }
}