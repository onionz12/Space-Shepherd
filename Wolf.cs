using UnityEngine;
using System.Collections;

public class Wolf : SpaceAnimal
{
    //[SerializeField] private float speed = 0.2f;
    [SerializeField] private float attackRange = 0.2f;
    [SerializeField] private float searchRadius = 3f;

    [SerializeField] private int sheepEatenInShed = 2;
    [SerializeField] private float timeoutAfterShed = 5f;
    [SerializeField] private float timeoutAfterOutBounds = 15f;

    private bool alive = true;

    private bool reachedTarget = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //find sheep manager
        sheepManager = GameObject.Find("SheepManager").GetComponent<SheepManager>();
        StartCoroutine(BeEvil());
    }

    Sheep FindTarget(float searchRadius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        float minDistance = float.MaxValue;
        Sheep nearestSheep = null;

        float currentDistance;

        foreach (Collider2D hit in hits)
        {
            Debug.Log("in reach: " + hit.name);
            if (!hit.TryGetComponent<Sheep>(out var sheep)) {
                continue;
            }
            currentDistance = Vector2.Distance(transform.position, hit.transform.position);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                nearestSheep = sheep;
            }
        }
        return nearestSheep;
    }

    IEnumerator MoveTowardTarget(Transform targetTransform)    // what if sheep dies?
    {
        reachedTarget = false;
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        while (targetTransform != null && distance > attackRange && distance < searchRadius)
        {
            //Debug.Log("distance found: " + distance.ToString());
            if(hit) {   //abort attack when hit
                reachedTarget = false;
                yield break;
            }
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            
            distance = Vector3.Distance(transform.position, targetTransform.position);
            yield return null;
        }
        if(distance <= attackRange)
        {
            reachedTarget = true;
            yield break;
        }
        reachedTarget = false;
        yield break;
    }

    void AttackTarget(Sheep sheep)
    {
        Debug.Log("Wolf is killing a sheep");
        sheep.Die(DeathReason.WOLF);
    }

    IEnumerator BeEvil()
    {
        Debug.Log("Wolf activated");
        float currentSearchRadius = searchRadius;
        while (alive)
        {
            if(!hit)
            {
                Sheep target = FindTarget(searchRadius);
                if(target == null)
                {
                    //wait 1 second then make search radius larger
                    Debug.Log("Wolf found no target in reach, now waiting 2s");
                    yield return new WaitForSeconds(2f);
                    //make radius bigger
                    currentSearchRadius = currentSearchRadius * 1.2f;
                    continue;
                }
                yield return StartCoroutine(MoveTowardTarget(target.transform));
                if (reachedTarget)
                {
                    AttackTarget(target);
                    yield return new WaitForSeconds(5f);
                }
                //reset search radius when a target was found and wolf moved (independent of whether attack was success)
                currentSearchRadius = searchRadius;
            } else
            {
                //wait three seconds before next attack when hit
                yield return new WaitForSeconds(3f);
            }
        }
    }

    public override void Die(DeathReason deathReason) {
        StartCoroutine(DieRoutine(deathReason));
    }

    IEnumerator DieRoutine(DeathReason deathReason)
    {
        Debug.Log(deathReason.ToString());
        StopCoroutine(BeEvil());
        //disappear
        Disappear();
        yield return new WaitForSeconds(timeoutAfterOutBounds);
        AppearAtRandomLocation();
        StartCoroutine(BeEvil());
    }

    IEnumerator ReachedGoal()
    {
        sheepManager.WolfInShed(sheepEatenInShed);
        StopCoroutine(BeEvil());
        //disappear
        Disappear();
        yield return new WaitForSeconds(timeoutAfterShed);
        AppearAtRandomLocation();
        StartCoroutine(BeEvil());
    }

    void AppearAtRandomLocation()
    {
        transform.position = sheepManager.RandomLocation();
        //appear
        Appear();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected");
        if(collision.gameObject.tag == "Bounds")
        {
            Debug.Log("WOLF HIT BOUNDARY!!");
            Die(DeathReason.OUT_OF_BOUNDS);
        } else if (collision.gameObject.tag == "Finish")
        {
            StartCoroutine(ReachedGoal());
        }
        else if(collision.gameObject.tag == "Player")   //only player can make wolf roll
        {
            FloatInSpace();
        } else
        {
            Debug.Log("Wolf collided with sth, probably a sheep");
        }
    }

    private void Disappear()
    {
        //disable interaction with other objects
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        //dont render while on pause
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.enabled = false;
        }
    }

    private void Appear()
    {
        //render
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.enabled = true;
        }
        //enable interaction with other objects
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }
}
