using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class CartItem : KnetikModel
    {
        public Cart Cart { get; set; }

        public int ID { get; set; }
        public string TypeHint { get; set; }
        public double SystemPrice { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public int Quantity { get; set; }
        public int SkuID { get; set; }
        public string Sku { get; set; }
        public string SkuDescription { get; set; }
        public int CatalogID { get; set; }
        public int Inventory { get; set; }
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }

        public CartItem(KnetikClient client, Cart cart)
            : base(client)
        {
            this.Cart = cart;
        }

        public void FromSku(CatalogSku sku, int quantity = 1)
        {
            ID = -1;
            TypeHint = sku.Item.TypeHint;
            SystemPrice = sku.Price;
            UnitPrice = sku.Price;
            TotalPrice = sku.Price * quantity;
            SkuID = sku.ID;
            Sku = sku.Sku;
            SkuDescription = sku.Description;
            Inventory = sku.Inventory;
            CatalogID = sku.CatalogID;
            ItemID = sku.Item.ID;
            Name = sku.Item.Name;
            Thumbnail = null;
            ErrorMessage = null;
            ErrorCode = 0;
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);

            ID = json["id"].AsInt;
            TypeHint = json["type_hint"].Value;
            SystemPrice = json["system_price"].AsDouble;
            UnitPrice = json["unit_price"].AsDouble;
            TotalPrice = json["total_price"].AsDouble;
            SkuID = json["sku_id"].AsInt;
            Sku = json["sku"].Value;
            SkuDescription = json["sku_description"].Value;
            Inventory = json["inventory"].AsInt;
            CatalogID = json["catalog_id"].AsInt;
            ItemID = json["store_item_id"].AsInt;
            Name = json["name"].Value;
            Thumbnail = json["thumbnail"].Value;
            ErrorMessage = json["error_message"].Value;
            ErrorCode = json["error_code"].AsInt;
        }
    }
}