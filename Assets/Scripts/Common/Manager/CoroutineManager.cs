using UnityEngine;
using System.Collections;

namespace Puzzle
{
    /// <summary>
    /// TODO: 나중에 UniTask로 바꾸든 MonoBehaviour를 때는 방향으로 하든 하는 것도 추천.
    /// </summary>
    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _instance;
        public static CoroutineManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("CoroutineManager");
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<CoroutineManager>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// 코루틴 실행
        /// </summary>
        public Coroutine Run(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        /// <summary>
        /// 코루틴 정지
        /// </summary>
        public void Stop(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// 다음 프레임에 실행
        /// </summary>
        public void RunNextFrame(System.Action action)
        {
            StartCoroutine(NextFrame(action));
        }

        private IEnumerator NextFrame(System.Action action)
        {
            yield return null;
            action?.Invoke();
        }

        /// <summary>
        /// 지정 시간 후 실행
        /// </summary>
        public void RunDelayed(float seconds, System.Action action)
        {
            StartCoroutine(Delay(seconds, action));
        }

        private IEnumerator Delay(float seconds, System.Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}
