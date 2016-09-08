﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This is the super class for weapons it contains functions for weapons that will be exposed to the player. 
/// 
/// Weapons have their own gam,e object which instantiated buleets and effects for the weapon. 
/// Each child class will have its own implemetation of the attack method which will create its own effects.
/// 
/// It also cointains a method for getting a bullet prefab which the child classes may use.
/// </summary>
public abstract class Weapon: Item{

    // Attack variables
    public int damage;
    public int knockBack;
    public int energyCost;
    public string weaponTag;
    public string WeaponTag
    {
        get
        {
            return weaponTag;
        }
        set
        {
            weaponTag = value;
        }
    }


    protected float qualityModifier;

    //Firerate of the gun.
    public float weaponAttackSpeed;

    /// <summary>
    /// This function will be called when the player left clicks with a weapon. It handles animation the attack and instianting the bullets for each weapon.
    /// 
    /// This function will have very different for different weapons eg: Pistol vs flamethrower.
    /// </summary>
    abstract public void attack(float playerDamage, Vector3 mousePositionInWorldCoords);


    

    /// <summary>
    /// Returns a bullet prefab.
    /// </summary>
    /// <returns>A bullet prefab at the specified position.</returns>
    protected GameObject getBulletPrefab()
    {
        GameObject bullet = Instantiate(Resources.Load("Prefabs/Weapons/Bullet"), this.transform.position, this.transform.rotation) as GameObject;

        switch (tag)
        {
            case "PlayerWeapon":
                bullet.tag = "PlayerProjectile";
                break;
            case "EnemyWeapon":
                bullet.tag = "EnemyProjectile";
                break;
            default:
                break;
        }
        return bullet;
    }  

    public int getDamage()
    {
        return damage;
    }

    public int getKnockBack()
    {
        return knockBack;
    }

    public float getAttackSpeed()
    {
        return weaponAttackSpeed;
    }

    public abstract string getModelPath();
}
