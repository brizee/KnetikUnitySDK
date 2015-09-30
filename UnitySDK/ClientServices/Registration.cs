using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace Knetik
{
    public partial class KnetikClient
	{
		public KnetikApiResponse Register(
			string username,
			string password,
			string email,
			string fullname,
			Action<KnetikApiResponse> cb = null
		) {
			// Then register the new user
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("username", username);
			j.AddField ("password", password);
			j.AddField ("email", email);
			j.AddField ("fullname", fullname);
			
			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(RegisterEndpoint, body);
			
			KnetikApiResponse registerResponse = new KnetikApiResponse(this, req, cb);
			return  registerResponse;
		}


		public KnetikApiResponse teamRockRegister(Dictionary<string,string> parameters,
			Action<KnetikApiResponse> cb = null
			) {

			JSONObject j = KnetikApiUtil.buildJSONObjectFromDictionary (parameters);
			
			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(TeamRockRegisterEndpoint, body);
			
			KnetikApiResponse registerResponse = new KnetikApiResponse(this, req, cb);
			return  registerResponse;
		}


        public KnetikApiResponse GuestRegister(
            Action<KnetikApiResponse> cb = null
            ) {
           
            
            // Then register the new user
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(GuestRegisterEndpoint, body);
            
            KnetikApiResponse registerResponse = new KnetikApiResponse(this, req, cb);
            return  registerResponse;
        }

        public KnetikApiResponse UpgradeFromRegisteredGuest(
            string username,
            string password,
            string email,
            string fullname,
            Action<KnetikApiResponse> cb = null
            ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("username", username);
            j.AddField ("password", password);
            j.AddField ("email", email);
            j.AddField ("fullname", fullname);
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(UpgradeFromRegisteredGuestEndpoint, body);
            
            KnetikApiResponse registerResponse = new KnetikApiResponse(this, req, cb);
            return  registerResponse;
        }
	}
}

