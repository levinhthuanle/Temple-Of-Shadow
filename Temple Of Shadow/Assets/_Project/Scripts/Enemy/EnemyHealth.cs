using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 5;
    private SpriteRenderer spriteRenderer;

    private int currentHp;

    private void Awake()
    {
        currentHp = maxHp;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;


        Debug.Log($"{gameObject.name} took {damage} damage. HP left: {currentHp}");
        StartCoroutine(FlashRed());

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = Color.white;
    }
}