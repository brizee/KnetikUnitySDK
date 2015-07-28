using System;
using UnityEngine;

namespace Knetik
{
    public partial class KnetikClient
	{
        public KnetikApiResponse GetGameOption(
            int gameId,
            string optionName,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("productId", gameId);
            j.AddField ("optionName", optionName);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(GetGameOptionEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return response;
        }

        public KnetikApiResponse CreateUserGameOption(
            int gameId,
            string optionName,
            string optionValue,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("productId", gameId);
            j.AddField ("optionName", optionName);
            j.AddField ("optionValue", optionValue);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(CreateGameOptionEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return response;
		}

        public KnetikApiResponse UpdateUserGameOption(
            int gameId,
            string optionName,
            string optionValue,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("productId", gameId);
            j.AddField ("optionName", optionName);
            j.AddField ("optionValue", optionValue);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(UpdateGameOptionEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return response;
		}
	}
}

