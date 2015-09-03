using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using KnetikSimpleJSON;

namespace Knetik
{
	public partial class KnetikClient
	{
	
		/*
	TO Get Invoice you need to have your cartGUID 
		this can be done by 
		1-create cart get the number 
		2-Add Item to Cart 
		3-Modify shipping address and confirm your address 
		4-Get Invoice by cartGUID =cartNumber 
		 */
		public KnetikApiResponse GetInvoice(String  cartGUID,
		    Action<KnetikApiResponse> cb = null
		     ) {
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("cartGUID", cartGUID);

			return GetInvoice(j , cb);
		}

		public String buildInvoiceUrl(JSONObject j)
		{
			StringBuilder storeBuilder = new StringBuilder();
			storeBuilder.Append (GetInvoiceEndpoint);
			storeBuilder.Append ("?");
			for (int i=0; i<j.keys.Count; i++) {
			if(i!=0)
			{
					storeBuilder.Append ("&");

			}
				string name=j.keys[i];
				storeBuilder.Append(name);
				storeBuilder.Append("=");
				storeBuilder.Append(j.GetField(name).str);			
			}

			return storeBuilder.ToString();
			
		}

		public KnetikApiResponse GetInvoice(JSONObject j,Action<KnetikApiResponse> cb = null
			) {

			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(buildInvoiceUrl(j), body,"GET");
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

	

	}
}

