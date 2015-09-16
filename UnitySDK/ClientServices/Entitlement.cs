using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using KnetikSimpleJSON;

namespace Knetik
{
	public partial class KnetikClient
	{
	
		/*entitlementCheck
		 *@param String itemId  
		 *@parm skuId 
		 */

		public KnetikApiResponse entitlementCheck(String itemId,string skuId,
		       Int64 invoiceId, Action<KnetikApiResponse> cb = null
		     ) {
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("item_id", itemId);
			j.AddField ("sku_id", skuId);
			return verifyReceipt(j , cb);
		}


		public KnetikApiResponse entitlementCheck(JSONObject j,Action<KnetikApiResponse> cb = null
			) {

			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(EntitlementEndpoint, body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

	

	}
}

