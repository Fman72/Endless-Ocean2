﻿using UnityEngine;
using System.Collections;
using System;

public class EnemyAI : NPCBehaviour
{

    //enemy variables
    public float attackRange;
    public float detectRange;

    public Transform target;

	// Use this for initialization
	new void Start () {
        this.health = 100;
        this.maxHealth = 100;

        base.Start();

        // Assign objects that damage this character upon collision
        base.fears = "Player";


        equipWeapon(Club.modelPathLocal, weaponMounts.Primary, "EnemyWeapon");
        equipWeapon(Pistol.modelPathLocal, weaponMounts.Secondary, "EnemyWeapon");

        // Set primary/active player weapon as the one stored in the first slot
        weapon = meeleMount.Weapon;
    }
	
	// Update is called once per frame
	new void Update () {
	
	}

    new void FixedUpdate()
    {
        base.FixedUpdate();
        //transform.LookAt(target);

        //check if player is in range
        if (Vector3.Distance(transform.position, target.position) <= detectRange)
        {
            
            if (Vector3.Distance(transform.position, target.position) >= attackRange)
            {
                pathToLocation(target.position);
            }else
            {
                //attackTarget(target);
            }

        }
    }

    protected override void updateHealthBar()
    {
        healthBar.fillAmount = (float)this.health / (float)this.maxHealth;
    }
}
