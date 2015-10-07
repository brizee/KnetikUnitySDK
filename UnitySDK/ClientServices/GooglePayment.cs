using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using KnetikSimpleJSON;

namespace Knetik
{
	public partial class KnetikClient
	{
	
		/*Mark an invoice payed with Google. Verifies signature from Google and treats 
		 * the developerPayload field inside the json payload as the id of the invoice to pay. 
		 * 
		 *@param String receipt  A receipt will only be accepted once 
		 *@parm transactionId    details of the transaction must match the invoice, 
		 *@parm  invoiceId
		 * Returns the transaction Id if successful.
		 */

		public KnetikApiResponse handleGooglePayment(String  jsonPayload,string signature,
		       Action<KnetikApiResponse> cb = null
		     ) {
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("jsonPayload", jsonPayload);
			j.AddField ("signature", signature);
			return verifyGooglePayment(j , cb);
		}


		public KnetikApiResponse verifyGooglePayment(JSONObject j,Action<KnetikApiResponse> cb = null
			) {

			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(GooglePaymentEndpoint, body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

	

	}
}

