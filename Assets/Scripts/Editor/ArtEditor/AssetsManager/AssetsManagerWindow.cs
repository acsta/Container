#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace TaoTie
{
    public class AssetsManagerWindow : EditorWindow
    {
        private AssetsManagerConfig config;

        Vector2 scrollViewPos;
        Vector2 scrollViewPos2;
        Vector2 scrollViewPos3;
        float splitterPos = 100;
        float splitterPos2 = 200;
        Rect splitterRect;
        Rect splitterRect2;
        Vector2 dragStartPos;
        bool dragging;
        bool dragging2;
        float splitterWidth = 5;



        private List<GameObject> prefabs = new List<GameObject>();

        HashSet<string> temp = new HashSet<string>();
        private int curType = -1;
        private int curLabel = 0;
        private string type;
        private string label;

        [MenuItem("Tools/工具/地编/模型库")]
        private static void Open()
        {
            var window = GetWindow<AssetsManagerWindow>(false, "模型库");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 600);
            window.splitterPos = 100;
            window.splitterPos2 = 200;
        }


        private void OnGUI()
        {
            // var ids = AssetDatabase.FindAssets("t:Mesh", new[] { "Assets/AssetsPackage" });
            // var path = AssetDatabase.GUIDToAssetPath(ids);
            // AssetDatabase.LoadAssetAtPath<Mesh>(path)
            if (config == null)
            {
                config = AssetDatabase.LoadAssetAtPath<AssetsManagerConfig>(AssetsManagerConfig.ConfigPath);
                Selection.activeObject = config;
            }
            if (config == null)
            {
                var cc = CreateInstance<AssetsManagerConfig>();
                AssetDatabase.CreateAsset(cc, AssetsManagerConfig.ConfigPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return;
            }
            EditorGUILayout.BeginHorizontal();
            //scrollView
            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, GUILayout.Width(splitterPos),
                GUILayout.Height(position.height - 10));
            temp.Clear();
            for (int i = 0; i < config.Labels.Count; i++)
            {
                temp.Add(config.Labels[i].Label);
            }

            var typeList = temp.ToArray();
            int type = GUILayout.SelectionGrid(curType, typeList, 1);
            if (type != curType)
            {
                curType = type;
                curLabel = 0;
            }

            EditorGUILayout.EndScrollView();
            // Splitter
            GUILayout.Box("",
                GUILayout.Width(splitterWidth),
                GUILayout.MaxWidth(splitterWidth),
                GUILayout.MinWidth(splitterWidth),
                GUILayout.ExpandHeight(true));
            splitterRect = GUILayoutUtility.GetLastRect();

            //scrollView
            scrollViewPos3 = EditorGUILayout.BeginScrollView(scrollViewPos3,
                GUILayout.Width(splitterPos2 - splitterPos), GUILayout.Height(position.height - 10));
            temp.Clear();
            if (curType >= 0)
            {
                temp.Add("全部");
                for (int i = 0; i < config.Labels.Count; i++)
                {
                    if (config.Labels[i].Label == typeList[curType])
                    {
                        for (int j = 0; j < config.Labels[i].Collects.Count; j++)
                        {
                            temp.Add(config.Labels[i].Collects[j].Label);
                        }
                    }
                }
            }

            var labelList = temp.ToArray();
            curLabel = GUILayout.SelectionGrid(curLabel, labelList, 1);

            EditorGUILayout.EndScrollView();

            // Splitter
            GUILayout.Box("",
                GUILayout.Width(splitterWidth),
                GUILayout.MaxWidth(splitterWidth),
                GUILayout.MinWidth(splitterWidth),
                GUILayout.ExpandHeight(true));
            splitterRect2 = GUILayoutUtility.GetLastRect();


            //prefabs
            EditorGUILayout.BeginVertical();

            var size = 100;
            var maxButtonsPerRow = Mathf.FloorToInt((position.width - splitterPos2 - 40) / size); // 每行最大按钮数目
            var buttonCount = 0;
            var bottonHeight = 0;
            if (curType != -1)
            {
                SearchPrefab(typeList[curType], labelList[curLabel]);
            }

            scrollViewPos2 = EditorGUILayout.BeginScrollView(scrollViewPos2,
                GUILayout.Width(position.width - splitterPos2 - 40), GUILayout.Height(position.height - 10));
            Event evt = Event.current;
            GUILayout.BeginHorizontal();

            foreach (var prefab in prefabs)
            {
                if (buttonCount >= maxButtonsPerRow)
                {
                    GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    buttonCount = 0;
                    bottonHeight++;
                }

                Rect assetItemRect =
                    GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
                GUI.Box(assetItemRect, GetPreview(prefab, bottonHeight * 100));
                if (evt.isMouse && assetItemRect.Contains(evt.mousePosition))
                {
                    if (evt.type == EventType.MouseDown)
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.StartDrag("Dragging title");
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.objectReferences = new UnityEngine.Object[] {prefab};
                        evt.Use();
                        if (evt.button == 0)
                        {
                            Selection.activeGameObject = prefab;
                        }
                        //右键弹窗
                        else if (evt.button == 1)
                        {
                            GenericMenu menu = new GenericMenu();

                            menu.AddItem(new GUIContent("删除"), false, () =>
                            {

                                if (EditorUtility.DisplayDialog("删除", "确定要删除吗？", "确定", "取消"))
                                {
                                    prefabs.Remove(prefab);
                                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prefab));
                                    AssetDatabase.Refresh();
                                }
                            });
                            menu.ShowAsContext();
                        }
                    }
                }

                buttonCount++;
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            // Splitter events
            if (Event.current != null)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (splitterRect.Contains(Event.current.mousePosition))
                        {
                            // Debug.Log ("Start dragging");
                            dragging = true;
                        }

                        if (splitterRect2.Contains(Event.current.mousePosition))
                        {
                            // Debug.Log ("Start dragging");
                            dragging2 = true;
                        }

                        break;
                    case EventType.MouseDrag:
                        if (dragging)
                        {
                            // Debug.Log ("moving splitter");
                            splitterPos += Event.current.delta.x;
                            splitterPos = Mathf.Clamp(splitterPos, 100, splitterPos2 - 100);
                            Repaint();
                        }

                        if (dragging2)
                        {
                            // Debug.Log ("moving splitter");
                            splitterPos2 += Event.current.delta.x;
                            splitterPos2 = Mathf.Clamp(splitterPos2, splitterPos + 100, position.width - 150);
                            Repaint();
                        }

                        break;
                    case EventType.MouseUp:
                        if (dragging)
                        {
                            dragging = false;
                        }

                        if (dragging2)
                        {
                            dragging2 = false;
                        }

                        break;
                }
            }

        }


        private Texture2D GetPreview(GameObject obj, float height)
        {
            if (Mathf.Abs(scrollViewPos2.y - height) < position.height)
                return AssetPreview.GetAssetPreview(obj);
            return AssetPreview.GetMiniThumbnail(obj);
        }

        private void SearchPrefab(string type, string label)
        {
            if (this.type != type || this.label != label)
            {
                this.type = type;
                this.label = label;
                prefabs.Clear();
                temp.Clear();
                for (int i = 0; i < config.Labels.Count; i++)
                {
                    if (this.type == "全部" || config.Labels[i].Label == this.type)
                    {
                        for (int j = 0; j < config.Labels[i].Collects.Count; j++)
                        {
                            var obj = config.Labels[i].Collects[j];
                            if (this.label == "全部" || obj.Label == this.label)
                            {
                                for (int k = 0; k < obj.Objects.Count; k++)
                                {
                                    var path = AssetDatabase.GetAssetPath(obj.Objects[k]);
                                    if (AssetDatabase.IsValidFolder(path))
                                    {
                                        temp.Add(path);
                                    }
                                    else if (obj.Objects[k] is GameObject prefab)
                                    {
                                        prefabs.Add(prefab);
                                    }
                                }
                            }

                        }
                    }
                }

                if (temp.Count <= 0) return;
                var assetGuids = AssetDatabase.FindAssets("t:Prefab", temp.ToArray());

                foreach (var guid in assetGuids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    prefabs.Add(prefab);
                }
            }

        }
    }
}
#endif