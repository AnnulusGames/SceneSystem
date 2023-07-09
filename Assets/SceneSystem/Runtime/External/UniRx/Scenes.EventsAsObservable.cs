#if SCENESYSTEM_SUPPORT_UNIRX
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UniRx;

namespace AnnulusGames.SceneSystem
{
    public static partial class Scenes
    {
        public static IObservable<(Scene scene, LoadSceneMode loadSceneMode)> OnSceneLoadedAsObservable()
        {
            return Observable.FromEvent<UnityAction<Scene, LoadSceneMode>, (Scene, LoadSceneMode)>(
                h => (x, y) => h((x, y)),
                h => SceneManager.sceneLoaded += h,
                h => SceneManager.sceneLoaded -= h
            );
        }

        public static IObservable<Scene> OnSceneUnloadedAsObservable()
        {
            return Observable.FromEvent<UnityAction<Scene>, Scene>(
                h => x => h(x),
                h => SceneManager.sceneUnloaded += h,
                h => SceneManager.sceneUnloaded -= h
            );
        }

        public static IObservable<(Scene current, Scene next)> OnActiveSceneChangedAsObservable()
        {
            return Observable.FromEvent<UnityAction<Scene, Scene>, (Scene, Scene)>(
                h => (x, y) => h((x, y)),
                h => SceneManager.activeSceneChanged += h,
                h => SceneManager.activeSceneChanged -= h
            );
        }
    }
}
#endif