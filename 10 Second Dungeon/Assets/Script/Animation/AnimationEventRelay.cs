using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    public void EndAttack()
    {
        player.EndAttack();
    }

    public void PerformAttack()
    {
        player.PerformAttack();
    }
}