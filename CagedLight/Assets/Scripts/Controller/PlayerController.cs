using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StrengthType
{
    DmgGain
}

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Model related attributes
    Rigidbody2D playerBody;
    BoxCollider2D collider;
    Vector2 position;
    bool facingRight = true;

    // Character related attributes
    [SerializeField] BaseAttributes attributes;

    CharacterAnimation animation;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        playerBody.gravityScale = 0;
        position = new Vector2();
        animation = GetComponent<CharacterAnimation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        position.x = Input.GetAxis("Horizontal");
        position.y = Input.GetAxis("Vertical");

        animation.horizontal = position.x;

        if (position.x > 0 && !facingRight)
        {
            Flip();
        }

        if (position.x < 0 && facingRight)
        {
            Flip();
        }

        /*
        if (Input.GetKey(KeyCode.Alpha1))
        {
            anim.SetTrigger("Hit");
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            anim.SetTrigger("Dead");
        }
        */

    }

    private void FixedUpdate()
    {
        playerBody.MovePosition(playerBody.position + position * attributes.speed * Time.fixedDeltaTime);
    }

    void Flip()
    {
        Vector2 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }
}
