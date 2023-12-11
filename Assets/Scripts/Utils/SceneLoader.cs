using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Utils
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(int value)
        {
            Time.timeScale = 1;
            
            AudioManager.instance.PlaySFX("Confirm");
            StartCoroutine(FadeAndLoad(value));
        }

        IEnumerator FadeAndLoad(int value)
        {
            UIController.instance.FadeScreen(true);
            AudioManager.instance.FadeMusicVolume(true);
            
            yield return new WaitForSeconds(UIController.Duration);
            SceneManager.LoadSceneAsync(value);
            AudioManager.instance.FadeMusicVolume(false);
            UIController.instance.FadeScreen(false);
            UIController.instance.ResetMenu();

            if (value == 0)
            {
                AudioManager.instance.PlayMusic("Main Theme");
                GameManager.gameState = GameState.Menu;

                Cursor.visible = true;
                
                yield break;
            }
            
            AudioManager.instance.PlayMusic($"Stage0{value}");
            GameManager.gameState = GameState.Play;
            UIController.instance.backCanvas.GetComponent<Image>().color = new Color(0, 0, 0, .3f);
            UIController.instance.middleCanvas.SetActive(true);
            UIController.instance.menus.SetActive(false);
        }
    }
}