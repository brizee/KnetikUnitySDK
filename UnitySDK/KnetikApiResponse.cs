using System;
using System.Collections;
using System.Net;
using System.IO;
using UnityEngine;
using KnetikSimpleJSON;

namespace Knetik
{
	public class KnetikApiResponse
	{
		public enum StatusType
		{
			Pending,
			Success,
			Failure,
			Error
		}

		public KnetikClient Client {
			get;
			protected set;
		}

		public KnetikRequest Request {
			get;
			protected set;
		}

		public StatusType Status {
			get;
			set;
		}

				public bool IsPending {
						get {
								return Status == StatusType.Pending;
						}
				}

				public bool IsSuccess {
						get {
								return Status == StatusType.Success;
						}
				}

				public bool IsFailure {
						get {
								return Status == StatusType.Failure;
						}
				}

				public bool IsError {
						get {
								return Status == StatusType.Error;
						}
				}

		public Action<KnetikApiResponse> Callback {
			get;
			protected set;
		}

		public bool IsAsync {
			get {
				return Callback != null;
			}
		}

		public KnetikJSONNode Body {
			get;
			protected set;
		}

				public string ErrorMessage {
						get;
						set;
				}

		public KnetikApiResponse (KnetikClient client, KnetikRequest req, Action<KnetikApiResponse> callback = null)
		{
			Status = StatusType.Pending;
			Client = client;
			Request = req;
			Callback = callback;

			if (callback == null) {
				req.synchronous = true;
				req.Send();
				CompleteCallback (req);
			} else {
				Debug.Log("Executing async!");
				req.Send(CompleteCallback);
			}
		}

		protected virtual void ValidateResponse(KnetikRequest req)
		{
			Status = StatusType.Success;
			if (req.response == null) {
				Log("Knetik Labs SDK - ERROR 1: The response from SAPI is null.");
								Status = StatusType.Failure;
								ErrorMessage = "Connection error - No connection";
				return;
			}
			
			if (req.response.status != 200) {
				LogError("Knetik Labs SDK - ERROR 2: Response returned a status of " + req.response.status);
								Status = StatusType.Failure;
								ErrorMessage = "Connection Error - Server problem";
				return;
			}
			
			try {
				Body = KnetikJSON.Parse(req.response.Text);
				
				if (Body == null) {
					LogError("Knetik Labs SDK - ERROR 3: Failed to Properly Parse JSON response");
										Status = StatusType.Failure;
										ErrorMessage = "Connection error - Invalid format";
					return;
				}
			} catch(Exception e) {
				LogException(e);
				Status = StatusType.Failure;
				ErrorMessage = "Connection error - Unknown exception";
				return;
			}
			
            if (Body["error"] != null && ((Body["error"]["success"] == null) || (Body["error"]["success"].AsBool == false))) {
				LogError("Knetik Labs SDK - ERROR 5: Response JSON does NOT report success!");
				Status = StatusType.Error;
				ErrorMessage = Body["message"];
				return;
			}
		}

		private void CompleteCallback(KnetikRequest req)
		{
			if (req.response != null) {
				Log ("Body:\n" + req.response.Text);
				ValidateResponse (req);
			} else {
				Status = StatusType.Failure;
				ErrorMessage = "Connection error - Could not connect to host";
			}
			
			if (Callback != null) {
				Callback(this);
			}
		}

        private void Log(String msg)
        {
#if UNITY_EDITOR
            Debug.Log(msg);
#endif
        }

        private void LogError(String msg)
        {
            #if UNITY_EDITOR
            Debug.LogError(msg);
            #endif
        }

        private void LogException(Exception e)
        {
            #if UNITY_EDITOR
            Debug.LogException(e);
            #endif
        }
	}
}

