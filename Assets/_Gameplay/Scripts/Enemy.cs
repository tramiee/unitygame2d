using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float attackRange;
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject attackArea;

    private IState currentState;

    private bool isRight = true;

    public Character target;
    public Character Target => target;

    // Update is called once per frame
    void Update()
    {
        if(currentState != null && !IsDead)
        {
            currentState.OnExecute(this);
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        ChangeState(new IdleState());
        DeActiveAttack();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        Destroy(healhBar.gameObject);
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        ChangeState(null);
        base.OnDeath();
    }

    public void ChangeState(IState newState)
    {
        if(currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        if(currentState != null)
        {
            currentState.OnEnter(this);
        }

    }
    public void Moving()
    {
        ChangeAnim("Run");
        rb.velocity = transform.right * speed;
    }

    public void StopMoving()
    {
        ChangeAnim("Idle");
        rb.velocity = Vector2.zero;
    }

    public void Attack()
    {
        ChangeAnim("Attack");
        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 0.5f);
    }

    internal void SetTarget(Character character)
    {
        this.target = character;
        if(IsTargetInRange())
        {
            ChangeState(new AttackState());
        }else if (Target != null)
        {
            ChangeState(new PatrolState());
        }
        else
        {
            ChangeState(new IdleState());
        }
    }

    public bool IsTargetInRange()
    {
        if(target != null && Vector2.Distance(target.transform.position, transform.position) <= attackRange)
        {
            return true;
        }
        else
        {
            return false;
        } 
    }

    public void ChangeDirection(bool isRight)
    {
        this.isRight = isRight;
        transform.rotation = Quaternion.Euler(new Vector3(0, isRight ? 0 : 180, 0));
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeActiveAttack()
    {
        attackArea.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "EnemyWall")
        {
            Debug.Log("Wall");
            ChangeDirection(!isRight);
        }
    }
}
