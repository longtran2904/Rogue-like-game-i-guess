﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour, IPooledObject
{
    public float speed;
    public float timer;
    protected Rigidbody2D rb;

    protected int damage;
    protected float knockbackForce;
    protected float knockbackTime;
    protected bool isCritical;
    protected bool isEnemy;
    public GameObject hitEffect;

    // State for enemy
    private State state;

    public bool canTouchGround; // can go through wall and grounds
    public bool canTouchPlayer; // go through player and still damage him
    
    public virtual void OnObjectSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        StartCoroutine(GameUtils.Deactive(gameObject, timer));
    }

    public void Init(int damage, float knockbackForce, float knockbackTime, bool isEnemy, bool isCritical)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.knockbackTime = knockbackTime;
        this.isEnemy = isEnemy;
        this.isCritical = isCritical;
    }

    public void Init(int damage, float knockbackForce, float knockbackTime, State state)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.knockbackTime = knockbackTime;
        this.state = state;
    }

    public void SetVelocity(float speed)
    {
        rb.velocity = speed * transform.right;
    }

    public void SetVelocity(float speed, float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
        SetVelocity(speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitCollider(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HitCollider(collision.collider);
    }

    protected virtual void HitCollider(Collider2D collision)
    {
        if (hitEffect) Instantiate(hitEffect, transform.position, transform.rotation);

        if (collision.CompareTag("Enemy") && !isEnemy)
        {
            Hit(collision, x => {
                Enemy enemy = x.GetComponent<Enemy>();
                Vector2 knockbackForce = (-transform.position + enemy.transform.position).normalized * this.knockbackForce;
                enemy.Hurt(damage);
                if (state != null)
                    StateManager.AddStateToEnemy(enemy, state);
            }, true);
        }
        else if (collision.CompareTag("Player") && isEnemy)
        {
            Hit(collision, x => x.GetComponent<Player>().Hurt(damage), false, canTouchPlayer);
        }
        else if (collision.CompareTag("Boss") && !isEnemy)
        {
            Hit(collision, x => x.GetComponent<Boss>().Hurt(damage), true);
        }
        if (collision.CompareTag("Ground") && !canTouchGround)
        {
            AudioManager.instance.PlaySfx("HitWall");
            gameObject.SetActive(false);
        }
    }

    void Hit(Collider2D collision, Action<Collider2D> hurtDelegate, bool spawnPopup, bool setActive = false)
    {
        if (spawnPopup) DamagePopup.Create(collision.transform.position, damage, isCritical);
        hurtDelegate?.Invoke(collision);
        gameObject.SetActive(setActive);
    }
}
