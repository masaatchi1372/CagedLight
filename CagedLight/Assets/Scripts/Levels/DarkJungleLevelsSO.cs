using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DarkJungleLevel_", menuName = "Scriptable Objects/Dark Jungle Level", order = 0)]
public class DarkJungleLevelsSO : ScriptableObject
{
    [Header("General Info")]
    [Space(5)]
    public int levelNo = 0;
    public int tryCount = 0;
    public int enemyCount = 0;
    public GameObject levelPrefab;
    public GameObject environment;

    #region editor code
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (HelperUtilities.ValidateCheckNonZeroIng(this, nameof(levelNo), levelNo)) return;
        if (HelperUtilities.ValidateCheckNonZeroIng(this, nameof(enemyCount), enemyCount)) return;
        if (levelPrefab == null) Debug.Log($"{this.name.ToString()} : levelPrefab is null");
        if (environment == null) Debug.Log($"{this.name.ToString()} : environment is null");
    }
#endif
    #endregion
}