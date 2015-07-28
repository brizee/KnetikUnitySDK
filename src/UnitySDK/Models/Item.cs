using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Item : KnetikModel
    {
        public static Item Parse(KnetikClient client, KnetikJSONNode json)
        {
            string typeHint = json ["type_hint"].Value;
            Item item;
            switch (typeHint) {
                case "game":
                item = new Game(client);
                break;
                case "physical_item":
                item = new PhysicalItem(client);
                break;
                case "virtual_item":
                item = new VirtualItem(client);
                break;
                case "entitlement":
                item = new Entitlement(client);
                break;
                case "subscription":
                item = new Subscription(client);
                break;
                case "achievement_item":
                item = new Achievement(client);
                break;
                case "leaderboard":
                item = new Leaderboard(client);
                break;
                default:
                item = new Item(client);
                break;
            }
            item.Deserialize(json);
            return item;
        }

        public int ID {
            get;
            set;
        }

        public string UniqueKey {
            get;
            set;
        }

        public string TypeHint {
            get;
            set;
        }
        
        public string Name {
            get;
            set;
        }
        
        public string ShortDescription  {
            get;
            set;
        }
        
        public string LongDescription {
            get;
            set;
        }

        public DateTime DeletedAt {
            get;
            set;
        }

        public DateTime DateCreated {
            get;
            set;
        }

        public DateTime DateUpdated {
            get;
            set;
        }
        
        public List<ItemAsset> Assets {
            get;
          	protected set;
	    }

        public List<CatalogSku> Skus {
            get;
            protected set;
        }

        public Dictionary<string, Behavior> Behaviors {
            get;
            protected set;
        }

        public ItemAsset ThumbnailAsset {
            get {
                if (Assets.Count == 0) {
                    return null;
                } else {
                    return Assets[0];
                }
            }
        }

        public bool IsPurchasable {
            get {
                return Skus.Count > 0;
            }
        }

        public Item (KnetikClient client)
            : base(client)
        {
            Assets = new List<ItemAsset>();
            Skus = new List<CatalogSku>();
            Behaviors = new Dictionary<string, Behavior>();
        }

        public Item (KnetikClient client, int id)
            : this(client)
        {
            ID = id;
        }

        public StoreQuery GetAchievements()
        {
            StoreQuery query = GetRelatedItems();
            query.ItemTypes = new List<string> { "achievement_item", "game_achievement_item" };
            return query;
        }

        public StoreQuery GetRelatedItems()
        {
            StoreQuery query = new StoreQuery(Client);
            query.Related = new List<string> { this.UniqueKey };
            query.UseCatalog = false;
            return query;
        }

        public bool HasBehavior(string behavior)
        {
            return Behaviors.ContainsKey(behavior);
        }
        
        public override void Deserialize (KnetikJSONNode json)
        {
            if (json ["id"] != null && json ["id"] != "null")
            {
                ID = json ["id"].AsInt;
            }
            UniqueKey = json ["unique_key"].Value;
            TypeHint = json ["type_hint"].Value;
            Name = json ["name"].Value;
            ShortDescription = json ["short_description"].Value;
            LongDescription = json ["long_description"].Value;

            Assets.Clear ();
            foreach (KnetikJSONNode node in json["assets"].Children)
            {
                ItemAsset asset = new ItemAsset(Client);
                asset.Deserialize(node);
                Assets.Add(asset);
            }
            
            Skus.Clear ();
            foreach (KnetikJSONNode node in json["skus"].Children)
            {
                CatalogSku sku = new CatalogSku(Client, this);
                sku.Deserialize(node);
                Skus.Add(sku);
            }

            Behaviors.Clear();
            foreach (KnetikJSONNode node in json["behaviors"].Children)
            {
                Behavior behavior = Behavior.Parse(Client, node);
                Behaviors[behavior.TypeHint] = behavior;
            }

            if (json ["deleted_at"] != null && json ["deleted_at"] != "null")
            {
                DeletedAt = new DateTime (json ["deleted_at"].AsInt);
            }

            if (json ["date_created"] != null && json ["date_created"] != "null")
            {
                DateCreated = new DateTime (json ["date_created"].AsInt);
            }

            if (json ["date_updated"] != null && json ["date_updated"] != "null")
            {
                DateUpdated = new DateTime (json ["date_updated"].AsInt);
            }
        }
    }
}

