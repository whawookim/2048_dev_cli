using Puzzle.Data;
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
            nameText.text = data.Nickname;
            scoreText.text = data.Score.ToString();
        }
    }
}
