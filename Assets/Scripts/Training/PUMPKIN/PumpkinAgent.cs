using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PumpkinAgent : Agent
{
    Rigidbody2D rigidbody;
    Animator animator;
    public Player player;

    public float speed = 4;
    public int damage = 10;
    public int hp = 50;
    float distanceToDetect = 50f;
    private bool isFacingRight = true;

    private bool isDetected()
    {
        return Mathf.Abs(Vector3.Distance(player.transform.position, transform.position)) < distanceToDetect;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (hp <= 0)
        {
            StartCoroutine(killHead());
        }
    }

    private IEnumerator killHead()
    {
        animator.SetBool("Die", true);
        yield return new WaitForSeconds(0.66f);
        Destroy(this.gameObject);
    }

    public override void OnEpisodeBegin()
    {
        //player.health = 100;//

        //player.transform.localPosition = new Vector3(Random.Range(17f, 57f), Random.Range(26f, 46f), 0f);//
        //ransform.localPosition = new Vector3(Random.Range(17f, 57f), Random.Range(26f, 46f), 0f);//
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(player.transform.localPosition);
        sensor.AddObservation(Mathf.Abs(Vector3.Distance(player.transform.localPosition, transform.localPosition)));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (isDetected())
        {
            float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
            if ((moveX > 0) && !isFacingRight) flipSprite();
            if ((moveX < 0) && isFacingRight) flipSprite();

            float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
            transform.position += new Vector3(moveX, moveY, 0f) * Time.deltaTime * speed;
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            hp -= 20;//
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

    private IEnumerator hitPlayer()
    {
        player.health -= damage;
        Debug.Log("PLAYER GOT HIT");

        if (playerIsDead())
        {
            SetReward(+30f);
            EndEpisode();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    private bool playerIsDead()
    {
        if (player.health <= 0)
        {
            return true;
        }
        else { return false; }
    }

    private void flipSprite()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;

        isFacingRight = !isFacingRight;
    }
}