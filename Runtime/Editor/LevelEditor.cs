using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using YodeGroup.BubbleShooter.GameElements;
using YodeGroup.BubbleShooter.GameElements.Bubbles;
using YodeGroup.BubbleShooter.Map;
using YodeGroup.BubbleShooter.Utility;
using YodeGroup.BubbleShooter;

namespace YodeGroup.BubbleShooter.Editor
{
    [CustomEditor(typeof(BubbleFactory))]
    public class LevelEditor : UnityEditor.Editor
    {
        private const string BubbleColliderPath = "BubbleCollider";
        private const string PathToData = "Assets/BubbleShooterEngine/Resources/Stages/";
        private const string ParentNameForLevelEditor = "LEVEL EDITOR";

        private const int ColumnsCount = 12;
        private const float OffsetBetweenBubblesOnX = 1f;
        private const float OffsetBetweenBubblesOnY = 1f;

        private readonly Vector3 _startPositionForBubbleSpawn = new Vector3(-5.497f, -2.505f, 0f);

        private BaseBubble _bubblePrefab;
        private CollectableItem _itemPrefab;
        private bool _isBubbleInstantiate;

        private BubbleFactory _bubbleFactory;

        private int _currentRows;
        private int _rows;

        private int _currentStage;
        private string _stageName;

        private void OnSceneGUI()
        {
            Event e = Event.current;

            _bubbleFactory = target as BubbleFactory;
            if (Application.isPlaying) return;

            if (_bubbleFactory == null) return;

            if (e.button == 0 && e != null && e.type == EventType.MouseDown)
            {
                var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                var hit = Physics2D.GetRayIntersection(ray, 100f);

                if (hit.collider && hit.collider.TryGetComponent(out BubbleMapEditorCollider collider))
                {
                    SetNewElement(collider);
                }
            }

            if (_rows != _currentRows)
            {
                GenerateBubbleColliders(_rows, GetBubbleCollider());
                _currentRows = _rows;
            }


            Handles.BeginGUI();

            DrawSettings();
            DrawBrush();

            Handles.EndGUI();
        }

        private BubbleMapEditorCollider GetBubbleCollider() =>
            Resources.Load<BubbleMapEditorCollider>(BubbleColliderPath);

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _bubbleFactory = (BubbleFactory) target;

            DrawEditorLine();
            DrawTitle();
            DrawEditorLine();
            DrawLevelDescription();
            DrawEditorLine();
            DrawSaveAndLoadButtons();
            DrawTestLevelButton();

            Repaint();
        }

        private void DrawTitle()
        {
            var style = new GUIStyle
            {
                fontSize = 22,
                fontStyle = FontStyle.Normal,
                normal = {textColor = Color.blue},
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.BeginHorizontal();
            GUILayout.Label("STAGE EDITOR", style);
            GUILayout.EndHorizontal();
        }

        private void DrawTestLevelButton()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Test current stage", GUILayout.Height(32)))
            {
                PlayerPrefs.SetInt("currentStage", _currentStage);
                PlayerPrefs.Save();

                Debug.Log(PlayerPrefs.GetInt("currentStage"));
            }

            GUILayout.EndHorizontal();
        }

        private void DrawSaveAndLoadButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save stage", GUILayout.Height(32)))
            {
                LevelData data = CreateInstance<LevelData>();
                data.Init(_stageName);
                SaveData(data);
            }

            if (GUILayout.Button("Load from file", GUILayout.Height(32)))
            {
                LoadStage(_stageName);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawLevelDescription()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name of stage:");
            _stageName = EditorGUILayout.TextField(_stageName);
            GUILayout.Label("Number of stage:");
            _currentStage = EditorGUILayout.IntField(_currentStage);
            GUILayout.EndHorizontal();
        }

        private static void DrawEditorLine()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("------------------------------------------------------------------------");
            GUILayout.EndHorizontal();
        }

        private Transform GetParent() =>
            serializedObject.FindProperty("bubblesParent").objectReferenceValue as Transform;

        private void SetNewElement(BubbleMapEditorCollider hit)
        {
            GameElement clone = _isBubbleInstantiate
                ? CreateObject(_bubblePrefab)
                : (GameElement) CreateObject(_itemPrefab);

            clone.transform.position = hit.transform.position;
            clone.transform.SetParent(GetParent());

            DestroyImmediate(hit.gameObject);
        }

        private void DrawSettings()
        {
            GUILayout.BeginArea(new Rect(0, 0, 500, 50));
            GUILayout.BeginHorizontal("Button");
            if (GUILayout.Button("Reset Bubbles!"))
            {
                ResetBubbles();
            }

            GUILayout.Label("Number of rows:");
            _rows = EditorGUILayout.IntSlider(_rows, 0, 18);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawBrush()
        {
            GUILayout.BeginArea(new Rect(0, 30, 150, 500));
            GUILayout.BeginVertical("box");

            foreach (var bubble in _bubbleFactory.bubbles)
            {
                if (GUILayout.Button(bubble.name, GUILayout.Width(140), GUILayout.Height(20)))
                {
                    _bubblePrefab = bubble;
                    _isBubbleInstantiate = true;
                }
            }

            foreach (var item in _bubbleFactory.heroes)
            {
                if (GUILayout.Button(item.name, GUILayout.Width(80), GUILayout.Height(20)))
                {
                    _itemPrefab = item;
                    _isBubbleInstantiate = false;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private static T CreateObject<T>(T prefab) where T : Component
        {
            if (Application.isPlaying)
                return Instantiate(prefab);
            return PrefabUtility.InstantiatePrefab(prefab) as T;
        }

        private void GenerateBubbleColliders(int nRows, BubbleMapEditorCollider bubbleCollider)
        {
            if (bubbleCollider == null)
                throw new ArgumentNullException(nameof(bubbleCollider));

            float newPosX = _startPositionForBubbleSpawn.x;
            float newPosY = _startPositionForBubbleSpawn.y;
            BubbleMapEditorCollider[] objs = FindObjectsOfType<BubbleMapEditorCollider>();

            foreach (BubbleMapEditorCollider obj in objs)
                DestroyImmediate(obj.gameObject);

            Transform parent = GameObject.Find(ParentNameForLevelEditor).transform;
            if (parent == null)
                parent = new GameObject(ParentNameForLevelEditor).transform;

            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < ColumnsCount; j++)
                {
                    var clone = PrefabUtility.InstantiatePrefab(bubbleCollider) as BubbleMapEditorCollider;
                    clone.transform.position = new Vector3(newPosX, newPosY, _startPositionForBubbleSpawn.z);
                    clone.transform.SetParent(parent);
                    newPosX += OffsetBetweenBubblesOnX;
                }

                newPosY += OffsetBetweenBubblesOnY;
                newPosX = _startPositionForBubbleSpawn.x;
            }
        }

        private void ResetBubbles()
        {
            FindObjectsOfType<BaseBubble>().ToList().ForEach(bubble => DestroyImmediate(bubble.gameObject));
            FindObjectsOfType<CollectableItem>().ToList().ForEach(item => DestroyImmediate(item.gameObject));
        }

        private void SaveData(LevelData stageData)
        {
            Debug.Log(stageData.name);
            string path = GetAssetPathToLevel(stageData.stageName);
            AssetDatabase.CreateAsset(stageData, path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void LoadStage(string stageName)
        {
            var stageData = AssetDatabase.LoadAssetAtPath<LevelData>(GetAssetPathToLevel(stageName));

            ResetBubbles();

            foreach (BubbleInfo bubbleInfo in stageData.bubblesInfo)
            {
                BaseBubble prefab =
                    _bubbleFactory.bubbles.FirstOrDefault(bubble => bubble.Type == bubbleInfo.bubbleType);
                BaseBubble bubbleClone = CreateObject(prefab);
                bubbleClone.transform.position = bubbleInfo.position;
                bubbleClone.transform.SetParent(GetParent());
            }

            foreach (HeroInfo itemInfo in stageData.heroesInfo)
            {
                CollectableItem prefab = _bubbleFactory.heroes.FirstOrDefault(item => item.Type == itemInfo.heroType);
                CollectableItem clone = CreateObject(prefab);
                clone.transform.position = itemInfo.position;
                clone.transform.SetParent(GetParent());
            }
        }

        private string GetAssetPathToLevel(string stageName)
        {
            return $"{PathToData}{stageName}.asset";
        }
    }
}