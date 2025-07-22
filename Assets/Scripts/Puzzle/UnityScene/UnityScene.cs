using UnityEngine.SceneManagement;

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
        public static IAddressableManager GetCurrentAddressableManager()
        {
            var activeScene = SceneManager.GetActiveScene();

            if (activeScene.name == UnityScene.Lobby.ToString())
            {
                return LobbyManager.Instance;
            }
            else if (activeScene.name == UnityScene.Stage.ToString())
            {
                return StageManager.Instance;
            }

            return null;
        }
        
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