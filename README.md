# Knetik Unity SDK

  This document details the usage and a brief overview of how to utilize the Knetik Unity SDK, a Unity web service that ties into our backend Service API (or jSAPI) to provide a wealth of features for Unity games and applications.  This is of course a living document that will change as features change so please be sure to read this for initial usage and whenever updated.

##1. Knetik (Java) Service API

  The Knetik jSAPI provides features that enable numerous ways to communicate, store, and interact with users, as well as integration with web portals (if desired). For example, games could use the Login/Registration feature of jSAPI to authenticate users. After successful authentication, games could send metric updates to main server for that user to be stored for future use. 

  This GitHub project contains just the Unity SDK itself.  An example of its use can be found in another GitHub repo, knetikmedia/SampleGame.

##2. Getting Started

  To use the Knetik Unity SDK, add a new empty GameObject to your scene (GameObject -> Create Empty).  Name it "KnetikInitializer" and add the "KnetikInitializationScript" component to the new object.  This script will configure your connection to the Knetik Service API.  There are a few properties on the script which you will want to set:

  - **Base URL:** The URL of the Knetik Java Service API as provided by Knetik including the protocol (http:// or https://).  Example: *https://jsapi.knetik.com:8080*
  - **Client ID:** Generated along with the secret key in the Admin Panel under Products -> Game Vendors for a particular Game Vendor.
  - **Client Secret:** Generated along with the Client ID.

  Once you configure the initialization script, you can connect to the API via the *KnetikClient.Instance* variable.

##3. Services

The Knetik API Client provides several services, such as Login, Registration, UserInfo, and Metrics.  The service methods can be executed either syncronously or asyncronously.

###3.1 Syncronous vs. Asyncronous Execution

To call a service method syncronously, call the method which returns a KnetikApiResponse instance:

```
KnetikApiResponse response = KnetikClient.Instance.Login(username, password)
// Handle response here
```

To call a service method asyncronously, call the method with a callback function (Action<KnetikApiResponse>):

```
KnetikClient.Instance.Login(username, password, (KnetikApiResponse response) => {
  // Handle response here.
});

Using the asyncronous execution will allow you to make nonblocking calls to the API.

###4. Login Service

Login requires a valid username/password pair to proceed, thus the user would already be registered.  For registration, please see section 2.2. The sample request below involves passing username/password by a Unity form:

EXAMPLE:

```
// Login a user with the given username and password entered
var response = KnetikClient.Instance.Login (loginView.data.login, loginView.data.password);

if (response.Status == KnetikApiResponse.StatusType.Success) 
{
  // Save off the created user session information
  UserSessionUtils.setUserSession(KnetikClient.Instance.UserID, KnetikClient.Instance.Username, KnetikClient.Instance.ClientID);
  // Launch the Level Scene (main game)
  Application.LoadLevel(2);
}
// If login failed, display the error message that SAPI returns
else 
{
  loginView.error = true;
  loginView.errorMessage = response.ErrorMessage;
}
```

###5. Registration Service

Registration requires three fields: username, password, email, and fullname.  When a user is registered they are not automatically logged in, so if you want to log them in transparently after registration, call the Login service after successful registration.

EXAMPLE:

```
// Check if passwords match, and if not display the appropriate error message
if (registrationView.data.password != registrationView.data.passwordConfirm) 
{
  registrationView.error = true;
  registrationView.errorMessage = "Password and Confirm password don't match!";     
  return;
}

var response = KnetikClient.Instance.Register(registrationView.data.login, 
                                              registrationView.data.password, 
                                              registrationView.data.email, 
                                              registrationView.data.login);

if (response.Status != KnetikApiResponse.StatusType.Success) {
  registrationView.error = true;
  registrationView.errorMessage = response.ErrorMessage;
  return;
}

// Save off the existing user session
UserSessionUtils.setUserSession(0, registrationView.data.login, KnetikClient.Instance.ClientID);
// Load the StartMenu again, the registered user must now login
Application.LoadLevel(1);
```

###6. UserInfo Service

There are three methods for UserInfo:
- GetUserInfo, which requires no arguments and gets the info of the current user.
- GetUserInfoWithProduct, which requires a product ID and gets the info of the current user and the info of the product with respect to the current user.
- PutUserInfo, which requires a name and a value and updates a setting for the current user.

EXAMPLE:

```
var response = KnetikClient.Instance.GetUserInfo();
Debug.Log(response.Body);

int productId = 1;
var response = KnetikClient.Instance.GetUserInfoWithProduct(productId);
Debug.Log(response.Body);

string configName = "lang";
string configValue = "es";

var response = KnetikClient.Instance.PutUserInfoWithProduct(configName, configValue);
Debug.Log(response.Body);
```

###7. Metrics Service

```
int metricId = 1;
int score = 10;
string level = "de_dust";
Dictionary<string, string> obj = new Dictionary<string, string> {
  "aKey" => "aValue"
};

var response = KnetikClient.Instance.RecordValueMetric(metricId, score, level);
Debug.Log(response.Body);

var response = KnetikClient.Instance.GetUserInfoWithProduct(metricId, obj, level);
Debug.Log(response.Body);

var response = KnetikClient.Instance.GetLeaderboard(leaderboardId, level);
Debug.Log(response.Body);
```

###8. GameOptions Service

```
int productId = 1;
string optionName = "invertY";
string optionValue = "true";
var response = KnetikClient.Instance.CreateGameOption(productId, optionName, optionValue);
Debug.Log(response.Body);

string optionValue = "false";

var response = KnetikClient.Instance.UpdateGameOption(productId, optionName, optionValue);
Debug.Log(response.Body);
```
