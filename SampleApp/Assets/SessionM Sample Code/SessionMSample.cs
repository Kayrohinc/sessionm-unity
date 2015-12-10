using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SessionMSample : MonoBehaviour
{
	public SessionM sessionM;

	public TextMesh sessionMStateLabel;

	public TextMesh optOutLabel;
	public TextMesh isRegisteredLabel;
	public TextMesh isLoggedInLabel;
	public TextMesh pointBalanceLabel;
	public TextMesh unclaimedAchCountLabel;
	public TextMesh unclaimedAchValueLable;
	public TextMesh tierLable;

  public string signUpUser = "unittest@sessionm.com";

	public string action1;
	public string action2;
	public string action3;

	public AchievementToast toaster;

	//Exposed Methods

	public void OnAction1Clicked()
	{
		sessionM.StartSession("0bfeb00013f0f634420a04ed5806a66a58d49d8b");
	}

	public void OnAction2Clicked()
	{
		sessionM.LogAction(action2);
	}

	public void OnAction3Clicked()
	{
		sessionM.LogAction(action3);
	}

	public void OnTierClicked()
	{
  }

	public void OnLoginClicked() {
	  Debug.Log("LogIn: Email: " + signUpUser + ", Password: " + "sessionm");
	  sessionM.LogInUserWithEmail (signUpUser, "sessionm");
	}

	public void OnSignupUser() {
	  signUpUser = "Testing-" + Random.Range(0.0f, 1.0f) + "@sessionm.com";
	  Debug.Log("Signup: Email: " + signUpUser + ", Password: " + "sessionm");
	  sessionM.SignUpUser(signUpUser, "sessionm", "1960", "Male", "01752");
	}

	public void OnLogoutClicked() {
	  Debug.Log("LogOut: Email: " + signUpUser);
	  sessionM.LogOutUser ();
	}

	public void OnPortalClicked()
	{
		sessionM.ShowPortal();
	}

	//Helper Methods

	private void NotifySessionError(int errorCode, string error)
	{
		Debug.LogError("SessionM Error Reported: " + errorCode.ToString() + " - " + error);
	}

	private void NotifySessionStateChanged(SessionState state)
	{
		Debug.Log("Event: NotifySessionStateChanged Fired");
		sessionMStateLabel.text = "SessionM State: " + state.ToString();
	}

  	private void NotifyFeedChanged(string latest) {
	    List<FeedMessageData> messages = sessionM.GetMessagesList();
	    foreach (FeedMessageData message in messages) {
	      Debug.Log("Message Id: " + message.GetID() + ", Description: " + message.GetDescription());
	    }
	}

	private void NotifyUnclaimedAchievementDataUpdated(IAchievementData achievementData)
	{
		Debug.Log("Recieved New Achievement: " + achievementData.GetName() + " - Worth: " + achievementData.GetMpointValue() + "\n With Message: " + achievementData.GetMessage());

		toaster.ShowAchievementToast(achievementData.GetName(), achievementData.GetMpointValue(), achievementData.GetMessage());
	}

	private void UserChanged(IDictionary<string, object> userInfo)
	{
		UserData user = SessionM.GetInstance().GetUserData();

		if(user == null)
			return;

		optOutLabel.text = "Opt Out: " + user.IsOptedOut().ToString();
		isRegisteredLabel.text = "Is Registered: " + user.IsRegistered();
		isLoggedInLabel.text = "Is Logged In: " + user.IsLoggedIn();
		pointBalanceLabel.text = "Point Balance: " + user.GetUserPointBalance();
		unclaimedAchCountLabel.text = "Unclaimed Achievement Count: " + user.GetUnclaimedAchievementCount();
		unclaimedAchValueLable.text = "Unclaimed Achievement Value: " + user.GetUnclaimedAchievementValue();
		tierLable.text = "My Tier: " + "Not Functioning";

		string[] rewards = sessionM.GetRewards();
		Debug.Log("Rewards: " + rewards.Length);
		for (int i = 0 ; i < rewards.Length ; i++) {
	 	  Debug.Log("Reward: (" + i + ") :" + rewards[i]);
		}
	}

	//Unity Lifecycle

	private void Awake()
	{
		//Set service region before SessionM instance is activated
		// SessionM.SetServiceRegion(ServiceRegion.USA);
		// SessionM.SetServerType("https://api.tour-sessionm.com");
		SessionM.SetShouldAutoUpdateAchievementsList(true);
		SessionM.SetMessagesEnabled(true);
		SessionM.SetSessionAutoStartEnabled(false);
		sessionM.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		//Assign useful events to Helper Functions in the class.
		SessionMEventListener.NotifySessionStateChanged += NotifySessionStateChanged;
		SessionMEventListener.NotifySessionError += NotifySessionError;
		SessionMEventListener.NotifyFeedChanged += NotifyFeedChanged;
		SessionMEventListener.NotifyUnclaimedAchievementDataUpdated += NotifyUnclaimedAchievementDataUpdated;
		SessionMEventListener.NotifyUserInfoChanged += UserChanged;

		UserChanged(null);
	}

	private void OnDisable()
	{
		//Clean Up the events in case this object is destroyed.
		SessionMEventListener.NotifySessionStateChanged -= NotifySessionStateChanged;
		SessionMEventListener.NotifySessionError -= NotifySessionError;
		SessionMEventListener.NotifyFeedChanged -= NotifyFeedChanged;
		SessionMEventListener.NotifyUnclaimedAchievementDataUpdated -= NotifyUnclaimedAchievementDataUpdated;
		SessionMEventListener.NotifyUserInfoChanged -= UserChanged;
	}

}
