namespace Puzzle
{
    public enum UnityScene
    {
        Game,
        Lobby,
        Stage,
    }
    
    public static class UnitySceneManager
    {
        public static IAddressableManager GetAddressableManager(UnityScene unityScene)
        {
            switch (unityScene)
            {
                case UnityScene.Lobby:
                    return LobbyManager.Instance;
                case UnityScene.Stage:
                    return StageManager.Instance;
            }

            return null;
        }
    }
}