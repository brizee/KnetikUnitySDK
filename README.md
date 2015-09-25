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

This also applies to model methods.  For example:

```
// sync
string gameKey = "angry-bots-sample";
var syncResult = KnetikClient.Instance.UserInfo.LoadWithGame(gameKey);
Game syncGame = syncResult.Value;

// async
KnetikClient.Instance.UserInfo.LoadWithGame(gameKey, (asyncResult) => {
	Game asyncGame = asyncResult.Value;
});
```

Note that using synchronous methods can have a serious impact on performance, as the call will block the current thread until a response is received.

###3.2 Session Service

####3.2.1 Login Service

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

####3.2.2 Login With Custome Grant Type 

```

Dictionary<string,string> paramters = new Dictionary<string,string > ();

paramters.Add ("grant_type","grant_type_value");

paramters.Add ("username","username_value");

paramters.Add ("password","password_value");

paramters.Add ("client_id","client_id_value");

paramters.Add ("client_secret","client_secret_value");

KnetikClient.Instance.Login(paramters , (res) => {
  if (res.IsSuccess) {
   // Save off the created user session information
   UserSessionUtils.setUserSession(KnetikClient.Instance.UserID,  
   KnetikClient.Instance.Username,KnetikClient.Instance.ClientID);
  // Launch the Level Scene (main game)
  Application.LoadLevel(2);
  } else { 
    loginView.error = true;
    loginView.errorMessage = response.ErrorMessage;
  }
});
```

####3.2.3 Login as Guest

Instead of presenting the user with a login form to start the game, you can start a guest session to let the user start playing right away. this can be done by register a guest account then login.

```
var response=KnetikClient.Instance.GuestRegister(); //register a guest account

if (response.Status == KnetikApiResponse.StatusType.Success) 
{
   // The user now is registered  as a guest.  then you can use the generated username and password to login 
   KnetikApiResponse response = KnetikClient.Instance.Login(guestUsername, guestPassword)
}
```

####3.2.4 Persisting session 

To keep a session between launches of your game -- for example, to keep an authenticated guest session -- use the SaveSession methods:

SaveSession:
```
KnetikClient.Instance.Login(UsernameInput.text, PasswordInput.text, (res) => {
  if (res.IsSuccess) {
    // Store the user's session for retrieving later.
    KnetikClient.Instance.SaveSession();
    Menu.SetScreen("profile");
  } else { 
    ErrorMessageText.text = res.ErrorMessage;
    ErrorMessageText.gameObject.SetActive(true);
  }
});
```
When the user logs out, their saved session will be deleted as well.


####3.2.5 Session Expired  

If your session has been expired and you received an Error with message "Your session has expired; Please refresh." and you got an error code equal "5" you will need to call login method again to refresh your session .

### how to check your error Code ?

```
int errorCode= res.body["error"][“code”]

if( errorCode == 5 )
{

		KnetikClient.Instance.refreshSession((res) => {
			if (res.IsSuccess) {
				// prase response
			} else { 
				// prase response

			}
		});
	
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

var response = KnetikClient.Instance.Register(
  registrationView.data.username,
  registrationView.data.password,
  registrationView.data.email,
  registrationView.data.fullname
);

if (response.Status != KnetikApiResponse.StatusType.Success) {
  registrationView.error = true;
  registrationView.errorMessage = response.ErrorMessage;
  return;
}

// Load the StartMenu again, the registered user must now login
Application.LoadLevel(1);
```

#### Register as Guest

It is possible to register a user a guest which will let them call endpoints that require a user.

```
KnetikClient.Instance.GuestRegister();
// Now logged in as a registered guest.
```

#### Upgrade a Guest to a Registered User

As a registered guest, the user can enter registration info to upgrade the guest user to a registered user.

```
KnetikClient.Instance.GuestRegister();
KnetikClient.Instance.UpgradeFromRegisteredGuest(
  registrationView.data.username,
  registrationView.data.password,
  registrationView.data.email,
  registrationView.data.fullname
);
// The user is now fully registered.
```


#### CustomLogin

CustomLogin lets you call a custom service and will create an authenticated session if it succeeds.  It has a few arguments:

- **serviceEndpoint** - the name of the service to call
- **usernameOrEmail** - the username or email to login with
- **password** - the password
- **isEmail** - true if the usernameOrEmail is an email, false if it's a username

Example
```
var serviceEndpoint = "customSSO/login";
var username = "bobby@email.com";
var password = "bobbyftw";

KnetikClient.Instance.CustomLogin(serviceEndpoint, username, password, true, (res) => {
  Debug.Log("Logged in!");
});
```

#### CustomCall

CustomCall works like CustomLogin but isn't specific to logging in. It lets you call a custom service and you can parse the response.  It has a couple arguments:

- **serviceEndpoint** - the name of the service to call
- **parameters** - the parameters to send to the endpoint

Example
```
var serviceEndpoint = "customSSO/register";

var email = "bobby@email.com";
var password = "bobbyftw";
var fullname = "Bobby User";

var parameters = new Dictionary<string, string>();
parameters.Add("email", email);
parameters.Add("password", password);
parameters.Add("fullname", fullname);


KnetikClient.Instance.CustomCall(serviceEndpoint, parameters, (res) => {
  Debug.Log("Registered!");
});
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


#### ListAchievements:

```
int pageIndex = 1;
int pageSize = 25;
KnetikClient.Instance.ListAchievements(pageIndex, pageSize, (res) => {
  Debug.Log(“Handle achievements response”);
});
```

#### ListUserAchievements:

Note that this endpoint can only retrieve the achievements for the currently logged in user.

```
int userId = 1;
int pageIndex = 1;
int pageSize = 25;
KnetikClient.Instance.ListUserAchievements(pageIndex, pageSize, (res) => {
  Debug.Log(“Handle achievements response”);
});
```

###3.9 Store Service

The Store Service lists the sellable items available to the user.  It uses pagination similar to the Achievements Service.  Store items are filtered by terms or related items.  When you search by terms, at least one of the terms you pass in will be matched in the result items.  When you search by related items, the results will be the items related to the items you pass in.

The Store service lists items with their catalog ID and catalog SKU ID.  These are used for adding item to the cart and purchasing.

#### ListStorePage:

```
int pageIndex = 1;
int pageSize = 25;
List<string> terms = new List<string> { “T-Shirts”, “Knetik” };
KnetikClient.Instance.ListStorePage(pageIndex, pageSize, terms, null, (res) => {
  /* res.Body will look like this:
  
{
  "error": {
    "code": 0,
    "success": true
  },
  "result": [
    {
      "_id": 1,
      "id": 1,
      "sort": 0,
      "long_description": "<p>Official Merch!</p>",
      "short_description": "Black/Grey T-Shirt",
      "type_hint": "physical_item",
      "date_updated": 1415642480000,
      "name": "Black/Grey Short Sleeve T-Shirt",
      "deleted_at": null,
      "unique_key": null,
      "date_created": 1399327791000,
      "terms": [
        11,
        12,
        16
      ],
      "skus": [
        {
          "id": 30,
          "min_inventory_threshold": null,
          "price": 15.0,
          "inventory": 1000,
          "description": "Small",
          "stop_date": null,
          "deleted_at": null,
          "published": true,
          "sku": "Small",
          "start_date": null,
          "catalog_id": 42
        },
        {
          "id": 31,
          "min_inventory_threshold": null,
          "price": 15.0,
          "inventory": 1000,
          "description": "Medium",
          "stop_date": null,
          "deleted_at": null,
          "published": true,
          "sku": "Medium",
          "start_date": null,
          "catalog_id": 42
        },
        {
          "id": 32,
          "min_inventory_threshold": null,
          "price": 15.0,
          "inventory": 1000,
          "description": "Large",
          "stop_date": null,
          "deleted_at": null,
          "published": true,
          "sku": "Large",
          "start_date": null,
          "catalog_id": 42
        },
        {
          "id": 33,
          "min_inventory_threshold": null,
          "price": 15.0,
          "inventory": 1000,
          "description": "XXXL",
          "stop_date": null,
          "deleted_at": null,
          "published": true,
          "sku": "XXXL",
          "start_date": null,
          "catalog_id": 42
        }
      ],
      "assets": [
        {
          "id": 21,
          "hash": "1216688581",
          "description": null,
          "item_id": 66,
          "deleted_at": null,
          "path": "/uploads/attachments/1216688581/originals/2_18803.gif",
          "sort_order": 2,
          "type": "image",
          "url": "http://dev.store.teamrock.com/uploads/attachments/1216688581/originals/2_18803.gif"
        }
      ],
      "behaviors": [],
      "summary": "Official Merch!",
      "weight_unit_of_measurement": null,
      "height": null,
      "weight": null,
      "width": null,
      "dimension_unit_of_measurement": null,
      "shipping_tier": 1,
      "length": null,
      "store_start": 1230789600000,
      "virtual_currency_id": null,
      "displayable": true,
      "vendor_id": 1,
      "catalog_id": 42,
      "store_end": 1420092000000
    }
  ],
  "cached": false,
  "message": "",
  "parameters": [],
  "requestId": “123456789-0”
}
	*/ 
});

```
###3.10 Cart Service

The Cart service allows the user to manage a shopping cart, including Create Cart ,Get Cart Details ,modify shipping address ,is cart shippable, adding, removing, and updating items in the cart as well as checking out.

#### CartCreate
CartCreate Create a new Cart 
```
Client.CartCreate (createResponse);
createResponse.Body will look like :
{
  "error": {
    "code": 0,
    "success": true
  },
  "result": "ae88766d-5c45-4143-b510-0ba1e66d463d",  // result is the new Cart Number 
  "cached": false,
  "message": "",
  "parameters": [
    
  ],
  "requestId": "1429886757938-1002"
}
```
#### CartGet

GetCart retrieves the items in the user’s cart as well as information such as the Sub-Total, Total, and Status of the cart.  Note that this call will fail if the user does not have a cart (meaning they haven’t added an item their cart yet.)

```
var cartGetResponse = client.Client.CartGet (cartNumber);  
/*
cartGetResponse.Body will look like:
{
  "error": {
    "code": 0,
    "success": true
  },
  "result": {
    "cart": {
      "shipping_address_line2": null,
      "shipping_cost": 0.0,
      "shipping_address_line1": null,
      "currency": "GBP",
      "city": null,
      "first_name": null,
      "cart_id": 663,
      "postal_state_id": null,
      "error_message": "You must select a shipping object for these items.",
      "iso2": null,
      "grand_total": 13.0,
      "zip": null,
      "state_code": null,
      "status": "active",
      "sub_total_after_discounts": 13.0,
      "sub_total": 13.0,
      "country_id": null,
      "error_code": 112,
      "system_sub_total": 13.0,
      "discount_total": 0.0,
      "country": null,
      "tax": 0.0,
      "session": “ABCDEF1234567890,
      "email": null,
      "last_name": null,
      "postal_state": null
    },
    "discounts": null,
    "items": [
      {
        "cart_item_id": 309,
        "type_hint": "physical_item",
        "system_price": 13.0,
        "error_code": 112,
        "qty": 1,
        "sku_description": "Large",
        "vendor_id": 1,
        "sku": "Large",
        "total_price": 13.0,
        "unit_price": 13.0,
        "item_url": "",
        "thumbnail": "/uploads/attachments/1216688581/originals/3.jpg",
        "cart_id": 663,
        "inventory": 1000,
        "error_message": "You must select a shipping object for these items.",
        "name": "Black/Grey Short Sleeve T-Shirt",
        "store_item_id": 66,
        "stock_status": "",
        "sku_id": 32,
        "catalog_id": 42
      }
    ],
    "shipping": []
  },
  "cached": false,
  "message": "",
  "parameters": [],
  "requestId": "123456789-1"
}
*/
```

#### CartAdd

CartAdd adds an item to the cart by its SKU and quantity.

```
var catalogId = 42;
var skuId = 32;
vart quantity = 2;
var cartAddResponse = client.CartAdd(cartNumber,catalogId, skuId, quantity);

// cartAddResponse.IsSuccess and the message says “Item has been added to your cart.”
```

#### CartModify

CartModify works the same as CartAdd except that it sets the quantity to be exactly the amount passed in (whereas CartAdd will add the quantity onto the existing amount).

```
var catalogId = 42;
var skuId = 32;
vart quantity = 5;
var cartModifyResponse = client.CartModify(cartNumber,catalogId, skuId, quantity);

// cartModifyResponse.IsSuccess is true
```
#### CartShippingAddress

CartShippingAddress Modify Shipping Address for a specific Cart

```
ShippingAddress shipping; //Create a shipping address Object 
  shipping.PrefixName ="prefixName"; 
  shipping.AddressLine1 ="AdressLin1";
  shipping.AddressLine2 ="AdressLin1";
  shipping.City ="AdressLin1";
  shipping.Country ="United Kingdom";
  shipping.Email ="knetik@knetik.com";
  shipping.FirstName ="First Name";
  shipping.LastName ="Last Name";
  shipping.PostalState ="163"; //Postal state ID
  shipping.Zip ="123123";
  shipping.OrderNotes="notes";
  shipping.Country_id="225";
  
var CartShippingAddressResponse = Client.CartShippingAddress(cartNumber,shipping);

// CartShippingAddressResponse.IsSuccess is true
```

#### CartCheckout

CartCheckout attempts to checkout the cart.  It will use the user’s appropriate virtual currency as payment for the items in the cart.

```
var cartCheckoutResponse = client.CartCheckout(cartNumber);

cartCheckoutResponse.Body will look like :

{
  "error": {
    "code": 0,
    "success": true
  },
  "result": {
    "invoices": [
      {
        "name_prefix": null,
        "phone": null,
        "city_name": null,
        "phone_number": null,
        "billing_postal_code": null,
        "current_fulfillment_status": 8,
        "current_fulfillment_status_name": "unfulfilled",
        "billing_city_name": null,
        "address1": null,
        "address2": null,
        "current_fulfillment_status_description": "The invoice has not yet been fulfilled",
        "current_status_name": "paid",
        "state_name": null,
        "country_name": null,
        "currency": "GOL",
        "id": 10535,
        "shipping": 0.0,
        "cart_id": 10534,
        "create_date": "2015-04-24 17:54:42",
        "fed_tax": 0.0,
        "current_status_description": "The invoice has been paid for",
        "user_id": 7700,
        "billing_state_name": null,
        "billing_country_name": null,
        "grand_total": 0.0,
        "billing_address1": null,
        "billing_address2": null,
        "state_tax": 0.0,
        "subtotal": 0.0,
        "vendor_id": 1,
        "deleted": 0,
        "current_status": 2,
        "discount": 0.0,
        "parent_invoice_id": null,
        "order_notes": null,
        "email": null,
        "postal_code": null,
        "update_date": "2015-04-24 17:54:42",
        "full_name": "null null"
      }
    ],
    "discounts": [
      
    ],
    "items": [
      {
        "type_hint": "physical_item",
        "system_price": 5.99,
        "qty": 0,
        "sku_description": "sm-white-pocket",
        "affiliate_id": null,
        "sku": "KIA1425393515-9623",
        "deleted": 0,
        "unit_price": 5.99,
        "total_price": 0.0,
        "id": 1083,
        "thumbnail": "",
        "item_name": "test_item_2",
        "item_id": 9,
        "sku_id": 5,
        "invoice_id": 10535
      }
    ]
  },
  "cached": false,
  "message": "",
  "parameters": [
    
  ],
  "requestId": "1429887282012-1004"
}
```

###3.11 Custom Calls

There are two methods provided to allow you to make calls to custom service endpoints for client-specific logic; one is for custom login calls and the other is for other, general type of calls.

###3.11 Items

#### UseItem

To use an item (such as a consumable), use the UseItem endpoint:

```
int itemID = 197
var response = Client.UseItem(itemID);
Debug.Log("Success: " + response.IsSuccess ? "yes" : "no");
```

This will consume the item from the user's inventory.

##4. Models

The simplest method of accessing the Knetik API is through the Model interface.  The Model interface is a convenience wrapper around the API Services and parses the responses into C# objects.

###4.1 UserInfo

The UserInfo model represents information about the current user.  As long as the user is signed in, you can retrieve its UserInfo from the UserInfo property on the KnetikClient instance:

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

###4.1.1 User Relationships

The UserInfo object has a Relationships object that allows you to retrieve information about the hierarchical relationships that the user is in.

The Ancestors are returned in order of ancestry with the highest ancestor listed first.

For example:

[Great Grandparent, Grandparent, Parent]

By default, the results include the direct ancestor (AncestorDepth = 1), the direct descendants (DescendantDepth = 1), and siblings (IncludeSiblings = true).

```
/*
You can set how many ancestors, descendants to find and whether to return siblings:

KnetikClient.Instance.UserInfo.Relationships.AncestorDepth = 10;
KnetikClient.Instance.UserInfo.Relationships.DescendantDepth = 10;
KnetikClient.Instance.UserInfo.Relationships.IncludeSiblings = true;
*/

KnetikClient.Instance.UserInfo.Relationships.Load((relationshipsResult) => {
  if (relationshipsResult.Response.IsSuccess) {
    foreach (var kvp in relationshipsResult.Value.Relationships) {
      Debug.Log("Name: " + kvp.Key);
      Debug.Log("Ancestors:");
      foreach (var ancestor in kvp.Value.Ancestors) {
        Debug.Log(ancestor.Username);
      }
      Debug.Log("Siblings:");
      foreach (var sibling in kvp.Value.Siblings) {
        Debug.Log(sibling.Username);
      }
      Debug.Log("Descendants:");
      foreach (var descendant in kvp.Value.Descendants) {
        Debug.Log(descendant.Username);
      }
    }
  }
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

###4.4.1 Achievements by Game

To list only the Achievements related to your game (set up using the Related Items tool), use:

```
game.GetAchievements().Load((res) => {
  // res.Value.Items is a list of all the Achievements
});
```

To check if a user has received an Achievement, use the user's UserInfo.Inventory:

```
// "game" variable is a Game instance loaded through KnetikClient.UserInfo.LoadWithGame(gameID)

game.GetAchievements().Load((res) => {
  // res.Value.Items is a list of all the Achievements
  foreach (Item item in res.Value.Items) {
    var achievement = item as Achievement;
    if (achievement != null && KnetikClient.UserInfo.Inventory.Contains(achievement.ID)) {
      // User has earned the achievement
    }
  }
});
```

###4.5 Store

The store can be accessed through KnetikClient.Instance.Store, which is a StoreQuery instance for pagination.

```
Action<KnetikResult<StoreQuery>> callback = null;
callback = (KnetikResult<StoreQuery> result) => {
    // Process the current page of items
    Debug.Log (result.Value.Items);

    // If there’s more, load the next page with this same callback.
    if (result.Value.HasMore) {
        result.Value.NextPage(callback);
    }
};

var store = KnetikClient.Instance.Store;
store.Terms = new List<string> { “T-Shirts” };
store.Load (callback);
```

Example searching for all related items to a game:

```
Action<KnetikResult<StoreQuery>> callback = null;
callback = (KnetikResult<StoreQuery> result) => {
    // Process the current page of items
    Debug.Log (result.Value.Items);

    // If there’s more, load the next page with this same callback.
    if (result.Value.HasMore) {
        result.Value.NextPage(callback);
    }
};

client.userInfo.LoadWithGame(gameId, (res) => {
  Game game = res.Value;

  var store = KnetikClient.Instance.Store;
  store.Terms = null; // not going to search by terms
  store.Related = new string[] { game.UniqueKey };
  store.Load (callback);  
});
```

###4.6 Cart

The cart can be accessed through KnetikClient.Instance.Cart, which is a Cart instance and stores the items in the cart and provides some methods for manipulating it.

```
var client = Knetik.KnetikClient.Instance;
client.Store.Load((resStore) => {
    var firstItem = resStore.Value.Items[0];
    var sku = firstItem.Skus[0];

    // Let's add 10 of the first item's first SKU
    client.Cart.Add(sku, 10, (resCartAdd) => {
        Debug.Log("Item added to cart!");

        client.Cart.Load((resCartLoad) => {
          Debug.Log("Grand Total is " + resCartLoad.Value.GrandTotal);

          client.Checkout((resCheckout) => {
            if (resCheckout.IsSuccess) {
              Debug.Log("Checkout successful!");
            } else {
              Debug.Log("Checkout failed!");
            }
          });
        });
    });
});
```

###4.7 Leaderboard

To load a leaderboard, you need a leaderboard ID from the admin.  You can load and display the entries from the leaderboard like so:

```
KnetikClient.Instance.Leaderboard(KnetikGlobal.LeaderboardID).Load((res) => {
  if (res.Response.IsSuccess) {
    var leaderboard = res.Value;

    string output = "";
    
    // only show top 10 entries
    int count = Math.Min(10, leaderboard.Entries.Count);

    for (int i = 0; i < count; i++) {
      var entry = leaderboard.Entries[i];
      output += entry.DisplayName + ": " + (int)entry.Score + "\n";
    }

    LeaderboardText.text = output;
  }
});
```

####4.8 Quick Purchase

For one-click purchase scenarios, there is a shortcut method QuickPurchase that will add an item to the cart and checkout in a single call:

```
var client = Knetik.KnetikClient.Instance;
client.Store.Load((resStore) => {
    var firstItem = resStore.Value.Items[0];
    var sku = firstItem.Skus[0];

    client.Cart.QuickPurchase(sku, (resCartQuickPurchase) => {
        if (resCartQuickPurchase.IsSuccess) {
            Debug.Log(“Quick purchase successful!");
        } else {
            Debug.Log(“Quick purchase failed!");
       }
    });
});
```

####4.9 Inventory

When you use UserInfo.LoadWithGame, the UserInfo will contain the Inventory object.  The inventory contains a list of all the items the user has that are related to the game.  You can get a list of all the items in the inventory using the "Inventory.Items" property.

Each item is an instance of InventoryItem.  This instance contains some information about the user's instance of that item, such as how many times it has been used and when it expires (null if it doesn't).  You can call InventoryItem.Consume() to use the item, causing the UseCount to increase.  If the UseCount matches the the item's Consumable behavior's MaxUse value, the item will be used up and will not be in the user's inventory anymore.

For downloadable content, the Downloadable behavior contains the URLs by name as set up in the admin.  With the URL, you can load images or sound files in dynamically based on what is in the user's inventory.


####5.0 Invoices

to load your cart invoices you need to have Card Number (cart GUID)

### GetInvoice

to Get your invoice by cart ID Your cart should has at least one item and the shipping address of your cart should be correct

How to prepare your cart ?

1-Create cart by calling Client.CartCreate (createResponse); mentioned before in details

2-Add Item to a cart by calling client.CartAdd(cartNumber,catalogId, skuId, quantity); mentioned before in details 

3-Modify your cart shipping address Client.CartShippingAddress(cartNumber,shipping); mentioned before in details 

How to call GetInvoice EndPoint?

```
Client.GetInvoice (cartNumber, (res) => {
	if (res.IsSuccess) {
	//parse response
	Console.WriteLine("GetInvoice PASS");

	}else
{
	Console.WriteLine("GetInvoice Failed");
	return;
}
	});
```

###5.1 GetInvoice Response 
```
{
  "error": {
    "code": 0,
    "success": true
  },
  "result": {
    "invoices": [
      {
        "name_prefix": "prefixName",
        "phone": null,
        "city_name": "AdressLin1",
        "phone_number": null,
        "billing_postal_code": null,
        "current_fulfillment_status": 9,
        "current_fulfillment_status_name": "unfulfilled",
        "billing_city_name": null,
        "address1": "AdressLin1",
        "address2": "AdressLin1",
        "current_fulfillment_status_description": "The invoice has not yet been fulfilled",
        "current_status_name": "new",
        "state_name": "Isle of Man",
        "country_name": "United Kingdom",
        "currency": "USD",
        "id": 348483,
        "shipping": 4.59,
        "cart_id": 348479,
        "create_date": "2015-09-03 19:48:21",
        "fed_tax": 0.0,
        "current_status_description": "The invoice has been created.",
        "user_id": 348408,
        "billing_state_name": null,
        "billing_country_name": null,
        "grand_total": 29.58,
        "billing_address1": null,
        "billing_address2": null,
        "state_tax": 0.0,
        "subtotal": 24.99,
        "vendor_id": 3,
        "deleted": 0,
        "current_status": 1,
        "discount": 0.0,
        "parent_invoice_id": null,
        "order_notes": "notes",
        "email": "knetik@knetik.com",
        "postal_code": "123123",
        "update_date": "2015-09-03 19:48:21",
        "full_name": "First Name First Name"
      }
    ],
    "discounts": [
      
    ],
    "items": [
      {
        "type_hint": "physical_item",
        "system_price": 16.0,
        "qty": 1,
        "sku_description": "Symbiosis Black, Size: M",
        "affiliate_id": null,
        "sku": "27977-M",
        "deleted": 0,
        "unit_price": 24.99,
        "total_price": 24.99,
        "id": 584,
        "thumbnail": "/uploads/item_assets/noise/ingested/abulletforprettyboy-abpbsymbbl-ts.jpg",
        "item_name": "A Bullet For Pretty Boy, Symbiosis Black, T-Shirt",
        "item_id": 55218,
        "sku_id": 7225,
        "invoice_id": 348483
      },
      {
        "type_hint": "shipping_item",
        "system_price": 2.98,
        "qty": 1,
        "sku_description": "Tracked",
        "affiliate_id": null,
        "sku": "579230",
        "deleted": 0,
        "unit_price": 4.59,
        "total_price": 4.59,
        "id": 585,
        "thumbnail": "",
        "item_name": "UK Tracked 1KG",
        "item_id": 68714,
        "sku_id": 38336,
        "invoice_id": 348483
      }
    ]
  },
  "cached": false,
  "message": "",
  "parameters": [
    
  ],
  "requestId": "1441309701613-8667"
}
```

###5.2 Apple transaction

Mark an invoice paid using Apple payment recipt.

#### VerifyReceipt

Requirements 

1- The details of the transaction must match the invoice details , including the product_id matching the sku text of the item in the invoice.

#### How to call VerifyReceipt EndPoint ?

```
Client.verifyReceipt (string recipent , string TRANSACTION_ID , int64 invoiceID,(res) => 
	{
	if (res.IsSuccess) {
	//parse response
	Console.WriteLine("verifyReceipt PASS");
	}else
	{
	Console.WriteLine("verifyReceipt Failed");
	return;
	}
	});

```
####Response 

Returns the transaction Id if successful.

```
{
  "error": {
    "code": 0,
    "success": true
  },
  "result": "1000000170178593", // transactionID should be the same of result 
  "cached": false,
  "message": "",
  "parameters": [
    
  ],
  "requestId": null
}


```

##5.4 Entitlement services

#### How to call entitlement Check EndPoint ?

```
KnetikClient.Instance.entitlementCheck("itemId","SkuId",(ress)=>{
	if(ress.IsSuccess){
		// parse response 
	}
	else{

	}
		});
```

####Response 

```
{
  "error": {
    "code": 0,
    "success": true
  },
  "result": null,
  "cached": false,
  "message": "",
  "parameters": null,
  "requestId": "1442383363750-1268"
}
```

```
{
  "error": {
    "code": 10,
    "success": false
  },
  "result": null,
  "cached": false,
  "message": "You are not entitled to this item. It may require a purchase or subscription.",
  "parameters": null,
  "requestId": "1442383363750-1268"
}
```

##5.5 Android Compatibility

On Android, the ACCESS_WIFI_STATE permission must be set in the AndroidManifest.xml file in your project’s Assets->Plugins->Android folder.  An example AndroidManifest.xml has been included in this project, called “AndroidManifest.example.xml”.
