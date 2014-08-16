using System;

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
	}
}

