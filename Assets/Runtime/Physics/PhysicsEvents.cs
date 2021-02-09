using System;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsEvents : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<Collision2D> OnCollisionEnter = new UnityEvent<Collision2D>();

    [SerializeField]
    private UnityEvent<Collision2D> OnCollisionStay = new UnityEvent<Collision2D>();

    [SerializeField]
    private UnityEvent<Collision2D> OnCollisionExit = new UnityEvent<Collision2D>();

    [SerializeField]
    private UnityEvent<Collider2D> OnTriggerEnter = new UnityEvent<Collider2D>();

    [SerializeField]
    private UnityEvent<Collider2D> OnTriggerStay = new UnityEvent<Collider2D>();

    [SerializeField]
    private UnityEvent<Collider2D> OnTriggerExit = new UnityEvent<Collider2D>();

    void OnCollisionEnter2D(Collision2D col)
    {
        OnCollisionEnter.Invoke(col);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        OnCollisionStay.Invoke(col);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        OnCollisionExit.Invoke(col);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        OnTriggerEnter.Invoke(col);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        OnTriggerStay.Invoke(col);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        OnTriggerExit.Invoke(col);
    }
}
