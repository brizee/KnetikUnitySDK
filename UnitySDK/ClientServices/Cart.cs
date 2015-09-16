using System;
using System.Collections.Generic;
using System.Text;
using KnetikSimpleJSON;

namespace Knetik
{
	public struct ShippingAddress {
		public string OrderNotes;
		public string PrefixName;
		public string FirstName;
		public string LastName;
		public string AddressLine1;
		public string AddressLine2;
		public string City;
		public string PostalState;
		public string Zip;
		public string Country;
		public string Email;
		public string Country_id;
	}
    public partial class KnetikClient
    {
        private Cart _cart = null;
        public Cart Cart {
            get {
                if (_cart == null) {
                    _cart = new Cart(this);
                }
                return _cart;
            }
        }

		/*JSAPI2 Function
		Add Item to A Cart 
		@params int catalogId
		@params int catalogSkuId
		@params int quantity
		@params Action<KnetikApiResponse> cb
		 */
		public KnetikApiResponse CartAdd(string cartNumber,int catalogId,
			int catalogSkuId,
			int quantity,
			Action<KnetikApiResponse> cb = null
			) {
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("catalog_id", catalogId);
			j.AddField ("catalog_sku_id", catalogSkuId);
			j.AddField ("quantity", quantity);
			String body = j.Print ();
			StringBuilder CartItemsEndpointBuilder = new StringBuilder();
			CartItemsEndpointBuilder.AppendFormat(CartItemsEndpoint,cartNumber);
			KnetikRequest req = CreateRequest(CartItemsEndpointBuilder.ToString(), body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

		[Obsolete("use CartAdd(cartNumber,catalogId,catalogSkuId,quantity,Action<KnetikApiResponse>)")]
        public KnetikApiResponse CartAdd(
            int catalogId,
            int catalogSkuId,
            int quantity,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("catalog_id", catalogId);
            j.AddField ("catalog_sku_id", catalogSkuId);
            j.AddField ("quantity", quantity);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CartAddEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
		/*
		JSAPI2 CartModify Item Quantity
		@param cartNumber 
		@param int item catalogId 
		@param int item catalogSkuId 
		@param int quantity To be modified
		 */
		public KnetikApiResponse CartModify(string cartNumber,
			int catalogId,
			int catalogSkuId,
			int quantity,
			Action<KnetikApiResponse> cb = null
			) {
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("catalog_id", catalogId);
			j.AddField ("catalog_sku_id", catalogSkuId);
			j.AddField ("quantity", quantity);
			String body = j.Print ();
			StringBuilder CartItemsEndpointBuilder = new StringBuilder();
			CartItemsEndpointBuilder.AppendFormat(CartItemsEndpoint,cartNumber);
			KnetikRequest req = CreateRequest(CartItemsEndpointBuilder.ToString(), body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

		[Obsolete("use CartModify(cartNumber,catalogId,catalogSkuId,quantity,Action<KnetikApiResponse>)")]
		public KnetikApiResponse CartModify(
            int catalogId,
            int catalogSkuId,
            int quantity,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("catalog_id", catalogId);
            j.AddField ("catalog_sku_id", catalogSkuId);
            j.AddField ("quantity", quantity);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CartModifyEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

		/**Closes a cart and generates an invoice
    	*@params cart {"cartguid": "cart GUID"}
    	*/
		public KnetikApiResponse CartCheckout(string cartNumber,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            String body = j.Print ();

			StringBuilder CartCheckoutEndpointBuilder = new StringBuilder();
			CartCheckoutEndpointBuilder.AppendFormat(CartCheckoutEndpoint,cartNumber);
			KnetikRequest req = CreateRequest(CartCheckoutEndpointBuilder.ToString(), body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

		[Obsolete("use CartCheckout(string cartNumber,Action<KnetikApiResponse> cb = null)")]
		public KnetikApiResponse CartCheckout(Action<KnetikApiResponse> cb = null)
		{
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(CartCheckoutEndpoint, body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}
        public KnetikApiResponse CartShippingAddress(string cartNumber,
            ShippingAddress address,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("name_prefix", address.PrefixName);
			j.AddField ("first_name", address.FirstName);
			j.AddField ("last_name", address.FirstName);
			j.AddField ("shipping_address_line1", address.AddressLine1);
			j.AddField ("shipping_address_line2", address.AddressLine2);
            j.AddField ("city", address.City);
			j.AddField ("postal_state_id", address.PostalState);
            j.AddField ("zip", address.Zip);
			j.AddField ("country", address.Country);
			j.AddField ("country_id", address.Country_id);
			j.AddField ("email", address.Email);
			j.AddField ("order_notes", address.OrderNotes);

            String body = j.Print ();
			StringBuilder cartShippingAddressEndPoint = new StringBuilder();
			cartShippingAddressEndPoint.AppendFormat(CartModifyShippingAddressEndpoint,cartNumber);
			KnetikRequest req = CreateRequest(cartShippingAddressEndPoint.ToString(), body,"PUT");
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
		[Obsolete]
        public KnetikApiResponse CartStatus(
            string status,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("status", status);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CartStatusEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
		//Returns whether a cart requires shipping
		public KnetikApiResponse CartShippable(
			string cartNumber,
			Action<KnetikApiResponse> cb = null
			) {
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			String body = j.Print ();
			StringBuilder CartShippableEndpointBuilder = new StringBuilder();
			CartShippableEndpointBuilder.AppendFormat(CartShippableEndpoint,cartNumber);

			KnetikRequest req = CreateRequest(CartShippableEndpointBuilder.ToString(), body,"GET");
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

		/*
		JSAPI2 Get Cart Details
		EndPoint :services/latest/carts/{cartNumber}
		@params cartNumber string 
		@param Action<KnetikApiResponse> 
		 */
        public KnetikApiResponse CartGet(string cartNumber,Action<KnetikApiResponse> cb = null)
		{
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            String body = j.Print ();
            
			KnetikRequest req = CreateRequest(CartGetEndpoint+cartNumber, body,"GET");
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
		[Obsolete("use CartGet(string cartNumber,Action<KnetikApiResponse> cb = null)")]
		public KnetikApiResponse CartGet(Action<KnetikApiResponse> cb = null)
		{
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(CartGetEndpoint, body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}
		public KnetikApiResponse CartCreate(Action<KnetikApiResponse> cb = null)
		{
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(CartCreateEndpoint, body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

		public KnetikApiResponse CartCreateWithVirtualCurrency(KnetikJSONArray currencies,CatalogSku catlog,Action<KnetikApiResponse> cb = null)
		{
			StringBuilder createCartBuilder = new StringBuilder();
			createCartBuilder.Append(CartCreateEndpoint);

			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			if (catlog.Item != null) {
				if(catlog.Item.TypeHint == "virtual_item" )
				{
					foreach(KnetikJSONNode element in currencies)
					{
						if(element["id"].AsInt == catlog.CurrencyId)
						{
							createCartBuilder.Append("?currency_code="+element["code"].Value);
						}
					}

				}
			}

			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(createCartBuilder.ToString(), body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

		public KnetikApiResponse CartAddDiscount(string cartNumber,
			string sku,
			Action<KnetikApiResponse> cb = null
			) {
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("sku", sku);
			String body = j.Print ();

			StringBuilder CartDiscountEndpointBuilder = new StringBuilder();
			CartDiscountEndpointBuilder.AppendFormat(CartAddDiscountEndpoint,cartNumber);
			KnetikRequest req = CreateRequest(CartDiscountEndpointBuilder.ToString(), body);		
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

		[Obsolete("use CartAddDiscount(string cartNumber,string sku,Action<KnetikApiResponse> cb = null)")]
		public KnetikApiResponse CartAddDiscount(
            string sku,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("sku", sku);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CartAddDiscountEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

        public KnetikApiResponse CartCountries(string cartNumber,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            String body = j.Print ();
			StringBuilder CartCountriesEndpointBuilder = new StringBuilder();
			CartCountriesEndpointBuilder.AppendFormat(CartCountriesEndpoint,cartNumber);
			KnetikRequest req = CreateRequest(CartCountriesEndpointBuilder.ToString(), body,"GET");
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }      
    }
}
