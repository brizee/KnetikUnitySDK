using System;
using System.Collections;
using UnityEngine;
using KnetikSimpleJSON;
using System.Collections.Generic;

namespace Knetik
{
	public class KnetikApiRequest
	{
		private bool currentResult;
		private string url;
		private byte[] data;
		private Hashtable headers;
        private string envelope = null;
		
		protected string m_endpointUrl = null;
		protected string m_clientId = null;
		protected string m_clientSecret = null;
		protected string m_method = null;
//		protected string m_gameOptionName = null;
//		protected string m_gameOptionValue = null;
//		protected string m_avatarUrl = null;
//		protected string m_lang = null;
//		protected string m_hashId = null;
		protected string m_errorMsg = null;
		protected static int m_userId = 0;
//		protected long m_score = 0;
//		protected long m_productId = 0;
//		protected long m_levelId = 0;
//		protected long m_leaderboardId = 0;

        protected string m_requestString = null;
        protected int timestamp = 0;
        protected string ipAddress = null;
        protected static string m_username = null;
        protected static string m_password = null;
        protected string m_signature = null;
        protected static string m_session = "";
        protected bool m_register = false;
              

        // Build JSON Envelope for all Requests
        string createJSAPIEnvelope()
        {
            ipAddress = Network.player.ipAddress;
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("request", m_requestString.Replace("\"", "\\\""));
            j.AddField ("timestamp", timestamp);
            j.AddField ("clientId", KnetikApiUtil.API_CLIENT_KEY);
            j.AddField ("ip_address", ipAddress);
            j.AddField ("username", m_username);
            j.AddField ("password", m_password);
            j.AddField ("signature", m_signature);
            j.AddField ("session", m_session);
            envelope = j.Print ();
            return envelope;    
        }

		// Build up and send off the request envelope to jSAPI
		public bool sendApiRequest(string request_str, ref KnetikJSONNode jsonRes) 
		{
            m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
            m_clientId = KnetikApiUtil.API_CLIENT_KEY;
            m_requestString = request_str;
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            timestamp = (int) t.TotalSeconds;  // Convert to int

            if (m_register)
            {
                m_username = "";
                m_password = "";
            }
            else
            {
                m_password = KnetikApiUtil.sha1(KnetikApiUtil.sha1(m_password) + timestamp.ToString()).ToUpper();
            }
            
            if (m_username == "")
            {
                // For a guest session, ensure that the session is not yet defined or we will reuse the last one
                m_session = "";
            }
            
            m_signature = signRequest();
			createJSAPIEnvelope();
			System.Text.ASCIIEncoding encoding=new System.Text.ASCIIEncoding();
	    	data = encoding.GetBytes(envelope);
            Debug.Log("Knetik Labs SDK - Signed url request: " + m_endpointUrl);
            List<string> headers = new List<string>();
            headers.Add("Host: " + KnetikApiUtil.API_HOST);
            headers.Add("Content-type: application/json");
            headers.Add("User-Agent: Unity Knetik SDK");

            KnetikHTTP.KnetikRequest theRequest = new KnetikHTTP.KnetikRequest("post", m_endpointUrl, data);
			theRequest.synchronous = true;
			theRequest.Send();
            Debug.Log ("Envelope: " + envelope);

			if (theRequest.response == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 1: The response from SAPI is null.");
				return false;
			}

			string strResp = theRequest.response.Text;

			if (theRequest.response.status != 200) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 2: Response returned a status of " + theRequest.response.status);
				return false;
			}
			 
			try 
			{
				jsonRes = KnetikJSON.Parse(strResp);

				if (jsonRes == null) 
				{
					Debug.LogError("Knetik Labs SDK - ERROR 3: Failed to Properly Parse JSON response");
					return false;
				}
			} 
			catch(Exception e) 
			{
				Debug.LogException(e);
				return false;
			}

		    if (jsonRes["error"] == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 4: JSON Response does NOT contain an error node!");
				return false;
			}

			KnetikJSONNode error = jsonRes["error"];
			
		    if ((error["success"] == null) || (error["success"].AsBool == false)) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 5: Response JSON does NOT report success!");
				m_errorMsg = jsonRes[3];
		        return false;
		    }

			return true;
		}

		// Builds the Signature header for a request
		private string signRequest() 
		{
            string req_text = m_requestString + timestamp.ToString() + m_clientId + m_username + m_password;
			Debug.Log("Knetik Labs SDK - Signature Request text: '" + req_text +"'");
			// This is automatically base64 encoded by the hashHmac function
			string sign = KnetikApiUtil.hashHmac(req_text, m_clientSecret);
			return sign;
		}
		
		public string getErrorMessage() 
		{
			return m_errorMsg;
		}
	}
}

