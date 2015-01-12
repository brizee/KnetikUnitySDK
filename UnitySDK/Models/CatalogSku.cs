using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class CatalogSku : KnetikModel
    {
        public int ID {
            get;
            set;
        }

        public Item Item {
            get;
            protected set;
        }

        public string Sku {
            get;
            set;
        }

        public double Price {
            get;
            set;
        }

        public string CurrencyCode
        {
            get;
            set;
        }

        public string Description {
            get;
            set;
        }

        public int Inventory {
            get;
            set;
        }

        public int CatalogID {
            get;
            set;
        }

        public CatalogSku (KnetikClient client, Item item)
            : base(client)
        {
            this.Item = item;
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);

            ID = json["id"].AsInt;
            Sku = json["sku"].Value;
            Price = json["price"].AsDouble;
            Description = json["description"].Value;
            Inventory = json["inventory"].AsInt;
            CatalogID = json["catalog_id"].AsInt;
            CurrencyCode = json ["code"].Value;
        }
    }
}

