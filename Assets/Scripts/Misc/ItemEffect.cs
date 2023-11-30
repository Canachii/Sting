using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType
{
    Score,
    Power,
    Bomb,
    Life
}

public class ItemEffect : MonoBehaviour
{
    public ItemType itemType;
    public int value = 1;

    public ItemMovement itemMovement;

    private Sequence mySequence;
    public static UnityAction onPlayerTouch;

    private void Start()
    {
        var targetPosition = transform.position + (Vector3)Random.insideUnitCircle * itemMovement.distance;

        mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOMove(targetPosition, itemMovement.spreadTime).SetEase(Ease.OutSine))
            .AppendInterval(itemMovement.delay)
            .Append(transform.DOMoveX(targetPosition.x - 10, itemMovement.lifeTime))
            .OnComplete(() => Destroy(gameObject));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        mySequence.Kill();
        GiveEffect(itemType, value);
        Destroy(gameObject);
    }

    public static void GiveEffect(ItemType type, int value = 1)
    {
        AudioManager.instance.PlaySFX("Pick Up");
        
        switch (type)
        {
            case ItemType.Score:
                GameManager.Score += value;
                UIController.instance.UpdateScore();
                break;
            case ItemType.Power:
                if (GameManager.Power <= 100)
                {
                    GameManager.Power += value;
                }
                else
                {
                    GameManager.Power += value;
                    GameManager.Score += 500;
                }
                
                onPlayerTouch?.Invoke();

                UIController.instance.UpdatePower();
                UIController.instance.UpdateScore();

                break;
            case ItemType.Bomb:
                GameManager.Bomb += value;
                UIController.instance.UpdateBomb();
                break;
            case ItemType.Life:
                GameManager.Life += value;
                UIController.instance.UpdateLife();
                break;
        }
    }
}