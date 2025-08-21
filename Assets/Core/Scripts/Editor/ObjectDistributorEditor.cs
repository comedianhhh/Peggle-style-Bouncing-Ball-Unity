#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectDistributor))]
public class ObjectDistributorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ObjectDistributor distributor = (ObjectDistributor)target;
        
        GUILayout.Space(10);
        
        // Distribute Evenly button
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 14;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fixedHeight = 35;
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🎯 Distribute Objects Evenly", buttonStyle))
        {
            Undo.RecordObjects(GetTransforms(distributor), "Distribute Objects");
            distributor.DistributeObjects();
            EditorUtility.SetDirty(distributor);
        }
        
        // Reset button
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("↩️ Reset to Original Positions", buttonStyle))
        {
            ResetToOriginalPositions(distributor);
        }
        
        // Set current position as start button
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("📍 Set Current Position as Start", buttonStyle))
        {
            Undo.RecordObject(distributor, "Set Start Position");
            distributor.startPosition = distributor.transform.position;
            EditorUtility.SetDirty(distributor);
        }
        
        GUI.backgroundColor = Color.white;
        
        // Info display
        if (distributor.objectsToDistribute != null && distributor.objectsToDistribute.Length > 0)
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox($"Will distribute {distributor.objectsToDistribute.Length} objects", MessageType.Info);
            
            float distance = Vector3.Distance(distributor.startPosition, distributor.endPosition);
            EditorGUILayout.HelpBox($"Distribution distance: {distance:F2} units", MessageType.None);
        }
        else
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Please add objects to the array first", MessageType.Warning);
        }
        
        // Quick fill button
        GUILayout.Space(10);
        GUI.backgroundColor = Color.magenta;
        if (GUILayout.Button("🔍 Auto Fill Selected Objects", buttonStyle))
        {
            FillWithSelectedObjects(distributor);
        }
        GUI.backgroundColor = Color.white;
    }
    
    Transform[] GetTransforms(ObjectDistributor distributor)
    {
        if (distributor.objectsToDistribute == null) return new Transform[0];
        
        Transform[] transforms = new Transform[distributor.objectsToDistribute.Length];
        for (int i = 0; i < distributor.objectsToDistribute.Length; i++)
        {
            if (distributor.objectsToDistribute[i] != null)
                transforms[i] = distributor.objectsToDistribute[i];
        }
        return transforms;
    }
    
    void ResetToOriginalPositions(ObjectDistributor distributor)
    {
        if (distributor.objectsToDistribute == null) return;
        
        Undo.RecordObjects(GetTransforms(distributor), "Reset Positions");
        
        EditorUtility.DisplayDialog("Reset Location", "Reset requires storing original positions first. Please undo manually or reposition the objects.", "OK");
    }
    
    void FillWithSelectedObjects(ObjectDistributor distributor)
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Objects Selected", "Please select objects in the scene to distribute", "OK");
            return;
        }
        
        Undo.RecordObject(distributor, "Fill Selected Objects");
        
        distributor.objectsToDistribute = new Transform[selectedObjects.Length];
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            distributor.objectsToDistribute[i] = selectedObjects[i].transform;
        }
        
        EditorUtility.SetDirty(distributor);
        EditorUtility.DisplayDialog("Fill Complete", $"Added {selectedObjects.Length} objects to the distribution list", "OK");
    }
}
#endif