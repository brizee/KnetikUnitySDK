using System;
using System.Collections.Generic;

namespace Knetik
{
    public partial class KnetikClient
    {
        public KnetikApiResponse CartAdd(
            int catalogId,
            int catalogSkuId,
            int quantity = 1,
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

        public KnetikApiResponse CartModify(
            int catalogId,
            int catalogSkuId,
            int quantity = 1,
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

        public KnetikApiResponse CartCheckout(
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CartCheckoutEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

        public KnetikApiResponse CartShippingAddress(
            CartShippingAddress address,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("first_name", address.FirstName);
            j.AddField ("last_name", address.FirstName);
            j.AddField ("address_line_1", address.AddressLine1);
            j.AddField ("address_line_1", address.AddressLine2);
            j.AddField ("city", address.City);
            j.AddField ("postal_state", address.PostalState);
            j.AddField ("zip", address.Zip);
            j.AddField ("country", address.Country);
            j.AddField ("email", address.Email);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CartShippingAddressEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

        public CartS CartStatus(
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

        public KnetikApiResponse CartGet(
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CartGetEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

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

        public KnetikApiResponse CartCountries(
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CartCountriesEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

        struct CartShippingAddress {
            string FirstName;
            string LastName;
            string AddressLine1;
            string AddressLine2;
            string City;
            string PostalState;
            string Zip;
            string Country;
            string Email;
        }
    }
}
