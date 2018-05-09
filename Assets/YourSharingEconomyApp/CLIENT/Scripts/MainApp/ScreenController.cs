using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;


public enum TypePreviousActionEnum
{
    DESTROY_ALL_SCREENS = 0x00,
    DESTROY_CURRENT_SCREEN = 0x01,
    KEEP_CURRENT_SCREEN = 0x02,
    HIDE_CURRENT_SCREEN = 0x03
}

/******************************************
 * 
 * YourCollaborativeEconomyApp
 * 
 * ScreenManager controller that handles all the screens's creation and disposal
 * 
 * @author Esteban Gallardo
 */
public class ScreenController : MonoBehaviour
{
    // ----------------------------------------------
    // CONFIGURATION
    // ----------------------------------------------	
    public const string URL_BASE_PHP = "http://localhost:8080/impaciencia/";

    // ENCRYPTION KEYS: CHANGE IT TO MATCH IN THE SERVER SIDE
    public const string KYRJEncryption = "sK1rwpD1p+5e#bvt31CK13z77n=ES8jR"; //32 chr shared ascii string (32 * 8 = 256 bit)
    public const string SIVJEncryption = "A9q2N2haeQybv8#Aq!N9ybc1Cnrx12@y"; //32 chr shared ascii string (32 * 8 = 256 bit)

    // COOKIES
    public const string USER_EMAIL_COOCKIE = "USER_EMAIL_COOCKIE";
    public const string USER_NAME_COOCKIE = "USER_NAME_COOCKIE";
    public const string USER_PASSWORD_COOCKIE = "USER_PASSWORD_COOCKIE";
    public const string USER_FACEBOOK_CONNECTED_COOCKIE = "USER_FACEBOOK_CONNECTED_COOCKIE";

    // TABLES
    public const string TABLE_REQUESTS  = "requests";
    public const string TABLE_USERS     = "users";

    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------	
    public const string EVENT_SCREENMANAGER_OPEN_SCREEN         = "EVENT_SCREENMANAGER_OPEN_SCREEN";
    public const string EVENT_SCREENMANAGER_DESTROY_SCREEN      = "EVENT_SCREENMANAGER_DESTROY_SCREEN";
    public const string EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON = "EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON";
    public const string EVENT_GENERIC_MESSAGE_INFO_OK_BUTTON    = "EVENT_GENERIC_MESSAGE_INFO_OK_BUTTON";

    // ----------------------------------------------
    // SINGLETON
    // ----------------------------------------------	
    private static ScreenController _instance;

    public static ScreenController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType(typeof(ScreenController)) as ScreenController;
            }
            return _instance;
        }
    }

    // ----------------------------------------------
    // PUBLIC MEMBERS
    // ----------------------------------------------	
    public TextAsset ReadMeFile;
    [Tooltip("It allows the debug of most common messages")]
    public bool DebugMode = true;
    [Tooltip("It allows the debug communications")]
    public bool DebugComms = true;
    [Tooltip("It allows the iaps")]
    public bool DebugIAPs = true;

    [Tooltip("All the screens used by the application")]
    public GameObject[] ScreensPrefabs;

    // ----------------------------------------------
    // DRESSMAKERS PUBLIC VARIABLES
    // ----------------------------------------------	
    [Tooltip("Slot request with image")]
    public GameObject SlotInfoRequestImage;

    [Tooltip("Slot offer")]
    public GameObject SlotOffer;

    [Tooltip("Slot image")]
    public GameObject SlotImage;

    [Tooltip("Image reference link")]
    public Sprite ImageReferenceLink;

    [Tooltip("Image Toxic Confirmed")]
    public Sprite ImageToxicConfirmed;

    [Tooltip("Image Toxic Possible")]
    public Sprite ImageToxicPossible;

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------	
    private List<GameObject> m_screensPool = new List<GameObject>();
    private List<GameObject> m_screensOverlay = new List<GameObject>();
    private bool m_enableScreens = true;
    private bool m_enableDebugTestingCode = false;

    private SearchModel m_lastSearchModel;

    // HOURS BE ABLE TO MAKE A NEW PROPOSAL
    private int m_hoursToEnableANewProposal = 3;

    // NUMBER OF REPORTS FROM USERS TO SHOW A WARNING MESSAGE ON THE REQUEST
    private int m_totalReportsToWarnRequest = 3;

    // TOTAL NUMBER OF FREE REQUESTS AVAILABLE AT THE SAME TIME
    private int m_totalNumberOfFreeRequests = 1;

    // TOTAL NUMBER OF IMAGES THAT CAN BE USED AS A REFERENCE
    private int m_totalNumberImagesAsReference = 4;

    // TOTAL NUMBER OF IMAGES THAT CAN BE USED AS A FINISHED JOB
    private int m_totalNumberImagesAsFinished = 4;

    // TOTAL NUMBER OF IMAGES THAT THE PROVIDER CAN POST TO SHOW HIS EXPERIENCE
    private int m_totalNumberImagesAsProviderExperience = 10;

    // SIZE TO SCALE THE IMAGES SELECTED BY THE USER
    private int m_sizeHeightAllowedImages = 600;

    // SKILLS OF THE PROVIDERS
    private string m_providerSkills;

    // ----------------------------------------------
    // GETTERS/SETTERS
    // ----------------------------------------------	
    public bool EnableDebugTestingCode
    {
        get { return m_enableDebugTestingCode; }
        set { m_enableDebugTestingCode = value; }
    }
    public SearchModel LastSearchModel
    {
        get { return m_lastSearchModel; }
        set { m_lastSearchModel = value; }
    }
    public int HoursToEnableANewProposal
    {
        get { return m_hoursToEnableANewProposal; }
        set { m_hoursToEnableANewProposal = value; }
    }
    public int TotalReportsToWarnRequest
    {
        get { return m_totalReportsToWarnRequest; }
        set { m_totalReportsToWarnRequest = value; }
    }
    public int TotalNumberOfFreeRequests
    {
        get { return m_totalNumberOfFreeRequests; }
        set { m_totalNumberOfFreeRequests = value; }
    }
    public int TotalNumberImagesAsReference
    {
        get { return m_totalNumberImagesAsReference; }
        set { m_totalNumberImagesAsReference = value; }
    }
    public int TotalNumberImagesAsFinished
    {
        get { return m_totalNumberImagesAsFinished; }
        set { m_totalNumberImagesAsFinished = value; }
    }
    public int TotalNumberImagesAsProviderExperience
    {
        get { return m_totalNumberImagesAsProviderExperience; }
        set { m_totalNumberImagesAsProviderExperience = value; }
    }
    public int SizeHeightAllowedImages
    {
        get { return m_sizeHeightAllowedImages; }
        set { m_sizeHeightAllowedImages = value; }
    }
    public string ProviderSkills
    {
        get { return m_providerSkills; }
        set { m_providerSkills = value; }
    }

    // -------------------------------------------
    /* 
	 * Initialitzation listener
	 */
    void Start()
    {
        if (DebugMode)
        {
            Debug.Log("YourVRUIScreenController::Start::First class to initialize for the whole system to work");
        }

        BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);

        LanguageController.Instance.LoadTextsXML();
        UsersController.Instance.Init();
        CommController.Instance.Init();
        RequestsController.Instance.Init();
        ProposalsController.Instance.Init();
        ImagesController.Instance.Init();
        SoundsController.Instance.Init();
        IAPController.Instance.Init();
        CreateNewScreenNoParameters(ScreenInitialView.SCREEN_INITIAL, TypePreviousActionEnum.DESTROY_ALL_SCREENS);

#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = true;
#endif
    }

    // -------------------------------------------
    /* 
	 * Destroy all references
	 */
    public void Destroy()
    {
        BasicEventController.Instance.BasicEvent -= OnBasicEvent;
        LanguageController.Instance.Destroy();
        UsersController.Instance.Destroy();
        CommController.Instance.Destroy();
        RequestsController.Instance.Destroy();
        ProposalsController.Instance.Destroy();
        ImagesController.Instance.Destroy();
        SoundsController.Instance.Destroy();
        IAPController.Instance.Destroy();
        DestroyObject(_instance);
        _instance = null;
    }

    // -------------------------------------------
    /* 
	 * Create a new screen
	 */
    public void CreateNewScreenNoParameters(string _nameScreen, TypePreviousActionEnum _previousAction)
    {
        CreateNewScreen(_nameScreen, _previousAction, true, null);
    }

    // -------------------------------------------
    /* 
	 * Create a new screen
	 */
    public void CreateNewScreenNoParameters(string _nameScreen, bool _hidePreviousScreens, TypePreviousActionEnum _previousAction)
    {
        CreateNewScreen(_nameScreen, _previousAction, _hidePreviousScreens, null);
    }

    // -------------------------------------------
    /* 
	 * Create a new screen
	 */
    public void CreateNewInformationScreen(string _nameScreen, TypePreviousActionEnum _previousAction, string _title, string _description, Sprite _image, string _eventData)
    {
        List<PageInformation> pages = new List<PageInformation>();
        pages.Add(new PageInformation(_title, _description, _image, _eventData));

        CreateNewScreen(_nameScreen, _previousAction, false, pages);
    }

    // -------------------------------------------
    /* 
	 * Create a new screen
	 */
    public void CreateNewScreen(string _nameScreen, TypePreviousActionEnum _previousAction, bool _hidePreviousScreens, params object[] _list)
    {
        if (!m_enableScreens) return;

        if (DebugMode)
        {
            Debug.Log("EVENT_SCREENMANAGER_OPEN_SCREEN::Creating the screen[" + _nameScreen + "]");
        }
        if (_hidePreviousScreens)
        {
            EnableAllScreens(false);
        }

        // PREVIOUS ACTION
        switch (_previousAction)
        {
            case TypePreviousActionEnum.HIDE_CURRENT_SCREEN:
                if (m_screensPool.Count > 0)
                {
                    m_screensPool[m_screensPool.Count - 1].SetActive(false);
                }                
                break;

            case TypePreviousActionEnum.KEEP_CURRENT_SCREEN:
                break;

            case TypePreviousActionEnum.DESTROY_CURRENT_SCREEN:
                if (m_screensPool.Count > 0)
                {
                    GameObject sCurrentScreen = m_screensPool[m_screensPool.Count - 1];
                    if (sCurrentScreen.GetComponent<IBasicScreenView>() != null)
                    {
                        sCurrentScreen.GetComponent<IBasicScreenView>().Destroy();
                    }
                    GameObject.Destroy(sCurrentScreen);
                    m_screensPool.RemoveAt(m_screensPool.Count - 1);
                }
                break;

            case TypePreviousActionEnum.DESTROY_ALL_SCREENS:
                DestroyScreensPool();
                DestroyScreensOverlay();
                break;
        }

        // CREATE SCREEN
        GameObject currentScreen = null;
        for (int i = 0; i < ScreensPrefabs.Length; i++)
        {
            if (ScreensPrefabs[i].name == _nameScreen)
            {
                currentScreen = (GameObject)Instantiate(ScreensPrefabs[i]);
                currentScreen.GetComponent<IBasicScreenView>().Initialize(_list);
                break;
            }
        }

        if (_hidePreviousScreens)
        {
            SoundsController.Instance.PlayFxSelection();
            m_screensPool.Add(currentScreen);
        }        
        else
        {
            if (_nameScreen != ScreenInformationView.SCREEN_WAIT) SoundsController.Instance.PlayFxSubSelection();
            m_screensOverlay.Add(currentScreen);
        }
    }


    // -------------------------------------------
    /* 
	 * LoadEmailLoginLocal
	 */
    public ItemMultiTextEntry LoadEmailLoginLocal()
    {
        try
        {
            string email = RJEncryptor.DecryptString(PlayerPrefs.GetString(USER_EMAIL_COOCKIE, ""), false);
            string password = RJEncryptor.DecryptString(PlayerPrefs.GetString(USER_PASSWORD_COOCKIE, ""), false);
            string nameUser = PlayerPrefs.GetString(USER_NAME_COOCKIE, "");

            if (email.Length > 0)
            {
                return new ItemMultiTextEntry(email, password, nameUser);
            }
            else
            {
                return null;
            }
        }
        catch (Exception err)
        {
            if (DebugMode)
            {
                Debug.Log(err.StackTrace);
            }
            return null;
        }
    }

    // -------------------------------------------
    /* 
     * Destroy all the screens in memory
     */
    public void DestroyScreensPool()
    {
        for (int i = 0; i < m_screensPool.Count; i++)
        {
            if (m_screensPool[i] != null)
            {
                if (m_screensPool[i].GetComponent<IBasicScreenView>() != null)
                {
                    m_screensPool[i].GetComponent<IBasicScreenView>().Destroy();
                }
                GameObject.Destroy(m_screensPool[i]);
                m_screensPool[i] = null;
            }
        }
        m_screensPool.Clear();
    }

    // -------------------------------------------
    /* 
     * Destroy all the screens in memory
     */
    public void DestroyScreensOverlay()
    {
        for (int i = 0; i < m_screensOverlay.Count; i++)
        {
            if (m_screensOverlay[i] != null)
            {
                if (m_screensOverlay[i].GetComponent<IBasicScreenView>() != null)
                {
                    m_screensOverlay[i].GetComponent<IBasicScreenView>().Destroy();
                }
                GameObject.Destroy(m_screensOverlay[i]);
                m_screensOverlay[i] = null;
            }
        }
        m_screensOverlay.Clear();
    }


    // -------------------------------------------
    /* 
     * Changes the enable of the screens
     */
    private void EnableScreens(bool _activation)
    {
        if (m_screensPool.Count > 0)
        {
            if (m_screensPool[m_screensPool.Count - 1] != null)
            {
                if (m_screensPool[m_screensPool.Count - 1].GetComponent<IBasicScreenView>() != null)
                {
                    m_screensPool[m_screensPool.Count - 1].GetComponent<IBasicScreenView>().SetActivation(_activation);
                }
            }
        }
    }

    // -------------------------------------------
    /* 
     * Changes the enable of the screens
     */
    private void EnableAllScreens(bool _activation)
    {
        for (int i = 0; i < m_screensPool.Count; i++)
        {
            if (m_screensPool[i] != null)
            {
                if (m_screensPool[i].GetComponent<IBasicScreenView>() != null)
                {
                    m_screensPool[i].GetComponent<IBasicScreenView>().SetActivation(_activation);
                }
            }
        }
    }

    // -------------------------------------------
    /* 
	 * Remove the screen from the list of screens
	 */
    private void DestroyGameObjectSingleScreen(GameObject _screen, bool _runDestroy)
    {
        if (_screen == null) return;

        for (int i = 0; i < m_screensPool.Count; i++)
        {
            GameObject screen = (GameObject)m_screensPool[i];
            if (_screen == screen)
            {
                if (_runDestroy)
                {
                    screen.GetComponent<IBasicScreenView>().Destroy();
                }
                m_screensPool.RemoveAt(i);
                return;
            }
        }
    }

    // -------------------------------------------
    /* 
     * Manager of global events
     */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (_nameEvent == EVENT_SCREENMANAGER_DESTROY_SCREEN)
        {
            m_enableScreens = true;
            GameObject screen = (GameObject)_list[0];
            DestroyGameObjectSingleScreen(screen, true);
            EnableScreens(true);
            SoundsController.Instance.PlayFxSelection();
        }
        if (_nameEvent == ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP)
        {
            DestroyScreensOverlay();
        }
    }

    // -------------------------------------------
    /* 
     * Update
     */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BasicEventController.Instance.DispatchBasicEvent(EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON);
        }
    }
}