namespace Puzzle
{
	public static class Constants
	{
		/// <summary>
		/// 게임 클리어시 필요한 수치 조건
		/// </summary>
		public static readonly int[] MaxValue = {512, 2048, 4096};

		/// <summary>
		/// 드래그가 작동하는 최소 수치
		/// </summary>
		public static readonly float DragThreshold = 10;

		/// <summary>
		/// 각 스테이지모드 별로 보드 사이즈
		/// </summary>
		public static readonly int[] BoardSizes = {160, 120, 100};

		/// <summary>
		/// 각 스테이지모드 별로 그리드 사이즈 (보드 사이즈랑 다름)
		/// </summary>
		public static readonly int[] GridSizes = {180, 140, 114};

		/// <summary>
		/// 블럭이 초기 배치될때 초기값 배열
		/// </summary>
		public static readonly int[] InitValues = {2, 4};

		/// <summary>
		/// 블럭 생성시 초기 수치가 나올 확률로 다 합하여 1
		/// </summary>
		public static readonly float[] InitValuesProb = {0.95f, 0.05f};
	}
}
