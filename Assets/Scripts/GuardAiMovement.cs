using static System.Math;
using System.Collections.Generic;
using UnityEngine;

public class GuardAiMovement : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float maxVelocity = 4f;
    public Vector2 movement;
    public Vector2 direction;

    public Rigidbody2D rb2D;
    public Animator animator;

    private const int FOLLOW_PATH = 0;
    private const int FOLLOW_PLAYER = 1;
    private const int ALERT = 2;

    [SerializeField] private int state = FOLLOW_PATH;
    private int goForward = 1;

    private const string ANIM_HORIZONTAL_PARAM = "Horizontal";
    private const string ANIM_VERTICAL_PARAM = "Vertical";
    private const string ANIM_SPEED_PARAM = "Speed";

    private List<Transform> waypoints;
    [SerializeField] private Transform waypointsParent;
    [SerializeField] private int waypointIndex = 0;
    [SerializeField] private Transform visionCollider;

    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.speed = movementSpeed;

        waypoints = new List<Transform>();
        foreach (Transform waypoint in waypointsParent)
        {
            waypoints.Add(waypoint);
        }

        movement = Vector2.zero;
        direction = Vector2.zero;

        SetDestinationGivenWaypoint(waypoints[waypointIndex].position);
    }

    private void Update()
    {
        Vector3 position = transform.position;
        direction.x = (float) Clamp(movement.x - position.x, -1.0, 1.0);
        direction.y = (float) Clamp(movement.y - position.y, -1.0, 1.0);

        RotateColliderBasedOnDirection();

        animator.SetFloat(ANIM_HORIZONTAL_PARAM, direction.x);
        animator.SetFloat(ANIM_VERTICAL_PARAM, direction.y);
        animator.SetFloat(ANIM_SPEED_PARAM, movement.sqrMagnitude);
    }

    private void FixedUpdate ()
    {
        switch (state)
        {
            case FOLLOW_PATH:
                FollowPath();
                break;
            case FOLLOW_PLAYER:
                break;
            case ALERT:
                break;
        }
    }

    private void FollowPath()
    {
        MoveAlongPath(waypoints[waypointIndex].position);
    }

    private void MoveAlongPath(Vector2 waypoint)
    {
        movement.x = waypoint.x;
        movement.y = waypoint.y;

        rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, maxVelocity);
    }

    private void RotateColliderBasedOnDirection()
    {
        Vector3 rotation = visionCollider.rotation.eulerAngles;
        visionCollider.rotation = Quaternion.Euler(rotation.x, rotation.y, GetZetaValueBasedOnAnimationClip());
    }

    private float GetZetaValueBasedOnAnimationClip()
    {
        float z = 0;

        if (Abs(direction.x) > Abs(direction.y))
        {
            z = direction.x >= 0 ? 90 : 270;
        }
        else
        {
            z = direction.y >= 0 ? 180 : 0;
        }

        return z;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Contains("Waypoint"))
        {
            ManageBodyCollider(other);
        }
        else if (other.name.Equals("Player"))
        {
            ManageVisionCollider(other);
        }

    }

    private void ManageBodyCollider(Collider2D other)
    {
        if (other.transform.name != waypoints[waypointIndex].transform.name)
        {
            return;
        }

        waypointIndex += goForward;

        if (waypointIndex >= waypoints.Count - 1)
        {
            goForward = -1;
        }
        else if (waypointIndex < 1)
        {
            goForward = 1;
        }

        SetDestinationGivenWaypoint(waypoints[waypointIndex].transform.position);
    }

    private void SetDestinationGivenWaypoint(Vector3 waypoint)
    {
        Vector3 currentPos = transform.position;

        navMeshAgent.SetDestination(new Vector3(waypoint.x, waypoint.y, currentPos.z));
    }

    private void ManageVisionCollider(Collider2D other)
    {
        if (other.transform.name.Equals("Player"))
        {
            Debug.Log("Encontrou JOGADOR!");
        }
    }
}
