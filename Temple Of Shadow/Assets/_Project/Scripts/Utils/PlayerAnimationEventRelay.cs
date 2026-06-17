using UnityEngine;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    private PlayerCombat combat;
    private PlayerHealth health;

    private void Awake()
    {
        combat = GetComponentInParent<PlayerCombat>();
        health = GetComponentInParent<PlayerHealth>();
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

    public void FinishHurt()
    {
        health.FinishHurt();
    }


}