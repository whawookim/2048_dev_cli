using System.Collections.Generic;

/// <summary>
/// 메시지 브로드캐스트 시스템 (간단한 Pub-Sub 구조)
/// - 이벤트 타입별로 리스너 등록 가능
/// - 타입 문자열을 기준으로 딕셔너리에 저장 및 호출
/// </summary>
public class MessageSystem
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
	public static readonly MessageSystem Instance = new MessageSystem();

    /// <summary>
    /// 이벤트 핸들러 델리게이트 정의 (true 반환 시 처리 완료로 간주)
    /// </summary>
	public delegate bool PublishEvent(Events e);
    
    /// <summary>
    /// 이벤트 타입별 등록된 핸들러 딕셔너리
    /// 키: 이벤트 타입 문자열, 값: 해당 타입에 대한 이벤트 처리 델리게이트
    /// </summary>
	private readonly Dictionary<string, PublishEvent> _publishDict = new Dictionary<string, PublishEvent>();

    /// <summary>
    /// 특정 이벤트 타입을 발행 (등록된 핸들러가 있으면 처리됨)
    /// </summary>
    /// <param name="e">발행할 이벤트</param>
    /// <returns>처리 성공 여부</returns>
	public bool Publish(Events e)
	{
		var name = e.GetType().ToString();

		return _publishDict.ContainsKey(name) && _publishDict[name].Invoke(e);
	}

    /// <summary>
    /// 이벤트 구독 등록
    /// </summary>
    /// <typeparam name="T">구독할 이벤트 타입</typeparam>
    /// <param name="e">이벤트 처리 델리게이트</param>
	public void Subscribe<T>(PublishEvent e) where T : Events
	{
		var name = typeof(T).ToString();

		if (!_publishDict.ContainsKey(name))
		{
			_publishDict.Add(name, e);
		}
		else
		{
			_publishDict[name] += e;
		}
	}

    /// <summary>
    /// 이벤트 구독 해제
    /// </summary>
    /// <typeparam name="T">해제할 이벤트 타입</typeparam>
    /// <param name="e">해제할 이벤트 처리 델리게이트</param>
    /// <param name="deleteKey">모든 구독이 사라지면 딕셔너리 키 자체 제거 여부</param>
	public void Unsubscribe<T>(PublishEvent e, bool deleteKey = false) where T : Events
	{
		var name = typeof(T).ToString();

		if (_publishDict.ContainsKey(name))
		{
			_publishDict[name] -= e;
			
			// 더이상 key 자체를 안 쓸 거 같다 싶으면 날려버린다
			if (deleteKey)
			{
				_publishDict.Remove(name);
			}
		}
	}
}
