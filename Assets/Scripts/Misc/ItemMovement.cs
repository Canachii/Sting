using UnityEngine;

[CreateAssetMenu(fileName = "Default Movement", menuName = "Scriptable Object Asset/Item Movement", order = 0)]
public class ItemMovement : ScriptableObject
{
    public float distance = .5f;
    public float spreadTime = .5f;
    public float delay = 1f;
    public float lifeTime = 7f;
}