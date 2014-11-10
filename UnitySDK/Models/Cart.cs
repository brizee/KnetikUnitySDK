using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Cart : KnetikModel
    {
        public List<CartItem> Items { get; set; }

        public double SubTotal { get; protected set; }
        public double DiscountTotal { get; set; }
        public double GrandTotal { get; set; }
        public double Tax { get; set; }
        public string Status { get; set; }

    	public Cart(KnetikClient client)
    		: base(client)
    	{
            Items = new List<CartItem>();
    	}

    	public void Load(Action<KnetikResult<Cart>> cb)
        {
            Client.CartGet ((res) => {
                var result = new KnetikResult<Cart> {
                    Response = res
                };
                if (!res.IsSuccess) {
                    cb(result);
                    return;
                }
                Response = res;

                this.Deserialize(res.Body["result"]);

                result.Value = this;
                cb(result);
            });
        }

        public void Add(CatalogSku sku, int quantity, Action<KnetikResult<CartItem>> cb)
        {
            Client.CartAdd(sku.CatalogID, sku.ID, quantity, (res) => {
                var result = new KnetikResult<CartItem> {
                    Response = res
                };
                if (!res.IsSuccess) {
                    cb(result);
                    return;
                }

                var item = new CartItem(Client, this);
                item.FromSku(sku, quantity);
                Items.Add(item);

                result.Value = item;
                cb(result);
            });
        }

        public void SetStatus(string status, Action<KnetikResult<Cart>> cb)
        {
            Client.CartStatus(status, (res) => {
                var result = new KnetikResult<Cart> {
                    Response = res
                };
                if (!res.IsSuccess) {
                    cb(result);
                    return;
                }

                Status = status;

                result.Value = this;
                cb(result);
            });
        }

        public void Checkout(Action<KnetikResult<Cart>> cb)
        {
            Client.CartCheckout((res) => {
                var result = new KnetikResult<Cart> {
                    Response = res
                };
                if (!res.IsSuccess) {
                    cb(result);
                    return;
                }
                result.Value = this;
                cb(result);
            });
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);

            var cartJson = json["cart"];
            SubTotal = cartJson["sub_total"].AsDouble;
            DiscountTotal = cartJson["discount_total"].AsDouble;
            GrandTotal = cartJson["grand_total"].AsDouble;
            Tax = cartJson["tax"].AsDouble;
            Status = cartJson["status"].Value;

            Items.Clear();
            foreach (var node in json["items"].Children) {
                var item = new CartItem(Client, this);
                item.Deserialize(node);
                Items.Add(item);
            }
        }
    }
}