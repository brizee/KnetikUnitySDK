using System;
using UnityEngine;
using System.Net.NetworkInformation;

namespace Knetik
{
	public class ApiUtil
	{
		private static string API_Version = "1.0";
		//public static string API_HOST = "sapi.dev.rsigaming.com";
		
		
		public static string API_HOST = null;
		public static string API_URL = null;
		public static string API_CLIENT_KEY = null;
		public static string API_CLIENT_SECRET = null;

		public ApiUtil ()
		{
		}
		
		public static void setApiHost(string host) {
			API_HOST = host;
//			API_URL = "https://" + API_HOST;
			API_URL = "http://" + API_HOST;
		}
		
		public static void setClientKey(string key) {
			API_CLIENT_KEY = key;
		}
		
		public static void setClientSecret(string secret) {
			API_CLIENT_SECRET = secret;
		}

		public static string getDeviceSignature() {
		
			string platformType = "Unknown";
			string platformName = "Unknown";
			string platformVersion;
		
			platformVersion = "Unknown";
	
			switch(Application.platform) {
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
				case RuntimePlatform.WiiPlayer: //	 In the player on Nintendo Wii.
					platformType = "PC";
					platformName = "Mac";
					break;
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
	
		public static string getDeviceSerial() {
	        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
	
			if (nics.Length == 0) {
	        	Debug.Log("No Network interface found!");
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
	
		public static string sha1(string input) {
			System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = sha1.ComputeHash(inputBytes);
			
			string result = "";
			for (int i = 0; i < hash.Length; i++) {
				result += hash[i].ToString("X2");
			}
			
			return result.ToLower();			
		}
		
		public static string md5(string input) {
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);
			
			string result = "";
			for (int i = 0; i < hash.Length; i++) {
				result += hash[i].ToString("X2");
			}
			
			return result.ToLower();			
		}
		
		public static string hashHmac(string input, string key) {
			byte[] keyBytes = System.Text.Encoding.ASCII.GetBytes(key);
			System.Security.Cryptography.HMACSHA256 hashmac = new System.Security.Cryptography.HMACSHA256(keyBytes);
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			hashmac.Key = keyBytes;
			byte[] hash = hashmac.ComputeHash(inputBytes);
			
			return Convert.ToBase64String(hash);			
		}
	}
}

