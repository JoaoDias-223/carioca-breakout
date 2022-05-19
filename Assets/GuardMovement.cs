using System;
using static System.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float maxVelocity = 5f;
    public Vector2 movement;
    
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

    private void Start ()
    {
        waypoints = new List<Transform>();
        foreach (Transform waypoint in waypointsParent)
        {
            waypoints.Add(waypoint);
        }
        
        movement = waypoints[0].position;
    }

    private void Update()
    {
        animator.SetFloat(ANIM_HORIZONTAL_PARAM, (float)Clamp(movement.x - transform.position.x, -1.0, 1.0));
        animator.SetFloat(ANIM_VERTICAL_PARAM, (float)Clamp(movement.y - transform.position.y, -1.0, 1.0));
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
        if (other.transform.name == waypoints[waypointIndex].transform.name)
        {
            waypointIndex += goForward;

            if (waypointIndex >= waypoints.Count - 1)
            {
                goForward = -1;
            }
            else if (waypointIndex < 1)
            {
                goForward = 1;
            }
        }
    }

    private void ManageVisionCollider(Collider2D other)
    {
        if (other.transform.name.Equals("Player"))
        {
            Debug.Log("Encontrou JOGADOR!");
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

        Vector2 newPosition = Vector2.MoveTowards(transform.position, waypoint, Time.deltaTime * movementSpeed);
        rb2D.MovePosition(newPosition);
        rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, maxVelocity);
    }
}
