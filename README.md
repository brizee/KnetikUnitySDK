# Knetik Unity SDK

  This document details the usage and a brief overview of how to utilize the Knetik Unity SDK, a Unity web service that ties into our backend Service API (or jSAPI) to provide a wealth of features for Unity games and applications.  This is of course a living document that will change as features change so please be sure to read this for initial usage and whenever updated.

##1. Knetik (Java) Service API

  The Knetik jSAPI provides features that enable numerous ways to communicate, store, and interact with users, as well as integration with web portals (if desired). For example, games could use the Login/Registration feature of jSAPI to authenticate users. After successful authentication, games could send metric updates to main server for that user to be stored for future use. 

  This GitHub project contains just the Unity SDK itself.  An example of its use can be found in another GitHub repo, knetikmedia/SampleGame.

##2. Configuration

  The Knetik Unity SDK contains a number of variables necessary to communicate with jSAPI which are easily stored in a configuration file.  This file is the KnetikConfig.json file.  As seen in the file extension, this config file is in JSON format. It contains essential global variables that get imported in the KnetikApiUtil.cs file and used throughout the SDK.

EXAMPLE:
```
{
    "version": "1.0",
    "apiHost": "sb.knetik.com:8080",
    "urlPrefix": "http://",
    "clientKey": "test",
    "clientSecret": "test_secret",
    "endpointPrefix": "/rest/services/latest/",
    "sessionEndpoint": "session",
    "leaderboardEndpoint": "gameleaderboard",
    "metricEndpoint": "metric",
    "userEndpoint": "user",
    "productEndpoint": "product",
    "registerEndpoint": "registration/register"
}
```

  The above example KnetikConfig.json shows some of the variables in place.
  - version: Version is used to help build the signature of the platform making calls.
  - apiHost: URL to the Knetik jSAPI server, as supplied by Knetik
  - urlPrefix: Either http:// or https:// depending on security requirements
  - clientKey: Generated along with the secret key in the Admin Panel under Products -> Game Vendors for a particular Game Vendor
  - clientSecret: See clientKey
  - endpointPrefix: The starting URL for all jSAPI endpoints.  This should not change unless specified by Knetik.
  - *Endpoint: These are predefined tags that append to the endpointPrefix to make particular types of calls to jSAPI.

  For initial setup, it is necessary upon the startup of your game/application to call the KnetikApiUtil.readConfig() function to set up all these variables and allow a process connection to jSAPI.

###2.1 Login
Login requires valid username/password to proceed, thus the user would already be registered.  For registration, please see section 2.2. The sample request below involves passing username/password by a Unity form. The strings of username and password should be passed to KnetikLoginRequest object. Then, by calling doLogin method, you could get the response from server. This C# sample shows how to use the SDK for Login with an already registered user:

EXAMPLE:
```
	KnetikLoginRequest login = new KnetikLoginRequest(loginView.data.login, loginView.data.password);
	if (login.doLogin()) {
		m_key = login.getKey();
		UserSessionUtils.setUserSession(login.getUserId(), login.getUsername(), login.getKey());
		Application.LoadLevel(1);
	} else {
		loginView.error = true;
		loginView.errorMessage = login.getErrorMessage();			
	}		
```

###2.2 Registration
Registering a new user requires a little more information and is actually a 3 step process.  The first part involves creating a guest session, then registering the new user, followed by authenticating the new user's previous guest session to complete the login. The user will provide these strings: Username, Password, Email and Fullname as a minimum (more choices are optional). The SDK returns true or false to notify caller if it was successful or not and a proper error message if necessary.  In the case of an error, getErrorMessage() method could be called for the complete description.

EXAMPLE:
```
void onRegisterClick() {
		if (registrationView.data.password != registrationView.data.passwordConfirm) {
			registrationView.error = true;
    		registrationView.errorMessage = "Password and Confirm password don't match!";			
			return;
		}
		
		KnetikLoginRequest lr = new KnetikLoginRequest();
        
        // First build a guest session
		if (!lr.doLogin(true)) {
			registrationView.error = true;
    		registrationView.errorMessage = lr.getErrorMessage();
			return;
		}

		Debug.Log("Guest session key: lr.getKey()");
        	// Register the new user
		KnetikRegisterRequest ur = new KnetikRegisterRequest(lr.getKey(), 
			registrationView.data.login, 
			registrationView.data.password, 
			registrationView.data.email, 
			registrationView.data.login);
		if(!ur.doRegister()) {
			registrationView.error = true;
    		registrationView.errorMessage = ur.getErrorMessage();			
			return;
		}

        KnetikLoginRequest nlr = new KnetikLoginRequest(registrationView.data.login, 
                                                       registrationView.data.password);
        
        // After registration, upgrade session to authenticated
        if (!nlr.doLogin())
        {
                registrationView.error = true;
                registrationView.errorMessage = lr.getErrorMessage();
                return;
        }
                        
        m_key = nlr.getKey();
        UserSessionUtils.setUserSession(0, registrationView.data.login, nlr.getKey());
		Application.LoadLevel(1);
}
```

The above example is passing in the necessary parameters to perform a guest login while registering the new user.  It makes references to a registration screen that the user will see.
