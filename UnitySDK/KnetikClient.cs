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

        public string Username {
            get;
            set;
        }

        public string Password {
            get;
            set;
        }

        public string Session {
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

        #endregion

        #region Public Methods

        public void Logout() {
            Username = null;
            Password = null;
            Session = null;
            UserID = 0;
            _userInfo = new UserInfo(this);
        }

        #endregion

        #region Internal Methods

        protected KnetikRequest CreateRequest(string path, string body = "[]", string method = "post", int timestamp = -1)
        {
            if (timestamp == -1) {
                TimeSpan t = (DateTime.UtcNow - new DateTime (1970, 1, 1));
                timestamp = (int)t.TotalSeconds;
            }

            string url = BuildUrl (path);
            string signature = BuildSignature (body, timestamp);
            string envelope = BuildEnvelope (body, timestamp, signature);

            Debug.Log ("URL: " + url);
            Debug.Log ("Envelope:\n" + envelope);

            System.Text.ASCIIEncoding encoding=new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(envelope);
            
            KnetikRequest req = new KnetikRequest (method, url, data);

            req.SetHeader("Content-type", "application/json");
            req.SetHeader("User-Agent", "Knetik Unity SDK");

            return req;
        }

        private string BuildUrl(string path)
        {
            return BaseURL + Prefix + path;
        }

        private string BuildSignature(string request, int timestamp)
        {
            string text = request +
                                timestamp.ToString () +
                                ClientID +
                                Username +
                                Password;

            Debug.Log ("Signature Text: " + text);

            return KnetikApiUtil.hashHmac (text, ClientSecret);
        }

        // Build JSON Envelope for all Requests
        private string BuildEnvelope(string request, int timestamp, string signature)
        {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("request", request.Replace("\"", "\\\""));
            j.AddField ("timestamp", timestamp.ToString());
            j.AddField ("clientId", ClientID);
            j.AddField ("username", Username);
            j.AddField ("password", Password);
            j.AddField ("signature", signature);
            j.AddField ("session", Session);
            return j.Print ();
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
        
        private static string Prefix = "/rest/services/latest/";
        private static string SessionEndpoint = "session";
        private static string RecordMetricEndpoint = "metrics/record";
        private static string GetLeaderboardEndpoint = "metrics/getLeaderboard";
        private static string GetUserInfoEndpoint = "user/getinfo";
        private static string GetUserInfoWithProductEndpoint = "user/getinfowithproduct";
        private static string PutUserInfoEndpoint = "user/update";
        private static string CreateGameOptionEndpoint = "user/addgameoption";
        private static string UpdateGameOptionEndpoint = "user/updategameoption";
        private static string RegisterEndpoint = "registration/register";
        private static string GuestRegisterEndpoint = "registration/guestRegister";
        private static string UpgradeFromRegisteredGuestEndpoint = "registration/guestUpgrade";
        private static string FireEventEndpoint = "BRE/fireEvent";
        private static string ListAchievementsEndpoint = "badge/list";
        private static string ListUserAchievementsEndpoint = "user/getachievement";
        private static string GetGameOptionEndpoint = "product/getgameoption";
        private static string ListStorePageEndpoint = "store/getpage";
        private static string CartAddEndpoint = "cart/addtocart";
        private static string CartModifyEndpoint = "cart/modify";
        private static string CartCheckoutEndpoint = "cart/checkout";
        private static string CartShippingAddressEndpoint = "cart/shippingaddress";
        private static string CartStatusEndpoint = "cart/status";
        private static string CartGetEndpoint = "cart/get";
        private static string CartAddDiscountEndpoint = "cart/adddiscount";
        private static string CartCountriesEndpoint = "cart/getcountries";
    }
}