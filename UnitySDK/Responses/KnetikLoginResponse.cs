using System;
using UnityEngine;
using KnetikSimpleJSON;

namespace Knetik
{
	public class KnetikLoginResponse : KnetikApiResponse
	{
        public KnetikLoginResponse (KnetikClient client, KnetikRequest req, Action<KnetikApiResponse> cb = null)
			: base(client, req, cb)
		{
		}

		protected override void ValidateResponse(KnetikRequest req)
		{
			base.ValidateResponse(req);

			if (Status != StatusType.Success) {
				return;
			}

			KnetikJSONNode result = Body ["access_token"];
			KnetikJSONNode refreshToken = Body ["refresh_token"];
			if (result == null || result.Value == "null") {
				Debug.LogError("Knetik Labs SDK - ERROR 304: Existing User could not be successfully logged in, server has no response!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + req);
                ErrorMessage = "Connection error - Invalid access token";
				Status = StatusType.Error;
			}

			Debug.Log ("Existing User successfully logged in.");
			Client.AccessToken = result.Value;
			if (refreshToken != null)
				Client.RefreshToken = refreshToken.Value;
//            if (result["user_id"] == null || result["user_id"].Value == "null") {
//				Debug.Log ("Guest Session has been established.");
//			} else {
//				Debug.Log ("Authenticated Session has been established. (" + result["user_id"] + ")");
//				Client.UserID = result["user_id"].AsInt;
//			}
		}
	}
}

