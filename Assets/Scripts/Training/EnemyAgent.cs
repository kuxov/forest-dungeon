using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class EnemyAgent : Agent
{
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] private Player player;

    public float speed;
    public int damage = 30;
    public int hp = 100;
    float distanceToDetect = 10f; 

    private bool isDetected()
    {
        return Mathf.Abs(Vector3.Distance(player.transform.position, transform.position)) < distanceToDetect;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (hp <= 0)
        {
            StartCoroutine(killTree());
        }
    }

    private IEnumerator killTree()
    {
        animator.SetTrigger("TreeIsDead");
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }

    public override void OnEpisodeBegin()
    {
        //float[] possiblePlayerX = { -15f, -3f, 10f };
       // float[] possiblePlayerY = { 15f, 3f, -10f };
        //float[] possibleEnemyX = { -12f, 2f, 15f };
       // float[] possibleEnemyY = { 12f, -2f, -15f};

       // player.transform.localPosition = new Vector3(possiblePlayerX[Random.Range(0, possiblePlayerX.Length)], possiblePlayerY[Random.Range(0, possiblePlayerY.Length)], 0f);
       // transform.localPosition = new Vector3(possibleEnemyX[Random.Range(0, possibleEnemyX.Length)], possibleEnemyY[Random.Range(0, possibleEnemyY.Length)], 0f);
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
            float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

            transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * speed;
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
            Debug.Log("PLAYER FOUND");
            StartCoroutine(hitPlayer());
            AddReward(+15f);
        }
        else if (collision.gameObject.CompareTag("walls"))
        {
            Debug.Log("HIT THE WALL");
            SetReward(-5f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("obstacles"))
        {
            SetReward(-5f);
            EndEpisode();
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
}