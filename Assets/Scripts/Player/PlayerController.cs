using System;
using System.Collections;
using UnityEngine;
using BulletPro;
using Utils;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] public GameObject player;
    public float maxSpeed = 4;
    public float minSpeed = 2;
    private float x, y, speed;

    [Header("Attack")] public EmitterProfile[] emitterProfiles;
    public EmitterProfile bombProfile;

    [Header("Component")] public ParticleSystem particle;
    internal BulletEmitter bulletEmitter;
    internal BulletReceiver bulletReceiver;
    private SpriteRenderer spriteRenderer;

    [Header("Misc")] public float invincibleDuration = 3;
    private bool isCooldown;

    private float X
    {
        get => x;
        set => x = Mathf.Clamp(value, GameManager.instance.leftWall.position.x,
            GameManager.instance.rightWall.position.x);
    }

    private float Y
    {
        get => y;
        set => y = Mathf.Clamp(value, GameManager.instance.bottomWall.position.y,
            GameManager.instance.topWall.position.y);
    }

    private void OnEnable()
    {
        ItemEffect.onPlayerTouch += SetEmitterProfile;
    }

    private void Start()
    {
        bulletEmitter = player.GetComponent<BulletEmitter>();
        bulletReceiver = player.GetComponent<BulletReceiver>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        SetEmitterProfile();

        // 좌표 초기화
        X = player.transform.position.x;
        Y = player.transform.position.y;

        // 처음 3초는 쿨다운
        isCooldown = true;
        StartCoroutine(CoolDownAtStart(invincibleDuration));
    }

    private void OnDisable()
    {
        ItemEffect.onPlayerTouch -= SetEmitterProfile;
    }

    private static EmitterProfile GetEmitterProfile(EmitterProfile[] arr)
    {
        if (arr.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(arr));
        return GameManager.Power switch
        {
            < 60 and >= 30 => arr[1],
            < 90 and >= 60 => arr[2],
            >= 90 => arr[3],
            _ => arr[0]
        };
    }

    // 파워에 따른 플레이어의 발사 패턴 변화
    private void SetEmitterProfile()
    {
        bulletEmitter.emitterProfile = GetEmitterProfile(emitterProfiles);
    }

    private void Update()
    {
        // Play나 Wait 상태일 때는 움직일 수 있음
        if (GameManager.gameState == GameState.Menu) return;
        Move();

        // Wait 상태일 땐 움직이는 것만 가능
        if (GameManager.gameState == GameState.Wait) return;
        Attack();
        Bomb();
    }

    private void Move()
    {
        // 키를 입력받고 좌표를 변경
        X += Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        Y += Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;

        // 쉬프트를 누르면 감속
        speed = Input.GetKey(KeyCode.LeftShift) ? minSpeed : maxSpeed;

        // 움직임
        player.transform.position = new Vector3(X, Y);
    }

    private void Attack()
    {
        // 공격 : Z키를 누르고 있으면 공격
        if (Input.GetKey(KeyCode.Z))
            bulletEmitter.Play();
        else bulletEmitter.Stop();
    }

    private void Bomb()
    {
        // 폭탄 사용 : X키를 누르면 3초 간 무적 상태
        if (Input.GetKeyDown(KeyCode.X) && !isCooldown && GameManager.Bomb > 0)
        {
            GameManager.Bomb--;
            UIController.instance.UpdateBomb();
            UIController.instance.FadeBackground(invincibleDuration);
            AudioManager.instance.PlaySFX("Bomb");

            // 폭탄 프로필이 있으면 변경
            if (bombProfile != null)
            {
                bulletEmitter.emitterProfile = bombProfile;
                GameManager.gameState = GameState.Wait;
                bulletEmitter.Play();
            }

            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemy.GetComponent<BulletEmitter>().Kill();
                try
                {
                    if (enemy.GetComponent<EnemyMovement>().isLeaving) return;
                }
                catch (NullReferenceException)
                {
                    Debug.Log("Can't find Component");
                }

                enemy.GetComponent<BulletEmitter>().Play();
            }

            StartCoroutine(SetInvincible(invincibleDuration));
        }
    }

    // 무적 상태 활성화
    IEnumerator SetInvincible(float time)
    {
        isCooldown = true;
        bulletReceiver.enabled = false;
        yield return new WaitForSeconds(time);
        isCooldown = false;
        bulletReceiver.enabled = true;
        GameManager.gameState = GameState.Play;
        SetEmitterProfile();
    }

    IEnumerator CoolDownAtStart(float time)
    {
        yield return new WaitForSeconds(time);
        isCooldown = false;
    }

    public void HitBullet()
    {
        GameManager.Life--;
        UIController.instance.UpdateLife();

        GameManager.Power = (int)(GameManager.Power * .9f);
        UIController.instance.UpdatePower();
        SetEmitterProfile();

        // gameCamera.DOShakePosition(ShakeDuration, ShakePower);
        CameraController.instance.ShakeCamera();
        AudioManager.instance.PlaySFX("Player Hit");
        particle.transform.position = player.transform.position;
        particle.Play();

        // 목숨이 남아있을 때는 지정된 시간 동안 무적. 남은 목숨이 없다면 삭제.
        if (GameManager.Life > 0)
        {
            StartCoroutine(Revive(invincibleDuration));
        }
        else
        {
            spriteRenderer.color = Color.clear;
            Destroy(gameObject, particle.main.duration);
            Destroy(player);
            GameManager.instance.GameOver();
        }
    }

    IEnumerator Revive(float time)
    {
        spriteRenderer.color = new Color(1, 1, 1, .6f);
        yield return SetInvincible(time);
        spriteRenderer.color = Color.white;
    }
}