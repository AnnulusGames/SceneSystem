using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace AnnulusGames.SceneSystem
{
    public static partial class Scenes
    {
        private static List<ILoadSceneCallbackReceiver> _callbackReceivers = new List<ILoadSceneCallbackReceiver>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeEvents()
        {
            _callbackReceivers.Clear();
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                foreach (var receiver in _callbackReceivers) receiver?.OnLoad(scene, mode);
            };
            SceneManager.sceneUnloaded += scene =>
            {
                foreach (var receiver in _callbackReceivers) receiver?.OnUnload(scene);
            };
            SceneManager.activeSceneChanged += (current, next) =>
            {
                foreach (var receiver in _callbackReceivers) receiver?.OnActiveSceneChanged(current, next);
            };
        }

        public static event UnityAction<Scene, LoadSceneMode> onSceneLoaded
        {
            add
            {
                SceneManager.sceneLoaded += value;
            }
            remove
            {
                SceneManager.sceneLoaded -= value;
            }
        }

        public static event UnityAction<Scene> onSceneUnLoaded
        {
            add
            {
                SceneManager.sceneUnloaded += value;
            }
            remove
            {
                SceneManager.sceneUnloaded += value;
            }
        }

        public static event UnityAction<Scene, Scene> onActiveSceneChanged
        {
            add
            {
                SceneManager.activeSceneChanged += value;
            }
            remove
            {
                SceneManager.activeSceneChanged += value;
            }
        }

        public static void AddCallbackReceiver(ILoadSceneCallbackReceiver receiver)
        {
            _callbackReceivers.Add(receiver);
        }

        public static void RemoveCallbackReceiver(ILoadSceneCallbackReceiver receiver)
        {
            _callbackReceivers.Remove(receiver);
        }
    }
}