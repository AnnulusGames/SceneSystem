using System;

namespace AnnulusGames.SceneSystem
{
    internal static class ThrowHelper
    {
        public static void Throw_StackEmpty_Exception()
        {
            throw new InvalidOperationException("Stack empty.");
        }

        public static void Throw_ArgArraySizeZero_Exception()
        {
            throw new ArgumentException("Argument array size is 0.");
        }

        public static void Throw_Scene_NotFound_Exception(string scenePath)
        {
            throw new ArgumentException("Scene: " + scenePath + " not found in build settings.");
        }

        public static void Throw_Loading_InvalidOperationException()
        {
            throw new InvalidOperationException("Cannot access the loading scene container.");
        }

        public static void Throw_ContainerAlreadyBuilt_Exception()
        {
            throw new InvalidOperationException("The container is already built.");
        }

        public static void Throw_ContainerNotBuilt_Exception()
        {
            throw new InvalidOperationException("The container not built. Call Build() first.");
        }

        public static void Throw_LoadingScreenAlreadySet_Exception()
        {
            throw new InvalidOperationException("loadingScreen is already set.");
        }

        public static void Throw_LoadingScreenControlsAllowSceneActivation_Exception()
        {
            throw new InvalidOperationException("AllowSceneActivation is not available because loadingScreen is set.");
        }

        public static void Throw_ObjectDisposedException(string name)
        {
            throw new ObjectDisposedException(name);
        }
    }
}