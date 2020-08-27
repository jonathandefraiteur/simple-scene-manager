using UnityEditor;
using UnityEngine;

namespace JonathanDefraiteur.SimpleSceneManager.Editor
{
    public class SsmGUI
    {
        public static void NoSceneInBuildSettings()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.HelpBox("No scenes in BuildSettings", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
        
        public static bool BuildSettingsButton()
        {
            if (!GUILayout.Button("Build Settings..."))
                return false;
            
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
            return true;
        }
    }
}
