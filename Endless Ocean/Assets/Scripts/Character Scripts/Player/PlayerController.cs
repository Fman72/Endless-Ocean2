﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlayerController : CharacterSuper
{
    // Player's second weapon
    public Weapon weaponSecond;

    // Player HUD elements
    public Image playerHealthBar;
    public Image playerEnergyBar;

    //Other game objects.
    public CameraController playerCameraController;

    //Interfaces that separate the player controller from weapons and utility items (eg: grapple)
    public Grapple grapple;

    public int totalExperience;
    public int totalTreasure;

    //Reference to the items menu UI element.
    public GameObject itemsMenu;

    // ENERGY RELATED VARIABLES
    protected int energy;
    protected int maxEnergy;
    // How often energy regens
    protected float energyRegenSpeed = 0.1f;
    // How much energy regens each tick
    protected int regenAmount = 2;
    // Pentalty timer for when energy reaches 0
    protected int penaltyTimer = 0;
    // When pentaltyTimer reaches this value, penalty period is over
    protected int pentaltyLength = 25;


    // Use this for initialization
    new void Start()
    {
        base.Start();

        // Assign objects that damage this character upon collision
        base.fears = "EnemyWeapon";

        // Set health
        this.health = 100;
        this.maxHealth = 100;

        // Set energy and attack variables
        this.energy = 100;
        this.maxEnergy = 100;
        this.nextMelee = 0.0f;
        this.itemsMenu.SetActive(false);
        //Hide Menu at start.
        //COMMENTED THIS OUT FOR NOW, WAS BREAKING GAME
        //this.itemsMenu.SetActive(false);

        //this.utilityItem = this.utilityItem.GetComponent<UtilityItem>();

        // Get weapon mount location
        weaponMount = GameObject.Find("Player/Armature/Root/Torso/Chest/Upper_Arm_R/Lower_Arm_R/Hand_R/WeaponMount");

        this.weapon = this.weaponObject.GetComponentInChildren<Club>();
        // TODO: Having trouble added the string path below in the load to each weapon, like club or pistol. This is a refernece to the mesh model
        // The problem is if you put a shared variable in the super class weapon, that can only be assigned AFTER the object is instantiated
        // The line below needs the path to the model to instantiate it. Will look into this later, but if you guys have ideas, feel free

        // This is the code that will go in the method where we drag/drop our weapons in, btw, Fraser.
        GameObject weapon = Instantiate(Resources.Load("Prefabs/Weapons/Medieval Sward"),weaponMount.transform.position,weaponMount.transform.rotation) as GameObject;
        weapon.transform.parent = weaponMount.transform;


        //this.playerGrapple = this.AddComponent<Grapple>();
        this.facingRight = true;
        this.usingItem = false;

        // Energy regeneration, invoke repeating method
        InvokeRepeating("RegenEnergy", energyRegenSpeed, energyRegenSpeed);
    }

    /// <summary>
    /// Used to regenerate the player's energy reserves over time
    /// </summary>
    private void RegenEnergy()
    {
        if(this.energy == 0 && this.penaltyTimer < this.pentaltyLength)
        {
            this.penaltyTimer += 1;
        }
        else if (this.energy < 100)
        {
            this.penaltyTimer = 0;
            energy += regenAmount;
            playerEnergyBar.fillAmount = (float)energy / (float)maxEnergy;

        }
    }

    /// <summary>
    /// Runs before every frame. Performs physics calculates for game objects to be displayed when the next frame is rendered and updates the animator.
    /// </summary>
    void FixedUpdate()
    {
        if(this.health <= 0)
        {
            die();
        }

        // Button events
        if (Input.GetButtonDown("OpenItemsMenu"))
        {
            itemsMenu.SetActive(!itemsMenu.activeInHierarchy);
        }
        // Weapon event
        if (Input.GetAxis("Fire 1") > 0 && nextMelee < Time.time)
        {
            nextMelee = Time.time + weapon.getAttackSpeed();
            if (energy > 0)
            {
                if ( energy - weapon.energyCost < 0)
                {
                    energy = 0;
                }
                else
                {
                    energy -= weapon.energyCost;
                }

                this.playerEnergyBar.fillAmount = (float)this.energy / (float)this.maxEnergy;
                this.animator.SetTrigger("MeleeAttackTrigger");
                this.weapon.attack(this.attack, playerCameraController.getMouseLocationInWorldCoordinates());
            }
        }
        if (!grapple.grappling || (grapple.grappling && onGround))
        {
            float horizontalMove = Input.GetAxis("Horizontal");
            float verticalMove = Input.GetAxis("Vertical");

            //IF NOT USING ITEM
            //CODE FOR JUMPING.
            if (onGround && (Input.GetAxis("Jump") > 0))
            {
                this.onGround = false;
                this.animator.SetBool("grounded", this.onGround);
                this.rigidbody.AddForce(new Vector3(0, jumpHeight, 0));
            }
            checkIfOnGround();
            this.animator.SetBool("grounded", this.onGround);

            //CODE FOR MOVING.
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            //If the user if moving apply movement force to player.
            if (horizontalMove != 0)
            {
                rigidbody.velocity = new Vector3(horizontalMove * movementSpeed, this.rigidbody.velocity.y, 0);
            }

            //If the game object starts moving left and is facing right turn the object around.
            if (horizontalMove > 0 && !facingRight)
            {
                this.turnAround();
            }
            //If the game object starts moving right and is facing left turn the object around.
            if (horizontalMove < 0 && facingRight)
            {
                this.turnAround();
            }
        }
    }

    /// <summary>
    /// What happens when player collides with certain objects
    /// </summary>
    /// <param name="col">GameObject involved in collision</param>
    new void OnTriggerEnter(Collider col)
    {
        base.OnTriggerEnter(col);

        // When player is hit with an enemy weapon
        if (col.gameObject.tag == "EnemyWeapon")
        {
            Debug.Log("Player: My health is now " + this.health + "and I took " + "some" + "damage");
        }
    }

    protected override void updateHealthBar()
    {
        // Update health bar with new health
        playerHealthBar.fillAmount = (float)this.health / (float)this.maxHealth;
    }

    public override void die()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
