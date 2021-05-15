using System.Collections;
using UnityEngine;
using System;

namespace Puzzle.UI
{
	public struct BlockData
	{
		public int Index { get; set; }

		public int Num { get; set; }
	}

	public class Block : MonoBehaviour, IPooledObject
	{
		#region colors

		private static readonly Color32[] Colors =
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
		private UIWidget widget;

		[SerializeField]
		private UILabel valueLabel;

		[SerializeField]
		private UISprite bgSprite;

		private BlockData blockData;

		public BlockData Data => blockData;

		/// <summary>
		/// 수치 합산 결과로 나오는 value로 세팅
		/// </summary>
		public void Init(int num, int index)
		{
			blockData = new BlockData()
			{
				Index = index,
				Num = num
			};

			UpdateBlock();

			transform.position = Stages.Instance.GetBoardPosition(index);
		}

		/// <summary>
		/// 특정 보드의 위치로 이동 애니메이션
		/// </summary>
		public IEnumerator MoveToBoard(int index, float duration, Action resCb)
		{
			var targetPos = Stages.Instance.GetBoardPosition(index);

			yield return UIAnimations.Position(widget, duration, targetPos, Interpolations.EaseInQuad);

			blockData.Index = index;
			transform.position = targetPos;

			resCb?.Invoke();
		}

		public IEnumerator ChangeValue(int num, float duration, Action resCb)
		{
			yield return UIAnimations.ElasticScale(transform, duration, Vector3.one * 1.2f,
				Interpolations.EaseInQuad, Interpolations.EaseOutQuad);

			blockData.Num = num;
			UpdateBlock();

			resCb?.Invoke();
		}

		/// <summary>
		/// <see cref="blockData"/>에 맞게 block UI 세팅
		/// </summary>
		private void UpdateBlock()
		{
			var num = blockData.Num;
			valueLabel.text = num.ToString();
			bgSprite.color = Colors[(int) Mathf.Log(num)];
		}

		public void SetSize(int size)
		{
			widget.width = size;
			widget.height = size;
		}
	}
}
