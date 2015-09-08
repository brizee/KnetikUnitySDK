using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using KnetikSimpleJSON;

namespace Knetik
{
	public partial class KnetikClient
	{
	
		/*Pay invoice with Apple receipt
		 * 
		 *@param String receipt  A receipt will only be accepted once 
		 *@parm transactionId    details of the transaction must match the invoice, 
		 *@parm  invoiceId
		 * Returns the transaction Id if successful.
		 */

		public KnetikApiResponse verifyReceipt(String  recipt,string transactionId,
		       Int64 invoiceId, Action<KnetikApiResponse> cb = null
		     ) {
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("recipt", recipt);
			j.AddField ("transactionId", transactionId);
			j.AddField ("invoiceId", invoiceId);
			return verifyReceipt(j , cb);
		}


		public KnetikApiResponse verifyReceipt(JSONObject j,Action<KnetikApiResponse> cb = null
			) {

			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(GetReceiptEndpoint, body);
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

	

	}
}

