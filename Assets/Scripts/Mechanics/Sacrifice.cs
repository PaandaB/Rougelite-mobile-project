﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SacrificeType { loot, consumable, mechanic, debuff}
[Serializable]
public class Sacrifice //the sacrifice class which sacrifices are made of and used in other scripts.
{
    public int level;
    public string description;
    public SacrificeType type;
    public int intensity;

    public Sacrifice (int level, string description, SacrificeType type, int intensity)
    {
        this.level = level;
        this.description = description;
        this.type = type;
        this.intensity = intensity;
    }
    public Sacrifice(Sacrifice sacrifice)
    {
        this.level = sacrifice.level;
        this.description = sacrifice.description;
        this.type = sacrifice.type;
        this.intensity = sacrifice.intensity;
    }
}
