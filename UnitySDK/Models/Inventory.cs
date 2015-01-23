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
            foreach (KnetikJSONNode node in json.Children)
            {
                InventoryItem invItem = new InventoryItem(Client);
                invItem.Deserialize(node);
                if (invItem.Item.ID > 0)
                {
                    if (!items.ContainsKey(invItem.Item.ID))
                    {
                        items.Add(invItem.Item.ID, new List<InventoryItem>());
                    }
                    items[invItem.Item.ID].Add(invItem);
                }
            }
        }
    }
}

