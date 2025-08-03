using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Puzzle.UI.Loader;
using Puzzle.UI.Scene;

namespace Puzzle.UI.Flow
{
    /// <summary>
    /// UI 씬 전환을 관리하는 클래스입니다.
    /// Addressable 기반으로 씬을 로드하며, 상태 전달 및 스택 기반 흐름을 지원합니다.
    /// </summary>
    public class UISceneManager : IUISceneHandler
    {
        private readonly List<IUIScene> _sceneStack = new();
        private readonly Dictionary<IUIScene, object> _savedSceneStates = new();

        public IUIScene CurrentScene => _sceneStack.Count > 0 ? _sceneStack[^1] : null;
        public UITransition CurrentTransition { get; private set; }

        /// <summary>
        /// 새로운 씬으로 전환합니다. 기존 씬은 Pause/Finish 처리되며, 새 씬은 Load → Begin → Open 흐름을 가집니다.
        /// </summary>
        /// <param name="transition">전환할 씬 정보 및 상태</param>
        public async Task SetTransitionAsync(UITransition transition)
        {
            if (CurrentTransition != null)
                return;

            try
            {
                UIBlocker.Instance.SetEnabled();
                CurrentTransition = transition;

                switch (transition.TransitionType)
                {
                    case UITransitionType.Push:
                    {
                        var nextSceneType = transition.NextSceneType;
                        var existingScene = _sceneStack.FirstOrDefault(s => s.GetType() == nextSceneType);
                        if (existingScene != null)
                        {
                            if (!_savedSceneStates.ContainsKey(existingScene))
                                SaveSceneState(existingScene, existingScene.GetState());

                            existingScene.Pause();
                            existingScene.Finish();
                            (existingScene as UnityEngine.MonoBehaviour)?.gameObject.SetActive(false);

                            _sceneStack.Remove(existingScene);
                        }

                        UnityEngine.MonoBehaviour currentSceneMono = null; 

                        if (CurrentScene != null)
                        {
                            CurrentScene.Pause();
                            currentSceneMono = (CurrentScene as UnityEngine.MonoBehaviour);
                        }

                        var nextScene = await UIAssetLoader.LoadSceneAsync(nextSceneType);

                        if (nextScene == null)
                        {
                            MyDebug.LogError($"next scene not found. Hint. {nextSceneType}");
                            break;
                        }
                        nextScene.UISceneManager = this;
                        _sceneStack.Add(nextScene);

                        if (_savedSceneStates.TryGetValue(nextScene, out var savedState))
                        {
                            await nextScene.LoadAsync(savedState);
                            _savedSceneStates.Remove(nextScene);
                        }
                        else
                        {
                            await nextScene.LoadAsync(transition.SavedState);
                        }

                        (nextScene as UnityEngine.MonoBehaviour)?.gameObject.SetActive(true);
                        nextScene.Begin();

                        if (currentSceneMono != null)
                        {
                            currentSceneMono.gameObject.SetActive(false);
                        }
                        
                        break;
                    }

                    case UITransitionType.Pop:
                    {
                        if (CurrentScene != null)
                        {
                            CurrentScene.Pause();
                            CurrentScene.Finish();
                            (CurrentScene as UnityEngine.MonoBehaviour)?.gameObject.SetActive(false);
                            _sceneStack.RemoveAt(_sceneStack.Count - 1);
                        }

                        if (_sceneStack.Count == 0)
                        {
                            MyDebug.LogWarning("PopTransition attempted but no scene remains in stack.");
                            break;
                        }

                        var previousScene = _sceneStack[^1];
                        (previousScene as UnityEngine.MonoBehaviour)?.gameObject.SetActive(true);
                        previousScene.Resume(transition.Result);
                        break;
                    }

                    default:
                        MyDebug.LogWarning($"Unhandled UITransitionType: {transition.TransitionType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                MyDebug.LogError($"[UISceneManager] Exception in SetTransitionAsync: {ex.Message}");
            }
            finally
            {
                transition.RaiseTransitionEnded();
                CurrentTransition = null;
                UIBlocker.Instance.SetDisabled();
            }
        }
        
        /// <summary>
        /// 현재 스택에 쌓인 씬들을 모두 종료합니다.
        /// </summary>
        public void ClearStackScenes()
        {
            foreach (var scene in _sceneStack)
            {
                scene.Finish();
                (scene as UnityEngine.MonoBehaviour)?.gameObject.SetActive(false);
            }
            _sceneStack.Clear();
        }
        
        /// <summary>
        /// 특정 씬의 상태(state)를 저장합니다.
        /// </summary>
        private void SaveSceneState(IUIScene scene, object state)
        {
            if (_savedSceneStates.ContainsKey(scene))
                _savedSceneStates[scene] = state;
            else
                _savedSceneStates.Add(scene, state);
        }

        /// <summary>
        /// 이전에 저장된 씬 상태를 반환합니다.
        /// </summary>
        private object GetSavedSceneState(IUIScene scene)
        {
            return _savedSceneStates.GetValueOrDefault(scene);
        }
    }
}
