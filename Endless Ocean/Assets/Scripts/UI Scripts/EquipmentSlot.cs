﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot: Slot
{

    public CharacterEquipment equipment;
    public String bodypart;

    private Text itemCount;

    /// <summary>
    /// This function runs when the user clicks on the slot.
    /// </summary>
    /// <param name="eventData">The event data from the click.</param>
    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    /// <summary>
    /// This function runs when the user mouses over a slot.
    /// 
    /// It shows the tooltip for the item.
    /// </summary>
    /// <param name="eventData">The data from the mouse over.</param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    /// <summary>
    /// This function runs when the user mouses out of a slot.
    /// 
    /// It hides the tooltip for the item.
    /// </summary>
    /// <param name="eventData">The data from the mouse over.</param>
    public override void OnPointerExit(PointerEventData eventData)
    {
    }

    /// <summary>
    /// This function runs when an item is dragged.
    /// 
    /// Starts dragging the item from the slot.
    /// </summary>
    /// <param name="eventData">The data from the drag.</param>
    public override void OnDrag(PointerEventData eventData)
    {
        if (!this.isSlotEmpty())
        {
            this.itemCount.enabled = false;
            this.inventory.startDraggingItem(this.equipment.equippedItems[bodypart]);
            this.equipment.equippedItems[bodypart] = new Item();
        }
    }

    /// <summary>
    /// Runs when the player clicks on a slot.
    /// 
    /// Equips the item in the slot if possible. Prompts the player if they are not equipping equipment or if they are putting equipment in the wrong slot.
    /// </summary>
    /// <param name="eventData">The data from the click.</param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.equipment.validationPrompt.text = "";
        if (this.inventory.draggingItem)
        {
            if (this.inventory.draggedItem.GetType().ToString() == "Equipment") {
                Equipment tempEquipment = (Equipment)this.inventory.draggedItem;
                if (tempEquipment.bodypart == this.bodypart)
                {
                    if (!this.isSlotEmpty())
                    {
                        Item tempDraggingItem = this.inventory.draggedItem;
                        this.inventory.draggedItem = this.equipment.equippedItems[bodypart];
                        this.inventory.draggedItemIcon.GetComponent<Image>().sprite = this.inventory.draggedItem.itemIcon;
                        this.equipment.equippedItems[bodypart] = tempDraggingItem;
                    }
                    else if (this.isSlotEmpty())
                    {
                        this.equipment.equippedItems[bodypart] = this.inventory.draggedItem;
                        this.inventory.stopDraggingItem();
                    }
                }
                else
                {
                    this.equipment.validationPrompt.text = "That is the wrong slot for that equipment.";
                }
            }
            else
            {
                this.equipment.validationPrompt.text = "You can only wear equipment.";
            }
        }
    }

    /// <summary>
    /// Runs when the Gameobject is first created. Initializes key variables.
    /// </summary>
    void Start()
    {
        this.equipment = this.transform.parent.gameObject.GetComponent<CharacterEquipment>();
        this.itemCount = this.gameObject.transform.GetChild(1).GetComponent<Text>();
        this.itemImage = this.GetComponentsInChildren<Image>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        this.itemCount.enabled = false;
        //If this slot is holdin an item - show it.
        if (!this.isSlotEmpty())
        {
            this.itemImage.enabled = true;
            this.itemImage.sprite = this.equipment.equippedItems[bodypart].itemIcon;
        }
        else
        {
            this.itemImage.enabled = false;
        }
    }

    public bool isSlotEmpty()
    {
        if (this.equipment.equippedItems[bodypart].itemName != null)
        {
            return false;
        }
        return true;
    }
}