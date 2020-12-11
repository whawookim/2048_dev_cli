using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
	#region colors

	private readonly Color32[] colors =
	{
		// 2
		new Color32(208, 211, 254, 204),
		// 4
		new Color32(133, 141, 250, 204),
		// 8
		new Color32(62, 74, 246, 204),
		// 16
		new Color32(253, 252, 197, 204),
		// 32
		new Color32(250, 248, 133, 204),
		// 64
		new Color32(253, 250, 77, 204),
		// 128
		new Color32(255, 206, 160, 204),
		// 256
		new Color32(253, 172, 97, 204),
		// 512
		new Color32(245, 131, 24, 204),
		// 1024
		new Color32(245, 131, 24, 204),
		// 2048
		new Color32(245, 24, 235, 204),
	};

	#endregion
	
	[SerializeField]
	private UILabel valueLabel;

	[SerializeField]
	private UISprite bgSprite;
	
	private int xIndex = 0;

	public int XIndex => xIndex;

	private int yIndex = 0;

	public int YIndex => yIndex;

	private int value;

	public int Value => value;

	public bool CheckSameValue(Block block)
	{
		return block.Value == Value;
	}

	public bool CheckBoard(int xIndex, int yIndex)
	{
		return this.xIndex == xIndex && this.yIndex == yIndex;
	}

	/// <summary>
	/// 수치 합산 결과로 나오는 value로 세팅
	/// </summary>
	public void Init(int value, int xIndex, int yIndex)
	{
		this.value = value;
		this.xIndex = xIndex;
		this.yIndex = yIndex;

		valueLabel.text = value.ToString();
		transform.position = Board.Instance.GetBoardPosition(xIndex, yIndex);
		SetColor();
	}

	/// <summary>
	/// 특정 보드의 위치로 이동 애니메이션
	/// </summary>
	public IEnumerator MoveToBoard(int xIndex, int yIndex, float duration = 1)
	{
		var timeSum = 0f;
		var targetPos = Board.Instance.GetBoardPosition(xIndex, yIndex);

		while (timeSum < duration)
		{
			transform.position = Vector3.Lerp(transform.position, targetPos, timeSum / duration);
			
			yield return null;

			timeSum += Time.deltaTime;
		}

		transform.position = targetPos;
		this.xIndex = xIndex;
		this.yIndex = yIndex;
	}

	public IEnumerator ChangeValue(int value, float duration)
	{
		yield return new WaitForSeconds(duration);
		this.value = value;
		valueLabel.text = value.ToString();
		SetColor();
		StageUi.Instance.SetGameScore(value);
	}

	public void SetColor()
	{
		bgSprite.color = colors[(int) Mathf.Log(value)];
	}
}
