using System;
using UnityEngine;

namespace Knetik
{
    public partial class KnetikClient
	{
		public KnetikApiResponse Login(
			string username,
			string password,
			Action<KnetikApiResponse> cb = null
		) {
			int timestamp = GetTimestamp ();
            string endpoint;
            string body;
            string serviceBundle = null;

            JSONObject json = new JSONObject (JSONObject.Type.OBJECT);
            json.AddField ("serial", KnetikApiUtil.getDeviceSerial());
            json.AddField ("mac_address", KnetikApiUtil.getMacAddress ());
            // Device Type is currently limited to 3 characters in the DB
            json.AddField ("device_type", KnetikApiUtil.getDeviceType().Substring(0, 3));
            json.AddField ("signature", KnetikApiUtil.getDeviceSignature());

            if (Authentication == null || Authentication == "" || Authentication == "default")
            {
                endpoint = SessionEndpoint;

                Username = username;
                Password = EncodePassword(password, timestamp);

            } else
            {
                // use SSO
                serviceBundle = Authentication;
                endpoint = "login";

                json.AddField("username", username);
                json.AddField("email", username);
                json.AddField("password", password);
            }

            body = json.Print ();
			
			KnetikRequest req = CreateRequest(endpoint, body, "post", timestamp, serviceBundle);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}
		
		public KnetikApiResponse GuestLogin(
			Action<KnetikApiResponse> cb = null
		) {
			Username = "";
			Password = "";
			Session = "";

            JSONObject json = new JSONObject (JSONObject.Type.OBJECT);
            json.AddField ("serial", KnetikApiUtil.getDeviceSerial());
            json.AddField ("mac_address", KnetikApiUtil.getMacAddress ());
            // Device Type is currently limited to 3 characters in the DB
            json.AddField ("device_type", KnetikApiUtil.getDeviceType().Substring(0, 3));
            json.AddField ("signature", KnetikApiUtil.getDeviceSignature());
			
			String body = json.Print();
			
			KnetikRequest req = CreateRequest(SessionEndpoint, body);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}
	}
}

