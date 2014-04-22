using UnityEngine;
using System.Collections;
using Knetik;

public class StartMenuController : MonoBehaviour {
	
	// Common GUI skin:
	public GUISkin guiSkin;
	
	// GUI styles for labels:
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle; 
	public GUIStyle formFieldStyle;
	public GUIStyle errorMessageStyle;

	public string m_Key;
	
	// Active view name:
	string activeViewName = LoginView.NAME;
	
	// Map views by name:
	Hashtable viewByName;
	
	// Login view:
	LoginView loginView = new LoginView();
	
	RegistrationView registrationView = new RegistrationView();
	
	// Do we need block UI:
	bool blockUI = false;
	
	// Handler of registration button click:
	// Use this for initialization
	void Start () {
	    // Setup of login view:
	    loginView.guiSkin = guiSkin;
	    loginView.header1Style = header1Style;
	    loginView.header2Style = header2Style;
	    loginView.header2ErrorStyle = header2ErrorStyle;
	    loginView.formFieldStyle = formFieldStyle;
		loginView.openRegistrationHandler = onRegisterButtonClick;
		loginView.enterGameHandler = onEnterGameClick;
		
		registrationView.guiSkin = guiSkin;
		registrationView.header2Style = header2Style;
		registrationView.formFieldStyle = formFieldStyle;
		registrationView.errorMessageStyle = errorMessageStyle;		
		registrationView.cancelHandler = onRegisterCancelClick;
		registrationView.registrationHandler = onRegisterClick;

	    viewByName = new Hashtable();
		// Adding login view to views by name map:
	    viewByName[LoginView.NAME] = loginView;	
		viewByName[RegistrationView.NAME] = registrationView;

		/****************************************
		// KNETIK-API
		// The below are placeholder host and keys
		****************************************/
		ApiUtil.setApiHost("dev.sapi.com");
		ApiUtil.setClientKey("client_key");
		ApiUtil.setClientSecret("secret_key");
		/****************************************
		// KNETIK-API
		****************************************/
	}

	void onRegisterButtonClick() {
        // Clear reistration fields:
        registrationView.data.clear();
        // Set active view to registration:
        activeViewName = RegistrationView.NAME;
	}
	
	void onRegisterCancelClick() {
 		// Clear reistration fields:
        loginView.data.clear();
		
        // Set active view to registration:
        activeViewName = LoginView.NAME;		
	}

	void login() {
		LoginRequest login = new LoginRequest(loginView.data.login, loginView.data.password);
		
		//yield return login.doLogin(); /* TODO */
		if (login.doLogin()) {
			m_Key = login.getKey();
			UserSessionUtils.setUserSession(login.getUserId(), login.getUsername(), login.getKey());
			Application.LoadLevel(1);
		} else {
			loginView.error = true;
    		loginView.errorMessage = login.getErrorMessage();			
		}		
		
		blockUI = false;
	}

	void onEnterGameClick() {
		blockUI = true; 
		
		//StartCoroutine(login());
		login();
	}
	
	void onRegisterClick() {
		if (registrationView.data.password != registrationView.data.passwordConfirm) {
			registrationView.error = true;
    		registrationView.errorMessage = "Password and Confirm password don't match!";			
			return;
		}
		
		LoginRequest lr = new LoginRequest();
		if (!lr.doLoginAsGuest()) {
			registrationView.error = true;
    		registrationView.errorMessage = lr.getErrorMessage();
			return;
		}

		Debug.Log("Guest session key: lr.getKey()");
		RegisterRequest ur = new RegisterRequest(lr.getKey(), 
			registrationView.data.login, 
			registrationView.data.password, 
			registrationView.data.email, 
			registrationView.data.login);
		if(!ur.doRegister()) {
			registrationView.error = true;
    		registrationView.errorMessage = ur.getErrorMessage();			
			return;
		}
		
		m_Key = lr.getKey();
		UserSessionUtils.setUserSession(0, registrationView.data.login, lr.getKey());
		Application.LoadLevel(1);
	}
	
	void OnGUI() {
		// Getting current view by active view name:
	    View currentView  = (View) viewByName[activeViewName];
	 
	    // Set blockUI for current view:
	    currentView.setBlockUI(blockUI);
	
	    // Rendering current view:
	    currentView.render();
	
	    // Show box with "Wait..." when UI is blocked:
	    int screenWidth = Screen.width;
	    int screenHeight = Screen.height;
	    if(blockUI) {
	        GUI.Box(new Rect((screenWidth - 200)/2, (screenHeight - 60)/2, 200, 60), "Wait...");
	    }		
	}
}
