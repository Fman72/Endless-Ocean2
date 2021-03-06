﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : Slot
{

    public int slotNumber;
    Text itemCount;

    /// <summary>
    /// This function runs when the user mouses over a slot.
    /// 
    /// It shows the tooltip for the item.
    /// </summary>
    /// <param name="eventData">The data from the mouse over.</param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!this.isSlotEmpty())
        {
            this.inventory.showToolTip(inventory.items[this.slotNumber], this.gameObject.GetComponent<RectTransform>().localPosition);
        }
    }

    /// <summary>
    /// This function runs when the user mouses out of a slot.
    /// 
    /// It hides the tooltip for the item.
    /// </summary>
    /// <param name="eventData">The data from the mouse over.</param>
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!this.isSlotEmpty())
        {
            this.inventory.hideToolTip();
        }
    }

    /// <summary>
    /// This function runs when an item is dragged.
    /// 
    /// Starts dragging the item from the slot.
    /// </summary>
    /// <param name="eventData">The data from the drag.</param>
    public override void OnDrag(PointerEventData eventData)
    {
        if (this.inventory.draggingItem)
        {
            return;
        }
        if (!this.isSlotEmpty())
        {
            this.inventory.hideToolTip();
            this.itemCount.enabled = false;
            this.inventory.startDraggingItem(inventory.items[slotNumber]);
            this.inventory.items[slotNumber] = new Item();
        }
    }

    /// <summary>
    /// Runs when the player clicks on a slot.
    /// 
    /// Places an item in the slot if the player was dragging one.
    /// </summary>
    /// <param name="eventData">The data from the click.</param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (this.inventory.draggingItem)
        {
            if (!this.isSlotEmpty())
            {
                Item tempDraggingItem = this.inventory.draggedItem;
                this.inventory.draggedItem = this.inventory.items[this.slotNumber];
                this.inventory.draggedItemIcon.GetComponent<Image>().sprite = this.inventory.items[this.slotNumber].itemIcon;
                this.inventory.handleAddItemFromUI(tempDraggingItem, this.slotNumber);
            }
            else if (this.isSlotEmpty())
            {
                this.inventory.handleAddItemFromUI(this.inventory.draggedItem, this.slotNumber);
                this.inventory.stopDraggingItem();
            }
        }
    }

    /// <summary>
    /// Runs when the Gameobject is first created. Initializes key variables.
    /// </summary>
    void Start()
    {
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
            this.itemImage.sprite = this.inventory.items[this.slotNumber].itemIcon;
            if (this.inventory.items[slotNumber].stackable)
            {
                this.itemCount.enabled = true;
                this.itemCount.text = "" + this.inventory.items[this.slotNumber].itemCount;
            }
        }
        else
        {
            this.itemImage.enabled = false;
        }
    }

    /// <summary>
    /// Returns a boolean indicating whether or not the slot is empty.
    /// </summary>
    /// <returns>A boolean indicating if the slot is empty or not.</returns>
    public bool isSlotEmpty()
    {
        try
        {
            if (this.inventory.items[this.slotNumber].itemName != null)
            {
                return false;
            }
            return true;
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("Slot was empty. Got Exception.");
            return true;
        }
    }
}
