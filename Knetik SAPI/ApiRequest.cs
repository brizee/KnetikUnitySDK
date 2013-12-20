using System;
using System.Collections;
using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

namespace Knetik
{
	public class ApiRequest
	{
		private bool currentResult;
	
		private string url;
		private byte[] data;
		private Hashtable headers;
		
		protected string m_url = null;
		protected string m_Key = null;
		protected string m_clientSecret = null;
		protected string m_method = null;
		protected string m_productId = null;
		protected string m_optionName = null;
		protected string m_optionValue = null;
		
		protected string m_errorMsg = "";
		
		public ApiRequest()
		{
		}
		
		public bool sendApiRequest(string request_str, ref JSONNode jsonRes) {
			Debug.Log("Request = " + request_str);
			
			System.Text.ASCIIEncoding encoding=new System.Text.ASCIIEncoding();
	    	data = encoding.GetBytes(request_str);

	        Debug.Log("m_url = " + m_url);
	        Debug.Log("data = " + data);
			
			HTTP.Request theRequest = new HTTP.Request("post", m_url, data);
			theRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			theRequest.synchronous = true;
			theRequest.Send();

	        Debug.Log("theRequest = " + theRequest);
			if (theRequest.response == null) {
		        Debug.Log("RESPONSE IS NULL!!!!");
				return false;
			}
	        Debug.Log("theRequest.response = " + theRequest.response);
			
			
			string strResp = theRequest.response.Text;
			Debug.Log("Response: (" + theRequest.response.status + "): " + strResp);

			if (theRequest.response.status != 200) {
				return false;
			}
			 
			try {
		    	jsonRes = JSON.Parse(strResp);
				if (jsonRes == null) {
					Debug.Log("json parse failed");
					return false;
				}
			} catch(Exception e) {
				Debug.Log("Json parse Exception: " + e);
				return false;
			}

		    if (jsonRes["error"] == null) {
				Debug.Log("No error node!");
				return false;
			}
			JSONNode error = jsonRes["error"];
			
		    if ((error["success"] == null) || (error["success"].AsBool == false)) {
				Debug.Log("json success is false");
				m_errorMsg = error["message"].Value;
		        return false;
		    }

			return true;
		}
		
		private string signRequest(string query, List<string> parameters, List<string> headers, string body) {
			string req_text = query.Replace("https", "http");
			parameters.Sort();
			headers.Sort();
			
			req_text += "\n";
			req_text += String.Join("\n", parameters.ToArray());
			if (body != null) {
				req_text += "\n";
				req_text += body;
			}
			req_text += "\n";
			req_text += String.Join("\n", headers.ToArray());
			
		    Debug.Log("Rquest text: '" + req_text +"'");
			
			string req_hash = ApiUtil.md5(req_text) + m_clientSecret;
			
		    Debug.Log("Md5 hash text: '" + req_hash +"'");
			
			string sign = ApiUtil.hashHmac(req_hash, m_clientSecret);
			
			return sign;
		}
		
		public bool sendSignedRequest(ref JSONNode jsonRes) {
			return sendSignedRequest(null, null, ref jsonRes);
		}
		
		public bool sendSignedRequest(List<string> paramArray, string postBody, ref JSONNode jsonRes) {
			TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
			string timestamp = t.ToString();

			List<string> parameters = (paramArray!=null) ? paramArray : new List<string>();
			parameters.Add("key=" + m_Key);
			parameters.Add("timestamp=" + timestamp);
	
			string url_params = String.Join("&", parameters.ToArray());
			string url = m_url + "?" + url_params;

			Debug.Log("Signed url request: " + url);

			List<string> headers = new List<string>();
			headers.Add("Host: " + ApiUtil.API_HOST);
			headers.Add("Content-type: application/json");
			headers.Add("User-Agent: Unity Knetik SDK");
			
			
				
			HTTP.Request theRequest;

			if (postBody != null) {
				System.Text.ASCIIEncoding encoding=new System.Text.ASCIIEncoding();
				headers.Add("Content-Length: " + postBody.Length);
		    	data = encoding.GetBytes(postBody);
				theRequest = new HTTP.Request((m_method != null) ? m_method : "post", url, data);
				theRequest.AddHeader("Signature", signRequest(m_url, parameters, headers, postBody));
				//theRequest.AddHeader("Content-length", postBody.Length.ToString());
			} else {
				theRequest = new HTTP.Request((m_method != null) ? m_method : "get", url);
				theRequest.AddHeader("Signature", signRequest(m_url, parameters, headers, null));
			}

			theRequest.AddHeader("Content-type", "application/json");
			//theRequest.AddHeader("connection", "Close");
			theRequest.AddHeader("User-Agent", "Unity Knetik SDK");
			theRequest.synchronous = true;
			theRequest.Send();
			
			
			string strResp = theRequest.response.Text;
			Debug.Log("Response: (" + theRequest.response.status + "): " + strResp);
			
			if (theRequest.response.status != 200) {
				return false;
			}
			 
			
			try {
		    	jsonRes = JSON.Parse(strResp);
				if (jsonRes == null) {
					Debug.Log("json parse failed");
					return false;
				}
			} catch(Exception e) {
				Debug.Log("Json parse Exception: " + e);
				return false;
			}

		    if (jsonRes["error"] == null) {
				Debug.Log("No error node!");
				return false;
			}
			JSONNode error = jsonRes["error"];
			
		    if ((error["success"] == null) || (error["success"].AsBool == false)) {
				Debug.Log("json success is false");
				m_errorMsg = error["message"].Value;
		        return false;
		    }

			return true;
		}
		
		public string getErrorMessage() {
			return m_errorMsg;
		}
	}
}

