using UnityEngine;
using UnityEngine.Events;

public class EventManager : SingletoneMonoBehaviour<EventManager>
{
    /* 
    * A template for new events in the eventManager
    * public static event UnityAction<Type> LineDie;
    * public static void OnLineDie(Type param1) => LineDie?.Invoke(param1);
    */

    public static event UnityAction EnemyDie;
    public static void OnEnemyDie() => EnemyDie?.Invoke();

    public static event UnityAction LineDie;
    public static void OnLineDie() => LineDie?.Invoke();

    public static event UnityAction DrawLine;
    public static void OnDrawLine() => DrawLine?.Invoke();

    public static event UnityAction LevelFinished;
    public static void OnLevelFinished() => LevelFinished?.Invoke();

    public static event UnityAction<LevelSO> LevelLoaded;
    public static void OnLevelLoaded(LevelSO level) => LevelLoaded?.Invoke(level);
}
