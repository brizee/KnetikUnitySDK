using System;
using UnityEngine;
using System.Text;

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
            string body;
            string serviceBundle = null;

            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.AppendFormat(
				"grant_type=password&username={0}&password={1}&client_id={2}&client_secret={3}",
                System.Uri.EscapeDataString(username),
                System.Uri.EscapeDataString(password),
                System.Uri.EscapeDataString(ClientID),
				System.Uri.EscapeDataString(ClientSecret)
            );
            body = bodyBuilder.ToString();
			
			KnetikRequest req = CreateRequest(SessionEndpoint, body, "post", timestamp, serviceBundle, true);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}

		/*
			Login With custome Grant Type 
			@param string username
			@param string password 
			@param string grantType
		 */

		public KnetikApiResponse Login(
			string username,
			string password,string grantType,
			Action<KnetikApiResponse> cb = null
			) {
			int timestamp = GetTimestamp ();
			string body;
			string serviceBundle = null;
			
			StringBuilder bodyBuilder = new StringBuilder();
			bodyBuilder.AppendFormat(
				"grant_type={0}&username={1}&password={2}&client_id={3}&client_secret={4}",
				System.Uri.EscapeDataString(grantType),
				System.Uri.EscapeDataString(username),
				System.Uri.EscapeDataString(password),
				System.Uri.EscapeDataString(ClientID),
				System.Uri.EscapeDataString(ClientSecret)
				);
			body = bodyBuilder.ToString();
			
			KnetikRequest req = CreateRequest(SessionEndpoint, body, "post", timestamp, serviceBundle, true);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}


		public KnetikApiResponse refreshSession(
			Action<KnetikApiResponse> cb = null
			) {
			int timestamp = GetTimestamp ();
			string body;
			string serviceBundle = null;
			
			StringBuilder bodyBuilder = new StringBuilder();
			bodyBuilder.AppendFormat(
				"grant_type=refresh_token&refresh_token={0}&client_id={1}&client_secret={2}",
				System.Uri.EscapeDataString(KnetikClient.Instance.RefreshToken),
				System.Uri.EscapeDataString(ClientID),
				System.Uri.EscapeDataString(ClientSecret)
				);
			body = bodyBuilder.ToString();
			
			KnetikRequest req = CreateRequest(SessionEndpoint, body, "post", timestamp, serviceBundle, true);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}


		public KnetikApiResponse GuestLogin(
			Action<KnetikApiResponse> cb = null
		) {
            KnetikApiResponse res = new KnetikApiResponse(KnetikClient.Instance, null);
			if (cb != null)
            {
                cb(res);
            }
            return res;
		}
	}


}

