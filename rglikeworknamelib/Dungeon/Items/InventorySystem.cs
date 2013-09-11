﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Item
{
    public class InventorySystem {
        public List<Item> items = new List<Item>();

        public InventorySystem() {
        }

        public List<Item> FilterByType(ItemType it) {
            if (it == ItemType.Nothing) return items;
            var a = new List<Item>();

            foreach (var item in items) {
                if(ItemDataBase.Data[item.Id].SType == it) a.Add(item);
            }

            return a;
        }

        public void StackSimilar() {
            var a = TakeDistinctItems();

            foreach (var item in items) {
                FindById(a, item.Id).Count += item.Count;
            }

            items = a;
        }

        private Item FindById(List<Item> list, string id) {
            foreach (var item in list) {
                if (item.Id == id) return item;
            }
            return null;
        }

        private List<Item> TakeDistinctItems() {
            List<Item> a = new List<Item>();

            foreach (var item in items) {
                Item item1 = item;
                if(a.Select(x => x.Id == item1.Id).All(y => y == false)) a.Add(new Item(item));
            }

            foreach (var item in a) {
                item.Count = 0;
            }

            return a;
        }

        public void UseItem(Item selectedItem, Player player)
        {
            if (selectedItem != null && player != null)
            {
                switch (ItemDataBase.Data[selectedItem.Id].SType)
                {
                    case ItemType.Hat:
                    case ItemType.Pants:
                    case ItemType.Helmet:
                    case ItemType.Meele:
                    case ItemType.Shirt:
                    case ItemType.Ammo:
                    case ItemType.Bag:
                    case ItemType.Boots:
                    case ItemType.Glaces:
                    case ItemType.Gloves:
                    case ItemType.Gun:
                        player.EquipItem(selectedItem, this);
                        break;
                    case ItemType.Food:
                        EventLog.Add(string.Format("Вы употребили {0}", ItemDataBase.Data[selectedItem.Id].Name), GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                        foreach (var buff in selectedItem.Buffs)
                        {
                            buff.ApplyToTarget(player);
                        }
                        if (items.Contains(selectedItem))
                        {
                            items.Remove(selectedItem);
                        }
                        break;
                }
            }
        }
    }
}