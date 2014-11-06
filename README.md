# Knetik Unity SDK

  This document details the usage and a brief overview of how to utilize the Knetik Unity SDK, a Unity web service that ties into our backend Service API (or jSAPI) to provide a wealth of features for Unity games and applications.  This is of course a living document that will change as features change so please be sure to read this for initial usage and whenever updated.

##1. Knetik (Java) Service API

  The Knetik jSAPI provides features that enable numerous ways to communicate, store, and interact with users, as well as integration with web portals (if desired). For example, games could use the Login/Registration feature of jSAPI to authenticate users. After successful authentication, games could send metric updates to main server for that user to be stored for future use. 

  This GitHub project contains just the Unity SDK itself.  An example of its use can be found in another GitHub repo, knetikmedia/SampleGame.

##2. Getting Started

  To use the Knetik Unity SDK, drag the KnetikInitializer prefab from the Knetik SDK onto the scene.  This object will connect Unity to the Knetik API with the settings in the “Knetik Initialization Script” component on the prefab:

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
```

Using the asyncronous execution will allow you to make nonblocking calls to the API.

###3.2 Login Service

Login requires a valid username/password pair to proceed, thus the user would already be registered.  For registration, please see section 5. The sample request below involves passing username/password by a Unity form:

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

####3.2.1 Login as Guest

Instead of presenting the user with a login form to start the game, you can start a guest session to let the user start playing right away.  They can login after and their session will be upgraded to the authenticated session.

```
var response = KnetikClient.Instance.GuestLogin();

if (response.Status == KnetikApiResponse.StatusType.Success) 
{
   // The user now is logged in as a guest.  They can login to get their guest session upgraded.
}
```

###3.3 Registration Service

Registration requires four fields: username, password, email, and fullname.  When a user is registered they are not automatically logged in, so if you want to log them in transparently after registration, call the Login service after successful registration.

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

###3.4 UserInfo Service

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

var response = KnetikClient.Instance.PutUserInfo(configName, configValue);
Debug.Log(response.Body);
```

###3.5 Metrics Service

The Metrics service is used to push numerical or object data to the Knetik API.

```
int metricId = 1;
float score = 10;
string level = "de_dust";
Dictionary<string, string> obj = new Dictionary<string, string> {
  "aKey" => "aValue"
};

var response = KnetikClient.Instance.RecordValueMetric(metricId, score, level);
Debug.Log(response.Body);

var response = KnetikClient.Instance.RecordObjectMetric(metricId, obj, level);
Debug.Log(response.Body);

var response = KnetikClient.Instance.GetLeaderboard(leaderboardId, level);
Debug.Log(response.Body);
```

###3.6 GameOptions Service

The GameOptions service is used to create or update key-value pairs for a given Game.  There are two types of options for Games: GameOptions and UserGameOptions.  GameOptions are configured in the Knetik Admin, shared amongst all users, and are read-only through the API.  UserGameOptions are defined by the client and every user has its own set.

To retrieve a list of all the options for a Game (both the GameOptions and the current user’s UserGameOptions), use the GetUserInfoWithProduct service.

```
int productId = 1;
// Retrieve a GameOption
string optionName = “messageOfTheDay”;
var response = KnetikClient.Instance.GetGameOption(productId, optionName);
Debug.Log(response.Body);

// Create a UserGameOption
string optionName = "invert-y“;
string optionValue = "true";
var response = KnetikClient.Instance.CreateUserGameOption(productId, optionName, optionValue);
Debug.Log(response.Body);

string optionValue = "false";
// Update it
var response = KnetikClient.Instance.UpdateUserGameOption(productId, optionName, optionValue);
Debug.Log(response.Body);
```

###3.7 Business Rules/Events Service

The Business Rules Engine (BRE) / Events service is used to trigger events in the BRE API.

```
string eventName = “game_finished”;
Dictionary<string, string> parameters = new Dictionary<string, string> {
  {“level”: “first”},
  {“score”: “50000”},
  {“duration”: “300”}
};
KnetikClient.Instance.FireEvent(eventName, parameters, (res) => {
  Debug.Log(“Event fired”);
});
```

###3.8 Achievements Service

The Achievements service lists the achievements available to the user.  The *pageIndex* and *pageSize* parameters .  There are two endpoints in the Achievements service: one for listing all the achievements (including ones the user does not have) and one for listing all the achievements the user has.


ListAchievements:

```
int pageIndex = 1;
int pageSize = 25;
KnetikClient.Instance.ListAchievements(pageIndex, pageSize, (res) => {
  Debug.Log(“Handle achievements response”);
});
```

ListUserAchievements:

Note that this endpoint can only retrieve the achievements for the currently logged in user.

```
int userId = 1;
int pageIndex = 1;
int pageSize = 25;
KnetikClient.Instance.ListUserAchievements(pageIndex, pageSize, (res) => {
  Debug.Log(“Handle achievements response”);
});
```

##4. Models

The simplest method of accessing the Knetik API is through the Model interface.  The Model interface is a convenience wrapper around the API Services and parses the responses into C# objects.

###4.1 UserInfo

The UserInfo model represents nformation about the current user.  As long as the user is signed in, you can retrieve its UserInfo from the UserInfo property on the KnetikClient instance:

```
Knetik.Instance.UserInfo
```

Initially, the UserInfo object isn’t populated with data as it hasn’t been retrieved yet.  To remedy this, use the Load or LoadWithGame methods:

```
Knetik.Instance.UserInfo.Load((result) {
  Debug.Log(“Finished loading user info: ” + result.Value); // result.Value is the UserInfo object
});

int gameId = 1;
Knetik.Instance.UserInfo.LoadWithGame(gameId, (result) {
  Debug.Log(“Finished loading user info: ” + result.Value); // result.Value is the Game object
});
```

You can also make changes to the UserInfo object.  Currently, only the *AvatarURL* and *Language* properties can be updated through the API.

```
Knetik.Instance.UserInfo.AvatarURL = “http://placehold.it/400x300.jpg”;
Knetik.Instance.UserInfo.Save((res) {
  Debug.Log(“Updated!”);
});
```

###4.2 Game

Game objects are retrieved through using the ```UserInfo.LoadWithGame```method and providing the game’s ID.  With the game object, you can read its properties, GameOptions, UserOptions, and record metrics by name for the game.  GameOptions are read-only but can be refreshed using the Refresh() method on the GameOption instance.  UserOptions can be created using the CreateUserOption method on the Game instance and saved (after being created or modified) using the Save method on the UserOption instance.

```
int gameId = 1;
Knetik.Instance.UserInfo.LoadWithGame(gameId, (result) {
  var game = result.Value;
  UserOption optionInvertY;
  if (game.UserOptions.ContainsKey(“invert-y”)) {
    optionInvertY = game.UserOptions[“invert-y”];
  } else {
    optionInvertY = game.UserOptions.CreateUserOption(“invert-y”, “true”);
  }
  Debug.Log(“Invert-Y: ” + optionInvertY.Value;

  if (game.GameOptions.ContainsKey(“messageOfTheDay”)) {
    Debug.Log(“Message of the Day: ” + game.GameOptions[“messageOfTheDay”].Value);
  }
});
```

###4.3 Metric

Metric objects are used to record new metrics (either values or objects).  Metric objects can be created right from the Client instance given a metric ID or from a Game instance given the metric name.

```
// ‘game’ variable is a Game object
ValueMetric score = game.CreateValueMetric(“score”);
score.Value = 300;
score.Save((res) {
  Debug.Log(“Score posted…”);
});

ObjectMetric savedGame = game.CreateObjectMetric(“saved_game”);
savedGame = new Dictionary<string, string> {{“level”, “first”}};
savedGame.Save((res) {
  Debug.Log(“Game saved…”);
});
```

###4.4 Achievement

The AchievementsQuery object handles paginating through the Achievements list.

```
Action<KnetikResult<AchievementsQuery>> callback = null;
callback = (KnetikResult<AchievementsQuery> result) => {
    // Process the current page of achievements
    Debug.Log (result.Value.Achievements);

    // If there’s more, load the next page with this same callback.
    if (result.Value.HasMore) {
        result.Value.NextPage(callback);
    }
};
KnetikClient.Instance.Achievements.Load (callback);
```

An AchievementQuery instance is also on UserInfo, for paginating through the user’s Achievements:

```
Action<KnetikResult<AchievementsQuery>> callback = null;
callback = (KnetikResult<AchievementsQuery> result) => {
    // Process the current page of achievements
    Debug.Log (result.Value.Achievements);

    // If there’s more, load the next page with this same callback.
    if (result.Value.HasMore) {
        result.Value.NextPage(callback);
    }
};
KnetikClient.Instance.UserInfo.Achievements.Load (callback);
```