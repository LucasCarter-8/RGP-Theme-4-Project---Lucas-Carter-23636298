using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    private float duration;
    // Start is called before the first frame update
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Move(float speed, float duration, Vector2 shootDirection)
    {
        this.duration = duration;
        rigidbody.velocity = new Vector2(shootDirection.x * speed, shootDirection.y * speed);
        Destroy(gameObject, duration);
    }
}
