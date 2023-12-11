using System.Collections;
using BulletPro;
using UnityEngine;
using DG.Tweening;

public class Boss01 : EnemyHealth
{
    [Header("Boss Settings")] [Range(0, 4)]
    public int maxExtraLife = 2; // 보스일 때 페이즈를 나누기 위한 추가 목숨

    private int ExtraLife { get; set; } // 남은 목숨 (보스에 적용)

    public EmitterProfile[] emitterProfiles;
    public EmitterProfile[] subEmitterProfiles;
    public BulletEmitter bulletEmitter;
    public BulletEmitter subBulletEmitter;
    private BulletReceiver bulletReceiver;

    private int phase;
    private Sequence sequence;
    private float t;
    private Vector3 targetPosition;
    private DialogueSystem dialogueSystem;

    protected override void Awake()
    {
        // 난이도에 따른 체력 (쉬움 = 0.5x, 보통 = 1.0x, 어려움 = 1.5x)
        maxHealth = GameManager.Difficulty switch
        {
            2 => (int)(maxHealth * 1.5f),
            1 => maxHealth,
            _ => maxHealth / 2
        };

        Health = maxHealth;
        ExtraLife = maxExtraLife;
    }

    private void Start()
    {
        // 초기화
        bulletReceiver = enemy.GetComponent<BulletReceiver>();
        dialogueSystem = GetComponent<DialogueSystem>();

        bulletReceiver.enabled = false;
        targetPosition = enemy.transform.position;
        UIController.instance.bossHealth.maxValue = maxHealth;

        // 위치 이동
        enemy.transform.position = new Vector3(GameManager.instance.rightWall.position.x + 1f, targetPosition.y);
        enemy.transform.DOMove(targetPosition, 3f)
            .OnComplete(() => dialogueSystem.EnableDialogue(true)); // 이동 후 대화 활성화

        // 페이즈 시작
        dialogueSystem.onDialogueFinish += () =>
        {
            if (!enemy.activeSelf) return;
            Health = maxHealth;
            ExtraLife = maxExtraLife;
            t = 60;
            phase = 0;
            bulletReceiver.enabled = true;

            BossStart(phase);
            GameManager.gameState = GameState.Play;
            UIController.instance.bossIndicator.SetActive(true);
            UIController.instance.bossHealth.value = Health;
            AudioManager.instance.PlayMusic("Boss01");
        };
    }

    private void Update()
    {
        if (GameManager.gameState != GameState.Play) return;

        t -= Time.deltaTime;
        UIController.instance.timer.text = $"{Mathf.FloorToInt(t)}";

        // 시간 초가 0이 되면 체력 1
        if (!(t <= 0)) return;
        Health = 1;
        t = 60;
        UIController.instance.bossHealth.value = Health;
    }

    public override void TakeDamage()
    {
        Health--;
        GameManager.Score += 10;
        UIController.instance.UpdateScore();
        UIController.instance.bossHealth.value = Health;

        if (Health > 0) return;

        // 추가 목숨(보스)이 있으면 체력 회복 및 페이즈 넘김
        if (ExtraLife > 0)
        {
            ExtraLife--;
            Health = maxHealth;

            bulletEmitter.Kill();
            subBulletEmitter.Kill();
            StopCoroutine(StartPhase(phase));
            phase++;
            t = 60;
            AudioManager.instance.PlaySFX("Bomb");
            StartCoroutine(StartPhase(phase));

            return;
        }

        // 대화문 활성화
        AudioManager.instance.FadeMusicVolume(true);
        bulletEmitter.Kill();
        subBulletEmitter.Kill();
        dialogueSystem.isVictory = true;
        dialogueSystem.EnableDialogue(true);
        dialogueSystem.DisplayDialogue();

        OnDeath();
    }

    protected override void OnDeath()
    {
        // 오브젝트 파괴
        AudioManager.instance.PlaySFX("Boss Destroy");
        particle.transform.position = enemy.transform.position;
        particle.Play();
        sequence.Kill();
        UIController.instance.bossIndicator.SetActive(false);
        Destroy(enemy);
    }

    private void BossStart(int value)
    {
        StartCoroutine(StartPhase(value));
    }

    IEnumerator StartPhase(int value)
    {
        bulletEmitter.emitterProfile = emitterProfiles[value];
        bulletEmitter.Play();
        subBulletEmitter.emitterProfile = subEmitterProfiles[value];
        subBulletEmitter.Play();
        sequence.Kill();

        sequence = DOTween.Sequence();

        switch (value)
        {
            case 1:
                sequence.Append(enemy.transform.DOMoveY(0, 2f).SetEase(Ease.InOutSine));
                break;
            default:
                sequence.Append(enemy.transform.DOMoveY(2, 2f).SetEase(Ease.Linear))
                    .Append(enemy.transform.DOMoveY(-2, 2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo));
                break;
        }

        yield return null;
    }
}