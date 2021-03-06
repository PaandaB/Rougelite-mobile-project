﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Item //the public item class which items are made from and used.
{
    public int id;
    public string tier;
    public string title;
    public string plural;
    public string lore;
    public string description;
    public CharacterStat statType;
    public string statString;
    public string modType;
    //public Sprite icon;
    public float statValue;
    public int amount;
    public int collection;
    public Item(int id, string tier, string title, string plural, string lore, string description, /*icon,*/ CharacterStat statType, string statString, string modType, float statValue, int amount, int collection)
    {
        this.id = id;
        this.tier = tier;
        this.title = title;
        this.plural = plural;
        this.lore = lore;
        this.description = description;
        //this.icon = Resources.Load<Sprite>(Sprite/Items + title);
        this.statType = statType;
        this.statString = statString;
        this.modType = modType;
        this.statValue = statValue;
        this.amount = amount;
        this.collection = collection;
    }
    public Item(Item item)
    {
        this.id = item.id;
        this.tier = item.tier;
        this.title = item.title;
        this.plural = item.plural;
        this.lore = item.lore;
        this.description = item.description;
        //this.icon = Resources.Load<Sprite>(Sprite/Items + item.title);
        this.statType = item.statType;
        this.statString = item.statString;
        this.modType = item.modType;
        this.statValue = item.statValue;
        this.amount = item.amount;
        this.collection = item.collection;
    }
}
