using TMPro;
using UnityEngine;

namespace Utils
{
    public class Score : MonoBehaviour
    {
        public TextMeshProUGUI scoreText, remainingLifeText, lifeScoreText, remainingBombText, bombScoreText, totalScoreText;

        private void OnEnable()
        {
            scoreText.text = $"<mspace=.41em>{GameManager.Score}</mspace>";
            remainingLifeText.text = $"LIFE (x{GameManager.Life})";
            lifeScoreText.text = $"<mspace=.41em>{GameManager.Life * 100000}</mspace>";
            remainingBombText.text = $"BOMB (x{GameManager.Bomb})";
            bombScoreText.text = $"<mspace=.41em>{GameManager.Bomb * 50000}</mspace>";
            totalScoreText.text = $"<mspace=.41em>{GameManager.Score + GameManager.Life * 100000 + GameManager.Bomb * 50000}</mspace>";
        }
    }
}