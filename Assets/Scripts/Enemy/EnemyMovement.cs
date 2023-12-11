using System;
using System.Collections;
using BulletPro;
using UnityEngine;
using DG.Tweening;

// 일반 동작은 등장 후 시간이 지나면 알아서 퇴장.
// Wave는 왼쪽으로 직진하되 위아래로 물결치면서 이동. 진폭을 0으로 설정하면 직진만 함.
public enum MovementType
{
    Normal,
    Wave,
    Blink
}

public class EnemyMovement : MonoBehaviour
{
    public GameObject enemy;
    public MovementType movementType;
    private BulletEmitter bulletEmitter;
    public float delay = 3;
    public float duration = 10;

    [Header("Wave Settings")] public float xSpeed = 1;
    public float ySpeed = 1;
    public float amplitude = 1;

    private Vector3 targetPosition;
    private Sequence mySequence;
    internal bool isLeaving;

    private float t;
    private float yOffset;

    private void Start()
    {
        bulletEmitter = enemy.GetComponent<BulletEmitter>();

        targetPosition = enemy.transform.position;
        isLeaving = false;
        t = 0;
        yOffset = targetPosition.y;
        StartCoroutine(ShootWithDelay());

        enemy.transform.position = new Vector3(GameManager.instance.rightWall.position.x + 1f, targetPosition.y);

        // 시간이 지나면 자동으로 삭제
        if (movementType == MovementType.Wave)
        {
            Destroy(gameObject, delay + duration);
        }

        switch (movementType)
        {
            case MovementType.Normal:
                // 벽 y좌표
                const float wallPos = 3;
                mySequence = DOTween.Sequence();
                mySequence.Append(enemy.transform.DOMove(targetPosition, delay))
                    .AppendInterval(duration)
                    .Append(enemy.transform.DOPath(
                        new[]
                        {
                            targetPosition, targetPosition + Vector3.left,
                            targetPosition + (targetPosition.y > 0 ? Vector3.up : Vector3.down) * wallPos
                        }, 10, PathType.CatmullRom))
                    .OnComplete(() => Destroy(gameObject));
                break;
            case MovementType.Blink:
                enemy.transform.position = targetPosition;
                enemy.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                mySequence = DOTween.Sequence();
                mySequence.Append(enemy.GetComponent<SpriteRenderer>().DOFade(1, delay))
                    .AppendInterval(duration)
                    .Append(enemy.GetComponent<SpriteRenderer>().DOFade(0, delay))
                    .OnComplete(() => Destroy(gameObject));
                break;
        }
    }

    private void Update()
    {
        if (movementType != MovementType.Wave) return;
        if (!enemy) return;

        // 목표 좌표 재설정
        targetPosition = enemy.transform.position;

        // 등장 후 걸린 시간
        t += Time.deltaTime;

        // 이동 로직
        targetPosition.x -= xSpeed * Time.deltaTime;
        targetPosition.y = yOffset + amplitude * Mathf.Sin(ySpeed * t);
        enemy.transform.position = targetPosition;
    }

    IEnumerator ShootWithDelay()
    {
        yield return new WaitForSeconds(delay);
        if (!enemy) yield break;
        bulletEmitter.Play();
        yield return new WaitForSeconds(duration);
        bulletEmitter.Stop();
        isLeaving = true;
    }

    public void StopBehavior()
    {
        StopCoroutine(ShootWithDelay());
        mySequence.Kill();
    }
}