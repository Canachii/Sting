using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CharacterFade : MonoBehaviour
{
    public int id;
    private const float Duration = .2f;
    private Image portrait;

    private void Start()
    {
        portrait = GetComponent<Image>();
        FadePortrait();
    }

    public void FadePortrait()
    {
        portrait.DOColor(id == GameManager.PlayerID ? Color.white : Color.gray, Duration).SetEase(Ease.OutSine);
    }
}