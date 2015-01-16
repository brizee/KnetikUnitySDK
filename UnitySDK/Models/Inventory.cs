using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Inventory : KnetikModel
    {
        private Dictionary<int, List<InventoryItem>> items;
        public IEnumerable<InventoryItem> Items {
            get {
                List<InventoryItem> result = new List<InventoryItem>();
                foreach (IEnumerable<InventoryItem> itemlist in items.Values) {
                    result.AddRange(itemlist);
                }
                return result;
            }
        }
        
        public Inventory (KnetikClient client)
            : base(client)
        {
        }

        public bool Contains(int itemID)
        {
            return items.ContainsKey(itemID) && items[itemID].Count > 0;
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            items = new Dictionary<int, List<InventoryItem>>();
            foreach (KnetikJSONNode node in json.Children) {
                Item item = Item.Parse(Client, json);
                item.ID = node ["item_id"].AsInt;
                int inventoryID = node["inventory_id"].AsInt;
                if (!items.ContainsKey(item.ID)) {
                    items.Add(item.ID, new List<InventoryItem>());
                }
                items[item.ID].Add(new InventoryItem {
                    InventoryID = inventoryID,
                    Item = item
                });
            }
        }

        public struct InventoryItem {
            public int InventoryID;
            public Item Item;
        }
    }
}

