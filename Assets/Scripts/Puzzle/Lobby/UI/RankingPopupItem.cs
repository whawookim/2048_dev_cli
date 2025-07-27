using TMPro;
using UnityEngine;

namespace Puzzle.UI
{
    public class RankingPopupItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI rankText;
        
        [SerializeField]
        private TextMeshProUGUI nameText;
        
        [SerializeField]
        private TextMeshProUGUI scoreText;

        public void SetData(RankingData data)
        {
            rankText.text = data.Rank.ToString();
            nameText.text = data.NickName;
            scoreText.text = data.Score.ToString();
        }
    }
}
