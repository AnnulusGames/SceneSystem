#if SCENESYSTEM_SUPPORT_UNIRX
using System;
using UniRx;

namespace AnnulusGames.SceneSystem
{
    public static class SceneContainerUniRxExtensions
    {
        public static IObservable<(string current, string next)> OnBeforePushAsObservable(this SceneContainer self)
        {
            return Observable.FromEvent<Action<string, string>, (string, string)>(
                h => (x, y) => h((x, y)),
                h => self.OnBeforePush += h,
                h => self.OnBeforePush -= h
            );
        }

        public static IObservable<(string current, string next)> OnAfterPushAsObservable(this SceneContainer self)
        {
            return Observable.FromEvent<Action<string, string>, (string, string)>(
                h => (x, y) => h((x, y)),
                h => self.OnAfterPush += h,
                h => self.OnAfterPush -= h
            );
        }

        public static IObservable<(string current, string next)> OnBeforePopAsObservable(this SceneContainer self)
        {
            return Observable.FromEvent<Action<string, string>, (string, string)>(
                h => (x, y) => h((x, y)),
                h => self.OnBeforePop += h,
                h => self.OnBeforePop -= h
            );
        }

        public static IObservable<(string current, string next)> OnAfterPopAsObservable(this SceneContainer self)
        {
            return Observable.FromEvent<Action<string, string>, (string, string)>(
                h => (x, y) => h((x, y)),
                h => self.OnAfterPop += h,
                h => self.OnAfterPop -= h
            );
        }
    }
}
#endif