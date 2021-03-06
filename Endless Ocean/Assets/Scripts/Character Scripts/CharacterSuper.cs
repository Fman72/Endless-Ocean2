﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Weapon mount struct, used for managing weapons
/// </summary>
public struct weaponMount
{
    GameObject mountPoint;
    Weapon weapon;

    /// <summary>
    /// the physical mount point the mount point refers to
    /// </summary>
    public GameObject MountPoint
    {
        get
        {
            return mountPoint;
        }
        set
        {
            mountPoint = value;
        }
    }

    /// <summary>
    /// currently attached weapon
    /// </summary>
    public Weapon Weapon
    {
        get
        {
            return weapon;
        }
        set
        {
            weapon = value;
        }
    }

    /// <summary>
    /// converts a unity gameOject into a mountable / usable weapon
    /// </summary>
    public GameObject WeaponFromGameObject
    {
        set
        {
            value.transform.parent = mountPoint.transform;
            value.transform.localPosition = Vector3.zero;
            value.transform.localRotation = Quaternion.identity;

            weapon = value.GetComponent<Weapon>();
        }
    }
    
    /// <summary>
    /// tests if a weapon is currently loaded on the weapon mount
    /// </summary>
    /// <returns>boolean, is a weapon loaded</returns>
    public bool weaponLoaded()
    {
        if (weapon == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

/// <summary>
/// Character super class that manages the featues of all characters
/// </summary>
public abstract class CharacterSuper : MonoBehaviour
{
    // Character variables involved in leveling
    public float movementSpeed;
    // Additional damage player does ontop of weapon damage
    public int attack;
    // Determines how much health the player has
    public int stamina;
    // Current Armour Level
    [SerializeField]
    protected int armour;

    //The height the player will jump when the user makes them jump.
    public float jumpHeight;
    protected Vector3 velocity;

    //Stun Variables 
    [SerializeField]
    protected float recovery;
    protected float recoveryTimer;

    // Determines energy
    public int vigor;

    // Current level
    public int currentLevel;

    //modifier for how likely a character is to get good weapons - increase for reforger, decrease for lowly mobs
    public float luck = 1;

    // Highest possible health
    public int maxHealth;
    // current health
    public int health;

    #region movement variables
    //Movement Variables 

    protected bool facingRight;
    [HideInInspector]
    public bool enableMove;
    public bool enableJump;
    [SerializeField]
    public LayerMask groundLayerMask;
    #endregion

    #region mesh component variables
    //Character Mesh Components.
    new public Rigidbody rigidbody;
    protected Animator animator;

    public GameObject primaryWeaponSlot;
    public GameObject secondaryWeaponSlot;

    #endregion

    #region Jumping Variables
    //VARIABLES USED FOR JUMPING
    //Bool that indicates whether or not the gameobject is touching the ground.
    public bool onGround = true;
    //An array that contains the collision objects that the circle collides with when jumping.
    protected Collider[] groundCollisions;
    //The radius of the cirle to check for objects in the ground layer when jumping.
    protected float groundCheckRadius = 0.5f;
    //A layer mask that filters out game objects that are not in the ground layer.
    public LayerMask canWalkOnLayerMask;
    //The transform of a gameobject used to position the cicle used to determine if the game object is on the ground when jumping.
    public Transform groundCheck;

    #endregion

    #region Attacking Variables

    #region weapon mounts and equiping

    public Weapon weapon;

    // Mount where weapon is attached to
    public GameObject weaponMount;
    public weaponMount primaryMount;
    public weaponMount secondaryMount;

    public enum weaponMounts
    {
        Primary, Secondary
    };

    public weaponMounts activeWeaponType = weaponMounts.Primary;
    #endregion



    //Objects used for getting interface references.

    protected float nextMelee;

    #endregion

    #region Tools

    #endregion


    // Those objects which, upon collision, can damage this instance of character
    protected string fears = "Nothing";

    //A boolean indicating if the player is using an item that effects their movement.
    protected bool usingItem;

    //Character collider
    public Collider col;

    public Material damageMaterial;
    public Material bodyMaterial;

    public bool flashing = false;

    // Use this for initialization
    protected void Start()
    {
        //Retrieving components from the game objects this script is attatched to.
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.animator = this.GetComponent<Animator>();
        this.facingRight = true;
        this.enableMove = true;
        this.enableJump = true;
        //this.replaceMat = this.transform.Find("Body").GetComponent<SkinnedMeshRenderer>().material;

        // Get weapon mount location so that we can easily attach weapons to them
        secondaryMount.MountPoint = primaryWeaponSlot;
        primaryMount.MountPoint = secondaryWeaponSlot;
        if (primaryMount.weaponLoaded())
        {
            weapon = primaryMount.Weapon;
        }
        else if (secondaryMount.weaponLoaded())
        {
            weapon = secondaryMount.Weapon;
        }
    }

    // Update is called once per frame
    protected void Update()
    {

    }

    // Update that is called after a fixed time
    protected void FixedUpdate()
    {
        if (recovery == 0)
        {
            recovery = 1;
        }

        if (recoveryTimer < 0)
        {
            recoveryTimer = 0;
        }
        else if (recoveryTimer > 0)
        {
            recoveryTimer -= Time.deltaTime * recovery;
        }
    }

    // <summary>
    // This function flips the game object when the user turns it around my moving it.
    // </summary>
    protected void turnAround()
    {
        this.facingRight = !facingRight;
        Vector3 reversescale = transform.localScale;
        reversescale.z *= -1;
        transform.localScale = reversescale;
    }

    /// <summary>
    /// Moves the character and activates the character walking animation
    /// </summary>
    /// <param name="move">float refering to the level of motion on the X axis (1 being right, -1 being to the left)</param>
    protected virtual void moveCharacter(float move)
    {
        //Stun Timer
        if (recoveryTimer != 0)
        {
            Debug.Log("Stunned");
            move = 0;
        }

        //If the user if moving apply movement force to player.
        if (move != 0 && this.enableMove)
        {
            //movement while in air
            if (!onGround)
            {
                // if trying to move left when maximum left speed is reached
                if (velocity.x <= -movementSpeed && move == -1)
                {
                }
                // if trying to move left when maximum right speed is reached
                else if (velocity.x >= movementSpeed && move == 1)
                {
                }
                // move right
                else if (move == 1)
                {
                    velocity.x += (movementSpeed / 8);
                }
                // move left
                else if (move == -1)
                {
                    velocity.x -= (movementSpeed / 8);
                }

                rigidbody.velocity = new Vector3(velocity.x, this.rigidbody.velocity.y, 0);

            }
            else
            {
                rigidbody.velocity = new Vector3(move * movementSpeed, this.rigidbody.velocity.y, 0);
            }
        }

        setDirection(move);

        animator.SetFloat("Speed", Mathf.Abs(move));

        this.stopHangingOnWall();
    }

    /// <summary>
    /// Set the direction the character is currently facing
    /// </summary>
    /// <param name="move">float refering to the level of motion on the X axis (1 being right, -1 being to the left)</param>
    protected void setDirection(float move)
    {
        //If the game object starts moving left and is facing right turn the object around.
        if (move > 0 && !facingRight)
        {
            this.turnAround();
        }
        //If the game object starts moving right and is facing left turn the object around.
        if (move < 0 && facingRight)
        {
            this.turnAround();
        }
    }

    /// <summary>
    /// Function that updates the onGround variable.
    /// </summary>
    protected void checkIfOnGround()
    {
        this.onGround = groundCollisionCheck(groundCheck.position);
    }

    /// <summary>
    /// Runs the code for a ground check at the given location
    /// </summary>
    /// <param name="position">Location to use as center of ground check </param>
    /// <returns></returns>
    protected bool groundCollisionCheck(Vector3 position)
    {
        groundCollisions = Physics.OverlapSphere(position, this.groundCheckRadius, this.canWalkOnLayerMask);
        return (groundCollisions.Length > 0);
    }

    /// <summary>
    /// What happens when character collides with certain objects
    /// </summary>
    /// <param name="col">GameObject involved in collision</param>
    protected virtual void OnTriggerEnter(Collider col)
    {
        Animator animController = col.transform.root.GetComponent<Animator>();
        int attackState = Animator.StringToHash("Attack Layer.Melee Attack");

        // When character collides DeathFromfalling gameObject (fall down hole)
        if (col.gameObject.tag == "DeathCollider")
        {
            health = 0;
        }
        // If the thing hitting the character is a projectile
        else if (col.gameObject.tag == fears + "Projectile")
        {
            if (!flashing)
            {
                Bullet collidingBullet = col.gameObject.GetComponent<Bullet>();
                if (collidingBullet != null)
                {
                    int damage = collidingBullet.getDamage();
                    int knockBack = collidingBullet.getKnockBack();
                    float stun = collidingBullet.getStun();

                    this.takeDamage(damage, col.gameObject.GetComponentInParent<Rigidbody>().position, knockBack);
                    recoveryTimer = stun;
                }
                if (col.gameObject.GetComponent<Bullet>() != null)
                {
                    Destroy(col.gameObject.GetComponentInParent<Rigidbody>().gameObject);
                }
                else if (col.gameObject.GetComponent<FloatyAI>() != null)
                {
                    int damage = col.gameObject.GetComponent<FloatyAI>().attack;
                    int knockBack = col.gameObject.GetComponent<FloatyAI>().knockBack;
                    float stun = col.gameObject.GetComponent<FloatyAI>().stun;

                    this.takeDamage(damage, col.gameObject.GetComponentInParent<Rigidbody>().position, knockBack);
                    recoveryTimer = stun;
                }
            }
            if (flashing)
            {
                if (col.gameObject.GetComponent<Bullet>() != null)
                {
                    Destroy(col.gameObject.GetComponentInParent<Rigidbody>().gameObject);
                }
            }
        }
        // When character is hit with an enemy weapon
        else if (col.gameObject.tag == fears + "Weapon")
        {
            if (!flashing)
            {
                int damage = col.transform.parent.gameObject.GetComponent<Weapon>().getDamage() + col.transform.root.GetComponent<CharacterSuper>().attack;
                float stun = col.transform.parent.gameObject.GetComponent<Weapon>().getStun();
                int knockBack = col.transform.parent.gameObject.GetComponent<Weapon>().getKnockBack();
                bool collisionHandled = col.transform.parent.gameObject.GetComponent<Weapon>().collisonHandled;


                if (animController.GetCurrentAnimatorStateInfo(1).IsName("Melee Attack") && !collisionHandled)
                {
                    this.takeDamage(damage, col.gameObject.GetComponentInParent<Rigidbody>().position, knockBack);
                    recoveryTimer = stun;

                    col.transform.parent.gameObject.GetComponent<Weapon>().collisonHandled = true;
                }
            }
        }
        updateHealthBar();
    }

    public abstract void updateHealthBar();
    public abstract void die();

    /// <summary>
    /// Cause character to take damage and apply knockback
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    /// <param name="source">Position/direction of the damage source</param>
    /// <param name="knockBack">Amount of knock back stored within damage gameObject</param>
    protected virtual void takeDamage(int damage, Vector3 source, int knockBack)
    {

        this.health -= (armour > 0) ? (int)(damage - ((damage * 0.8) * (armour / 100))) : damage;

        //Debug.Log("I took "+damage+" damage, now my health is "+this.health +"out of a possible "+maxHealth);

        Vector3 direction = transform.position - source;

        direction.Normalize();
        direction.y += 0.4f;

        this.rigidbody.AddForce(direction * knockBack);

        if (this.health <= 0)
        {
            die();
        }

        StartCoroutine(this.flashOnDamageTaken());
    }

    /// <summary>
    /// Used to equip enemies and player with weapons
    /// </summary>
    /// <param name="weaponGameObject">The weapon gameObject to equip.</param>
    /// <param name="mount">Mount point for weapon to be attached (slot1 or slot2)</param>
    /// <param name="tag">Tag for weapon, determines whether enemy or player is equiping</param>
    public void equipWeapon(GameObject weaponGameObject, weaponMounts mount, string tag)
    {
        switch (mount)
        {
            case weaponMounts.Primary:
                weaponGameObject.SetActive(true);
                primaryMount.WeaponFromGameObject = weaponGameObject;
                primaryMount.Weapon.WeaponTag = tag;
                primaryMount.Weapon.tag = tag;
                this.weapon = weaponGameObject.GetComponent<Weapon>();
                break;
            case weaponMounts.Secondary:
                weaponGameObject.SetActive(true);
                secondaryMount.WeaponFromGameObject = weaponGameObject;
                secondaryMount.Weapon.WeaponTag = tag;
                secondaryMount.Weapon.tag = tag;
                break;
        }
        //swapWeapons();
    }

    /// <summary>
    /// This function removes a weapon from the specified weaponMount.
    /// </summary>
    /// <param name="mount">The weapon mount to remove the weapon for.</param>
    public void removeWeaponFromMount(weaponMounts mount)
    {
        switch (mount)
        {
            case weaponMounts.Primary:
                primaryMount.Weapon = null;
                //Destroy(primaryMount.MountPoint.gameObject.transform.GetChild(0).gameObject);
                if (activeWeaponType == weaponMounts.Primary)
                {
                    this.weapon = null;
                }
                break;
            case weaponMounts.Secondary:
                secondaryMount.Weapon = null;
                //Destroy(secondaryMount.MountPoint.gameObject.transform.GetChild(0).gameObject);
                if (activeWeaponType == weaponMounts.Secondary)
                {
                    this.weapon = null;
                }
                break;
        }
    }

    /// <summary>
    /// Swaps the character's currently held weapon from the two possible slots
    /// </summary>
    /// <param name="animationToWaitFor">The animation to wait for.</param>
    /// <returns>An int specifying the new active weapon slot. 0 if the weapon was not swapped.</returns>
    public int swapWeapons(weaponMounts mount)
    {
        if (activeWeaponType == mount)
        {
            return 0;
        }

        // No empty slots and active weapon is the first one, switch to second wep
        if (mount == weaponMounts.Primary)
        {
            weapon = primaryMount.Weapon; // set as new active weapons
            activeWeaponType = weaponMounts.Primary;
            primaryMount.MountPoint.gameObject.SetActive(true); // show weapon
            secondaryMount.MountPoint.gameObject.SetActive(false); // hide weapon
            return 1;
        }
        // No empty slots and active weapon is the second one, switch to first wep
        else if (mount == weaponMounts.Secondary)
        {
            weapon = secondaryMount.Weapon; // set as new active weapon
            activeWeaponType = weaponMounts.Secondary;
            secondaryMount.MountPoint.gameObject.SetActive(true); // show weapon
            primaryMount.MountPoint.gameObject.SetActive(false); // hide weapon
            return 2;
        }
        return 0;
    }

    public int swapWeapons()
    {
        if (activeWeaponType == weaponMounts.Primary)
        {
            return swapWeapons(weaponMounts.Secondary);
        }
        else
        {
            return swapWeapons(weaponMounts.Primary);
        }
    }

    /// <summary>
    /// Flashes the character model red over several frames when the ytake damage.
    /// </summary>
    /// <returns>Return null. Is a co-routine so returns at the end of each frame.</returns>
    protected virtual IEnumerator flashOnDamageTaken()
    {
        if (!flashing)
        {
            this.flashing = true;
            //Initializing colors.
            Transform bodyTransform = this.gameObject.transform.Find("Body");
            SkinnedMeshRenderer body = bodyTransform.gameObject.GetComponent<SkinnedMeshRenderer>();
            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    body.material = this.damageMaterial;
                }
                else if (i % 2 == 0)
                {
                    body.material = this.damageMaterial;
                }
                else
                {
                    body.material = this.bodyMaterial;
                }
                yield return new WaitForSeconds(.15f);
            }
            body.material = this.bodyMaterial;
            this.flashing = false;
        }
    }

    /// <summary>
    /// Makes the character wait until an animation is completed in a co-routine.
    /// </summary>
    /// <param name="animationToWaitFor">The animation to wait for.</param>
    /// <returns></returns>
    public static IEnumerator WaitForAnimation(Animation animationToWaitFor)
    {
        do
        {
            yield return null;
        } while (animationToWaitFor.isPlaying);
    }

    /// <summary>
    /// This functions stops the player's horizontal movement if it is going to collide with something.
    /// 
    /// This should prevent the wall stickiness problem.
    /// </summary>
    private void stopHangingOnWall()
    {
        float direction;
        if (facingRight)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        //Fire two raycasts to calc the gradient of the ground in front of the player.
        RaycastHit lowerRaycastHitData;
        //Physics.Raycast(this.transform.position, new Vector3(direction, 0, 0), out lowerRaycastHitData, 1f, groundLayerMask);
        //RaycastHit higherRaycastHitData;
        //Physics.Raycast(this.transform.position + new Vector3(0, 1, 0), new Vector3(direction, 0, 0), out higherRaycastHitData, 1f, groundLayerMask);
        //Debug.DrawRay(this.transform.position + new Vector3(0, 1, 0), new Vector3(direction, 0, 0), Color.red, 100f);
        //If land has a gradient grater than 1:1 player cannot climb up it.
        if (Physics.Raycast(this.transform.position + new Vector3(0, 1.5f, 0), new Vector3(direction, 0, 0), out lowerRaycastHitData, 1f, groundLayerMask))
        {
            this.rigidbody.velocity = new Vector3(0, this.rigidbody.velocity.y, 0);
        }
    }
}
