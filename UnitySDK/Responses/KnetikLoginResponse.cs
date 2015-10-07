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

			KnetikJSONNode accessToken = Body ["access_token"];
			if (accessToken == null || accessToken.Value == "null") {
				Debug.LogError("Knetik Labs SDK - ERROR 304: Existing User could not be successfully logged in, server has no response!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + req);
                ErrorMessage = "Connection error - Invalid access token";
				Status = StatusType.Error;
			}

			Debug.Log ("Existing User successfully logged in.");
			Client.AccessToken = accessToken.Value;

			KnetikJSONNode refreshToken = Body ["refresh_token"];
			if (refreshToken == null || refreshToken.Value == "null") {
				ErrorMessage = "Connection error - Invalid refresh token";
				Status = StatusType.Error;
			} 
			else
			{
				Debug.Log ("Refresh Token Set.");
				DateTime expirationDate = DateTime.Now;

				int secondsLeft = Body["expires_in"].AsInt;
				Debug.Log("expires in "+secondsLeft);

				expirationDate = expirationDate.AddSeconds(secondsLeft);
				Debug.Log("expires at "+expirationDate);

				Client.ExpirationDate = expirationDate;
				Client.RefreshToken = refreshToken.Value;
			}

//            if (result["user_id"] == null || result["user_id"].Value == "null") {
//				Debug.Log ("Guest Session has been established.");
//			} else {
//				Debug.Log ("Authenticated Session has been established. (" + result["user_id"] + ")");
//				Client.UserID = result["user_id"].AsInt;
//			}
		}
	}
}

