using BulletPro;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.gameState == GameState.Menu) return;
        
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ItemEffect.GiveEffect(ItemType.Life);
            UIController.instance.UpdateLife();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ItemEffect.GiveEffect(ItemType.Bomb);
            UIController.instance.UpdateBomb();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            foreach (var o in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                o.GetComponent<BulletEmitter>().Kill();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            ItemEffect.GiveEffect(ItemType.Power, 10);
            UIController.instance.UpdatePower();
        }
    }
}