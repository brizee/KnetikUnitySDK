using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class InventoryItem : KnetikModel
    {
        public int ID { get; set; }
        public Item Item { get; set; }
        public int UseCount { get; set; }

        public InventoryItem(KnetikClient client)
            : base(client)
        {

        }

        public override void Deserialize(KnetikJSONNode json)
        {
            ID = json ["inventory_id"].AsInt;
            Item = Item.Parse(Client, json ["item"]);
            UseCount = json ["use_count"].AsInt;
        }

        public bool IsConsumable
        {
            get {
                return Item.HasBehavior("consumable");
            }
        }

        public void Consume(Action<KnetikApiResponse> cb)
        {
            Client.UseItem(Item.ID, cb);
        }
    }
}

