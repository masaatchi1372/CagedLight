using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DarkJungleLevel_", menuName = "Scriptable Objects/Dark Jungle Level", order = 0)]
public class LevelSO : ScriptableObject
{
    [Header("General Info")]
    [Space(5)]
    [Tooltip("Level Number")]
    public int levelNo = 0;
    [Tooltip("how many turns does player have for this level? LEAVE -1 FOR INFINITE TRIES")]
    public int tryAllowed = -1;
    [Tooltip("how many enemies are there in the level")]
    public int enemyCount = 0;
    [Tooltip("populate this part with the level prefab")]
    public GameObject levelPrefab;
    [Tooltip("populate this part with the background environment prefab you designed")]
    public GameObject environmentPrefab;

    public void Info()
    {
        Debug.Log($"Level General Info: no:{levelNo}, try count:{tryAllowed}, enemies:{enemyCount}");
    }

    #region editor code
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (HelperUtilities.ValidateCheckNonZeroInt(this, nameof(levelNo), levelNo)) return;
        if (HelperUtilities.ValidateCheckNonZeroInt(this, nameof(enemyCount), enemyCount)) return;
        if (levelPrefab == null) Debug.Log($"{this.name.ToString()} : levelPrefab is null");
        if (environmentPrefab == null) Debug.Log($"{this.name.ToString()} : environment is null");
    }
#endif
    #endregion
}