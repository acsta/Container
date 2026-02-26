using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TaoTie
{
    public class FontSubsetEditor: EditorWindow
    {
        private const string exeName = "Bin/fontsubset-console.dll";
        private Object font;
        private bool collectAscii = true;
        private bool collectI18nConfig = true;
        private bool collectCS;
        private bool collectPrefab;
        private bool collectConfig;
        private void OnGUI()
        {
            collectAscii = EditorGUILayout.Toggle("保留Ascii字符", collectAscii);
            collectI18nConfig = EditorGUILayout.Toggle("保留多语言表", collectI18nConfig);
            collectCS = EditorGUILayout.Toggle("保留代码硬编码文本", collectCS);
            collectPrefab = EditorGUILayout.Toggle("保留预制体硬编码文本", collectPrefab);
            collectConfig = EditorGUILayout.Toggle("保留所有配置表硬编码文本", collectConfig);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("待裁剪字体:", GUILayout.Width(70));
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
            font = EditorGUI.ObjectField(rect, font, typeof(Object), false);
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.BeginDisabledGroup(!font);
            if (GUILayout.Button("裁剪"))
            {
                if (!File.Exists(exeName))
                {
                    Debug.LogError("先编译Tools/fontsubset.sln到" + exeName);
                    return;
                }
                
                var chars = DoSelectChars();
                File.WriteAllText("Bin/chars.txt", chars);
                List<string> param = new List<string>();
                param.Add(exeName);
                if(collectAscii) param.Add("-a");
                param.Add("-c \"Bin/chars.txt\"");
                var path = AssetDatabase.GetAssetPath(font);
                param.Add("\""+path  + "\"");
                if (!path.Contains("FontsAddon"))
                {
                    param.Add("\""+path+".new"  + "\"");
                }
                else
                {
                    param.Add("\""+path.Replace("FontsAddon","Fonts")  + "\"");
                }
                
                BashUtil.RunCommand2("", "dotnet", param.ToArray());
                AssetDatabase.Refresh();
            }
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("输出引用字符"))
            {
                var chars = DoSelectChars();
                File.WriteAllText("Bin/chars.txt", chars);
                Debug.Log("输出到Bin/chars.txt");
            }
        }

        private string DoSelectChars()
        {
            HashSet<char> chars = new HashSet<char>();
            SelectI18NChars(chars);
            SelectCsChars(chars);
            SelectPrefabChars(chars);
            SelectConfigChars(chars);
            StringBuilder sb = new StringBuilder();
            foreach (var item in chars)
            {
                sb.Append(item);
            }
            return sb.ToString();
        }

        private void SelectI18NChars(HashSet<char> chars)
        {
            if(!collectI18nConfig) return;
            foreach (var lang in Enum.GetNames(typeof(LangType)))
            {
                var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/AssetsPackage/Config/{lang}.bytes")?.bytes;
                if (bytes != null)
                {
                    I18NConfigCategory category = ProtobufHelper.FromBytes<I18NConfigCategory>(bytes);
                    foreach (var item in category.GetAllList())
                    {
                        if(string.IsNullOrEmpty(item.Value)) continue;
                        for (int i = 0; i < item.Value.Length; i++)
                        {
                            if(item.Value[i] == ' '|| item.Value[i] == '\r'|| item.Value[i] == '\n') continue;
                            chars.Add(item.Value[i]);
                        }
                    }
                }
            }
        }

        private void SelectCsChars(HashSet<char> chars)
        {
            if(!collectCS) return;
            var csFiles = Directory.GetFiles("Assets/Scripts/Code", "*.cs", SearchOption.AllDirectories);
            foreach (var item in csFiles)
            {
                var lines = File.ReadAllLines(item);
                foreach (var line in lines)
                {
                    if(!line.Contains("\"")) continue;
                    var vs = line.Split("//");
                    foreach (var tt in vs[0])
                    {
                        chars.Add(tt);
                    }
                }
            }
        }

        private void SelectPrefabChars(HashSet<char> chars)
        {
            if(!collectPrefab) return;
            var guids = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/AssetsPackage"});
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var texts = prefab.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    foreach (var tt in texts[i].text)
                    {
                        chars.Add(tt);
                    }
                }
                
                var texts2 = prefab.GetComponentsInChildren<TMPro.TMP_Text>(true);
                for (int i = 0; i < texts2.Length; i++)
                {
                    foreach (var tt in texts2[i].text)
                    {
                        chars.Add(tt);
                    }
                }
            }
        }

        private void SelectConfigChars(HashSet<char> chars)
        {
            if(!collectConfig) return;
            var assm = typeof(Entry).Assembly;
            var types = assm.GetTypes();
            foreach (var type in types)
            {
                // 记录所有的有BaseAttribute标记的的类型
                object[] attributes = type.GetCustomAttributes(TypeInfo<ConfigAttribute>.Type, true);
                if (attributes.Length > 0)
                {
                    byte[] oneConfigBytes = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/AssetsPackage/Config/{type.Name}.bytes")?.bytes;
                    if (oneConfigBytes != null)
                    {
                        object category = ProtobufHelper.FromBytes(type, oneConfigBytes, 0, oneConfigBytes.Length);
                        MethodInfo methodInfo = type.GetMethod("GetAllList");
                        var list = methodInfo.Invoke(category, null) as IEnumerable;
                        Type itemType = assm.GetType(type.FullName.Replace("Category", ""));
                        var props = itemType.GetProperties();
                        foreach (var item in list)
                        {
                            for (int i = 0; i < props.Length; i++)
                            {
                                if (props[i].PropertyType == typeof(string))
                                {
                                    var val = props[i].GetValue(item) as string;
                                    foreach (var ii in val)
                                    {
                                        chars.Add(ii);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }
    }
}