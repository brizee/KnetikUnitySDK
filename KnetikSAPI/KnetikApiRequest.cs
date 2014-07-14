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
		
		protected string m_url = null;
		protected string m_key = null;
		protected string m_clientSecret = null;
		protected string m_method = null;
		protected string m_gameOptionName = null;
		protected string m_gameOptionValue = null;
		protected string m_avatarUrl = null;
		protected string m_lang = null;
		protected string m_hashId = null;
		protected string m_errorMsg = null;
        protected string m_username = null;
        protected string m_password = null;
        
		protected static long m_userId = 0;
		protected long m_score = 0;
		protected long m_productId = 0;
		protected long m_levelId = 0;
		protected long m_leaderboardId = 0;

		// Called by KnetikLoginRequest to connect to SAPI initially
		public bool sendApiRequest(string request_str, ref KnetikJSONNode jsonRes) 
		{
			System.Text.ASCIIEncoding encoding=new System.Text.ASCIIEncoding();
	    	data = encoding.GetBytes(request_str);

			KnetikHTTP.KnetikRequest theRequest = new KnetikHTTP.KnetikRequest("post", m_url, data);
			theRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			theRequest.synchronous = true;
			theRequest.Send();

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
				m_errorMsg = error["message"].Value;
		        return false;
		    }

			return true;
		}

		// Builds the Signature header for a request
		private string signRequest(string query, List<string> parameters, List<string> headers, string body) 
		{
			string req_text = query.Replace("https", "http");
			parameters.Sort();
			headers.Sort();
			req_text += "\n";
			req_text += String.Join("\n", parameters.ToArray());

			if (body != null) 
			{
				req_text += "\n";
				req_text += body;
			}

			req_text += "\n";
			req_text += String.Join("\n", headers.ToArray());
			Debug.Log("Knetik Labs SDK - Signature Request text: '" + req_text +"'");
			string req_hash = KnetikApiUtil.md5(req_text) + m_clientSecret;
			string sign = KnetikApiUtil.hashHmac(req_hash, m_clientSecret);
			return sign;
		}

		// If no parameters or post body is part of the request
		public bool sendSignedRequest(ref KnetikJSONNode jsonRes) 
		{
			return sendSignedRequest(null, null, ref jsonRes);
		}

		// Builds up request to send to SAPI
		public bool sendSignedRequest(List<string> paramArray, string postBody, ref KnetikJSONNode jsonRes) 
		{
			TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
			string timestamp = t.ToString();
			List<string> parameters = (paramArray!=null) ? paramArray : new List<string>();
			parameters.Add("key=" + m_key);
			parameters.Add("timestamp=" + timestamp);
			string url_params = String.Join("&", parameters.ToArray());
			string url = m_url + "?" + url_params;
			Debug.Log("Knetik Labs SDK - Signed url request: " + url);
			List<string> headers = new List<string>();
			headers.Add("Host: " + KnetikApiUtil.API_HOST);
			headers.Add("Content-type: application/json");
			headers.Add("User-Agent: Unity Knetik SDK");
			KnetikHTTP.KnetikRequest theRequest;

			if (postBody != null) 
			{
				System.Text.ASCIIEncoding encoding=new System.Text.ASCIIEncoding();
				headers.Add("Content-Length: " + postBody.Length);
		    	data = encoding.GetBytes(postBody);
				theRequest = new KnetikHTTP.KnetikRequest((m_method != null) ? m_method : "post", url, data);
				theRequest.AddHeader("Signature", signRequest(m_url, parameters, headers, postBody));
			} 
			else 
			{
				theRequest = new KnetikHTTP.KnetikRequest((m_method != null) ? m_method : "get", url);
				theRequest.AddHeader("Signature", signRequest(m_url, parameters, headers, null));
			}

			theRequest.AddHeader("Content-type", "application/json");
			theRequest.AddHeader("User-Agent", "Unity Knetik SDK");
			theRequest.synchronous = true;
			theRequest.Send();
			string strResp = theRequest.response.Text;
			Debug.Log("Knetik Labs SDK - Signed Response: (" + theRequest.response.status + "): " + strResp);

			if (theRequest.response == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 1: The response from SAPI is null.");
				return false;
			}
			
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
					Debug.LogError("Knetik Labs SDK - ERROR 3: Failed to Properly Parse JSON");
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
				m_errorMsg = error["message"].Value;
				return false;
			}

			return true;
		}
		
		public string getErrorMessage() 
		{
			return m_errorMsg;
		}
	}
}

