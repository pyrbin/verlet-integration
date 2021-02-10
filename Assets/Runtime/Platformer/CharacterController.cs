using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    public float JumpImpulse = 7f;
    public float Speed = 2f;

    // Default Contact filter
    public ContactFilter2D GroundedFilter = new ContactFilter2D
    {
        useNormalAngle = true,
        maxNormalAngle = 135,
        minNormalAngle = 45
    };

    private Rigidbody2D Rigidbody;
    private bool ShouldJump;
    private float Movement;

    public bool IsGrounded => Rigidbody.IsTouching(GroundedFilter);

    public float2 Velocity => Rigidbody.velocity;
    public bool IsFalling => !IsGrounded && Velocity.y < 0;

    [HideInInspector]
    public int MovementInput = 0;

    public void Jump() => ShouldJump = true;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.angularDrag = 0f;
        Rigidbody.interpolation = RigidbodyInterpolation2D.None;
        Rigidbody.freezeRotation = true;
    }

    void Update()
    {
        Movement = MovementInput * Speed;
    }

    void FixedUpdate()
    {
        // Handle jump.
        if (ShouldJump && IsGrounded)
            Rigidbody.AddForce(math.up().xy * JumpImpulse, ForceMode2D.Impulse);

        // Set sideways velocity.
        Rigidbody.velocity = new float2(Movement, Rigidbody.velocity.y);

        // Reset movement.
        ShouldJump = false;
        Movement = 0f;
    }
}
