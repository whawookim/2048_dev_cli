using System.Collections;
using UnityEngine;
using System;

namespace Puzzle.UI
{
	public struct BlockData
	{
		public int Index { get; set; }

		public int Num { get; set; }

		public bool IsMerged { get; set; }

		/// <summary>
		/// 합쳐져서 사라지길 기다리는 블락
		/// </summary>
		public Block WaitMergeBlock { get; set; }
	}

	public class Block : MonoBehaviour, IPooledObject
	{
		#region constant

		private static readonly float MoveOneBlockDuration = 0.25f;
		private static readonly float ChangeBlockDuration = 0.05f;
		private static readonly float ChangeBlockScale = 1.25f;

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

		public BlockData Data
		{
			get => blockData;
			set => blockData = value;
		}

		/// <summary>
		/// 블럭 이동 데이터로 이동 후의 위치 인덱스와 이동 후의 수치(Num)으로 구성.
		/// <remarks>Num이 -1인 경우 해당 오브젝트는 사라짐</remarks>
		/// </summary>
		public BlockData? MoveData { get; set; }

		/// <summary>
		/// 수치 합산 결과로 나오는 value로 세팅
		/// </summary>
		public void Init(int num, int index)
		{
			blockData = new BlockData()
			{
				Index = index,
				Num = num,
				IsMerged = false
			};

			UpdateBlock();

			transform.position = Stages.Instance.GetBoardPosition(index);
		}

		/// <summary>
		/// 블록의 이동과 Merge를 한번에 처리
		/// </summary>
		/// <param name="resCb">모든 연출 끝나고 콜백</param>
		/// <returns></returns>
		public IEnumerator MoveAndChange(Action resCb)
		{
			if (MoveData == null) yield break;

			var moveData = MoveData.Value;

			if (moveData.Index != Data.Index)
			{
				var maxSize = Game.Instance.CurrentStage.GetBoardSize();
				var targetPos = Stages.Instance.GetBoardPosition(moveData.Index);
				var moveXIndex = moveData.Index % maxSize;
				var moveYIndex = moveData.Index / maxSize;
				var xIndex = Data.Index % maxSize;
				var yIndex = Data.Index / maxSize;
				var distance = Mathf.Sqrt(Mathf.Pow(xIndex - moveXIndex, 2) +
					Mathf.Pow(yIndex - moveYIndex, 2));
				var moveDuration = MoveOneBlockDuration * (distance * 0.5f);

				// TODO: 이동에 걸리는 시간을 실제 거리로 해서 같게 맞추기 (지금 보드가 커지면 더 오래 걸림...)
				yield return UIAnimations.Position(widget, moveDuration, targetPos, Interpolations.EaseInQuad);

				blockData.Index = moveData.Index;
				transform.position = targetPos;
			}

			if (moveData.Num == -1)
			{
				//Debug.LogError($"index {blockData.Index} num{blockData.Num} is Delete");
				gameObject.SetActive(false);
			}
			else if (moveData.Num > Data.Num)
			{
				// 합쳐져서 안보이게될때까지 대기
				if (moveData.WaitMergeBlock != null)
				{
					yield return new WaitUntil(() => !moveData.WaitMergeBlock.gameObject.activeSelf);
				}

				yield return UIAnimations.ElasticScale(transform, ChangeBlockDuration, Vector3.one * ChangeBlockScale,
					Interpolations.EaseInQuad, Interpolations.EaseOutQuad);
				blockData.Num = moveData.Num;
				UpdateBlock();
				Stages.Instance.AddScore(blockData.Num);
			}

			blockData.IsMerged = false;

			MoveData = null;

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
