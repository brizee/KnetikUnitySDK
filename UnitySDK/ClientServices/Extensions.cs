using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knetik
{
    public partial class KnetikClient
    {
        public KnetikApiResponse CustomLogin(
            string serviceEndpoint,
            string usernameOrEmail,
            string password,
            bool isEmail,
            Action<KnetikApiResponse> cb = null
            ) {
            JSONObject json = new JSONObject (JSONObject.Type.OBJECT);
            if (isEmail)
            {
                json.AddField("email", usernameOrEmail);
            } else
            {
                json.AddField("username", usernameOrEmail);
            }
            json.AddField("password", password);
            json.AddField ("serial", KnetikApiUtil.getDeviceSerial());
            json.AddField ("mac_address", KnetikApiUtil.getMacAddress ());
            // Device Type is currently limited to 3 characters in the DB
            json.AddField ("device_type", KnetikApiUtil.getDeviceType());
            json.AddField ("signature", KnetikApiUtil.getDeviceSignature());
            string body = json.Print ();

            KnetikRequest req = CreateRequest(serviceEndpoint, body, "POST", -1, "");
            KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
            return res;
        }
        
        public KnetikApiResponse CustomCall(
            string serviceEndpoint,
            Dictionary<string, string> parameters,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject json = new JSONObject(parameters);
            string body = json.Print();
            
            KnetikRequest req = CreateRequest(serviceEndpoint, body, "POST", -1, "");
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }
    }
}

