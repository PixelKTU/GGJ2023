using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum EnemyState
{
    Idle,
    Attacking,
    Walking,
    Dying
};

public class Enemy : MonoBehaviour
{
    public float imaginaryHealth { get; private set; }
    public float health { get; private set; }
    public bool dead { get; private set; }


    [SerializeField] protected EnemyState state;

    [SerializeField] protected float maxHealth;
    [SerializeField] protected EnemyType enemyType;

    [Header("Attacks")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float attackDistance = 3;
    [SerializeField] protected float attackCooldown = 1;

    [Header("AI")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;



    private Transform target;
    private float currentAttackTime;

    protected virtual void Start()
    {
        Attack(MainTree.Instance.GetTransform());
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.GameState == GameState.Defeat || GameManager.Instance.GameState == GameState.Won) return;

        switch (state)
        {
            case EnemyState.Idle:
                Attack(MainTree.Instance.GetTransform());
                break;
            case EnemyState.Attacking:

                currentAttackTime += Time.deltaTime;

                transform.LookAt(target);

                if (currentAttackTime > attackCooldown)
                {
                    if (CanAttack())
                    {
                        currentAttackTime = 0;
                        animator.SetTrigger("Attack");
                        animator.SetBool("IsWalking", false);
                        MainTree.Instance.TakeDamage(damage);
                    }
                    else
                    {
                        if (target != null)
                        {
                            WalkTo(target.position);
                        }
                        else
                        {
                            Attack(MainTree.Instance.GetTransform());
                        }
                    }
                }
                break;
            case EnemyState.Walking:

                if (agent.isStopped)
                {
                    animator.SetBool("IsWalking", false);
                    state = EnemyState.Idle;
                }
                else if (!agent.isStopped && !animator.GetBool("IsWalking"))
                {
                    animator.SetBool("IsWalking", true);
                }

                if (CanAttack())
                {
                    Attack(MainTree.Instance.GetTransform());
                }
                break;
            case EnemyState.Dying:

                break;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!dead)
        {
            health -= damage;

            if (health <= 0)
            {
                Death();
            }
        }
    }

    void Death()
    {
        dead = true;

        state = EnemyState.Dying;
        EnemySpawningSystem.Instance.RemoveEnemy(gameObject, enemyType);
    }

    public void TakeImaginaryDamage(float damage)
    {
        imaginaryHealth -= damage;
    }

    public void ResetData()
    {
        health = maxHealth;
        imaginaryHealth = maxHealth;
        dead = false;
        state = EnemyState.Idle;
        target = null;
        currentAttackTime = attackCooldown;
    }

    protected virtual void WalkTo(Vector3 position)
    {
        state = EnemyState.Walking;
        animator.SetBool("IsWalking", true);
        agent.SetDestination(position);
    }

    protected virtual void Attack(Transform target)
    {
        this.target = target;
        state = EnemyState.Attacking;

        if (!CanAttack())
        {
            WalkTo(target.position);
        }
    }

    protected bool CanAttack()
    {
        return Vector3.Distance(target.position, transform.position) < attackDistance;
    }

    /// TODO
    /// Animation controls
    /// Attack animations and code
    /// Movement
}
