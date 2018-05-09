using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Facebook.Unity;

/******************************************
 * 
 * FacebookController
 * 
 * It manages all the Facebook operations, login and payments.
 * 
 * @author Esteban Gallardo
 */
public class FacebookController : StateManager
{
    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------	
    public const string EVENT_FACEBOOK_REQUEST_INITIALITZATION = "EVENT_FACEBOOK_REQUEST_INITIALITZATION";
    public const string EVENT_FACEBOOK_MY_INFO_LOADED = "EVENT_FACEBOOK_MY_INFO_LOADED";
    public const string EVENT_FACEBOOK_FRIENDS_LOADED = "EVENT_FACEBOOK_FRIENDS_LOADED";
    public const string EVENT_FACEBOOK_COMPLETE_INITIALITZATION = "EVENT_FACEBOOK_COMPLETE_INITIALITZATION";
    public const string EVENT_REGISTER_IAP_COMPLETED = "EVENT_REGISTER_IAP_COMPLETED";

    // ----------------------------------------------
    // SINGLETON
    // ----------------------------------------------	
    private static FacebookController _instance;

    public static FacebookController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType(typeof(FacebookController)) as FacebookController;
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "FacebookController";
                    _instance = container.AddComponent(typeof(FacebookController)) as FacebookController;
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }

    // ----------------------------------------------
    // MEMBERS
    // ----------------------------------------------
    private string m_id = null;
    private string m_nameHuman;
    private string m_email;
    private bool m_isInited = false;
    private bool m_invitationAccepted = false;
    private bool m_isConnectedToWebSocket = false;

    private List<ItemMultiTextEntry> m_friends = new List<ItemMultiTextEntry>();

    public string Id
    {
        get { return m_id; }
        set { m_id = value; }
    }
    public string NameHuman
    {
        get { return m_nameHuman; }
        set { m_nameHuman = value; }
    }
    public string Email
    {
        get { return m_email; }
    }
    public List<ItemMultiTextEntry> Friends
    {
        get { return m_friends; }
    }
    public bool IsInited
    {
        get { return m_isInited; }
    }
    public bool InvitationAccepted
    {
        get { return m_invitationAccepted; }
    }
    public bool IsConnectedToWebSocket
    {
        get { return m_isConnectedToWebSocket; }
    }

    // ----------------------------------------------
    // CONSTRUCTOR
    // ----------------------------------------------	
    // -------------------------------------------
    /* 
	 * Constructor
	 */
    private FacebookController()
    {
    }

    // -------------------------------------------
    /* 
     * InitListener
     */
    public void InitListener()
    {
        BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
    }

    // -------------------------------------------
    /* 
     * Destroy
     */
    public void Destroy()
    {
        BasicEventController.Instance.BasicEvent -= OnBasicEvent;
        DestroyObject(_instance.gameObject);
        _instance = null;
    }

    // -------------------------------------------
    /* 
     * Initialitzation
     */
    public void Initialitzation()
    {
        if (!FB.IsInitialized)
        {
            if (!m_isInited)
            {
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                InitListener();
                FB.Init(this.OnInitComplete, this.OnHideUnity);
            }
            else
            {
                InitListener();
                RegisterConnectionFacebookID(true);
            }
        }
        else
        {
            // Already initialized, signal an app activation App Event
            InitListener();
            FB.ActivateApp();
            OnInitComplete();
        }
    }

    // -------------------------------------------
    /* 
     * OnInitComplete
     */
    private void OnInitComplete()
    {
        BasicEventController.Instance.DispatchBasicEvent(EVENT_FACEBOOK_REQUEST_INITIALITZATION);
        if (ScreenController.Instance.DebugMode)
        {
            Debug.Log("Success - Check log for details");
            Debug.Log("Success Response: OnInitComplete Called");
            Debug.Log("OnInitCompleteCalled IsLoggedIn='{" + FB.IsLoggedIn + "}' IsInitialized='{" + FB.IsInitialized + "}'");
            if (AccessToken.CurrentAccessToken != null)
            {
                Debug.Log(AccessToken.CurrentAccessToken.ToString());
            }
        }
        LogInWithPermissions();
    }

    // -------------------------------------------
    /* 
     * OnHideUnity
     */
    private void OnHideUnity(bool _isGameShown)
    {
        if (ScreenController.Instance.DebugMode)
        {
            Debug.Log("Success - Check log for details");
            Debug.Log("Success Response: OnHideUnity Called {" + _isGameShown + "}");
            Debug.Log("Is game shown: " + _isGameShown);
        }
    }

    // -------------------------------------------
    /* 
     * LogInWithPermissions
     */
    private void LogInWithPermissions()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, LoggedWithPermissions);
    }

    // -------------------------------------------
    /* 
     * LoggedWithPermissions
     */
    private void LoggedWithPermissions(IResult _result)
    {
        if (_result == null)
        {
            if (ScreenController.Instance.DebugMode) Debug.Log("Null Response");
            return;
        }

        if (ScreenController.Instance.DebugMode) Debug.Log("FacebookController::LoggedWithPermissions::result.RawResult=" + _result.RawResult);
        FB.API("/me?fields=id,name,email", HttpMethod.GET, HandleMyInformation);
    }

    // -------------------------------------------
    /* 
     * HandleMyInformation
     */
    private void HandleMyInformation(IResult _result)
    {
        if (_result == null)
        {
            if (ScreenController.Instance.DebugMode) Debug.Log("Null Response");
            return;
        }

        JSONNode jsonResponse = JSONNode.Parse(_result.RawResult);

        m_id = jsonResponse["id"];
        m_nameHuman = jsonResponse["name"];
        m_email = jsonResponse["email"];

        if (ScreenController.Instance.DebugMode) Debug.Log("CURRENT PLAYER NAME=" + m_nameHuman + ";ID=" + m_id);

        BasicEventController.Instance.DispatchBasicEvent(EVENT_FACEBOOK_MY_INFO_LOADED);

        if (ScreenController.Instance.DebugMode) Debug.Log("FacebookController::HandleMyInformation::result.RawResult=" + _result.RawResult);
        FB.API("/me/friends", HttpMethod.GET, HandleListOfFriends);
    }

    // -------------------------------------------
    /* 
     * HandleListOfFriends
     */
    private void HandleListOfFriends(IResult _result)
    {
        if (_result == null)
        {
            if (ScreenController.Instance.DebugMode) Debug.Log("Null Response");
            return;
        }

        if (ScreenController.Instance.DebugMode) Debug.Log("FacebookController::HandleListOfFriends::result.RawResult=" + _result.RawResult);
        JSONNode jsonResponse = JSONNode.Parse(_result.RawResult);

        JSONNode friends = jsonResponse["data"];
        if (ScreenController.Instance.DebugMode) Debug.Log("FacebookController::HandleListOfFriends::friends.Count=" + friends.Count);
        for (int i = 0; i < friends.Count; i++)
        {
            string nameFriend = friends[i]["name"];
            string idFriend = friends[i]["id"];
            m_friends.Add(new ItemMultiTextEntry(idFriend, nameFriend));
            if (ScreenController.Instance.DebugMode) Debug.Log("   NAME=" + nameFriend + ";ID=" + idFriend);
        }

        BasicEventController.Instance.DispatchBasicEvent(EVENT_FACEBOOK_FRIENDS_LOADED);

        // INIT PAYMENT METHOD
        RegisterConnectionFacebookID(true);
    }

    // -------------------------------------------
    /* 
    * RegisterConnectionFacebookID
    */
    public void RegisterConnectionFacebookID(bool _dispatchCompletedFacebookInit)
    {
        // START BASIC CONNECTION
        if (m_id != null)
        {
            m_isInited = true;
        }
        else
        {
            m_isInited = false;
        }
        if (_dispatchCompletedFacebookInit)
        {
            BasicEventController.Instance.DispatchBasicEvent(EVENT_FACEBOOK_COMPLETE_INITIALITZATION, m_id, m_nameHuman, m_email);
        }
    }

    // -------------------------------------------
    /* 
	 * OnBasicEvent
	 */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (_nameEvent == EVENT_REGISTER_IAP_COMPLETED)
        {
        }
    }

    // -------------------------------------------
    /* 
     * GetPackageFriends
     */
    public string GetPackageFriends()
    {
        string output = "";
        for (int i = 0; i < m_friends.Count; i++)
        {
            output += m_friends[i].Items[0] + "," + m_friends[i].Items[1];
            if (i < m_friends.Count - 1)
            {
                output += ";";
            }
        }
        return output;
    }

    // -------------------------------------------
    /* 
     * SetFriends
     */
    public void SetFriends(string _data)
    {
        string[] friendsList = _data.Split(';');
        m_friends.Clear();
        for (int i = 0; i < friendsList.Length; i++)
        {
            string[] sFriendEntry = friendsList[i].Split(',');
            if (sFriendEntry.Length == 2)
            {
                m_friends.Add(new ItemMultiTextEntry(sFriendEntry[0], sFriendEntry[1]));
                if (ScreenController.Instance.DebugMode) Debug.Log("FacebookController::SetFriends::FRIEND[" + sFriendEntry[0] + "][" + sFriendEntry[1] + "]");
            }
        }
    }

    private string m_urlProduct;

    // -------------------------------------------
    /* 
     * RentIAP
     */
    public void PurchaseIAP(string _urlProduct)
    {
        m_urlProduct = _urlProduct;
        FB.Canvas.PayWithProductId(
          m_urlProduct,
          "purchaseiap",
          1,
          null, null, null, null, null,
          callback: delegate (IPayResult response)
          {
              if (ScreenController.Instance.DebugMode) Debug.Log("PurchaseIAP::CALLBACK RESPONSE=" + response.RawResult);
              PurchaseIAPResponse(response.RawResult);
          }
        );
    }

    // -------------------------------------------
    /* 
     * CheckResponseJSON
     */
    private bool CheckResponseJSON(JSONNode _response)
    {
        if (_response["error_code"] != null)
        {
            return false;
        }
        if (_response["error"] != null)
        {
            return false;
        }
        if (_response["cancelled"] != null)
        {
            if (_response["cancelled"].AsBool)
            {
                return false;
            }
        }
        return true;
    }

    // -------------------------------------------
    /* 
     * PurchaseIAPResponse
     */
    private void PurchaseIAPResponse(string _response)
    {
        JSONNode jsonEverything = JSONNode.Parse(_response);
        JSONNode jsonResponse = jsonEverything["response"];

        bool codeResponse = CheckResponseJSON(jsonEverything);
        if (codeResponse) codeResponse = CheckResponseJSON(jsonResponse);

        if (codeResponse)
        {
            // string paymentId = jsonResponse["payment_id"]; // string id
            // string productId = jsonResponse["product_id"]; // string id
            // int quantity = int.Parse(jsonResponse["quantity"]);  // int
            // string signedRequest = jsonResponse["signed_request"];  // signed_request string
            BasicEventController.Instance.DispatchBasicEvent(IAPController.EVENT_IAP_CONFIRMATION, true, m_urlProduct);
        }
        else
        {
            BasicEventController.Instance.DispatchBasicEvent(IAPController.EVENT_IAP_CONFIRMATION, false, m_urlProduct);
        }
    }

}


