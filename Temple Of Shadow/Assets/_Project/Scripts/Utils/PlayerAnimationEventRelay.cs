using UnityEngine;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    private PlayerCombat combat;

    private void Awake()
    {
        combat = GetComponentInParent<PlayerCombat>();
    }

    public void SlashHit()
    {
        combat.SlashHit();
    }

    public void KickHit()
    {
        combat.KickHit();
    }

    public void ThrowProjectile()
    {
        Debug.Log("ThrowProjectile CALLED");
        combat.ThrowProjectile();
    }

    public void Throw()
    {
        Debug.Log("Throw CALLED");
        combat.ThrowProjectile();
    }

    public void FinishAttack()
    {
        combat.FinishAttack();
    }
}