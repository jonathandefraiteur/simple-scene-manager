using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JonathanDefraiteur.SimpleSceneManager.Editor
{
    public class SsmSettingsWindow : EditorWindow
    {
        private static SsmSettingsWindow instance;
        
        private Vector2 m_pos;
        private bool m_error;

        [MenuItem("Window/Simple Scene Settings", false, 10001)]
        public static void Init() 
        {
            instance = GetWindow<SsmSettingsWindow>();
            instance.Show();
            instance.titleContent = new GUIContent("Settings");
        }
        
        private void OnEnable() 
        {
            instance = this;
            m_error = false;
        }

        private void OnGUI() 
        {
            EditorGUILayout.LabelField("Simple Scene Settings", EditorStyles.toolbar);
            EditorGUILayout.Space();
            
            m_pos = GUILayout.BeginScrollView(m_pos);
            using (var line = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Path to ignore", EditorStyles.helpBox);
                foreach (string path in SsmSettings.settings.pathsToIgnore.ToList())
                {
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    EditorGUILayout.TextField(path);
                    GUI.enabled = true;
                    if (GUILayout.Button("-"))
                    {
                        SsmSettings.RemovePathToIgnore(path);   
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add"))
                {
                    AddPath();
                }

                if (m_error)
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Path must be in Application Folder", EditorStyles.miniBoldLabel);
                    GUI.color = Color.white;
                }
               
            };
            GUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                SsmSettings.Save();   
            }
            if (GUILayout.Button("Reset"))
            {
                SsmSettings.Reset();   
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddPath()
        {
            string directory = EditorUtility.OpenFolderPanel("Select Directory", "", "");
            if (!directory.StartsWith(Application.dataPath))
            {
                m_error = true;
            }
            else
            {
                m_error = false;
                directory = $"Assets{directory.Substring(Application.dataPath.Length)}";
                Debug.Log(directory);
                SsmSettings.AddPathToIgnore(directory);
            }
        }
    }
}