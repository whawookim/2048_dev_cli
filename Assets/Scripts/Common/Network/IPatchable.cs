using System.Collections.Generic;

namespace Network
{
    /// <summary>
    /// JSON Dictionary로부터 객체에 부분 업데이트(Patch)를 적용할 수 있는 인터페이스.
    /// PatchHelper를 통해 재귀적으로 호출됨.
    /// </summary>
    public interface IPatchable
    {
        /// <summary>
        /// JSON 데이터(Dictionary)를 통해 객체 상태를 갱신
        /// </summary>
        void ApplyPatch(IDictionary<string, object> jsonObject);
    }   
}
