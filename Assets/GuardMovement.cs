using System;
using static System.Math;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Animations;
using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float maxVelocity = 5f;
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


    private void Start ()
    {
        waypoints = new List<Transform>();
        foreach (Transform waypoint in waypointsParent)
        {
            waypoints.Add(waypoint);
        }

        movement = Vector2.zero;
        direction = Vector2.zero;
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

    private void RotateColliderBasedOnDirection()
    {
        Vector3 rotation = visionCollider.rotation.eulerAngles;
        visionCollider.rotation = Quaternion.Euler(rotation.x, rotation.y, GetZetaValueBasedOnAnimationState());
    }

    private float GetZetaValueBasedOnAnimationClip()
    {
        float z = 0;
        AnimatorClipInfo[] currentAnim = animator.GetCurrentAnimatorClipInfo(0);

        Debug.Log(currentAnim[0].clip.name);

        z = currentAnim[0].clip.name switch
        {
            "Walk_Down" => 0,
            "Walk_Up" => 180,
            "Walk_Right" => 90,
            "Walk_Left" => 270,
            _ => z
        };

        return z;
    }

    private float GetZetaValueBasedOnAnimationState()
    {
        float z = 0;
        AnimatorController animatorController = GetAnimatorController();

        if (animatorController == null)
        {
            Debug.Log("Animator controller is Null");
            return z;
        }
        AnimatorStateMachine animatorStateMachine = animatorController.layers[0].stateMachine;
        AnimatorState movementState = animatorStateMachine.states[1].state;
        Motion blendTree = (BlendTree) movementState.motion;
        blendTree.

        if (movementState.name != "Movement")
        {
            Debug.Log("WRONG STATE");
            return z;
        }

        Debug.LogFormat("Current state: {0}", movementState.name);
        Debug.LogFormat("Current motion: {0}", blendTree.name);

        z = blendTree.name switch
        {
            "Walk_Down" => 0,
            "Walk_Up" => 180,
            "Walk_Right" => 90,
            "Walk_Left" => 270,
            _ => z
        };

        return z;
    }

    [CanBeNull]
    private AnimatorController GetAnimatorController()
    {
        RuntimeAnimatorController runtimeController = animator.runtimeAnimatorController;
        if (runtimeController == null)
        {
            Debug.LogErrorFormat("RuntimeAnimatorController must not be null.");
            return null;
        }
        AnimatorController assetAnimatorController =UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(runtimeController));
        if (assetAnimatorController == null)
        {
            Debug.LogErrorFormat("AnimatorController must not be null.");
            return null;
        }

        return assetAnimatorController;
    }
}
