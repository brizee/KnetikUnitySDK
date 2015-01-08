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
			Username = username;
			Password = EncodePassword(password, timestamp);
			
			String body = BuildLoginBody ();
			
			KnetikRequest req = CreateRequest(SessionEndpoint, body, "post", timestamp);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}
		
		public KnetikApiResponse GuestLogin(
			Action<KnetikApiResponse> cb = null
		) {
			Username = "";
			Password = "";
			Session = "";
			
			String body = BuildLoginBody ();
			
			KnetikRequest req = CreateRequest(SessionEndpoint, body);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}

		private string BuildLoginBody()
		{
			JSONObject json = new JSONObject (JSONObject.Type.OBJECT);
			json.AddField ("serial", KnetikApiUtil.getDeviceSerial());
			json.AddField ("mac_address", KnetikApiUtil.getMacAddress ());
			// Device Type is currently limited to 3 characters in the DB
            json.AddField ("device_type", KnetikApiUtil.getDeviceType().Substring(0, 3));
			json.AddField ("signature", KnetikApiUtil.getDeviceSignature());
			
			return json.Print ();
		}
	}
}

