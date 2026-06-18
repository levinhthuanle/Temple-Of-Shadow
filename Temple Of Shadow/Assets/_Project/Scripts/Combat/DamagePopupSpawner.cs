using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);

    public void ShowDamage(int damage)
    {
        DamagePopup popup = Instantiate(
            damagePopupPrefab,
            transform.position + offset,
            Quaternion.identity
        );

        popup.Setup(damage);
    }
}