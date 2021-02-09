using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DynamicGravity : MonoBehaviour
{
    public float AgainstGravityScale = 1f;
    public float FallGravityScale = 2f;

    private Rigidbody2D Rigidbody;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Calculate our direction relative to the global gravity.
        var direction = math.dot(Rigidbody.velocity, Physics2D.gravity);
        // Set the gravity scale accordingly.
        Rigidbody.gravityScale = direction > 0f ? FallGravityScale : AgainstGravityScale;
    }
}
