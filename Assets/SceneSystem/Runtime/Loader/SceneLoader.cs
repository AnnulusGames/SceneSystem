using UnityEngine.SceneManagement;
using AnnulusGames.SceneSystem.LoadSceneOperations;

namespace AnnulusGames.SceneSystem
{
    public sealed class SceneLoader : ISceneLoader
    {
        public LoadSceneOperation GetLoadSceneOperation(string sceneName, LoadSceneMode loadSceneMode)
        {
            return new LoadSceneOperation(
                () => SceneManager.LoadSceneAsync(sceneName, loadSceneMode)
            );
        }

        public LoadSceneOperation GetUnloadSceneOperation(string sceneName)
        {
            return new LoadSceneOperation(
                () => SceneManager.UnloadSceneAsync(sceneName)
            );
        }

        public void Load(string sceneName, LoadSceneMode loadSceneMode)
        {
            SceneManager.LoadScene(sceneName, loadSceneMode);
        }

        public LoadSceneOperationHandle LoadAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            var operation = GetLoadSceneOperation(sceneName, loadSceneMode);
            return operation.Execute();
        }

#pragma warning disable CS0618
        public void Unload(string sceneName)
        {
            SceneManager.UnloadScene(sceneName);
        }
#pragma warning restore CS0618

        public LoadSceneOperationHandle UnloadAsync(string sceneName)
        {
            var operation = GetUnloadSceneOperation(sceneName);
            return operation.Execute();
        }
    }
}