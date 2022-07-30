using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatConroller : MonoBehaviour
{
    [Header("Bat Controller")]
    [Space(5)]
    public int health = 1;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // this bat is dead
            health = 0;
            GetComponent<Collider2D>().enabled = false;

            Destroy(gameObject);

            // let the event manager knows that one enemy has died
            EventManager.OnEnemyDie();

            // TO DO PLAY DEATH ANIMATION
        }
    }
}
