using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathanDefraiteur.SimpleSceneManager.Editor
{
    [Serializable]
    public class SsmSettings
    {
        private static SsmSettings m_settings;
        public static SsmSettings settings
        {
            get
            {
                if (m_settings != null) 
                    return m_settings;
                
                return new SsmSettings();
            }
            set => m_settings = value;
        }
        
        private const string PlayerPrefsName = "SsmSetings";
        private static bool s_isLoaded = false;
        
        [SerializeField] private List<string> m_pathsToIgnore = new List<string>();
        public List<string> pathsToIgnore => m_pathsToIgnore;

        public SsmSettings()
        {
            m_settings = PlayerPrefs.HasKey(PlayerPrefsName) ? Load() : this;
        }

        #region PathToIgnore
        public static void AddPathToIgnore(string path)
        {
            if (!m_settings.pathsToIgnore.Contains(path))
            {
                m_settings.m_pathsToIgnore.Add(path);
            }
        }

        public static void RemovePathToIgnore(string path)
        {
            if (m_settings.pathsToIgnore.Contains(path))
            {
                m_settings.m_pathsToIgnore.Remove(path);
            }
        }
        #endregion

        #region Save
        public static void Reset()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsName))
            {
                PlayerPrefs.DeleteKey(PlayerPrefsName);
            }

            m_settings = new SsmSettings();
        }
        
        private static SsmSettings Load ()
        {
            if (s_isLoaded)
                return m_settings;
            string json = PlayerPrefs.GetString(PlayerPrefsName);
            Debug.Log("SsmSettings loaded");
            s_isLoaded = true;
            return JsonUtility.FromJson<SsmSettings>(json);
        }

        public static void Save()
        {
            string json = JsonUtility.ToJson(m_settings);
            Debug.Log("SsmSettings saved");
            PlayerPrefs.SetString(PlayerPrefsName, json);
        }
        #endregion
    }
}