using RTS;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class SoldierUnit : Unit
{
    public float range, buildingRange;
    public int damage;
    public float attackSpeed;
    public float attackDuration = 2;
    private float timerAttack, timerAttackDuration;

    private bool onRange;

    protected override void Start()
    {
        base.Start();
        timerAttack = attackSpeed;
        timerAttackDuration = 0;
    }
    protected override void Update()
    {
        base.Update();


        if (target)
        {
            if (timerAttackDuration <= 0)
            {
                if (!MovementToTarget())
                {
                    if (timerAttack >= attackSpeed)
                    {
                        timerAttack = 0;
                        Attack();
                        timerAttackDuration = attackDuration;
                    }
                }
            }
            else
            {
                timerAttackDuration -= Time.deltaTime;
            }
        }


        timerAttack += Time.deltaTime;
    }


    public override void RightMouseClick(GameObject hitObject, Vector3 hitpoint, Player controller, int index)
    {

        if (controller.username == this.GetPlayerName())
        {
            if (hitObject)
            {
                WorldObject hit = hitObject.GetComponentInParent<WorldObject>();
                if (hit && hit.GetPlayerName() != this.GetPlayerName())
                {
                    this.target = hit.gameObject;
                }
                else
                {
                    this.target = null;
                    base.RightMouseClick(hitObject, hitpoint, controller, index);
                }
            }
        }
        else
        {
            base.RightMouseClick(hitObject, hitpoint, controller, index);
        }

    }

    protected bool MovementToTarget() 
    {
        
        if ((target.transform.position - this.transform.position).magnitude >= ((target.GetComponent<WorldObject>() is Building)? buildingRange : range))
        {
            FindPathTo(target.transform.position);
            return true;
        }
        else
        {
            FindPathTo(this.transform.position);
            return false;
        }
    }

    protected void Attack()
    {
        WorldObject enemyUnit = target.GetComponent<WorldObject>();
        if (enemyUnit && enemyUnit.player != this.player) {
            animator.SetTrigger("Attack");
            enemyUnit.TakeDamage(this.damage);
        }
    }
}
