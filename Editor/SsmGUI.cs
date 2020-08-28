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

        public static bool LabelButton(string _scenePath)
        {
            if (!GUILayout.Button(SsmContent.SceneGUIContent(_scenePath), SsmContent.BtnScene.Style))
                return false;
            
            SsmAction.OpenScene(_scenePath);
            return true;
        }

        public static bool Button(SsmContent.GUIData _data)
        {
            return GUILayout.Button(_data.Content, _data.Style, _data.Width);
        }

        public static bool PlayModeButton(string _scenePath)
        {
            Color gc = GUI.color;
            GUI.color = SsmUtility.IsScenePlayedAtStart(_scenePath) ? SsmContent.BtnPlayMode.ActiveColor : gc;
            if (!Button(SsmContent.BtnPlayMode)) {
                GUI.color = gc;
                return false;
            }
            GUI.color = gc;
            SsmAction.TogglePlayModeStartScene(_scenePath);
            return true;
        }

        public static bool PlayButton(string _scenePath)
        {
            if (!Button(SsmContent.BtnPlay))
                return false;
            
            SsmAction.PlayScene(_scenePath);
            return true;
        }

        public static bool SelectButton(string _scenePath)
        {
            if (!Button(SsmContent.BtnSelect))
                return false;
            
            SsmAction.SelectScene(_scenePath);
            return true;
        }
    }
}
