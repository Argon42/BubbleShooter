using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using YodeGroup.BubbleShooter.GameElements.Bubbles;

public class BubbleTool : EditorWindow
{
    [SerializeField] private List<SimpleBubble> bubbleDataset;


    [MenuItem("Tools/FuckBubbles")]
    private static void Init()
    {
        BubbleTool window = (BubbleTool) EditorWindow.GetWindow(typeof(BubbleTool));
        window.Show();
    }

    private void OnGUI()
    {
        var obj = new SerializedObject(this);
        obj.Update();
        EditorGUILayout.PropertyField(obj.FindProperty(nameof(bubbleDataset)));
        obj.ApplyModifiedProperties();

        if (GUILayout.Button("ChangeBubbles"))
        {
            foreach (var transform in Selection.transforms)
            {
                bool CheckType(BaseBubble d) => d.Type == transform.GetComponent<BaseBubble>()?.Type;

                var prefab = bubbleDataset.FirstOrDefault(CheckType);
                var position = transform.position;
                var parent = transform.parent;
                DestroyImmediate(transform.gameObject);
                if(prefab == null) continue;
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject, parent);
                instance.transform.position = position;
            }
        }
    }
}