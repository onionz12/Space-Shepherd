using UnityEngine;
using System.Collections;

public enum DeathReason
{
    OUT_OF_BOUNDS,
    WOLF,
    ASTEROID
}

public abstract class SpaceAnimal : MonoBehaviour
{
    [SerializeField] protected float speed = 0.2f;
    protected Animator animator;
    protected bool hit;

    protected SheepManager sheepManager;

    protected virtual void FloatInSpace()
    {
        StartCoroutine(FloatInSpaceRoutine());
    }

    protected IEnumerator FloatInSpaceRoutine() { //used to be Roll
        hit = true;
        if (animator == null)
        {
            yield break;
        }
        speed = 1;
        for (float speed = 1f; speed > 0f; speed -= 0.1f)
        {
            if(speed < 0f)
            {
                speed = 0f;
            }
            animator.SetFloat("speedMultiplier", speed);
            // Wait for 0.5 seconds before the next iteration
            yield return new WaitForSeconds(.5f);
        }
        animator.SetFloat("speedMultiplier", 0);
        hit = false;
    }

    public abstract void Die(DeathReason deathReason);
}
