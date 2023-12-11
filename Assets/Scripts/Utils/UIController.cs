using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject backCanvas;
    public GameObject middleCanvas;
    public GameObject frontCanvas;
    public const float Duration = 1;

    [Header("Menu")] public GameObject menus;
    public GameObject mainMenu, startMenu, pauseMenu, gameOverMenu, gameClearMenu;

    [Header("Options")] public Slider bgmSlider;
    public Slider sfxSlider;
    public TMP_Dropdown characterDropdown, difficultyDropdown;

    [Header("Dialogue")] public GameObject dialogue;
    public Image playerPortrait, enemyPortrait;
    public TextMeshProUGUI dialogueTxt;

    [Header("HUD")] public GameObject hud;
    public GameObject lifeStarPrefab, bombStarPrefab;
    public Transform lifeStarStartPosition, bombStarStartPosition;
    public float starsDistance = .25f;
    public TextMeshProUGUI scoreTxt, powerTxt;
    public Slider power;
    private readonly List<GameObject> lifeStars = new(), bombStars = new();

    [Header("Boss Indicator")] public GameObject bossIndicator;
    public Slider bossHealth;
    public TextMeshProUGUI timer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += (arg0, mode) => Start();
    }

    private void Start()
    {
        // 저장되어있는 옵션 설정 불러오기
        bgmSlider.value = ES3.Load("BGM", 1f);
        sfxSlider.value = ES3.Load("SFX", 1f);
        characterDropdown.value = ES3.Load("PlayerID", 0);
        difficultyDropdown.value = ES3.Load("Difficulty", 1);
        
        // 최초 게임 시작 시 UI 초기화
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (lifeStars.Count > 0) return;

        for (int i = 0; i < 10; i++)
        {
            lifeStars.Add(Instantiate(lifeStarPrefab,
                lifeStarStartPosition.position + Vector3.right * i * starsDistance, Quaternion.identity,
                lifeStarStartPosition));
            bombStars.Add(Instantiate(bombStarPrefab,
                bombStarStartPosition.position + Vector3.right * i * starsDistance, Quaternion.identity,
                bombStarStartPosition));
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= (arg0, mode) => Start();
    }

    public void UpdateLife()
    {
        for (int i = 0; i < lifeStars.Count; i++)
        {
            lifeStars[i].SetActive(i < GameManager.Life);
        }
    }

    public void UpdateBomb()
    {
        for (int i = 0; i < bombStars.Count; i++)
        {
            bombStars[i].SetActive(i < GameManager.Bomb);
        }
    }

    public void UpdateScore()
    {
        scoreTxt.text = $"<mspace=.4em>{GameManager.Score:D9}";
    }

    public void UpdatePower()
    {
        powerTxt.text = GameManager.Power < GameManager.MaxPower
            ? GameManager.Power.ToString()
            : "Max Power";
        power.value = GameManager.Power;
    }

    public void FadeScreen(bool value)
    {
        frontCanvas.SetActive(true);
        frontCanvas.GetComponent<Image>().DOFade(value ? 1 : 0, Duration).OnComplete(() => frontCanvas.SetActive(value));
    }

    public void FadeBackground(float value)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(backCanvas.GetComponent<Image>().DOFade(.8f, value / 3))
            .AppendInterval(value / 3)
            .Append(backCanvas.GetComponent<Image>().DOFade(.3f, value / 3));
    }

    public void ResetMenu()
    {
        middleCanvas.SetActive(false);
        menus.SetActive(true);
        mainMenu.SetActive(true);
        startMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameClearMenu.SetActive(false);
        bossIndicator.SetActive(false);
        backCanvas.GetComponent<Image>().color = Color.clear;
    }

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }
}