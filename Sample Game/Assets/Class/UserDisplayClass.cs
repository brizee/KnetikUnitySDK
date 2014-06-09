using UnityEngine;
using System.Collections;
using Knetik;
using System;

public class UserDisplayClass : MonoBehaviour {

	public UserInfoRequest userGet;
	public UserInfoRequest userPut;
	public ProductInfoRequest product;
	public PostUserOptionsRequest userUpdate;
	public PostUserOptionsRequest userInsert;
	public LeaderboardRequest leaderboard;
	public bool userGet_result;
	public bool userPut_result;
	public bool product_result;
	public bool leaderboard_result;
	public bool test;
	public GUIText Currency;
	public GUIText BombCounter;
	public static int sharksDestroyedCount;
	public static int bombsLeft; 
	public static int productId;
	public static string avatar_url;
	public static string mode;
	public static string lang;
	public static int leaderboardId;
	public static int levelId;
	
	// Use this for initialization
	void Start () {
		productId = 43;
		sharksDestroyedCount = 0;
		leaderboardId = 6;
		bombsLeft = 30;
		avatar_url = "http://www.avatarist.com/avatars/Cartoons/Family-Guy/Brian-small.gif";
		lang = "ar";  // Change to Arabic
		levelId = 1;
		Currency = GameObject.Find("Currency").guiText;
		BombCounter = GameObject.Find("BombCounter").guiText;
		Debug.Log ("Grabbing User Info");
		userGet = new UserInfoRequest(UserSessionUtils.getApiKey(), productId);
		Debug.Log ("Grabbing Product Info");
		product = new ProductInfoRequest(UserSessionUtils.getApiKey(), productId);
		product_result = product.doGetInfo();
		userGet_result = userGet.doGetInfo();
		Debug.Log ("User ID: " + userGet.id);
		userPut = new UserInfoRequest(UserSessionUtils.getApiKey(), userGet.id, avatar_url, lang);

		Debug.Log ("Getting Leaderboard Info");
		leaderboard = new LeaderboardRequest(UserSessionUtils.getApiKey(), leaderboardId);
		leaderboard_result = leaderboard.doGetInfo();
		Debug.Log ("Leaderboard Result: " + leaderboard_result);
		Debug.Log ("Are we gonna print it?");
		foreach (string key in leaderboard.user_results.Keys)
		{
			Debug.Log ("User Results Key: " + key + ", User Results Value: " + leaderboard.user_results[key]);
		}
//		Debug.Log ("Sending User Options Info");
//		userUpdate = new PostUserOptionsRequest(UserSessionUtils.getApiKey(), user.id, productId, "GG", "NNN");
//		userUpdate.postUserInfo("update");

		Debug.Log ("Sending User Update Info");
		mode = "avatar";
		userPut_result = userPut.putUserInfo(mode);
		Debug.Log ("User Put Result for Avatar: " + userPut_result);
		mode = "lang";
		userPut_result = userPut.putUserInfo(mode);
		Debug.Log ("User Put Result for Lang: " + userPut_result);
	}
	
	// Update is called once per frame
	void Update () {
		if (userGet_result) {
			string money = null;
			if (userGet.user_options.TryGetValue("money_balance", out money))
			{
				if (money == null)
				{

					Currency.guiText.text = "$"+userGet.money_balance;
				}
				else
				{
					Currency.guiText.text = "$"+money;
				}
			}
		}
		else {
			Currency.guiText.text = "Currency Not Available";
		}
		if (product_result) {
			string bombed = null;

				if (bombed == null)
				{
					BombCounter.guiText.text = "Sharks Bombed: " + sharksDestroyedCount  + "\n" + "Bombs Left:" + bombsLeft;
				}
				else
				{
					BombCounter.guiText.text = "Sharks Bombed: " + bombed  + "\n" + "Bomb\ts Left:" + bombsLeft;
				}

		}
		else {
			BombCounter.guiText.text = "Sharks Bombed: " + sharksDestroyedCount + "\n" + "Bombs Left:" + bombsLeft;
		}
	}
	
	
}
