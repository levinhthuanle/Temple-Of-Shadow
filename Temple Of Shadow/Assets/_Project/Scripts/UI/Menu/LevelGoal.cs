using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelGoal : MonoBehaviour
{
    [SerializeField, Min(1)] private int rewardGold = 60;
    [SerializeField] private string goalLabel = "EXIT";

    private bool completed;

    private void Awake()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (completed || !other.CompareTag("Player"))
        {
            return;
        }

        completed = true;
        VictoryScreenController.CompleteLevel(rewardGold, goalLabel);
    }
}