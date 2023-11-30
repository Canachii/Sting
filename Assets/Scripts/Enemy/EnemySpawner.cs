using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    Easy = 0,
    Normal,
    Hard
}

[System.Serializable]
public class Enemy
{
    public Difficulty difficulty;
    public GameObject enemyPrefab;
    [Range(-2f, 2f)] public float yPosition;
    public float spawnTime;
}

public class EnemySpawner : MonoBehaviour
{
    private Enemy[] enemies;
    private float delta;

    private void ReadSpawnData()
    {
        var textFile = Resources.Load<TextAsset>(SceneManager.GetActiveScene().name);
        
        if (textFile == null)
        {
            Debug.Log("No spawn data");
            return;
        }

        // 첫 줄은 헤더 (난이도, 적 이름, y좌표, 생성 시간)
        string[] lines = textFile.text.Split('\n');
        enemies = new Enemy[lines.Length - 1];
        for (int i = 0; i < lines.Length - 1; i++)
        {
            string[] data = lines[i + 1].Split(',');
            enemies[i] = new Enemy
            {
                difficulty = (Difficulty)int.Parse(data[0]),
                enemyPrefab = Resources.Load<GameObject>($"Enemy/{data[1]}"),
                yPosition = float.Parse(data[2]),
                spawnTime = float.Parse(data[3])
            };
        }
    }

    IEnumerator SpawnEnemy()
    {
        foreach (var enemy in enemies)
        {
            // 적 난이도가 게임 난이도보다 높으면 생성되지 않음
            if (enemy.difficulty.GetHashCode() > GameManager.Difficulty) yield break;

            // 시간이 지나면 생성
            yield return new WaitForSeconds(enemy.spawnTime);
            Debug.Log($"{enemy.enemyPrefab} Spawned : {delta}");
            Instantiate(enemy.enemyPrefab,
                new Vector3(GameManager.instance.rightWall.position.x - 1.5f, enemy.yPosition), Quaternion.identity);
        }
    }

    private void Start()
    {
        delta = 0;
        ReadSpawnData();
        StartCoroutine(SpawnEnemy());
    }

    public void Update()
    {
        delta += Time.deltaTime;
    }
}