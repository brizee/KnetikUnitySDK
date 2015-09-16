using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using KnetikSimpleJSON;

namespace Knetik
{
	public partial class KnetikClient
	{
	
		public KnetikApiResponse getCurrencies(JSONObject j,Action<KnetikApiResponse> cb = null
			) {

			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(GetCurrenciesEndpoint, body,"GET");
			
			KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
			return  response;
		}

	

	}
}

