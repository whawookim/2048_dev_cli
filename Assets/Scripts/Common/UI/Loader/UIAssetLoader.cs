using System;
using System.Threading.Tasks;
using Puzzle.UI.Scene;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Puzzle.UI.Overlay;

namespace Puzzle.UI.Loader
{
    /// <summary>
    /// Addressables를 통해 UI Overlay 및 Scene 프리팹을 비동기 로드하고 활성화하는 유틸리티 클래스입니다.
    /// 각 UI 요소는 IUIOverlay 또는 IUIScene 인터페이스를 구현해야 하며, Instance 프로퍼티를 통해 접근됩니다.
    /// </summary>
    public static class UIAssetLoader
    {
        /// <summary>
        /// Addressables에서 UI Overlay를 로드하고 Instance를 반환합니다.
        /// </summary>
        /// <param name="overlayType">로드할 오버레이의 타입</param>
        /// <returns>IUIOverlay 인스턴스</returns>
        public static async Task<IUIOverlay> LoadOverlayAsync(Type overlayType)
        {
            if (!typeof(IUIOverlay).IsAssignableFrom(overlayType))
                throw new ArgumentException($"{overlayType.Name}은(는) IUIOverlay를 구현하지 않습니다.");

            var instance = GetStaticInstance<IUIOverlay>(overlayType);
            if (instance != null)
                return instance;

            string addressableName = GetAddressableName(overlayType);
            var handle = Addressables.InstantiateAsync(addressableName);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new Exception($"UI Overlay Addressable 로드 실패: {addressableName}");

            return GetStaticInstance<IUIOverlay>(overlayType);
        }

        /// <summary>
        /// Addressables에서 UI Scene을 로드하고 Instance를 반환합니다.
        /// </summary>
        /// <param name="sceneType">로드할 씬 타입</param>
        /// <returns>IUIScene 인스턴스</returns>
        public static async Task<IUIScene> LoadSceneAsync(Type sceneType)
        {
            if (!typeof(IUIScene).IsAssignableFrom(sceneType))
                throw new ArgumentException($"{sceneType.Name}은(는) IUIScene을 구현하지 않습니다.");

            var instance = GetStaticInstance<IUIScene>(sceneType);
            if (instance != null)
                return instance;

            string addressableName = GetAddressableName(sceneType);
            var handle = Addressables.InstantiateAsync(addressableName);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new Exception($"UI Scene Addressable 로드 실패: {addressableName}");

            return GetStaticInstance<IUIScene>(sceneType);
        }

        /// <summary>
        /// UI 클래스 타입에서 AddressableName 속성을 가져옵니다.
        /// </summary>
        /// <param name="uiType">UI 클래스 타입</param>
        /// <returns>주소값 문자열</returns>
        private static string GetAddressableName(Type uiType)
        {
            var prop = uiType.GetProperty("AddressableName");
            var value = prop?.GetValue(null) as string;

            if (string.IsNullOrEmpty(value))
                throw new Exception($"{uiType.Name} 클래스에 AddressableName 프로퍼티가 없습니다.");

            return value;
        }

        /// <summary>
        /// static Instance 속성에서 UI 인스턴스를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">IUIOverlay 또는 IUIScene</typeparam>
        /// <param name="uiType">UI 타입</param>
        /// <returns>Instance 값 또는 null</returns>
        private static T GetStaticInstance<T>(Type uiType) where T : class
        {
            return uiType.GetProperty("Instance")?.GetValue(null) as T;
        }
    }
}
