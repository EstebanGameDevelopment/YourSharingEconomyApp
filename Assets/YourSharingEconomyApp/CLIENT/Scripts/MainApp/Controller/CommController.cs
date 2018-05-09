using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

/******************************************
 * 
 * CommController
 * 
 * It manages all the communications with the server
 * 
 * @author Esteban Gallardo
 */
public class CommController : StateManager
{
    // ----------------------------------------------
    // CONSTANTS
    // ----------------------------------------------	
    public const char TOKEN_SEPARATOR_COMA = ',';
    public const string TOKEN_SEPARATOR_EVENTS = "<par>";
    public const string TOKEN_SEPARATOR_LINES = "<line>";

    // ----------------------------------------------
    // COMM EVENTS
    // ----------------------------------------------	
    public const string EVENT_COMM_CONFIGURATION_PARAMETERS     = "EVENT_COMM_CONFIGURATION_PARAMETERS";
    public const string EVENT_COMM_REQUEST_USER_BY_FACEBOOK     = "EVENT_COMM_REQUEST_USER_BY_FACEBOOK";
    public const string EVENT_COMM_REQUEST_USER_BY_LOGIN        = "EVENT_COMM_REQUEST_USER_BY_LOGIN";
    public const string EVENT_COMM_REQUEST_USER_REGISTER        = "EVENT_COMM_REQUEST_USER_REGISTER";
    public const string EVENT_COMM_REQUEST_USER_CONSULT         = "EVENT_COMM_REQUEST_USER_CONSULT";
    public const string EVENT_COMM_REQUEST_UPDATE_PROFILE       = "EVENT_COMM_REQUEST_UPDATE_PROFILE";
    public const string EVENT_COMM_REQUEST_RESET_PASSWORD       = "EVENT_COMM_REQUEST_RESET_PASSWORD";
    public const string EVENT_COMM_REQUEST_RESET_BY_EMAIL_PASSWORD = "EVENT_COMM_REQUEST_RESET_BY_EMAIL_PASSWORD";
    public const string EVENT_COMM_CHECK_VALIDATION_USER        = "EVENT_COMM_CHECK_VALIDATION_USER";
    public const string EVENT_COMM_CREATE_REQUEST_NEW_JOB       = "EVENT_COMM_CREATE_REQUEST_NEW_JOB";
    public const string EVENT_COMM_UPDATE_REQUEST_IMG_REFERENCE = "EVENT_COMM_UPDATE_REQUEST_IMG_REFERENCE";
    public const string EVENT_COMM_CONSULT_REQUESTS_BY_USER     = "EVENT_COMM_CONSULT_REQUESTS_BY_USER";
    public const string EVENT_COMM_CONSULT_REQUESTS_BY_PROVIDER = "EVENT_COMM_CONSULT_REQUESTS_BY_PROVIDER";
    public const string EVENT_COMM_CONSULT_REQUESTS_BY_DISTANCE = "EVENT_COMM_CONSULT_REQUESTS_BY_DISTANCE";
    public const string EVENT_COMM_CONSULT_SINGLE_REQUEST       = "EVENT_COMM_CONSULT_SINGLE_REQUEST";
    public const string EVENT_COMM_CONSULT_DELETE_REQUEST       = "EVENT_COMM_CONSULT_DELETE_REQUEST";
    public const string EVENT_COMM_SET_REQUEST_AS_FINISHED      = "EVENT_COMM_SET_REQUEST_AS_FINISHED";
    public const string EVENT_COMM_UPDATE_REQUEST_SCORE_AND_FEEDBACK = "EVENT_COMM_UPDATE_REQUEST_SCORE_AND_FEEDBACK";
    public const string EVENT_COMM_UPLOAD_IMAGE                 = "EVENT_COMM_UPLOAD_IMAGE";
    public const string EVENT_COMM_REMOVE_IMAGE                 = "EVENT_COMM_REMOVE_IMAGE";
    public const string EVENT_COMM_LOAD_IMAGE                   = "EVENT_COMM_LOAD_IMAGE";
    public const string EVENT_COMM_CONSULT_ALL_IMAGE_OF_REQUEST = "EVENT_COMM_CONSULT_ALL_IMAGE_OF_REQUEST";    
    public const string EVENT_COMM_CREATE_NEW_PROPOSAL          = "EVENT_COMM_CREATE_NEW_PROPOSAL";
    public const string EVENT_COMM_CONSULT_ALL_PROPOSALS_OF_REQUEST = "EVENT_COMM_CONSULT_ALL_PROPOSALS_OF_REQUEST";
    public const string EVENT_COMM_RESET_ALL_PROPOSALS_OF_REQUEST = "EVENT_COMM_RESET_ALL_PROPOSALS_OF_REQUEST";
    public const string EVENT_COMM_REACTIVATE_PROPOSAL          = "EVENT_COMM_REACTIVATE_PROPOSAL";
    public const string EVENT_COMM_REPORT_PROPOSAL              = "EVENT_COMM_REPORT_PROPOSAL";
    public const string EVENT_COMM_UPDATE_PROPOSAL              = "EVENT_COMM_UPDATE_PROPOSAL";
    public const string EVENT_COMM_REMOVE_PROPOSAL              = "EVENT_COMM_REMOVE_PROPOSAL";
    public const string EVENT_COMM_IAP_CODE_REGISTER            = "EVENT_COMM_IAP_CODE_REGISTER";
    public const string EVENT_COMM_IAP_RENT_TIME_PROVIDER       = "EVENT_COMM_IAP_RENT_TIME_PROVIDER";
    public const string EVENT_COMM_IAP_PREMIUM_OFFER            = "EVENT_COMM_IAP_PREMIUM_OFFER";
    public const string EVENT_COMM_IAP_PREMIUM_REQUEST          = "EVENT_COMM_IAP_PREMIUM_REQUEST";

    public const int STATE_IDLE             = 0;
    public const int STATE_COMMUNICATION    = 1;

    // ----------------------------------------------
    // SINGLETON
    // ----------------------------------------------	

    private static CommController _instance;

    public static CommController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType(typeof(CommController)) as CommController;
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "CommController";
                    _instance = container.AddComponent(typeof(CommController)) as CommController;
                }
            }
            return _instance;
        }
    }
	
	// ----------------------------------------------
	// MEMBERS
	// ----------------------------------------------
    private string m_event;
    private IHTTPComms m_commRequest;
    private List<TimedEventData> m_listTimedEvents = new List<TimedEventData>();
    private List<TimedEventData> m_listQueuedEvents = new List<TimedEventData>();
    private List<TimedEventData> m_priorityQueuedEvents = new List<TimedEventData>();

    private string m_inGameLog = "";

    public bool ReloadXML = false;

    // -------------------------------------------
    /* 
	 * Will delete from the text introduced by the user any special token that can break the comunication
	 */
    public static string FilterSpecialTokens(string _text)
    {
        string output = _text;

        string[] arrayEvents = output.Split(new string[] { TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
        output = "";
        for (int i = 0; i < arrayEvents.Length; i++)
        {
            output += arrayEvents[i];
            if (i + 1 < arrayEvents.Length)
            {
                output += " ";
            }
        }


        string[] arrayLines = output.Split(new string[] { TOKEN_SEPARATOR_LINES }, StringSplitOptions.None);
        output = "";
        for (int i = 0; i < arrayLines.Length; i++)
        {
            output += arrayLines[i];
            if (i + 1 < arrayLines.Length)
            {
                output += " ";
            }
        }

        return output;
    }

    // ----------------------------------------------
    // CONSTRUCTOR
    // ----------------------------------------------	
    // -------------------------------------------
    /* 
	 * Constructor
	 */
    private CommController() 
	{
        ChangeState(STATE_IDLE);
	}

    // -------------------------------------------
    /* 
     * Start
     */
    void Start()
    {
        ChangeState(STATE_IDLE);
    }
    
    // -------------------------------------------
    /* 
     * Init
     */
    public void Init()
    {
        ChangeState(STATE_IDLE);
    }


    // -------------------------------------------
    /* 
     * GetServerConfigurationParameters
     */
    public void GetServerConfigurationParameters()
    {
        Request(EVENT_COMM_CONFIGURATION_PARAMETERS);
    }

    // -------------------------------------------
    /* 
     * RequestUserByLogin
     */
    public void RequestUserByLogin(string _email, string _password)
    {
        Request(EVENT_COMM_REQUEST_USER_BY_LOGIN, _email, _password);
    }

    // -------------------------------------------
    /* 
     * RequestUserRegister
     */
    public void RequestUserRegister(string _email, string _password)
    {
        Request(EVENT_COMM_REQUEST_USER_REGISTER, _email, _password);
    }

    // -------------------------------------------
    /* 
     * RequestUserRegister
     */
    public void RequestConsultUser(long _idOwnUser, string _password, long _idUserSearch)
    {
        Request(EVENT_COMM_REQUEST_USER_CONSULT, _idOwnUser.ToString(), _password, _idUserSearch.ToString());
    }    

    // -------------------------------------------
    /* 
     * RequestUserByFacebook
     */
    public void RequestUserByFacebook(string _idFacebook, string _nameFacebook, string _emailFacebook, string _friendsPackage)
    {
        Request(EVENT_COMM_REQUEST_USER_BY_FACEBOOK, _idFacebook, _nameFacebook, _emailFacebook, _friendsPackage);
    }

    // -------------------------------------------
    /* 
     * RequestUpdateProfile
     */
    public void RequestUpdateProfile(string _idUser, string _currentPassword, string _password, string _email, string _name, string _village, string _mapdata, string _skills, string _description)
    {
        Request(EVENT_COMM_REQUEST_UPDATE_PROFILE, _idUser, _currentPassword, _password, _email, _name, _village, _mapdata, _skills, _description);
    }

    // -------------------------------------------
    /* 
     * RequestUpdateProfile
     */
    public void RequestResetPassword(string _idUser)
    {
        Request(EVENT_COMM_REQUEST_RESET_PASSWORD, _idUser);
    }

    // -------------------------------------------
    /* 
     * RequestUpdateProfile
     */
    public void RequestResetPasswordByEmail(string _email)
    {
        Request(EVENT_COMM_REQUEST_RESET_BY_EMAIL_PASSWORD, _email);
    }

    // -------------------------------------------
    /* 
     * CheckValidationUser
     */
    public void CheckValidationUser(string _idUser)
    {
        Request(EVENT_COMM_CHECK_VALIDATION_USER, _idUser);
    }

    // -------------------------------------------
    /* 
     * CheckValidationUser
     */
    public void CreateNewRequestDress(string _idUser, string _password, RequestModel _requestDress)
    {
        Request(EVENT_COMM_CREATE_REQUEST_NEW_JOB, _idUser, _password,
                                        _requestDress.Id.ToString(),
                                        _requestDress.Customer.ToString(),
                                        _requestDress.Provider.ToString(),
                                        _requestDress.Title,
                                        _requestDress.Description,
                                        _requestDress.Images.ToString(),
                                        _requestDress.Referenceimg.ToString(),
                                        _requestDress.Village,
                                        _requestDress.Mapdata,
                                        _requestDress.Latitude.ToString(),
                                        _requestDress.Longitude.ToString(),
                                        _requestDress.Price.ToString(),
                                        _requestDress.Currency,
                                        _requestDress.Distance.ToString(),
                                        _requestDress.GetFlags(),
                                        _requestDress.Notifications.ToString(),
                                        _requestDress.Creationdate.ToString(),
                                        _requestDress.Deadline.ToString(),
                                        _requestDress.FeedbackCustomerGivesToTheProvider,
                                        _requestDress.ScoreCustomerGivesToTheProvider.ToString(),
                                        _requestDress.FeedbackProviderGivesToTheCustomer,
                                        _requestDress.ScoreProviderGivesToTheCustomer.ToString()
            );
    }

    // -------------------------------------------
    /* 
     * CreateNewRequestDress
     */
    public void UpdateImageReferenceRequest(int _idUser, string _password, long _idRequest, long _imageReference)
    {
        Request(EVENT_COMM_UPDATE_REQUEST_IMG_REFERENCE, _idUser.ToString(), _password, _idRequest.ToString(), _imageReference.ToString());
    }

    // -------------------------------------------
    /* 
     * RequestConsultRecordsByUser
     */
    public void RequestConsultRecordsByUser(int _id, string _password, int _idUser)
    {
        RequestPriority(EVENT_COMM_CONSULT_REQUESTS_BY_USER, _id.ToString(), _password, _idUser.ToString());
    }

    // -------------------------------------------
    /* 
     * RequestConsultRecordsByProvider
     */
    public void RequestConsultRecordsByProvider(int _id, string _password, int _idProvider, bool _allRecords)
    {
        RequestPriority(EVENT_COMM_CONSULT_REQUESTS_BY_PROVIDER, _id.ToString(), _password, _idProvider.ToString(), (_allRecords?"1":"0"));
    }

    // -------------------------------------------
    /* 
     * RequestConsultRecordsByUser
     */
    public void RequestConsultRecordsByDistance(int _id, string _password, SearchModel _searchRequest, int _stateWorks)
    {
        RequestPriority(EVENT_COMM_CONSULT_REQUESTS_BY_DISTANCE, _id.ToString(), _password, _searchRequest.Latitude.ToString(), _searchRequest.Longitude.ToString(), _searchRequest.Distance.ToString(), _stateWorks.ToString());
    }

    // -------------------------------------------
    /* 
     * RequestConsultRecordsByUser
     */
    public void RequestConsultSingleRequest(int _id, string _password, long _request)
    {
        RequestPriority(EVENT_COMM_CONSULT_SINGLE_REQUEST, _id.ToString(), _password, _request.ToString());
    }

    // -------------------------------------------
    /* 
     * DeleteRequest
     */
    public void DeleteRequest(int _id, string _password, long _requestID)
    {
        RequestPriority(EVENT_COMM_CONSULT_DELETE_REQUEST, _id.ToString(), _password, _requestID.ToString());
    }

    // -------------------------------------------
    /* 
     * SetRequestAsFinished
     */
    public void SetRequestAsFinished(int _id, string _password, long _requestID, bool _broken)
    {
        RequestPriority(EVENT_COMM_SET_REQUEST_AS_FINISHED, _id.ToString(), _password, _requestID.ToString(), (_broken?"1":"0"));
    }

    // -------------------------------------------
    /* 
     * UpdateRequestScoreAndFeedback
     */
    public void UpdateRequestScoreAndFeedback(int _id, string _password, long _requestID, int _scoreConsumer, string _feedbackConsumer, int _scoreProvider, string _feedbackProvider)
    {
        RequestPriority(EVENT_COMM_UPDATE_REQUEST_SCORE_AND_FEEDBACK, _id.ToString(), _password, _requestID.ToString(), _scoreConsumer.ToString(), _feedbackConsumer, _scoreProvider.ToString(), _feedbackProvider);
    }

    // -------------------------------------------
    /* 
     * UploadImage
     */
    public void UploadImage(long _idImage, string _table, long _idOriginTable, int _type, byte[] _data, string _url)
    {
        Request(EVENT_COMM_UPLOAD_IMAGE, _idImage.ToString(), _table, _idOriginTable.ToString(), _type.ToString(), _url, _data);
    }

    // -------------------------------------------
    /* 
     * RemoveImage
     */
    public void RemoveImage(long _idImage)
    {
        Request(EVENT_COMM_REMOVE_IMAGE, _idImage.ToString());
    }

    // -------------------------------------------
    /* 
     * ConsultImagesRequest
     */
    public void ConsultImagesRequest(int _id, string _password, long _idOrigin, string _table)
    {
        Request(EVENT_COMM_CONSULT_ALL_IMAGE_OF_REQUEST, _id.ToString(), _password, _idOrigin.ToString(), _table);
    }
    

    // -------------------------------------------
    /* 
     * CreateNewProposal
     */
    public void CreateNewProposal(string _idUser, string _password, ProposalModel _proposal)
    {
        Request(EVENT_COMM_CREATE_NEW_PROPOSAL, _idUser, _password,
                                        _proposal.Id.ToString(),
                                        _proposal.User.ToString(),
                                        _proposal.Request.ToString(),
                                        _proposal.Type.ToString(),
                                        _proposal.Title,
                                        _proposal.Description,
                                        _proposal.Price.ToString(),
                                        _proposal.Deadline.ToString(),
                                        _proposal.Accepted.ToString());
    }

    // -------------------------------------------
    /* 
     * UpdateProposal
     */
    public void UpdateProposal(string _idUser, string _password, ProposalModel _proposal)
    {
        Request(EVENT_COMM_UPDATE_PROPOSAL, _idUser, _password,
                                        _proposal.Id.ToString(),
                                        _proposal.Request.ToString(),
                                        _proposal.Price.ToString(),
                                        _proposal.Deadline.ToString(),
                                        _proposal.User.ToString(),
                                        _proposal.Accepted.ToString());
    }

    // -------------------------------------------
    /* 
     * RemoveProposal
     */
    public void RemoveProposal(string _idUser, string _password, long _idProposal)
    {
        Request(EVENT_COMM_REMOVE_PROPOSAL, _idUser, _password, _idProposal.ToString());
    }


    // -------------------------------------------
    /* 
     * ConsultAllProposalsByRequest
     */
    public void ConsultAllProposalsByRequest(int _id, string _password, long _idRequest)
    {
        Request(EVENT_COMM_CONSULT_ALL_PROPOSALS_OF_REQUEST, _id.ToString(), _password, _idRequest.ToString());
    }

    // -------------------------------------------
    /* 
     * ResetProposalsForRequest
     */
    public void ResetProposalsForRequest(int _id, string _password, long _idRequest)
    {
        Request(EVENT_COMM_RESET_ALL_PROPOSALS_OF_REQUEST, _id.ToString(), _password, _idRequest.ToString());
    }

    // -------------------------------------------
    /* 
     * ReactivateProposal
     */
    public void ReactivateProposal(int _id, string _password, long _idProposal)
    {
        Request(EVENT_COMM_REACTIVATE_PROPOSAL, _id.ToString(), _password, _idProposal.ToString());
    }

    // -------------------------------------------
    /* 
     * ReportProposal
     */
    public void ReportProposal(int _id, string _password, long _idProposal, int _idReporter, long _idRequest)
    {
        Request(EVENT_COMM_REPORT_PROPOSAL, _id.ToString(), _password, _idProposal.ToString(), _idReporter.ToString(), _idRequest.ToString());
    }

    // -------------------------------------------
    /* 
     * LoadImage
     */
    public void LoadImage(long _idImage)
    {
        Request(EVENT_COMM_LOAD_IMAGE, _idImage.ToString());
    }

    // -------------------------------------------
    /* 
     * RentTimeAsAProvider
     */
    public void IAPRegisterCode(int _id, string _password, string _codeRegister)
    {
        Request(EVENT_COMM_IAP_CODE_REGISTER, _id.ToString(), _password, _codeRegister);
    }

    // -------------------------------------------
    /* 
     * RentTimeAsAProvider
     */
    public void IAPRentTimeAsAProvider(int _id, string _password, int _rentValue, string _codeValidation)
    {
        Request(EVENT_COMM_IAP_RENT_TIME_PROVIDER, _id.ToString(), _password, _rentValue.ToString(), _codeValidation);
    }

    // -------------------------------------------
    /* 
     * RentTimeAsAProvider
     */
    public void IAPPurchasePremiumOffer(int _id, string _password, string _codeValidation)
    {
        Request(EVENT_COMM_IAP_PREMIUM_OFFER, _id.ToString(), _password, _codeValidation);
    }

    // -------------------------------------------
    /* 
     * IAPPurchasePremiumRequest
     */
    public void IAPPurchasePremiumRequest(int _id, string _password, string _codeValidation)
    {
        Request(EVENT_COMM_IAP_PREMIUM_REQUEST, _id.ToString(), _password, _codeValidation);
    }

    // -------------------------------------------
    /* 
     * Destroy
     */
    public void Destroy()
    {
        DestroyObject(_instance.gameObject);
        _instance = null;
    }

    // -------------------------------------------
    /* 
     * Request
     */
    public void Request(string _event, params object[] _list)
    {
        if (m_state != STATE_IDLE)
        {
            QueuedRequest(_event, _list);
            return;
        }
        
        RequestReal(_event, _list);
    }

    // -------------------------------------------
    /* 
     * RequestPriority
     */
    public void RequestPriority(string _event, params object[] _list)
    {
        if (m_state != STATE_IDLE)
        {
            InsertRequest(_event, _list);
            return;
        }

        RequestReal(_event, _list);
    }

    // -------------------------------------------
    /* 
     * RequestNoQueue
     */
    public void RequestNoQueue(string _event, params object[] _list)
    {
        if (m_state != STATE_IDLE)
        {
            return;
        }

        RequestReal(_event, _list);
    }

    // -------------------------------------------
    /* 
     * RequestReal
     */
    private void RequestReal(string _event, params object[] _list)
    {
        m_event = _event;
        bool isBinaryResponse = true;

        switch (m_event)
        {
            case EVENT_COMM_CONFIGURATION_PARAMETERS:
                m_commRequest = new GetConfigurationServerParametersHTTP();
                break;

            case EVENT_COMM_REQUEST_USER_BY_LOGIN:
                isBinaryResponse = false;
                m_commRequest = new UserLoginByEmailHTTP();
                break;

            case EVENT_COMM_REQUEST_USER_REGISTER:
                isBinaryResponse = false;
                m_commRequest = new UserRegisterWithEmailHTTP();
                break;

            case EVENT_COMM_REQUEST_USER_CONSULT:
                isBinaryResponse = false;
                m_commRequest = new UserConsultByIdHTTP();
                break;

            case EVENT_COMM_REQUEST_USER_BY_FACEBOOK:
                isBinaryResponse = false;
                m_commRequest = new UserLoginByFacebookHTTP();
                break;

            case EVENT_COMM_REQUEST_UPDATE_PROFILE:
                m_commRequest = new UserUpdateProfileHTTP();
                break;

            case EVENT_COMM_REQUEST_RESET_PASSWORD:
                m_commRequest = new UserRequestResetPasswordHTTP();
                break;

            case EVENT_COMM_REQUEST_RESET_BY_EMAIL_PASSWORD:
                m_commRequest = new UserRequestResetByEmailPasswordHTTP();
                break;

            case EVENT_COMM_CHECK_VALIDATION_USER:
                m_commRequest = new UserCheckValidationUserHTTP();
                break;

            case EVENT_COMM_CREATE_REQUEST_NEW_JOB:
                m_commRequest = new RequestCreateNewJobHTTP();
                break;

            case EVENT_COMM_UPDATE_REQUEST_IMG_REFERENCE:
                m_commRequest = new RequestUpdateImageRefHTTP();
                break;                

            case EVENT_COMM_CONSULT_REQUESTS_BY_USER:
                isBinaryResponse = false;
                m_commRequest = new RequestConsultByUserHTTP();
                break;

            case EVENT_COMM_CONSULT_REQUESTS_BY_PROVIDER:
                isBinaryResponse = false;
                m_commRequest = new RequestConsultByProviderHTTP();
                break;

            case EVENT_COMM_CONSULT_REQUESTS_BY_DISTANCE:
                isBinaryResponse = false;
                m_commRequest = new RequestConsultByDistanceHTTP();
                break;

            case EVENT_COMM_CONSULT_SINGLE_REQUEST:
                isBinaryResponse = false;
                m_commRequest = new RequestSingleConsultHTTP();
                break;

            case EVENT_COMM_CONSULT_DELETE_REQUEST:
                m_commRequest = new RequestDeleteRecordHTTP();
                break;

            case EVENT_COMM_SET_REQUEST_AS_FINISHED:
                m_commRequest = new RequestSetAsFinishedHTTP();
                break;

            case EVENT_COMM_UPDATE_REQUEST_SCORE_AND_FEEDBACK:
                m_commRequest = new RequestUpdateScoreAndFeedbackHTTP();
                break;

            case EVENT_COMM_UPLOAD_IMAGE:
                m_commRequest = new ImageUploadHTTP();
                break;

            case EVENT_COMM_REMOVE_IMAGE:
                m_commRequest = new ImageRemoveHTTP();
                break;

            case EVENT_COMM_LOAD_IMAGE:
                m_commRequest = new ImageLoadHTTP();
                break;

            case EVENT_COMM_CONSULT_ALL_IMAGE_OF_REQUEST:
                m_commRequest = new ImageConsultAllHTTP();
                break;

            case EVENT_COMM_CREATE_NEW_PROPOSAL:
                m_commRequest = new ProposalCreateNewHTTP();
                break;

            case EVENT_COMM_CONSULT_ALL_PROPOSALS_OF_REQUEST:
                isBinaryResponse = false;
                m_commRequest = new ProposalConsultAllHTTP();
                break;

            case EVENT_COMM_RESET_ALL_PROPOSALS_OF_REQUEST:
                m_commRequest = new ProposalResetAllHTTP();
                break;

            case EVENT_COMM_REACTIVATE_PROPOSAL:
                m_commRequest = new ProposalReactivateAllHTTP();
                break;

            case EVENT_COMM_REPORT_PROPOSAL:
                m_commRequest = new ProposalReportToxicHTTP();
                break;

            case EVENT_COMM_UPDATE_PROPOSAL:
                m_commRequest = new ProposalUpdateHTTP();
                break;

            case EVENT_COMM_REMOVE_PROPOSAL:
                m_commRequest = new ProposalRemoveHTTP();
                break;

            case EVENT_COMM_IAP_CODE_REGISTER:
                m_commRequest = new IAPCreateValidationCodeHTTP();
                break;

            case EVENT_COMM_IAP_RENT_TIME_PROVIDER:
                m_commRequest = new IAPRentTimeProviderHTTP();
                break;

            case EVENT_COMM_IAP_PREMIUM_OFFER:
                m_commRequest = new IAPPremiumPostOfferHTTP();
                break;

            case EVENT_COMM_IAP_PREMIUM_REQUEST:
                m_commRequest = new IAPPremiumRequestHTTP();
                break;
        }

        ChangeState(STATE_COMMUNICATION);
        string data = m_commRequest.Build(_list);
        if (ScreenController.Instance.DebugComms)
        {
            Debug.Log("CommController::RequestReal:URL=" + m_commRequest.UrlRequest);
            Debug.Log("CommController::RequestReal:data=" + data);
        }
        if (m_commRequest.Method == BaseDataHTTP.METHOD_GET)
        {
            WWW www = new WWW(m_commRequest.UrlRequest + data);
            if (isBinaryResponse)
            {
                StartCoroutine(WaitForRequest(www));
            }
            else
            {
                StartCoroutine(WaitForStringRequest(www));
            }
        }
        else
        {
            WWW www = new WWW(m_commRequest.UrlRequest, m_commRequest.FormPost.data, m_commRequest.FormPost.headers);
            if (isBinaryResponse)
            {
                StartCoroutine(WaitForRequest(www));
            }
            else
            {
                StartCoroutine(WaitForStringRequest(www));
            }
        }
    }

    // -------------------------------------------
    /* 
     * DelayRequest
     */
    public void DelayRequest(string _nameEvent, float _time, params object[] _list)
    {
        m_listTimedEvents.Add(new TimedEventData(_nameEvent, _time, true, _list));
    }

    // -------------------------------------------
    /* 
     * QueuedRequest
     */
    public void QueuedRequest(string _nameEvent, params object[] _list)
    {
        m_listQueuedEvents.Add(new TimedEventData(_nameEvent, 0, _list));
    }

    // -------------------------------------------
    /* 
     * InsertRequest
     */
    public void InsertRequest(string _nameEvent, params object[] _list)
    {
        m_priorityQueuedEvents.Insert(0, new TimedEventData(_nameEvent, 0, _list));
    }    

    // -------------------------------------------
    /* 
    * WaitForRequest
    */
    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            if (ScreenController.Instance.DebugComms) Debug.Log("WWW Ok!: " + www.text);
            m_commRequest.Response(www.bytes);
        }
        else
        {
            if (ScreenController.Instance.DebugComms) Debug.LogError("WWW Error: " + www.error);
            m_commRequest.Response(Encoding.ASCII.GetBytes(www.error));
            
        }

        ChangeState(STATE_IDLE);
        ProcesQueuedComms();
    }

    // -------------------------------------------
    /* 
    * WaitForRequest
    */
    IEnumerator WaitForStringRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            if (ScreenController.Instance.DebugComms) Debug.Log("WWW Ok!: " + www.text);
            m_commRequest.Response(www.text);
        }
        else
        {
            if (ScreenController.Instance.DebugComms) Debug.LogError("WWW Error: " + www.error);
            m_commRequest.Response(Encoding.ASCII.GetBytes(www.error));

        }

        ChangeState(STATE_IDLE);
        ProcesQueuedComms();
    }

    // -------------------------------------------
    /* 
     * DisplayLog
     */
    public void DisplayLog(string _data)
    {
        m_inGameLog = _data + "\n";
        if (ScreenController.Instance.DebugComms)
        {
            Debug.Log("CommController::DisplayLog::DATA=" + _data);
        }        
    }

    // -------------------------------------------
    /* 
     * ClearLog
     */
    public void ClearLog()
    {
        m_inGameLog = "";
    }

    private bool m_enableLog = true;

    // -------------------------------------------
    /* 
     * OnGUI
     */
    void OnGUI()
    {
        if (ScreenController.Instance.DebugComms)
        {
            if (!m_enableLog)
            {
                if (m_inGameLog.Length > 0)
                {
                    ClearLog();
                }
            }

            if (m_enableLog)
            {
                if (m_inGameLog.Length > 0)
                {
                    GUILayout.BeginScrollView(Vector2.zero);
                    if (GUILayout.Button(m_inGameLog))
                    {
                        ClearLog();
                    }
                    GUILayout.EndScrollView();
                }
                else
                {
                    switch (m_state)
                    {
                        case STATE_IDLE:
                            break;

                        case STATE_COMMUNICATION:
                            GUILayout.BeginScrollView(Vector2.zero);
                            GUILayout.Label("COMMUNICATION::Event=" + m_event);
                            GUILayout.EndScrollView();
                            break;
                    }
                }
            }
        }
    }

    // -------------------------------------------
    /* 
     * ProcessTimedEvents
     */
    private void ProcessTimedEvents()
    {
        switch (m_state)
        {
            case STATE_IDLE:
                for (int i = 0; i < m_listTimedEvents.Count; i++)
                {
                    TimedEventData eventData = m_listTimedEvents[i];
                    eventData.Time -= Time.deltaTime;
                    if (eventData.Time <= 0)
                    {
                        m_listTimedEvents.RemoveAt(i);
                        Request(eventData.NameEvent, eventData.List);
                        eventData.Destroy();
                        break;
                    }
                }
                break;
        }
    }

    // -------------------------------------------
    /* 
     * ProcesQueuedComms
     */
    private void ProcesQueuedComms()
    {
        // PRIORITY QUEUE
        if (m_priorityQueuedEvents.Count > 0)
        {
            int i = 0;
            TimedEventData eventData = m_priorityQueuedEvents[i];
            m_priorityQueuedEvents.RemoveAt(i);
            Request(eventData.NameEvent, eventData.List);
            eventData.Destroy();
            return;
        }
        // NORMAL QUEUE
        if (m_listQueuedEvents.Count > 0)
        {
            int i = 0;
            TimedEventData eventData = m_listQueuedEvents[i];
            m_listQueuedEvents.RemoveAt(i);
            Request(eventData.NameEvent, eventData.List);
            eventData.Destroy();
            return;
        }
    }

    // -------------------------------------------
    /* 
     * ProcessQueueEvents
     */
    private void ProcessQueueEvents()
    {
        switch (m_state)
        {
            case STATE_IDLE:
                break;

            case STATE_COMMUNICATION:
                break;
        }
    }

    // -------------------------------------------
    /* 
     * Update
     */
    public void Update()
    {
        Logic();

        ProcessTimedEvents();
        ProcessQueueEvents();
    }
}

