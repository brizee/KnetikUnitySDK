using System;
using UnityEngine;

namespace Knetik
{
    public partial class KnetikClient
	{
        public KnetikApiResponse CreateGameOption(int productId, string optionName, string optionValue, Action<KnetikApiResponse> cb = null)
		{
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("productId", productId);
            j.AddField ("optionName", optionName);
            j.AddField ("optionValue", optionValue);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CreateGameOptionEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
		}

        public KnetikApiResponse UpdateGameOption(int productId, string optionName, string optionValue, Action<KnetikApiResponse> cb = null)
		{
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("productId", productId);
            j.AddField ("optionName", optionName);
            j.AddField ("optionValue", optionValue);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(UpdateGameOptionEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
		}
	}
}

