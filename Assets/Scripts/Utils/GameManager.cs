using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Play,
    Wait
}

/// <summary>
/// 게임 관리
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static GameState gameState;
    public GameObject[] playerPrefabs;
    public Transform topWall, bottomWall, leftWall, rightWall;
    public const int MaxPower = 100;

    private static int _power;
    public static float delta;
    public static int PlayerID { get; set; }
    public static int Difficulty { get; set; }
    public static int Life { get; set; }
    public static int Bomb { get; set; }
    public static int Score { get; set; }

    public static int Power
    {
        get => _power;
        set => _power = Mathf.Clamp(value, 0, MaxPower);
    }

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
        SceneManager.sceneLoaded += (scene, mode) => Start();
    }

    private void Start()
    {
        delta = 0;
        if (gameState != GameState.Play) return;

        // 데이터 불러온 후 캐릭터 생성
        LoadData();
        Instantiate(playerPrefabs[PlayerID]);
        
        Cursor.visible = false;

        // UI 업데이트
        UIController.instance.UpdateLife();
        UIController.instance.UpdateBomb();
        UIController.instance.UpdatePower();
        UIController.instance.UpdateScore();
    }
    
    private void Update()
    {
        delta += Time.deltaTime;
        if (!Input.GetButtonDown("Cancel")) return;
        PauseGame();
    }

    public static void PauseGame()
    {
        switch (UIController.instance.pauseMenu.activeSelf)
        {
            case false when gameState != GameState.Menu:
                Time.timeScale = 0;
                AudioManager.instance.PlaySFX("Cancel");
                AudioManager.instance.bgmSource.Pause();
                gameState = GameState.Menu;
                UIController.instance.pauseMenu.SetActive(true);
                Cursor.visible = true;
                break;
            case true when gameState == GameState.Menu:
                Time.timeScale = 1;
                gameState = GameState.Play;
                AudioManager.instance.bgmSource.Play();
                UIController.instance.pauseMenu.SetActive(false);
                Cursor.visible = false;
                break;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= (scene, mode) => Start(); 
    }

    // 게임오버 혹은 클리어했을 때 메인메뉴로 이동하면 데이터를 초기화
    public void InitData()
    {
        ES3.Save("Life", 3);
        ES3.Save("Bomb", 3);
        ES3.Save("Score", 0);
        ES3.Save("Power", 0);
    }

    // 클리어했을 때 다음 스테이지로 이동하면 데이터를 저장
    private static void SaveData()
    {
        ES3.Save("PlayerID", PlayerID);
        ES3.Save("Difficulty", Difficulty);
        ES3.Save("Life", Life);
        ES3.Save("Bomb", Bomb);
        ES3.Save("Score", Score);
        ES3.Save("Power", Power);
    }

    private static void LoadData()
    {
        PlayerID = ES3.Load("PlayerID", 0);
        Difficulty = ES3.Load("Difficulty", 1);
        Life = ES3.Load("Life", 3);
        Bomb = ES3.Load("Bomb", 3);
        Score = ES3.Load("Score", 0);
        Power = ES3.Load("Power", 0);
    }

    public void GameOver()
    {
        Debug.Log($"Game Over. C:{PlayerID}, D:{Difficulty}, S:{Score}, T:{delta}");
        gameState = GameState.Menu;
        UIController.instance.gameOverMenu.SetActive(true);
        Cursor.visible = true;
    }

    public void GameClear()
    {
        Debug.Log($"Game Clear! C:{PlayerID}, D:{Difficulty}, S:{Score}, T:{delta}");
        gameState = GameState.Menu;
        UIController.instance.gameClearMenu.SetActive(true);
        Cursor.visible = true;

        SaveData();
    }

    // 난이도 조절 및 캐릭터ID는 드롭다운으로
    public void SetDifficulty(int value)
    {
        Difficulty = value;
        ES3.Save("Difficulty", value);
    }

    public void SetPlayerID(int value)
    {
        PlayerID = value;
        ES3.Save("PlayerID", value);
    }

    public void QuitGame()
    {
        AudioManager.instance.PlaySFX("Confirm");
        AudioManager.instance.FadeMusicVolume(true);
        UIController.instance.FadeScreen(true);
        Application.Quit();
    }
}