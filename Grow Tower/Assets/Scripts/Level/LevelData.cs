using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{

    [HideInInspector] public int playerTower;
    [HideInInspector] public int playerStrength;
    
    [HideInInspector] public List<EnemyTowerDetails> enemyTower = new List<EnemyTowerDetails>();

    [HideInInspector] public Enums.RESTRICTION restriction;

}

[System.Serializable]
public class EnemyTowerDetails
{
    public int[] enemies;
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{

    private SerializedProperty _enemyTower;
    private readonly List<bool> etFoldout = new List<bool>();

    private void OnEnable()
    {
        _enemyTower = serializedObject.FindProperty("enemyTower");
        for (int i = 0; i < _enemyTower.arraySize; i++)
            etFoldout.Add(false);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        LevelData data = (LevelData)target;
        data.restriction = (Enums.RESTRICTION)EditorGUILayout.EnumPopup("Game Restriction: ", data.restriction);
        //Player Data
        EditorGUILayout.LabelField("How many floors does player have early?",
                new GUIStyle() { fontStyle = FontStyle.Bold });
        data.playerTower = EditorGUILayout.IntField("", data.playerTower);
        EditorGUILayout.LabelField("How much is the player strength?",
                new GUIStyle() { fontStyle = FontStyle.Bold });
        data.playerStrength = EditorGUILayout.IntField("", data.playerStrength);

        EditorGUILayout.Space();

        //Enemy Data
        EditorGUILayout.LabelField("How many floors does enemy have early?",
                new GUIStyle() { fontStyle = FontStyle.Bold });
        EditorGUI.BeginChangeCheck();
        _enemyTower.arraySize = EditorGUILayout.IntField("", _enemyTower.arraySize);
        if (_enemyTower.arraySize < 1) _enemyTower.arraySize = 1;
        if (EditorGUI.EndChangeCheck())
        {
            etFoldout.Clear();
            for (int i = 0; i < _enemyTower.arraySize; i++)
                etFoldout.Add(false);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        //Complex part of the enemy tower, which might be more simple but I couldn't figure it out
        if (_enemyTower.arraySize > 0)
        {
            for (int i = 0; i < _enemyTower.arraySize; i++)
            {
                var floor = _enemyTower.GetArrayElementAtIndex(i);

                if (i.Equals(0))
                    etFoldout[i] = EditorGUILayout.Foldout(etFoldout[i], "Ground Floor");
                else
                    etFoldout[i] = EditorGUILayout.Foldout(etFoldout[i], "Floor " + i);

                EditorGUI.indentLevel++;

                if (etFoldout[i])
                {
                    var enemies = floor.FindPropertyRelative("enemies");
                    EditorGUILayout.LabelField("Amount of enemies in the floor");
                    enemies.arraySize = EditorGUILayout.IntField("", enemies.arraySize);

                    EditorGUI.indentLevel++;
                    for (int j = 0; j < enemies.arraySize; j++)
                    {
                        var enemy = enemies.GetArrayElementAtIndex(j);
                        EditorGUILayout.PropertyField(enemy, new GUIContent("Enemy " + (j + 1) + " Strength"));
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        //Change Check
        if (GUI.changed)
        {
            if (data.playerTower < 1) data.playerTower = 1;
            
            EditorUtility.SetDirty(data);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif