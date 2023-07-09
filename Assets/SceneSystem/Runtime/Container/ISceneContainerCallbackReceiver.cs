namespace AnnulusGames.SceneSystem
{
    public interface ISceneContainerCallbackReceiver
    {
        void OnBeforePush(string enter, string exit);
        void OnAfterPush(string enter, string exit);
        void OnBeforePop(string enter, string exit);
        void OnAfterPop(string enter, string exit);
    }
}