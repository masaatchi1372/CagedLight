using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject target;
    bool facingRight;
    Rigidbody2D rigidBody;

    // Attribute specific
    [SerializeField] EnemyBaseAttributes attributes;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        //bellow is the movement 
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, attributes.speed * Time.deltaTime);

        if (target.transform.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
        if (target.transform.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        Vector2 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }
}












