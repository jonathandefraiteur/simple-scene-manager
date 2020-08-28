using UnityEditor;
using UnityEngine;

namespace JonathanDefraiteur.SimpleSceneManager.Editor
{
    public class SsmContent
    {
        public class GUIData
        {
            public GUIStyle Style;
            public GUIContent Content;
            public Color NormalColor;
            public Color ActiveColor;
            public GUILayoutOption Width;
        }

		private const float buttonsWidth = 20f;

        private static GUILayoutOption btnWidth;
        public static GUILayoutOption BtnWidth
        {
            get {
                if (null == btnWidth) {
                    btnWidth = GUILayout.Width(buttonsWidth);
                }
                return btnWidth;
            }
        }
        
        private static GUIData btnScene;
        public static GUIData BtnScene
        {
            get {
                if (null == btnScene) {
                    btnScene = new GUIData() {
                        Style = new GUIStyle(EditorStyles.miniButtonMid)
                    };
                    btnScene.Style.alignment = TextAnchor.MiddleLeft;
                }
                return btnScene;
            }
        }

		private static GUIData btnPlayMode;
        public static GUIData BtnPlayMode
        {
            get {
                if (null == btnPlayMode) {
                    btnPlayMode = new GUIData() {
                        Content = new GUIContent("»", "Start from this scene when play"),
                        Style = new GUIStyle(EditorStyles.miniButtonMid),
                        ActiveColor = Color.cyan,
                        Width = GUILayout.Width(buttonsWidth)
                    };
                    btnPlayMode.Style.fontStyle = FontStyle.Bold;
                }
                return btnPlayMode;
            }
        }
        
        private static GUIData btnPlay;
        public static GUIData BtnPlay
        {
            get {
                if (null == btnPlay) {
                    btnPlay = new GUIData() {
                        Content = new GUIContent("►", "Play this scene"),
                        Style = EditorStyles.miniButtonRight,
                        Width = GUILayout.Width(buttonsWidth)
                    };
                    btnPlay.Style.alignment = TextAnchor.MiddleCenter;
                }
                return btnPlay;
            }
        }

        private static GUIData btnAdd;
        public static GUIData BtnAdd
        {
            get {
                if (null == btnAdd) {
                    btnAdd = new GUIData() {
                        Content = new GUIContent("+", "Add this scene to the build"),
                        Style = new GUIStyle(EditorStyles.miniButtonMid),
                        Width = GUILayout.Width(buttonsWidth * .75f)
                    };
                    btnAdd.Style.fontStyle = FontStyle.Bold;
                }
                return btnAdd;
            }
        }

        private static GUIData btnRemove;
        public static GUIData BtnRemove
        {
            get {
                if (null == btnRemove) {
                    btnRemove = new GUIData() {
                        Content = new GUIContent("-", "Remove this scene from build"),
                        Style = new GUIStyle(EditorStyles.miniButtonMid),
                        Width = GUILayout.Width(buttonsWidth * .75f)
                    };
                    btnRemove.Style.fontStyle = FontStyle.Bold;
                }
                return btnRemove;
            }
        }

        private static GUIData btnSelect;
        public static GUIData BtnSelect
        {
            get {
                if (null == btnSelect) {
                    btnSelect = new GUIData() {
                        Content = new GUIContent("", "Select this scene in project"),
                        Style = new GUIStyle(EditorStyles.miniButtonLeft),
                        Width = GUILayout.Width(buttonsWidth / 2)
                    };
                    btnSelect.Style.fontStyle = FontStyle.Bold;
                }
                return btnSelect;
            }
        }

        private static GUIData btnIndex;
        public static GUIData BtnIndex
        {
            get {
                if (null == btnIndex) {
                    btnIndex = new GUIData() {
                        Content = new GUIContent("", "Index / Enable scene in build"),
                        Style = new GUIStyle(EditorStyles.miniButtonMid),
                        Width = GUILayout.Width(buttonsWidth * .75f)
                    };
                    btnIndex.Style.alignment = TextAnchor.MiddleCenter;
                    btnIndex.Style.fontSize -= 3;
                }
                return btnIndex;
            }
        }

        public static GUIContent SceneGUIContent(EditorBuildSettingsScene scene)
        {
            return SceneGUIContent(scene.path);
        }
        public static GUIContent SceneGUIContent(string path)
        {
            return new GUIContent(" " + SsmUtility.SceneFileName(path), $"Open {path}");
        }
    }
}