using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public enum Speaker
{
    Player,
    Enemy
}

[System.Serializable]
public class Dialogue
{
    public Speaker speaker;
    public string line;
}

public class DialogueSystem : MonoBehaviour
{
    [Header("Portrait Control")] private Image playerPortrait;
    private Image enemyPortrait;
    public Sprite enemyPortraitSprite;
    public float fadeTime = .2f;

    [Header("Conversation")] private TextMeshProUGUI txt;
    public Dialogue[] encounterDialogues;
    public Dialogue[] victoryDialogues;

    [HideInInspector] public int count;
    [HideInInspector] public bool isVictory;
    public UnityAction onDialogueFinish;
    private PlayerController player;

    public void EnableDialogue(bool value)
    {
        if (value)
        {
            count = 0;
            UIController.instance.hud.SetActive(false);
            UIController.instance.dialogue.SetActive(true);
            DisplayDialogue();

            // 대화창 활성화 시 플레이어 무적
            GameManager.gameState = GameState.Wait;
            player.bulletReceiver.enabled = false;
            player.bulletEmitter.Stop();
        }
        else
        {
            UIController.instance.hud.SetActive(true);
            UIController.instance.dialogue.SetActive(false);
        }
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponentInParent<PlayerController>();

        count = 0;
        isVictory = false;

        playerPortrait = UIController.instance.playerPortrait;
        enemyPortrait = UIController.instance.enemyPortrait;
        txt = UIController.instance.dialogueTxt;

        playerPortrait.sprite = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>().sprite;
        enemyPortrait.sprite = enemyPortraitSprite;
    }

    private void Update()
    {
        if (!UIController.instance.dialogue.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Z) && count < Situation().Length - 1)
        {
            count++;
            DisplayDialogue();
        }
        else if (Input.GetKeyDown(KeyCode.Z) && isVictory)
        {
            EnableDialogue(false);
            GameManager.instance.GameClear();
            Destroy(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            EnableDialogue(false);

            player.bulletReceiver.enabled = true;
            // 보스전 후 공격이 되는 것을 방지하기 위해 대화문 종료 후 로직은 보스전 스크립트에 있음.
            onDialogueFinish?.Invoke();
        }
    }

    public void DisplayDialogue()
    {
        txt.text = Situation()[count].line;
        FadePortrait(Situation()[count].speaker);
    }

    private void FadePortrait(Speaker speaker)
    {
        playerPortrait.DOColor(speaker == Speaker.Player ? Color.white : Color.gray, fadeTime);
        enemyPortrait.DOColor(speaker == Speaker.Enemy ? Color.white : Color.gray, fadeTime);
    }

    private Dialogue[] Situation()
    {
        return isVictory ? victoryDialogues : encounterDialogues;
    }
}