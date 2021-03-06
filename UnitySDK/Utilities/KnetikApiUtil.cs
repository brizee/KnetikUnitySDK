﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

#if !UNITY_WEBPLAYER
using System.Net.NetworkInformation;
#endif

namespace Knetik
{
	public class KnetikApiUtil
	{
		public static string API_VERSION = "1.0";

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
					+ API_VERSION;
			
			return signature;    
		}
		
#if UNITY_ANDROID && !UNITY_EDITOR
        public static string getDeviceSerial() 
        {
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            return clsSecure.CallStatic<string>("getString", objResolver, "android_id");
        }
        
        private static AndroidJavaObject mWiFiManager;
        
        public static string getMacAddress()
        {
            string macAddr = "";
            if (mWiFiManager == null)
            {
                using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    mWiFiManager = activity.Call<AndroidJavaObject>("getSystemService","wifi");
                }
            }
            macAddr = mWiFiManager.Call<AndroidJavaObject>("getConnectionInfo").Call<string>("getMacAddress");
            
            return macAddr;
        }
        
        public static string getDeviceType() 
        {
            return "MOB";
        }
        
#elif !UNITY_WEBPLAYER
        // Pulls the serial number (physical address) of the device being used
        public static string getDeviceSerial() 
		{
			NetworkInterface adapter = getActiveNetwork();

			if (adapter == null) {
				return null;
			}
			Debug.Log(adapter.Description);	
			Debug.Log("  Interface type .......................... : " + adapter.NetworkInterfaceType);
			Debug.Log("  Physical Address ........................ : " + adapter.GetPhysicalAddress().ToString());
			Debug.Log("  Is receive only.......................... : " + adapter.IsReceiveOnly);
			Debug.Log("  Multicast................................ : " + adapter.SupportsMulticast);
			return adapter.GetPhysicalAddress().ToString();
		}
		
		public static string getMacAddress()
		{
			NetworkInterface adapter = getActiveNetwork();
			
			if (adapter == null) {
				return "";
			}
			
			PhysicalAddress address = adapter.GetPhysicalAddress();
			byte[] bytes = address.GetAddressBytes();
			string mac = null;
			for (int i = 0; i < bytes.Length; i++)
			{
				mac = string.Concat(mac +(string.Format("{0}", bytes[i].ToString("X2"))));
			}
			Debug.Log("MAC: " + mac);
			return mac;
		}

        // Pulls the type of the device being used
        public static string getDeviceType()
        {
            Debug.Log("Device type: " + SystemInfo.deviceType);

            switch (SystemInfo.deviceType)
            {
                case DeviceType.Console:
                    return "STB";
                case DeviceType.Handheld:
                    return "MOB";
                case DeviceType.Desktop:
                    return "PC";
                case DeviceType.Unknown:
                    return "PC";
            }

            return "PC";
        }

		private static NetworkInterface getActiveNetwork()
		{
			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
			NetworkInterface result = null;
			long highestSpeed = -1;

			foreach (NetworkInterface adapter in nics) {
				if (adapter.Speed > highestSpeed && adapter.GetPhysicalAddress().ToString() != "") {
					result = adapter;
					highestSpeed = adapter.Speed;
				}
			}
			return result;
		}
#else
        // Pulls the serial number (physical address) of the device being used
        public static string getDeviceSerial() 
        {
            return "0:0:0:0";
        }
        
        public static string getMacAddress()
        {
            return "0:0:0:0";
        }
        
        // Pulls the type of the device being used
        public static string getDeviceType() 
        {
            return "WEB";
        }
#endif
		
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

		/*
			build string request from Dictionary 
			
		 */
		public static string buildStringRequestFromDictionary(Dictionary<string,string> parameters)
		{
			StringBuilder builder = new StringBuilder ();
			List<string> keysList = new List<string>(parameters.Keys);

			for (int i=0; i<keysList.Count; i++) {
				string key=keysList[i];
				string value=parameters[key];
				if(value != null)
				{
					builder.Append(key);
					builder.Append("=");
					builder.Append(value);
					if(i<keysList.Count-1){
					builder.Append("&");
					}
				}

			}
			return builder.ToString ();
		}

		/*
			build string request from Dictionary 
			
		 */
		public static JSONObject buildJSONObjectFromDictionary(Dictionary<string,string> parameters)
		{
			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			List<string> keysList = new List<string>(parameters.Keys);
			
			for (int i=0; i<keysList.Count; i++) {
				string key=keysList[i];
				string value=parameters[key];

				if(value != null)
				{
					j.AddField (key, value);

				}
				
			}
			return j;
		}



	}
}
