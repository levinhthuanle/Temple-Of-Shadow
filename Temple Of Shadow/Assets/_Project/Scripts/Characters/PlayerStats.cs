using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;

    public int MaxHP => characterData.maxHp;

    public int Damage => characterData.damage;

    public int Armor => characterData.armor;

    public float MoveSpeed => characterData.moveSpeed;

    public float AttackSpeed => characterData.attackSpeed;

    public float JumpForce => characterData.jumpForce;

    private void Start()
    {
        Debug.Log("Character: " + characterData.characterName);
        Debug.Log("HP: " + MaxHP);
        Debug.Log("Damage: " + Damage);
    }
}