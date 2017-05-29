using UnityEngine;
using UnityEditor;

namespace TOZ.Utilities
{
    public static class GameUtils
    {
        [MenuItem("Tools/OpenScene/Custom Patcher")]
        public static void OpenGameLoginScene()
        {
            OpenScene("Custom Patcher");
        }

        [MenuItem("Tools/Delete Prefs")]
        public static void DeletePrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        static void OpenScene(string name)
        {
            if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
            {
                EditorApplication.OpenScene("Assets/Scenes/" + name + ".unity");
            }
        }
    }
}
