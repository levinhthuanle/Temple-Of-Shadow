using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Info")]
    public string characterName;

    [Header("Base Stats")]
    public int maxHp;

    public int damage;

    public int armor;

    public float moveSpeed;

    public float attackSpeed;

    public float jumpForce;

    [Header("UI")]
    public Sprite portrait;
}