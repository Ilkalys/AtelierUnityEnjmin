using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    public SoldierUnit unit;
    public Animator anim;
    public float timeBeforeMovement;
    public float countDown;

    public void Start()
    {
        countDown = timeBeforeMovement;
        anim.SetTrigger("Zombie");
    }
    // Update is called once per frame
    void Update()
    {
        if(countDown <= 0) {
            if (unit.target == null)
            {
                GameObject[] spawns =  GameObject.FindGameObjectsWithTag("FirstHome");
                if(spawns.Length > 0)
                unit.target = spawns[Random.Range(0, spawns.Length)];
            }
        }
        else
        {
            countDown -= Time.deltaTime;
            unit.target = null;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (!unit.target || unit.target.gameObject.tag == "FirstHome")
        {
            WorldObject objectTouched = collision.gameObject.GetComponentInParent<WorldObject>();
            if (objectTouched && objectTouched.gameObject != unit.gameObject && unit.player.username != objectTouched.GetPlayerName())
            {

                unit.target = objectTouched.gameObject;
            }
        }
    }
}
