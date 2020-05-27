﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LootFound : MonoBehaviour
{
    public GameObject player;
    public List<Item> Loot = new List<Item>();
    public ItemDatabase ItemDatabase;
    public GameObject AcceptObject;
    public GameObject Button;
    public MenuManager MenuManager;
    public AcceptLoot AcceptLoot => AcceptObject.GetComponent<AcceptLoot>();


    public bool poppedUp = false;
    // Start is called before the first frame update
    void Start()
    {
        MenuManager = gameObject.GetComponent<MenuManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (player == null && SceneManager.GetActiveScene().buildIndex != 0) //gets current player and itemdatabase
        {
            player = GameObject.FindGameObjectWithTag("Player");
            ItemDatabase = player.GetComponentInChildren<ItemDatabase>();
            return;
        }

        if (Loot.Count > 0 && MenuManager.isPaused && AcceptLoot.Loot.Count == 0)
        {
            AcceptLoot.GiveItem(Loot[0].id, Loot[0].amount); //if this script has item in list and game is paused and nothing is in acceptloot. 
            //The first item in the list is sent to acceptloot script and removed from this one. The button is turned on as well.
            Button.SetActive(true);
            RemoveItem(Loot[0].id);
        }
    }

    public void GiveItem(int id)
    {
        Item itemToAdd = ItemDatabase.GetItem(id);
        bool AlreadyinInventory = false;
        foreach (Item item in Loot) //items are given here from loot drop prefabs, and are checked if there's copies or not.
        {
            if (item.id == itemToAdd.id)
            {
                item.amount += 1;
                AlreadyinInventory = true;
                Item itemCheck = CheckforItems(id);
            }
        }
        if (!AlreadyinInventory)
        {
            Loot.Add(itemToAdd);
            Item itemCheck = CheckforItems(id);

            Debug.Log("Got " + itemToAdd.title + itemToAdd.description);

        }
    }
    public Item CheckforItems(int id)
    {
        return Loot.Find(item => item.id == id);
    }
    public void RemoveItem(int id)
    {
        Item item = CheckforItems(id);
        if (item != null)
        {
            Loot.Remove(item);
        }
    }
}
