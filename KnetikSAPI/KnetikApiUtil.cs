using System;
using UnityEngine;
using UnityEditor;
using System.Net.NetworkInformation;
using System.IO;
using System.Collections;
using KnetikSimpleJSON;
using System.Text;

namespace Knetik
{
	public class KnetikApiUtil
	{
		private static string API_Version = null;

		public static string API_HOST = null;
		public static string API_URL = null;
		public static string API_CLIENT_KEY = null;
		public static string API_CLIENT_SECRET = null;
		public static string ENDPOINT_PREFIX = null;
        public static string AUTH_PREFIX = null;
		public static string SESSION_ENDPOINT = null;
		public static string LEADERBOARD_ENDPOINT = null;
		public static string METRIC_ENDPOINT = null;
		public static string USER_ENDPOINT = null;
		public static string PRODUCT_ENDPOINT = null;

		// Sets up all configuration based on JSON file
		public static void readConfig()
		{
			StringBuilder sb = new StringBuilder ();
			using (StreamReader sr = new StreamReader("Assets/Class/SAPI/KnetikConfig.json")) 
			{
				string line;
				while ((line = sr.ReadLine()) != null) 
				{
						sb.AppendLine (line);
				}
			}
			string allLines = sb.ToString ();
			KnetikJSONNode configJsonNode = KnetikJSON.Parse (allLines);
			API_Version = configJsonNode ["version"];
			API_HOST = configJsonNode ["apiHost"];
			API_URL = configJsonNode ["urlPrefix"] + API_HOST;
			API_CLIENT_KEY = configJsonNode ["clientKey"];
			API_CLIENT_SECRET = configJsonNode ["clientSecret"];
			ENDPOINT_PREFIX = configJsonNode ["endpointPrefix"];
            AUTH_PREFIX = configJsonNode ["authPrefix"];
			SESSION_ENDPOINT = configJsonNode ["sessionEndpoint"];
			LEADERBOARD_ENDPOINT = configJsonNode ["leaderboardEndpoint"];
			METRIC_ENDPOINT = configJsonNode ["metricEndpoint"];
			USER_ENDPOINT = configJsonNode ["userEndpoint"];
			PRODUCT_ENDPOINT = configJsonNode ["productEndpoint"];
		}

		// Determines the type of device being used
		public static string getDeviceSignature() 
		{
			string platformType = "Unknown";
			string platformName = "Unknown";
			string platformVersion;
			platformVersion = "Unknown";
	
			switch(Application.platform) 
			{
				case RuntimePlatform.OSXEditor: //	 In the Unity editor on Mac OS X.
				case RuntimePlatform.OSXPlayer: //	 In the player on Mac OS X.
				case RuntimePlatform.OSXDashboardPlayer: //	 In the Dashboard widget on Mac OS X.
					platformType = "PC";
					platformName = "Mac";
					break;
				case RuntimePlatform.WindowsPlayer: //	 In the player on Windows.
				case RuntimePlatform.WindowsEditor: //	 In the Unity editor on Windows.
					platformType = "PC";
					platformName = "Windows";
					break;
				case RuntimePlatform.WindowsWebPlayer: //	 In the web player on Windows.
					platformType = "WEB";
					platformName = "Windows";
					break;
				case RuntimePlatform.OSXWebPlayer: //	 In the web player on Mac OS X.
					platformType = "WEB";
					platformName = "Mac";
					break;
//				case RuntimePlatform.WiiPlayer: //	 In the player on Nintendo Wii.
//					platformType = "PC";
//					platformName = "Mac";
//					break;
				case RuntimePlatform.IPhonePlayer: //	 In the player on the iPhone.
					platformType = "MOBILE";
					platformName = "iOS";
					break;
				case RuntimePlatform.XBOX360: //	 In the player on the XBOX360.
					platformType = "MOBILE";
					platformName = "XBOX360";
					break;
				case RuntimePlatform.PS3: //	 In the player on the Play Station 3.
					platformType = "MOBILE";
					platformName = "PS3";
					break;
				case RuntimePlatform.Android: //	 In the player on Android devices.
					platformType = "MOBILE";
					platformName = "Android";
					break;
				case RuntimePlatform.NaCl: //	 Google Native Client.
					platformType = "MOBILE";
					platformName = "NaCl";
					break;
				case RuntimePlatform.FlashPlayer: //	 Flash Player.			
					platformType = "WEB";
					platformName = "Flash";
					break;
			}
			
			string signature = platformType 
				+ "-"
				+ "Unity"
				+ "-"
				+ platformVersion
				+ "-"
				+ platformName
				+ "-"
				+ API_Version;
		    
		    return signature;    
		}
	
		// Pulls the serial number (physical address) of the device being used
		public static string getDeviceSerial() 
		{
	        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
	
			if (nics.Length == 0) {
	        	Debug.LogWarning("No Network interface found! Using default 00000");
				return "00000";
			}
			
			NetworkInterface adapter = nics[nics.Length - 1];
            Debug.Log(adapter.Description);	
            Debug.Log("  Interface type .......................... : " + adapter.NetworkInterfaceType);
            Debug.Log("  Physical Address ........................ : " + adapter.GetPhysicalAddress().ToString());
            Debug.Log("  Is receive only.......................... : " + adapter.IsReceiveOnly);
            Debug.Log("  Multicast................................ : " + adapter.SupportsMulticast);
			return adapter.GetPhysicalAddress().ToString();
		}
	
		// SHA1 Encryption
		public static string sha1(string input) 
		{
			System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = sha1.ComputeHash(inputBytes);
			string result = "";

			for (int i = 0; i < hash.Length; i++) 
			{
				result += hash[i].ToString("X2");
			}
			
			return result.ToLower();			
		}

		// MD5 Sum Encryption
		public static string md5(string input) 
		{
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);
			string result = "";

			for (int i = 0; i < hash.Length; i++) 
			{
				result += hash[i].ToString("X2");
			}
			
			return result.ToLower();			
		}

		// Hash Encryption
		public static string hashHmac(string input, string key) 
		{
			byte[] keyBytes = System.Text.Encoding.ASCII.GetBytes(key);
			System.Security.Cryptography.HMACSHA256 hashmac = new System.Security.Cryptography.HMACSHA256(keyBytes);
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			hashmac.Key = keyBytes;
			byte[] hash = hashmac.ComputeHash(inputBytes);
			return Convert.ToBase64String(hash);			
		}
	}
}

