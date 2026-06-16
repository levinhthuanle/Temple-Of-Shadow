using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Temple Of Shadow/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    public CharacterClassType classType;
    public Sprite icon;
    public RuntimeAnimatorController animatorController;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Stats")]
    public int maxHealth = 100;
    public int attackDamage = 10;
}

