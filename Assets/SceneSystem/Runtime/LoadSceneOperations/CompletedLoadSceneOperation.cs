using System;

namespace AnnulusGames.SceneSystem.LoadSceneOperations
{
    public sealed class CompletedLoadSceneOperation : LoadSceneOperationBase
    {
        public override float Progress => 1f;
        public override bool IsDone => true;
        public override bool HasExecuted => true;
#pragma warning disable CS0067
        public override event Action onCompleted;
#pragma warning restore CS0067
        public override void AllowSceneActivation(bool allowSceneActivation) { }

        public override LoadSceneOperationHandle Execute()
        {
            return new LoadSceneOperationHandle(this);
        }
    }
}