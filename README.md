# Knetik Unity SDK

  This document details the usage and a brief overview of how to utilize the Knetik Unity SDK, a Unity web service that ties into our backend Service API (or SAPI) to provide a wealth of features for Unity games and applications.  This is of course a living document that will change as features change so please be sure to read this for initial usage and whenever updated.

##1. Knetik Service API

  The Knetik SAPI provides features that enable numerous ways to communicate, store, and interact with users, as well as integration with web portals (if desired). For example, games could use the Login/Registration feature of SAPI to authenticate users. After successful authentication, games could send metric updates to main server for that user to be stored for future use. 

  This GitHub project contains just the Unity SDK itself.  An example of its use can be found in another GitHub repo, knetikmedia/SampleGame.
  
##2. JSONObject
  As part of the requirements for building the Knetik Unity SDK, a folder called JSON is included that contains an imported project known as JSONObject.  This project is necessary for easier processing and creation of JSON objects, the input and output of SAPI.  Please ensure this folder is included when using the Knetik Unity SDK.  If there are issues with including it as is, you can also go to the Unity Asset Store and download and import the free plugin of JSONObject.

##3. Configuration

  The Knetik Unity SDK contains a number of variables necessary to communicate with SAPI which are easily stored in a configuration file.  This file is the KnetikConfig.json file.  As seen in the file extension, this config file is in JSON format. It contains essential global variables that get imported in the KnetikApiUtil.cs file and used throughout the SDK.

EXAMPLE:
```
{
    "version": "1.0",
    "apiHost": "sapi.net",
    "urlPrefix": "http://",
    "clientKey": "test_key",
    "clientSecret": "test_secret",
    "endpointPrefix": "/rest/api/latest/",
    "authPrefix": "/rest/auth/latest/",
    "sessionEndpoint": "session",
    "leaderboardEndpoint": "gameleaderboard",
    "metricEndpoint": "metric",
    "userEndpoint": "user",
    "productEndpoint": "product"
}
```

  The above example KnetikConfig.json shows some of the variables in place.
  - version: Version is used to help build the signature of the platform making calls.
  - apiHost: URL to the Knetik SAPI server, as supplied by Knetik
  - urlPrefix: Either http:// or https:// depending on security requirements
  - clientKey: Generated along with the secret key in the Admin Panel under Products -> Game Vendors for a particular Game Vendor
  - clientSecret: See clientKey
  - endpointPrefix: The starting URL for all SAPI endpoints except for login authentication.  This should not change unless specified by Knetik.
  - authPrefix: The starting URL for login authentication to create an authenticated session.
  - *Endpoint: These are predefined tags that append to the endpointPrefix to make particular types of calls to SAPI.

  For initial setup, it is necessary upon the startup of your game/application to call the KnetikApiUtil.readConfig() function to set up all these variables and allow a process connection to SAPI.

###3.1 Login
Login requires valid username/password to proceed, thus the user would already be registered.  For registration, please see section 3.2. The sample request below involves passing username/password by a Unity form. The strings of username and password should be passed to KnetikLoginRequest object. Then, by calling doLogin method, you could get the response from server. This C# sample shows how to use the SDK for Login with an already registered user:

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

###3.2 Registration
Registering a new user requires a little more information. The user will provide these strings: Username, Password, Email and Fullname. The SDK returns true or false to notify caller if it was successful or not.  Upon providing this information, the user is logged in as a Guest User, has their credentials added to the server, then is logged in as that user. In the case of an error, getErrorMessage() method could be called for the complete description.

EXAMPLE:
```
// RegistrationView is another class that builds the GUI registration screen
KnetikLoginRequest lr = new KnetikLoginRequest();

		if (!lr.doLoginAsGuest()) 
		{
			registrationView.error = true;
    		registrationView.errorMessage = lr.getErrorMessage();
			return;
		}

KnetikRegisterRequest ur = new KnetikRegisterRequest(lr.getKey(), 
		registrationView.data.login, 
		registrationView.data.password, 
		registrationView.data.email, 
		registrationView.data.fullname);
		
if(!ur.doRegister()) 
{
		registrationView.error = true;
		registrationView.errorMessage = ur.getErrorMessage();			
		return;
}

string m_key = lr.getKey();
UserSessionUtils.setUserSession(0, registrationView.data.login, lr.getKey());
Application.LoadLevel(1);
```

The above example is passing in the necessary parameters to perform a guest login while registering the new user.  It makes references to a registration screen that the user will see.

###3.3 Metric update
Metrics are simple key-value pairs that can be stored at any time for a particular user, product, and if desired level.  These Metrics are first created on the Admin Panel under the details of a particular Product/Game.  The call to KnetikPostMetricRequest requires an active session be created before and provide the Metric ID (from the admin panel) and value to be associated with it.

EXAMPLE:
```
	int metricId = 13;
	long metricValue = 1;
	KnetikPostMetricRequest ur = new KnetikPostMetricRequest(UserSessionUtils.getApiKey(), metricId, metricValue);
	if(ur.doPostMetric()) {
		Debug.Log("Metric post successful");
	} else {
		Debug.Log("METRIC POST FAILED!!!");
	}
```

In the above example a reference is made to the KnetikPostMetricRequest class to initialize an object that passes the API key which is the session key, the number 13 represents the metric ID, and the number 1 represents the metric value to be passed.  The metric value is a long int, so it needs to be numerical.  The UserSessionUtils.getApiKey() is NOT a Unity SDK function but is simply another class written for this example where the session information was stored upon logging in (see the above Login [section 3.1] function with the line UserSessionUtils.setUserSession(login.getUserId(), login.getUsername(), login.getKey());)

To post a metric update with a specific level ID (NOTE: Levels are created on Admin Panel for each game assuming the feature is active), the KnetikPostMetricRequest simply needs an additional parameter passed to it, another int that represents the Level ID to be included as the part of the metric post.

EXAMPLE:
```
	int metricId = 13;
	long metricValue = 1;
	int levelId = 2;
	KnetikPostMetricRequest ur = new KnetikPostMetricRequest(UserSessionUtils.getApiKey(), metricId, metricValue, levelId);
	if(ur.doPostMetric()) {
		Debug.Log("Metric post successful");
	} else {
		Debug.Log("METRIC POST FAILED!!!");
	}
```
	
###3.4 Product Game Option Retrieval
Product Game Options that are set in the Admin panel for a particular game (this feature must be enabled by Knetik, off by default) can be retrieved with a provided Product ID (which can also be seen on the Admin Panel).  Product ID should be passed as an int.  An active session is required as well.  Upon retrieving the game options, the option information is available as a look-up Dictionary, so option values can be looked up by name.  

EXAMPLE:
```
	int productId = 5;
	KnetikProductGameOptionsRequest product = new KnetikProductGameOptionsRequest(UserSessionUtils.getApiKey(), productId);
	bool product_result = product.doGetInfo();
	string optionValue = null;
	string optionName = "my_game_option_name";
	if (product_result && product.game_options.TryGetValue(optionName, out optionValue))
	{
		Debug.Log("Option Name: " + optionName + " Option Value: " + optionValue);
	}
```

In the above example, the look-up dictionary product.game_options allows us to look for the value for a particular option, optionName.
	
###3.5 User Information Retrieval
User Information can be retrieved using just the active session. Note that this retrieves several user information values.

EXAMPLE:
```
	KnetikUserInfoRequest user = new KnetikUserInfoRequest(UserSessionUtils.getApiKey());
	bool user_result = user.doGetInfo();
```

The below are the fields that can be retrieved by the KnetikUserInfoRequest user object:

```
	public int id;
	public string email;
	public string username;
	public string fullname;
	public string mobile_number;
	public string money_balance;
	public string coin_balance;
	public string avatar_url;
	public string level;
	public string experience;
	public string first_name;
	public string last_name;
	public string token;
	public string gender;
	public string lang;
	public string country;
```

User Game Options that are set in the Admin panel (this feature must be enabled by Knetik, off by default) or in the SDK (see 3.6) can be retrieved by adding the parameter for the associated Product ID (which can also be seen on the Admin Panel).

EXAMPLE:
```
	long productId = 5;
	KnetikUserInfoRequest user = new KnetikUserInfoRequest(UserSessionUtils.getApiKey(), productId);
	bool user_result = user.doGetInfo();
	string optionValue = null;
	string optionName = "my_user_option_name";
	if (user_result && user.user_options.TryGetValue(optionName, out optionValue))
	{
		Debug.Log("Option Name: " + optionName + " Option Value: " + optionValue);
	}
```

###3.6 User Game Option Storage
User Game Options can be created or updated from the SDK.  This requires the following fields sent: User ID (which can be retrieved with KnetikUserInfoRequest followed by a doGetInfo() on that object), Product ID (which can also be seen on the Admin Panel), the Option Name, the Option Value, as well as an active session.  Note that for User Game Options, the Option name and value are strings.

####3.6.1 User Game Option Insert
Creates a new user game option (name and value) based on the user ID and product ID.

EXAMPLE:
```
	long userId = 13;
	long productId = 5;
	string optionValue = null;
	string optionName = "my_user_option_name";
	string optionValue = "my_user_option_value";
	KnetikPostUserOptionsRequest userInsert = new KnetikPostUserOptionsRequest(UserSessionUtils.getApiKey(), userId, productId, optionName, optionValue);
	bool user_result = userInsert.postUserInfo("insert");
```

####3.6.1 User Game Option Update
Updates an existing user option (value) based on the session, user ID, product ID, and option name.  Note that each user game option can only have one corresponding value.

EXAMPLE:
```
	long userId = 13;
	long productId = 5;
	string optionValue = null;
	string optionName = "my_user_option_name";
	string optionValue = "my_user_option_value";
	KnetikPostUserOptionsRequest userInsert = new KnetikPostUserOptionsRequest(UserSessionUtils.getApiKey(), userId, productId, optionName, optionValue);
bool user_result = userInsert.postUserInfo("update");
```

###3.7 Changing User Settings
A number of settings for the user can be changed within the Unity SDK.  These assume that the user information is already known as shown in section 3.5.

####3.7.1 Changing The User Avatar
The avatar of the user can be changed using an existing URL that must contain an image type (i.e. .jpg, .png, .gif, etc.).  Mode must be set to "avatar".  Note that userGet.id is the ID of the user as pulled from the above section 3.5 userGet request and user_info_result is a boolean for whether the User Information pull worked.

EXAMPLE:
```
if (user_info_result)
{
	string avatar_url = "http://www.mywebsite/this_new_avatar.png";
	KnetikUserInfoRequest userPut = new KnetikUserInfoRequest(UserSessionUtils.getApiKey(), userGet.id, avatar_url, null);
	string mode = "avatar";
	bool userPut_result = userPut.putUserInfo(mode);
	Debug.Log ("User Put Result for Avatar: " + userPut_result);
}
```

####3.7.2 Changing The User Language
The language of the user can be changed using an existing set of 2 letter names of languages that the system is expecting to handle (example: en for English, ar for Arabic).  These languages are pre-set in the database, so if any are needed, please contact Knetik to ensure they are available.  Mode must be set to "lang".  Note that userGet.id is the ID of the user as pulled from the above section 3.5 userGet request and user_info_result is a boolean for whether the User Information pull worked.

EXAMPLE:
```
if (user_info_result)
{
	string lang = "en";
	KnetikUserInfoRequest userPut = new KnetikUserInfoRequest(UserSessionUtils.getApiKey(), userGet.id, null, lang);
	string mode = "lang";
	bool userPut_result = userPut.putUserInfo(mode);
	Debug.Log ("User Put Result for Lang: " + userPut_result);
}
```

###3.8 Leaderboard Information Retrieval
Leaderboards are created on the Admin Panel outside of the Unity SDK.  They each have a unique ID associated with them, are only attached to a single game and a single metric.  Optionally they can be tied to a single game level as well.  Retrieving the leaderboard information requires knowing the leaderboard ID.  This will give the option of a number of fields, as well as the Game Leaderboard, which is the users' results based on this leaderboard.

EXAMPLE:
```
	int leaderboardId = 10;
	KnetikLeaderboardRequest leaderboard = new KnetikLeaderboardRequest(UserSessionUtils.getApiKey(), leaderboardId);
	bool leaderboard_result = leaderboard.doGetInfo();
	Debug.Log ("Leaderboard Result: " + leaderboard_result);
	Debug.Log ("Leaderboard Product Title: " + leaderboard.product_title);  // The title of the product associated with this leaderboard
```

The Game Leaderboard is known as user_results and is a searchable dictionary, by user ID.  The user_results field requires a key of the user_id, (which can be requested from the User Results), and will return the following values:

```	
	public string currentscore;  // Value 0 of the user_results array
	public string username;		 // Value 1 of the user_results array
	public string avatar_url;	 // Value 2 of the user_results array
```

EXAMPLE continued from above:
	
```
	long productId = 5;
	KnetikUserInfoRequest userGet = new KnetikUserInfoRequest(UserSessionUtils.getApiKey(), productId);
	bool userGetResult = userGet.doGetInfo();
	if (userGetResult)
	{
		if(leaderboard.user_results.ContainsKey(userGet.id))
		{
			string[] user1_results = leaderboard.user_results[userGet.id];
			if (user1_results != null || user1_results.Length >= 1)
			{
				Debug.Log("User 1 Current Score: " + user1_results[0]);
				Debug.Log("User 1 Username: " + user1_results[1]);
				Debug.Log("User 1 Avatar URL: " + user1_results[2]);
			}
		}
		else
		{
			Debug.Log ("User " + userGet.id + " does not have results for this leaderboard");
		}
	}
```

The following fields are currently retrievable from LeaderboardRequest objects:

```
	public string leaderboard_id;
	public string copyright;
	public string date_created;
	public string date_updated;
	public string deleted;
	public string description;
	public string developer_id;
	public string image_url;
	public string lang;
	public string languages;
	public string metric_id;
	public string name;
	public string product_description;
	public string product_id;
	public string product_summary;
	public string product_title;
	public string product_translation_id;
	public string publisher_id;
	public string qualifying_value;
	public string rating_id;
	public string size;
	public string sort_style;
	public string update_date;
	public string level_id;
	public string active;
	public string create_date;
	public string level_name;
	public string metric_name;
	public Dictionary<string, string[]> user_results = new Dictionary<string, string[]>();
```
