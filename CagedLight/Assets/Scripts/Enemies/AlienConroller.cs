using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienConroller : MonoBehaviour
{
    [Header("Bat Controller")]
    [Space(5)]
    public int health = 1;
    private float timeToDie = 2;
    private bool shouldDie = false;
    private float timeToIdle = 0;


    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        timeToIdle = Random.Range(2,5);
    }

    private void Update() {
        if (shouldDie)
        {
            timeToDie -= Time.deltaTime;
        } else
        {
            timeToIdle -= Time.deltaTime;
        }
        if (timeToDie <= 0)
        {
            Destroy(gameObject);
        }
        if (timeToIdle <= 0)
        { 
            PlayAnimation("alien-idle");
            timeToIdle = Random.Range(2,5);
        }
    }

    public void PlayAnimation(string animation)
    {
        animator.Play(animation);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // this bat is dead
            health = 0;
            GetComponent<Collider2D>().enabled = false;

            shouldDie = true;
            PlayAnimation("alien-die");

            // let the event manager knows that one enemy has died
            EventManager.OnEnemyDie();

            // TO DO PLAY DEATH ANIMATION
        }
    }
}
