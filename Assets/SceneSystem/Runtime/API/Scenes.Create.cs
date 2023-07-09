using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnnulusGames.SceneSystem
{
    public static partial class Scenes
    {
        public static void CreateScene(string sceneName)
        {
            SceneManager.CreateScene(sceneName);
        }
        public static void CreateScene(string sceneName, CreateSceneParameters parameters)
        {
            SceneManager.CreateScene(sceneName, parameters);
        }

        public static void MergeScenes(Scene sourceScene, Scene destinationScene)
        {
            SceneManager.MergeScenes(sourceScene, destinationScene);
        }

        public static void MoveGameObjectToScene(GameObject go, Scene scene)
        {
            SceneManager.MoveGameObjectToScene(go, scene);
        }

        public static void SetActiveScene(Scene scene)
        {
            SceneManager.SetActiveScene(scene);
        }
    }
}