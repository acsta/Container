using UnityEngine;
using UnityEditor;
using System.IO;
namespace TaoTie
{
    [CustomEditor(typeof(Readme))]
    [InitializeOnLoad]
    public class ReadmeEditor : Editor
	{
		static string kShowedReadmeSessionStateName = "ReadmeEditor.showedReadme";
		static float kSpace = 16f;

		static readonly string WorkPath = "Assets/Scripts/Editor/Common/Readme";

		static ReadmeEditor()
		{
			EditorApplication.delayCall += SelectReadmeAutomatically;
		}

		static void SelectReadmeAutomatically()
		{
			if (!SessionState.GetBool(kShowedReadmeSessionStateName, false))
			{
				var readme = SelectReadme();
				SessionState.SetBool(kShowedReadmeSessionStateName, true);

				if (readme && !readme.loadedLayout)
				{
					LoadLayout();
					readme.loadedLayout = true;
				}
			}
		}

		static void LoadLayout()
		{
			//var assembly = typeof(EditorApplication).Assembly;
			//var windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
			//var method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
			//method.Invoke(null, new object[] { Path.Combine(Application.dataPath, "TutorialInfo/Layout.wlt"), false });
		}

		[MenuItem("Assets/Create/ScriptObject/Readme", false)]
		static void CreateReadme()
		{
			var asset = ScriptableObject.CreateInstance<Readme>();
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path == "")
			{
				path = "Assets";
			}
			else if (Path.GetExtension(path) != "")
			{
				path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}

			var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
			if (icons.Length > 0 && icons[0] != null)
			{
				asset.icon = icons[0];
			}
			
			asset.title = "使用说明";
			asset.sections = new Readme.Section[3];

			Readme.Section s1 = new Readme.Section();
			s1.heading = PlayerSettings.productName;

			Readme.Section s2 = new Readme.Section();
			s2.heading = "引擎版本";
			s2.text = "2022.3.30f1";
			Readme.Section s3 = new Readme.Section();
			s3.heading = "QA";
			s3.text = ">如何快速启动游戏？\n" +
			          "Shift+B切换到启动场景Init\n\n" +
			          ">...";
			asset.sections[0] = s1;
			asset.sections[1] = s2;
			asset.sections[2] = s3;
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/Readme.asset");
			AssetDatabase.CreateAsset(asset, assetPathAndName);
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}

		[MenuItem("Tools/帮助/简介")]
		static Readme SelectReadme()
		{
			var ids = AssetDatabase.FindAssets("Readme t:Readme");
			if (ids.Length == 1)
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0])) as Readme;
				var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
				if (icons.Length > 0 && icons[0] != null)
				{
					readmeObject.icon = icons[0];
				}
				Selection.objects = new UnityEngine.Object[] { readmeObject };

				return (Readme)readmeObject;
			}
			else
			{
				Debug.Log("Couldn't find a readme");
				return null;
			}
		}

		protected override void OnHeaderGUI()
		{
			var readme = (Readme)target;
			Init();

			var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

			GUILayout.BeginHorizontal("In BigTitle");
			{
				GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
				GUILayout.Label(readme.title, TitleStyle);
			}
			GUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI()
		{
			var readme = (Readme)target;
			Init();

			foreach (var section in readme.sections)
			{
				if (!string.IsNullOrEmpty(section.heading))
				{
					GUILayout.Label(section.heading, HeadingStyle);
				}
				if (!string.IsNullOrEmpty(section.text))
				{
					GUILayout.Label(section.text, BodyStyle);
				}
				if (!string.IsNullOrEmpty(section.linkText))
				{
					if (LinkLabel(new GUIContent(section.linkText)))
					{
						Application.OpenURL(section.url);
					}
				}
				GUILayout.Space(kSpace);
			}
		}


		bool m_Initialized;

		GUIStyle LinkStyle { get { return m_LinkStyle; } }
		[SerializeField] GUIStyle m_LinkStyle;

		GUIStyle TitleStyle { get { return m_TitleStyle; } }
		[SerializeField] GUIStyle m_TitleStyle;

		GUIStyle HeadingStyle { get { return m_HeadingStyle; } }
		[SerializeField] GUIStyle m_HeadingStyle;

		GUIStyle BodyStyle { get { return m_BodyStyle; } }
		[SerializeField] GUIStyle m_BodyStyle;

		void Init()
		{
			if (m_Initialized)
				return;
			m_BodyStyle = new GUIStyle(EditorStyles.label);
			m_BodyStyle.wordWrap = true;
			m_BodyStyle.fontSize = 14;

			m_TitleStyle = new GUIStyle(m_BodyStyle);
			m_TitleStyle.fontSize = 26;

			m_HeadingStyle = new GUIStyle(m_BodyStyle);
			m_HeadingStyle.fontSize = 18;

			m_LinkStyle = new GUIStyle(m_BodyStyle);
			m_LinkStyle.wordWrap = false;
			// Match selection color which works nicely for both light and dark skins
			m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
			m_LinkStyle.stretchWidth = false;

			m_Initialized = true;
		}

		bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
		{
			var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

			Handles.BeginGUI();
			Handles.color = LinkStyle.normal.textColor;
			Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI();

			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

			return GUI.Button(position, label, LinkStyle);
		}
	}
}