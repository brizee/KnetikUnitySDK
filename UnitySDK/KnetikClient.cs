using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.Collections.Generic;

namespace Knetik
{
    public partial class KnetikClient
    {
        #region Properties

        private static KnetikClient _main;
        public static KnetikClient Instance {
            get {
                if (_main == null) {
                    _main = new KnetikClient();
                }
                return _main;
            }
        }

        public delegate void KnetikEventSuccessDelegate();
        public delegate void KnetikEventFailDelegate(string message);
        public string Username {
            get;
            set;
        }

        public string Password {
            get;
            set;
        }

        public string AccessToken {
            get;
            set;
        }

        public int UserID {
            get;
            set;
        }

        public string BaseURL {
            get;
            set;
        }

        public string ClientID {
            get;
            set;
        }

        public string ClientSecret {
            get;
            set;
        }

        public string Authentication
        {
            get;
            set;
        }
        /// <summary>
        /// True if the user has any type of account, Guest or Regular
        /// </summary>
        public bool isRegistered
        {
            get
            {
                return !string.IsNullOrEmpty(AccessToken);
            }
        }
        /// <summary>
        /// True if the user has a Guest account, is !isRegistered behaviour is undefined
        /// </summary>
        public bool isGuest
        {
            get
            {
				if(string.IsNullOrEmpty(Username))
				{
					return false;
				}
				else if(Username.Length < 5)
				{
					return false;
				}
				else
				{
                	return Username.Substring(0, 5).ToLower() == "guest";
				}
            }
        }
        #endregion

        #region Public Methods

        public event KnetikEventSuccessDelegate OnLogoutComplete;

        public void Logout()
        {
            Username = null;
            Password = null;
            AccessToken = null;
            UserID = 0;
            _userInfo = new UserInfo(this);

            PlayerPrefs.DeleteKey(AccessTokenKey);
            PlayerPrefs.DeleteKey(UserIDKey);
            if(OnLogoutComplete != null)
            {
                OnLogoutComplete();
            }
        }

        public void CallUserInfoLoaded()
        {
            if (OnUserInfoLoaded != null)
                OnUserInfoLoaded();
        }
        public bool SaveSession()
        {
            if (!(AccessToken != null && AccessToken != ""))
            {
                return false;
            }
            PlayerPrefs.SetString(AccessTokenKey, AccessToken);
            PlayerPrefs.SetInt(UserIDKey, UserID);
            PlayerPrefs.SetString(UsernameKey, Username);
            PlayerPrefs.Save();
            return true;
        }

        internal bool LoadSession()
        {
            AccessToken = PlayerPrefs.GetString(AccessTokenKey);
            UserID = PlayerPrefs.GetInt(UserIDKey);
            return AccessToken != null && AccessToken != "";
        }

        #endregion

        #region Internal Methods

        protected KnetikRequest CreateRequest(string path, string body = "[]", string method = "post", int timestamp = -1, string serviceBundle = null, bool isForm = false)
        {
            if (timestamp == -1) {
                TimeSpan t = (DateTime.UtcNow - new DateTime (1970, 1, 1));
                timestamp = (int)t.TotalSeconds;
            }
            
            string url = BuildUrl (path, serviceBundle);

            Log ("URL: " + url);
            Log ("Body:\n" + body);

			System.Text.ASCIIEncoding encoding=new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(body);
            
            KnetikRequest req = new KnetikRequest (method, url, data);

            if (isForm) {
                req.SetHeader("Content-type", "application/x-www-form-urlencoded");
            } else {
                req.SetHeader("Content-type", "application/json");
				req.SetHeader("Accept", "application/json");
            }
            req.SetHeader("User-Agent", "Knetik Unity SDK");

			if (AccessToken != null && AccessToken != "")
            {
                req.SetHeader("Authorization", "Bearer " + AccessToken);
            }

            return req;
        }

        private string BuildUrl(string path, string serviceBundle)
        {
                return BaseURL + Prefix + path;
        }

        private string EncodePassword(string password, int timestamp)
        {
            return KnetikApiUtil.sha1 (KnetikApiUtil.sha1 (password) + timestamp.ToString ()).ToUpper ();
        }

        private int GetTimestamp()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime (1970, 1, 1));
            return (int)t.TotalSeconds;
        }

        #endregion
        
		private static string Prefix = "/rest/";
        private static string AccessTokenKey = "knetik.access_token";
        private static string UsernameKey = "knetik.username";
        private static string PasswordKey = "knetik.password";
        private static string UserIDKey = "knetik.userid";
        private static string SessionEndpoint = "oauth/token";
		private static string RecordMetricEndpoint = "services/v1/metrics/record";
		private static string GetLeaderboardEndpoint = "services/v1/metrics/getleaderboard";
        private static string GetUserInfoEndpoint = "services/latest/user/getinfo";
        private static string GetUserInfoWithProductEndpoint = "services/latest/user/getinfowithproduct";
        private static string PutUserInfoEndpoint = "services/latest/user/update";
        private static string CreateGameOptionEndpoint = "services/latest/user/addgameoption";
        private static string UpdateGameOptionEndpoint = "services/latest/user/updategameoption";
        private static string RegisterEndpoint = "services/latest/registration";
		private static string GuestRegisterEndpoint = "services/latest/registration/guests";
        private static string UpgradeFromRegisteredGuestEndpoint = "services/latest/registration/guestupgrade";
        private static string FireEventEndpoint = "services/latest/bre/fireevent";
        private static string ListAchievementsEndpoint = "services/latest/badge/list";
        private static string ListUserAchievementsEndpoint = "services/latest/user/getachievement";
        private static string GetGameOptionEndpoint = "services/latest/product/getgameoption";
        private static string ListStorePageEndpoint = "services/latest/store";
        private static string CartAddEndpoint = "services/latest/cart/addtocart";
		private static string CartItemsEndpoint = "services/latest/carts/{0}/items"; //{0} cart Number 
		private static string CartModifyEndpoint = "services/latest/cart/modify";
		private static string CartCheckoutEndpoint = "services/latest/carts/{0}/checkout";//cartNumber
        private static string CartShippingAddressEndpoint = "services/latest/cart/shippingaddress";
		private static string CartModifyShippingAddressEndpoint = "services/latest/carts/{0}/shipping-address"; //{0} cartNumber
		private static string CartStatusEndpoint = "services/latest/cart/status";
		private static string CartShippableEndpoint = "services/latest/carts/{0}/shippable"; //{0}cart Number
		private static string CartGetEndpoint = "services/latest/carts/"; //need to add CartNumber to Request Endpoint
		private static string CartEndpoint = "services/latest/carts/"; //need to add CartNumber to Request Endpoint
		private static string CartCreateEndpoint = "services/latest/carts";
		private static string CartAddDiscountEndpoint = "services/latest/carts/{0}/adddiscount";
		private static string CartCountriesEndpoint = "services/latest/carts/{0}/countries"; //{0}Cart Number 
        private static string UseItemEndpoint = "services/latest/game/gamestart";
        private static string UserGetRelationshipsEndpoint = "services/latest/user/getrelationships";
		private static string GetInvoiceEndpoint = "/services/latest/invoice";
		private static string GetReceiptEndpoint = "/services/latest/payment/provider/apple/receipt";

        private void Log(String msg)
        {
            Debug.Log(msg);
            
        }
        
        private void LogError(String msg)
        {
            Debug.LogError(msg);
        }
        
        private void LogException(Exception e)
        {
            Debug.LogException(e);
        }
    }

    class KnetikSavedSession {
        
    }
}