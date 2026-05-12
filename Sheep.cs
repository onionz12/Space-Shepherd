using UnityEngine;
using System.Collections;

public class Sheep : SpaceAnimal
{
    //[SerializeField] private float speed = 0.2f;
    [SerializeField] private Vector3 floatingDirection; // = new Vector3(0, 0, 0)
    [SerializeField] private Vector3 movementCenter;
    [SerializeField] private float maxDistance = 0.1f;

    //private SheepManager sheepManager;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;

    //private Animator animator;
    //private bool hit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        //floatingDirection.z = transform.position.z;
        floatingDirection = Random2DDirection();
        movementCenter = transform.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move();
        if(!hit)
        {
             MoveAroundPoint();
        }
    }

    void Move()
    {
        transform.position += speed * floatingDirection * Time.deltaTime;
    }

    void MoveAroundPoint()
    {
        transform.position += speed * floatingDirection * Time.deltaTime;
        if(Vector3.Distance(transform.position, movementCenter) >= maxDistance)
        {
            floatingDirection = Random2DDirection();
        }
    }

    Vector3 Random2DDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0); //transform.position.z
    }

    public override void Die(DeathReason deathReason)
    {
        Debug.Log(deathReason.ToString());
        sheepManager.SheepDies(this);
    }

    protected virtual void ReachedGoal()
    {
        sheepManager.SheepReachedGoal(this);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision detected");
        if(collision.gameObject.tag == "Bounds")    //OR wolf
        {
            //Debug.Log("BOUNDARY!!");
            Die(DeathReason.OUT_OF_BOUNDS);
        } else if (collision.gameObject.tag == "Finish")
        {
            ReachedGoal();
        } else if (collision.gameObject.tag == "Wolf")
        {
            //handled in wolf
        }
        else //if(collision.gameObject.tag == "Player")
        {
            FloatInSpace();
        }
    }

    public void SetSheepManager(SheepManager sheepManager)
    {
        this.sheepManager = sheepManager;
    }


    //get rolling when hit
    protected override void FloatInSpace() {
        StartCoroutine(FloatThenSetCenter());
    }

    private IEnumerator FloatThenSetCenter()
    {
        yield return StartCoroutine(FloatInSpaceRoutine());
        movementCenter = transform.position;
    }

    public void DisableComponents()
    {
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
    }
}
