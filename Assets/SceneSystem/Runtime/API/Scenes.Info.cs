using UnityEngine.SceneManagement;

namespace AnnulusGames.SceneSystem
{
    public static partial class Scenes
    {
        public static Scene GetActiveScene() => SceneManager.GetActiveScene();
        public static Scene GetSceneAt(int index) => SceneManager.GetSceneAt(index);
        public static Scene GetSceneByBuildIndex(int buildIndex) => SceneManager.GetSceneByBuildIndex(buildIndex);
        public static Scene GetSceneByName(string sceneName) => SceneManager.GetSceneByName(sceneName);
        public static Scene GetSceneByPath(string scenePath) => SceneManager.GetSceneByPath(scenePath);
        public static int Count => SceneManager.sceneCount;
        public static int CountInBuildSettings => SceneManager.sceneCountInBuildSettings;
    }
}