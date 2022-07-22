using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BossAgent : Agent
{
    public Rigidbody2D rigidbody;
    public Animator animator;
    public PumpkinAgent Head;

    public Player player;
    public int health = 100;
    public float speed = 5f;
    public int damage = 30;
    
    private bool isFacingRight = true;
    private float distanceToDetect = 50f;

    private bool Entered2ndPhase = false;
    
    public override void OnEpisodeBegin()
    {
        player.health = 100;
        
        //player.transform.localPosition = new Vector3(Random.Range(17f, 57f), Random.Range(26f, 46f), 0f);
        //transform.localPosition = new Vector3(Random.Range(17f, 57f), Random.Range(26f, 46f), 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(player.transform.localPosition);
        sensor.AddObservation(Vector3.Distance(player.transform.localPosition, transform.localPosition));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if(isDetected())
        {
            float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
            if ((moveX > 0) && !isFacingRight) flipSprite();
            if ((moveX < 0) && isFacingRight) flipSprite();

            float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

            animator.SetFloat("Speed", Mathf.Abs(moveX * speed));
            transform.position += new Vector3(moveX, moveY, 0f) * Time.deltaTime * speed;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Speed", Mathf.Abs(continuousActions[0] * speed));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            health -= 20;//
            Debug.Log("PLAYER FOUND");
            StartCoroutine(hitPlayer());
            AddReward(+5f);
        }
        else if (collision.gameObject.CompareTag("walls"))
        {
            Debug.Log("HIT THE WALL");
            SetReward(-5f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("enemy"))
        {
            AddReward(-2.5f);
        }
    }
    private bool isDetected() 
    { 
        return Vector3.Distance(
            player.transform.localPosition, 
            transform.localPosition) < distanceToDetect; 
    }

    private void flipSprite() 
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;

        isFacingRight = !isFacingRight;
    }

    private IEnumerator hitPlayer() 
    {
        player.health -= damage;

        if (playerIsDead())
        {
            SetReward(+30);
            EndEpisode();
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
    }

    private bool playerIsDead() 
    { 
        if (player.health <= 0)
        {
            return true;
        }
        else return false;
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Head.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (health <= 0)
        {
            StartCoroutine(killTree());
        }
        else if (health <= 60 && !Entered2ndPhase)
        {
            StartCoroutine(enter2ndPhase());
            Entered2ndPhase = true;
        } 
    }

    private IEnumerator killTree()
    {
        animator.SetBool("Die", true);
        yield return new WaitForSeconds(1.23f);
        Destroy(this.gameObject);
    }

    private IEnumerator enter2ndPhase()
    {
        animator.SetBool("Enter2ndPhase", true);
        yield return new WaitForSeconds(0.66f);
        speed = 1f;
        yield return new WaitForSeconds(1f);
        Head.gameObject.SetActive(true);
        Head.transform.localPosition = new Vector3(57f, 47f, 0f); 
    }
}
