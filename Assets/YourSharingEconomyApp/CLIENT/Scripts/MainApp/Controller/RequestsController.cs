using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * RequestsController
	 * 
	 * It manages all the operations related with the requests' management
	 * 
	 * @author Esteban Gallardo
	 */
	public class RequestsController : MonoBehaviour
	{
		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------
		public const string EVENT_REQUEST_CALL_CREATE_OR_UPDATE_REQUEST = "EVENT_REQUEST_CALL_CREATE_OR_UPDATE_REQUEST";
		public const string EVENT_REQUEST_RESULT_CREATED_RECORD_CONFIRMATION = "EVENT_REQUEST_RESULT_CREATED_RECORD_CONFIRMATION";

		public const string EVENT_REQUEST_CALL_CONSULT_RECORDS_BY_USER = "EVENT_REQUEST_CALL_CONSULT_RECORDS_BY_USER";
		public const string EVENT_REQUEST_CALL_CONSULT_BY_DISTANCE_RECORDS = "EVENT_REQUEST_CALL_CONSULT_BY_DISTANCE_RECORDS";
		public const string EVENT_REQUEST_CALL_CONSULT_BY_PROVIDER = "EVENT_REQUEST_CALL_CONSULT_BY_PROVIDER";
		public const string EVENT_REQUEST_RESULT_CONSULT_RECORDS = "EVENT_REQUEST_RESULT_CONSULT_RECORDS";
		public const string EVENT_REQUEST_RESULT_CONSULT_DISTANCE_RECORDS = "EVENT_REQUEST_RESULT_CONSULT_DISTANCE_RECORDS";
		public const string EVENT_REQUEST_RESULT_CONSULT_BY_PROVIDER_RECORDS = "EVENT_REQUEST_RESULT_CONSULT_BY_PROVIDER_RECORDS";
		public const string EVENT_REQUEST_RESULT_FORMATTED_RECORDS = "EVENT_REQUEST_RESULT_FORMATTED_RECORDS";

		public const string EVENT_REQUEST_CALL_CONSULT_SINGLE_RECORD = "EVENT_REQUEST_CALL_CONSULT_SINGLE_RECORD";
		public const string EVENT_REQUEST_RESULT_CONSULT_SINGLE_RECORD = "EVENT_REQUEST_RESULT_CONSULT_SINGLE_RECORD";
		public const string EVENT_REQUEST_RESULT_FORMATTED_SINGLE_RECORD = "EVENT_REQUEST_RESULT_FORMATTED_SINGLE_RECORD";

		public const string EVENT_REQUEST_CALL_DELETE_RECORDS = "EVENT_REQUEST_CALL_DELETE_RECORDS";
		public const string EVENT_REQUEST_RESULT_DELETE_RECORDS = "EVENT_REQUEST_RESULT_DELETE_RECORDS";

		public const string EVENT_REQUEST_CALL_UPDATE_IMG_REF = "EVENT_REQUEST_CALL_UPDATE_IMG_REF";
		public const string EVENT_REQUEST_RESULT_UPDATE_IMG_REF = "EVENT_REQUEST_RESULT_UPDATE_IMG_REF";

		public const string EVENT_REQUEST_CALL_CONSULT_IMAGES_REQUEST = "EVENT_REQUEST_CALL_CONSULT_IMAGES_REQUEST";
		public const string EVENT_REQUEST_RESULT_CONSULT_IMAGES_REQUEST = "EVENT_REQUEST_RESULT_CONSULT_IMAGES_REQUEST";
		public const string EVENT_REQUEST_RESULT_FORMATTED_IMAGES_REQUEST = "EVENT_REQUEST_RESULT_FORMATTED_IMAGES_REQUEST";

		public const string EVENT_REQUEST_CALL_SET_JOB_AS_FINISHED = "EVENT_REQUEST_CALL_SET_JOB_AS_FINISHED";
		public const string EVENT_REQUEST_RESULT_SET_JOB_AS_FINISHED = "EVENT_REQUEST_RESULT_SET_JOB_AS_FINISHED";

		public const string EVENT_REQUEST_CALL_SCORE_AND_FEEDBACK_UPDATE = "EVENT_REQUEST_CALL_SCORE_AND_FEEDBACK_UPDATE";
		public const string EVENT_REQUEST_RESULT_SCORE_AND_FEEDBACK_UPDATE = "EVENT_REQUEST_RESULT_SCORE_AND_FEEDBACK_UPDATE";

		public const string EVENT_REQUEST_TRANSACTION_REGISTERED_RESPONSE = "EVENT_REQUEST_TRANSACTION_REGISTERED_RESPONSE";

		// ----------------------------------------------
		// CONSTANTS
		// ----------------------------------------------
		public const int TYPE_CONSULT_BY_USER = 1;
		public const int TYPE_CONSULT_BY_DISTANCE = 2;
		public const int TYPE_CONSULT_BY_PROVIDER = 3;

		// ----------------------------------------------
		// SINGLETON
		// ----------------------------------------------
		private static RequestsController _instance;

		public static RequestsController Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType(typeof(RequestsController)) as RequestsController;
					if (!_instance)
					{
						GameObject container = new GameObject();
						container.name = "RequestsController";
						_instance = container.AddComponent(typeof(RequestsController)) as RequestsController;
					}
				}
				return _instance;
			}
		}

		// ----------------------------------------------
		// MEMBERS
		// ----------------------------------------------
		private List<RequestModel> m_userRequests = new List<RequestModel>();
		private List<RequestModel> m_currentResultRequests = new List<RequestModel>();
		private List<RequestModel> m_providerRequests = new List<RequestModel>();
		private RequestModel m_lastRequestConsulted;
		private bool m_mustReloadRequests = true;
		private bool m_mustReloadProviderRequests = true;

		private List<RequestModel> m_otherUserRequests = new List<RequestModel>();
		private int m_otherUserIDLastConsult = -1;

		private List<RequestModel> m_otherProviderRequests = new List<RequestModel>();
		private int m_otherProviderIDLastConsult = -1;

		// ----------------------------------------------
		// GETTERS/SETTERS
		// ----------------------------------------------
		public bool MustReloadRequests
		{
			get { return m_mustReloadRequests; }
			set
			{
				m_mustReloadRequests = value;
				m_mustReloadProviderRequests = value;
			}
		}
		public RequestModel LastRequestConsulted
		{
			get { return m_lastRequestConsulted; }
		}

		// ----------------------------------------------
		// CONSTRUCTOR
		// ----------------------------------------------	
		// -------------------------------------------
		/* 
		 * Constructor
		 */
		private RequestsController()
		{
		}

		// -------------------------------------------
		/* 
		 * Initialitzation
		 */
		public void Init()
		{
			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			UIEventController.Instance.UIEvent -= OnBasicEvent;
			DestroyObject(_instance.gameObject);
			_instance = null;
		}

		// -------------------------------------------
		/* 
		 * ClearUserRequests
		 */
		public void ClearUserRequests()
		{
			m_userRequests.Clear();
		}

		// -------------------------------------------
		/* 
		 * ClearProviderRequests
		 */
		public void ClearProviderRequests()
		{
			m_providerRequests.Clear();
		}

		// -------------------------------------------
		/* 
		 * DirtyExistence()
		 */
		private void DirtyExistence(int _requestType)
		{
			switch (_requestType)
			{
				case TYPE_CONSULT_BY_USER:
					for (int i = 0; i < m_userRequests.Count; i++)
					{
						m_userRequests[i].CheckExistence = false;
					}
					break;

				case TYPE_CONSULT_BY_PROVIDER:
					for (int i = 0; i < m_providerRequests.Count; i++)
					{
						m_providerRequests[i].CheckExistence = false;
					}
					break;

				default:
					break;
			}
		}

		// -------------------------------------------
		/* 
		 * CleanNotExistence()
		 */
		private void CleanNotExistence(int _requestType)
		{
			switch (_requestType)
			{
				case TYPE_CONSULT_BY_USER:
					for (int i = 0; i < m_userRequests.Count; i++)
					{
						if (!m_userRequests[i].CheckExistence)
						{
							m_userRequests.RemoveAt(i);
							i--;
						}
					}
					break;

				case TYPE_CONSULT_BY_PROVIDER:
					for (int i = 0; i < m_providerRequests.Count; i++)
					{
						if (!m_providerRequests[i].CheckExistence)
						{
							m_providerRequests.RemoveAt(i);
							i--;
						}
					}
					break;
			}
		}

		// -------------------------------------------
		/* 
		 * RemoveLocalRequests
		 */
		private void RemoveLocalRequests(long _idRequest)
		{
			for (int i = 0; i < m_userRequests.Count; i++)
			{
				if (m_userRequests[i].Id == _idRequest)
				{
					m_userRequests.RemoveAt(i);
					return;
				}
			}

			for (int i = 0; i < m_providerRequests.Count; i++)
			{
				if (m_providerRequests[i].Id == _idRequest)
				{
					m_providerRequests.RemoveAt(i);
				}
			}
		}

		// -------------------------------------------
		/* 
		 * GetLocalRequest
		 */
		public RequestModel GetLocalRequest(long _idRequest)
		{
			for (int i = 0; i < m_userRequests.Count; i++)
			{
				if (m_userRequests[i].Id == _idRequest)
				{
					return m_userRequests[i];
				}
			}

			for (int i = 0; i < m_providerRequests.Count; i++)
			{
				if (m_providerRequests[i].Id == _idRequest)
				{
					return m_providerRequests[i];
				}
			}

			for (int i = 0; i < m_currentResultRequests.Count; i++)
			{
				if (m_currentResultRequests[i].Id == _idRequest)
				{
					return m_currentResultRequests[i];
				}
			}
			return null;
		}

		// -------------------------------------------
		/* 
		 * CountActiveRequestByUser
		 */
		public int CountActiveRequestByUser(int _idUser)
		{
			int counter = 0;
			for (int i = 0; i < m_userRequests.Count; i++)
			{
				if (m_userRequests[i].Customer == _idUser)
				{
					if (((m_userRequests[i].Provider == -1) || (m_userRequests[i].Deliverydate == -1)) && (!m_userRequests[i].IsFlagged()))
					{
						counter++;
					}
				}
			}
			return counter;
		}

		// -------------------------------------------
		/* 
		 * AddUserRequests
		 */
		private void AddUserRequests(int _idUserRequested, long _idRequest, int _idCustomer, int _idProvider, string _title, int _price, long _deadline, int _score, long _deliveryDate, string _currency, string _mapData, long _referenceImage, string _village, long _proposal, string _feedbackcustomer, int _scorecustomer, string _feedbackprovider, int _scoreprovider, string _reported, int _flaged, string _transactionID)
		{
			int foundIndex = -1;
			if (UsersController.Instance.CurrentUser.Id == _idUserRequested)
			{
				for (int i = 0; i < m_userRequests.Count; i++)
				{
					if (m_userRequests[i].Id == _idRequest)
					{
						foundIndex = i;
					}
				}
			}

			RequestModel request = new RequestModel();
			if (foundIndex != -1)
			{
				request = m_userRequests[foundIndex];
			}
			request.Id = _idRequest;
			request.Customer = _idCustomer;
			request.Provider = _idProvider;
			request.Title = _title;
			request.Price = _price;
			request.Deadline = _deadline;
			request.Score = _score;
			request.Deliverydate = _deliveryDate;
			request.Currency = _currency;
			request.Mapdata = _mapData;
			request.Referenceimg = _referenceImage;
			request.Village = _village;
			request.Proposal = _proposal;
			request.FeedbackCustomerGivesToTheProvider = _feedbackcustomer;
			request.ScoreCustomerGivesToTheProvider = _scorecustomer;
			request.FeedbackProviderGivesToTheCustomer = _feedbackprovider;
			request.ScoreProviderGivesToTheCustomer = _scoreprovider;
			request.Reported = _reported;
			request.Flaged = _flaged;
			request.TransactionIdBitcoin = _transactionID;

			if (foundIndex == -1)
			{
				if (UsersController.Instance.CurrentUser.Id == _idUserRequested)
				{
					m_userRequests.Add(request);
				}
				else
				{
					m_otherUserRequests.Add(request);
				}
			}
			request.CheckExistence = true;
		}

		// -------------------------------------------
		/* 
		 * AddConsultProviderRequests
		 */
		private void AddConsultProviderRequests(int _idUserRequested, long _idRequest, int _idCustomer, int _idProvider, string _title, int _price, long _deadline, int _score, long _deliveryDate, string _currency, string _mapData, long _referenceImage, string _village, long _proposal, string _feedbackcustomer, int _scorecustomer, string _feedbackprovider, int _scoreprovider, string _reported, int _flaged, string _transactionID)
		{
			int foundIndex = -1;
			if (UsersController.Instance.CurrentUser.Id == _idUserRequested)
			{
				for (int i = 0; i < m_providerRequests.Count; i++)
				{
					if (m_providerRequests[i].Id == _idRequest)
					{
						foundIndex = i;
					}
				}
			}

			RequestModel request = new RequestModel();
			if (foundIndex != -1)
			{
				request = m_providerRequests[foundIndex];
			}
			request.Id = _idRequest;
			request.Customer = _idCustomer;
			request.Provider = _idProvider;
			request.Title = _title;
			request.Price = _price;
			request.Deadline = _deadline;
			request.Score = _score;
			request.Deliverydate = _deliveryDate;
			request.Currency = _currency;
			request.Mapdata = _mapData;
			request.Referenceimg = _referenceImage;
			request.Village = _village;
			request.Proposal = _proposal;
			request.FeedbackCustomerGivesToTheProvider = _feedbackcustomer;
			request.ScoreCustomerGivesToTheProvider = _scorecustomer;
			request.FeedbackProviderGivesToTheCustomer = _feedbackprovider;
			request.ScoreProviderGivesToTheCustomer = _scoreprovider;
			request.Reported = _reported;
			request.Flaged = _flaged;
			request.TransactionIdBitcoin = _transactionID;

			if (foundIndex == -1)
			{
				if (UsersController.Instance.CurrentUser.Id == _idUserRequested)
				{
					m_providerRequests.Add(request);
				}
				else
				{
					m_otherProviderRequests.Add(request);
				}
			}
			request.CheckExistence = true;
		}


		// -------------------------------------------
		/* 
		 * AddConsultDistanceRequests
		 */
		private void AddConsultDistanceRequests(long _idRequest, int _idCustomer, int _idProvider, string _title, int _price, long _deadline, int _score, long _deliveryDate, string _currency, string _mapData, long _referenceImage, string _village, long _proposal, string _feedbackcustomer, int _scorecustomer, string _feedbackprovider, int _scoreprovider, string _reported, int _flaged, string _transactionID)
		{
			RequestModel request = new RequestModel();
			request.Id = _idRequest;
			request.Customer = _idCustomer;
			request.Provider = _idProvider;
			request.Title = _title;
			request.Price = _price;
			request.Deadline = _deadline;
			request.Score = _score;
			request.Deliverydate = _deliveryDate;
			request.Currency = _currency;
			request.Mapdata = _mapData;
			request.Referenceimg = _referenceImage;
			request.Village = _village;
			request.Proposal = _proposal;
			request.FeedbackCustomerGivesToTheProvider = _feedbackcustomer;
			request.ScoreCustomerGivesToTheProvider = _scorecustomer;
			request.FeedbackProviderGivesToTheCustomer = _feedbackprovider;
			request.ScoreProviderGivesToTheCustomer = _scoreprovider;
			request.Reported = _reported;
			request.Flaged = _flaged;
			request.TransactionIdBitcoin = _transactionID;

			if (GetLocalRequest(_idRequest) == null)
			{
				m_currentResultRequests.Add(request);
			}
		}

		// -------------------------------------------
		/* 
		 * UpdateLocalRequest
		 */
		private void UpdateLocalRequest(RequestModel _request)
		{
			for (int i = 0; i < m_userRequests.Count; i++)
			{
				if (m_userRequests[i].Id == _request.Id)
				{
					m_userRequests[i].Copy(_request, true);
				}
			}

			for (int i = 0; i < m_providerRequests.Count; i++)
			{
				if (m_providerRequests[i].Id == _request.Id)
				{
					m_providerRequests[i].Copy(_request, true);
				}
			}

			for (int i = 0; i < m_currentResultRequests.Count; i++)
			{
				if (m_currentResultRequests[i].Id == _request.Id)
				{
					m_currentResultRequests[i].Copy(_request, true);
				}
			}
		}

		// -------------------------------------------
		/* 
		 * ParseRecordsData
		 */
		private void ParseRecordsData(int _requestType, string _buf)
		{
			switch (_requestType)
			{
				case TYPE_CONSULT_BY_USER:
					m_otherUserRequests.Clear();
					break;

				case TYPE_CONSULT_BY_PROVIDER:
					m_otherProviderRequests.Clear();
					break;

				case TYPE_CONSULT_BY_DISTANCE:
					m_currentResultRequests.Clear();
					break;
			}

			int idUserRequested = -1;
			string data = _buf;
			if (data.Length > 5)
			{
				DirtyExistence(_requestType);
				string[] lines = data.Split(new string[] { CommController.TOKEN_SEPARATOR_LINES }, StringSplitOptions.None);
				for (int i = 0; i < lines.Length; i++)
				{
					string[] tokens = lines[i].Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
					if (MenusScreenController.Instance.DebugMode)
					{
						Debug.Log("REQUEST[" + i + "] tokens.Length=" + tokens.Length);
					}
					if (tokens.Length >= 20)
					{
						long idRequest = long.Parse(tokens[0]);
						int idCustomer = int.Parse(tokens[1]);
						int idProvider = int.Parse(tokens[2]);
						string title = tokens[3];
						int price = int.Parse(tokens[4]);
						long deadline = long.Parse(tokens[5]);
						int score = int.Parse(tokens[6]);
						long deliveryDate = long.Parse(tokens[7]);
						string currency = tokens[8];
						string mapData = tokens[9];
						long idImageReference = long.Parse(tokens[10]);
						string village = tokens[11];
						long proposal = long.Parse(tokens[12]);
						string feedbackcustomer = tokens[13];
						int scorecustomer = int.Parse(tokens[14]);
						string feedbackprovider = tokens[15];
						int scoreprovider = int.Parse(tokens[16]);
						string reported = tokens[18];
						int flaged = int.Parse(tokens[19]);
						string transactionID = tokens[20];

						switch (_requestType)
						{
							case TYPE_CONSULT_BY_USER:
								idUserRequested = int.Parse(tokens[17]);
								AddUserRequests(idUserRequested, idRequest, idCustomer, idProvider, title, price, deadline, score, deliveryDate, currency, mapData, idImageReference, village, proposal, feedbackcustomer, scorecustomer, feedbackprovider, scoreprovider, reported, flaged, transactionID);
								break;

							case TYPE_CONSULT_BY_DISTANCE:
								AddConsultDistanceRequests(idRequest, idCustomer, idProvider, title, price, deadline, score, deliveryDate, currency, mapData, idImageReference, village, proposal, feedbackcustomer, scorecustomer, feedbackprovider, scoreprovider, reported, flaged, transactionID);
								break;

							case TYPE_CONSULT_BY_PROVIDER:
								idUserRequested = int.Parse(tokens[17]);
								AddConsultProviderRequests(idUserRequested, idRequest, idCustomer, idProvider, title, price, deadline, score, deliveryDate, currency, mapData, idImageReference, village, proposal, feedbackcustomer, scorecustomer, feedbackprovider, scoreprovider, reported, flaged, transactionID);
								break;
						}
					}
				}
				CleanNotExistence(_requestType);
			}
			switch (_requestType)
			{
				case TYPE_CONSULT_BY_USER:
					m_mustReloadRequests = false;
					if (idUserRequested == UsersController.Instance.CurrentUser.Id)
					{
						if (MenusScreenController.Instance.DebugMode)
						{
							Debug.Log("EVENT_REQUEST_RESULT_FORMATTED_RECORDS[TYPE_CONSULT_BY_USER]::LOCAL USER REQUESTS::m_userRequests[" + m_userRequests.Count + "]");
						}
						UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_userRequests, TYPE_CONSULT_BY_USER);
					}
					else
					{
						m_otherUserIDLastConsult = idUserRequested;
						if (MenusScreenController.Instance.DebugMode)
						{
							Debug.Log("EVENT_REQUEST_RESULT_FORMATTED_RECORDS::OTHER USER[" + m_otherUserIDLastConsult + "] REQUESTS::m_userRequests[" + m_otherUserRequests.Count + "]");
						}
						UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_otherUserRequests, TYPE_CONSULT_BY_USER);
					}
					break;

				case TYPE_CONSULT_BY_DISTANCE:
					if (MenusScreenController.Instance.DebugMode)
					{
						Debug.Log("EVENT_REQUEST_RESULT_FORMATTED_RECORDS[TYPE_CONSULT_BY_DISTANCE]::m_currentResultRequests[" + m_currentResultRequests.Count + "]");
					}
					UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_currentResultRequests, TYPE_CONSULT_BY_DISTANCE);
					break;

				case TYPE_CONSULT_BY_PROVIDER:
					m_mustReloadProviderRequests = false;
					if (idUserRequested == UsersController.Instance.CurrentUser.Id)
					{
						if (MenusScreenController.Instance.DebugMode)
						{
							Debug.Log("EVENT_REQUEST_RESULT_FORMATTED_RECORDS[TYPE_CONSULT_BY_PROVIDER]::LOCAL PROVIDER::m_providerRequests[" + m_providerRequests.Count + "]");
						}
						UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_providerRequests, TYPE_CONSULT_BY_PROVIDER);
					}
					else
					{
						m_otherProviderIDLastConsult = idUserRequested;
						if (MenusScreenController.Instance.DebugMode)
						{
							Debug.Log("EVENT_REQUEST_RESULT_FORMATTED_RECORDS[TYPE_CONSULT_BY_PROVIDER]::OTHER PROVIDER[" + m_otherProviderIDLastConsult + "]::m_otherProviderRequests[" + m_otherProviderRequests.Count + "]");
						}
						UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_otherProviderRequests, TYPE_CONSULT_BY_PROVIDER);
					}
					break;
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_NEW_IMAGE)
			{
				string tabla = (string)_list[1];
				long idRequests = (long)_list[2];
				int typeImage = (int)_list[3];
				if (tabla == MenusScreenController.TABLE_REQUESTS)
				{
					if (typeImage == RequestModel.IMAGE_TYPE_FINISHED)
					{
						RequestModel localRequest = GetLocalRequest(idRequests);
						if (localRequest != null)
						{
							localRequest.IsDataFull = false;
						}
					}
				}
			}
			if (_nameEvent == EVENT_REQUEST_CALL_CREATE_OR_UPDATE_REQUEST)
			{
				RequestModel request = (RequestModel)_list[0];
				RequestModel localRequest = GetLocalRequest(request.Id);
				if (localRequest != null)
				{
					localRequest.Copy(request, true);
					localRequest.IsDataFull = false;
				}
				CommsHTTPConstants.CreateNewRequestDress(UsersController.Instance.CurrentUser.Id.ToString(), UsersController.Instance.CurrentUser.Password, request);
			}
			if (_nameEvent == EVENT_REQUEST_RESULT_CREATED_RECORD_CONFIRMATION)
			{
				if ((bool)_list[0])
				{
					UsersController.Instance.CurrentUser.Additionalrequest = 0;
				}
			}
			if (_nameEvent == EVENT_REQUEST_CALL_CONSULT_RECORDS_BY_USER)
			{
				int idUser = UsersController.Instance.CurrentUser.Id;
				if (_list.Length > 0)
				{
					idUser = (int)_list[0];
				}
				if (m_mustReloadRequests || (idUser != UsersController.Instance.CurrentUser.Id))
				{
					if (m_otherUserIDLastConsult == idUser)
					{
						if (m_otherUserRequests.Count == 0)
						{
							CommsHTTPConstants.RequestConsultRecordsByUser(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idUser);
						}
						else
						{
							UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_otherUserRequests, TYPE_CONSULT_BY_USER, idUser);
						}
					}
					else
					{
						CommsHTTPConstants.RequestConsultRecordsByUser(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idUser);
					}
				}
				else
				{
					if (m_userRequests.Count == 0)
					{
						CommsHTTPConstants.RequestConsultRecordsByUser(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idUser);
					}
					else
					{
						UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_userRequests, TYPE_CONSULT_BY_USER, idUser);
					}
				}
			}
			if (_nameEvent == EVENT_REQUEST_CALL_CONSULT_BY_PROVIDER)
			{
				int idProvider = (int)_list[0];
				bool requestAll = (bool)_list[1];
				if (m_mustReloadProviderRequests || (idProvider != UsersController.Instance.CurrentUser.Id))
				{
					if (m_otherProviderIDLastConsult == idProvider)
					{
						if (m_otherProviderRequests.Count == 0)
						{
							CommsHTTPConstants.RequestConsultRecordsByProvider(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idProvider, requestAll);
						}
						else
						{
							UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_otherProviderRequests, TYPE_CONSULT_BY_PROVIDER);
						}
					}
					else
					{
						CommsHTTPConstants.RequestConsultRecordsByProvider(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idProvider, requestAll);
					}
				}
				else
				{
					if (m_providerRequests.Count == 0)
					{
						CommsHTTPConstants.RequestConsultRecordsByProvider(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idProvider, requestAll);
					}
					else
					{
						UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_RECORDS, m_providerRequests, TYPE_CONSULT_BY_PROVIDER);
					}
				}
			}
			if (_nameEvent == EVENT_REQUEST_CALL_CONSULT_BY_DISTANCE_RECORDS)
			{
				MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
				CommsHTTPConstants.RequestConsultRecordsByDistance(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, (SearchModel)_list[0], RequestModel.STATE_REQUEST_OPEN);
			}
			if (_nameEvent == EVENT_REQUEST_CALL_CONSULT_SINGLE_RECORD)
			{
				long idRequest = (long)_list[0];
				RequestModel request = GetLocalRequest(idRequest);
				bool makeServerCall = true;
				if (request != null)
				{
					if (request.IsDataFull)
					{
						makeServerCall = false;
					}
				}
				if (makeServerCall)
				{
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
					CommsHTTPConstants.RequestConsultSingleRequest(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idRequest);
				}
				else
				{
					UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_SINGLE_RECORD, request);
				}
			}
			if (_nameEvent == EVENT_REQUEST_RESULT_CONSULT_SINGLE_RECORD)
			{
				if (_list == null)
				{
					return;
				}
				if (_list.Length == 0)
				{
					return;
				}

				string buf = (string)_list[0];
				string[] lines = buf.Split(new string[] { CommController.TOKEN_SEPARATOR_LINES }, StringSplitOptions.None);

				RequestModel singleRecord = null;

				for (int k = 0; k < lines.Length; k++)
				{
					string[] tokens = lines[k].Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
					if (k == 0)
					{
						if (MenusScreenController.Instance.DebugMode)
						{
							Debug.Log("EVENT_REQUEST_RESULT_CONSULT_SINGLE_RECORD::tokens[" + tokens.Length + "]=" + lines[k]);
						}
						if (tokens.Length == 31)
						{
							singleRecord = new RequestModel(long.Parse(tokens[0]),
																		int.Parse(tokens[1]),
																		int.Parse(tokens[2]),
																		tokens[3],
																		tokens[4],
																		int.Parse(tokens[5]),
																		long.Parse(tokens[6]),
																		tokens[7],
																		tokens[8],
																		float.Parse(tokens[9]),
																		float.Parse(tokens[10]),
																		int.Parse(tokens[11]),
																		tokens[12],
																		int.Parse(tokens[13]),
																		tokens[14],
																		int.Parse(tokens[15]),
																		long.Parse(tokens[16]),
																		long.Parse(tokens[17]),
																		int.Parse(tokens[18]),
																		long.Parse(tokens[19]),
																		int.Parse(tokens[20]),
																		false,
																		true,
																		null,
																		long.Parse(tokens[21]),
																		tokens[22],
																		int.Parse(tokens[23]),
																		tokens[24],
																		int.Parse(tokens[25]),
																		tokens[26],
																		int.Parse(tokens[27]),
																		tokens[28],
																		tokens[29],
																		tokens[30]);
						}
					}
					else
					{
						if (MenusScreenController.Instance.DebugMode)
						{
							Debug.Log("EVENT_REQUEST_RESULT_CONSULT_SINGLE_RECORD::((IMAGE)) tokens[" + tokens.Length + "]=" + lines[k]);
						}
						if (tokens.Length == 6)
						{
							ImageModel img = new ImageModel();
							img.Id = long.Parse(tokens[0]);
							img.Table = tokens[1];
							img.IdOrigin = long.Parse(tokens[2]);
							img.Size = int.Parse(tokens[3]);
							img.Type = int.Parse(tokens[4]);
							img.Url = tokens[5];
							singleRecord.TemporalImageReferences.Add(img);
						}
					}
				}

				if (singleRecord != null)
				{
					m_lastRequestConsulted = singleRecord.CloneNoImages();
					UpdateLocalRequest(singleRecord);
					UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_SINGLE_RECORD, singleRecord);
				}
				else
				{
					UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_SINGLE_RECORD);
				}
			}
			if (_nameEvent == EVENT_REQUEST_RESULT_CONSULT_RECORDS)
			{
				ParseRecordsData(TYPE_CONSULT_BY_USER, (string)_list[0]);
			}
			if (_nameEvent == EVENT_REQUEST_RESULT_CONSULT_DISTANCE_RECORDS)
			{
				ParseRecordsData(TYPE_CONSULT_BY_DISTANCE, (string)_list[0]);
			}
			if (_nameEvent == EVENT_REQUEST_RESULT_CONSULT_BY_PROVIDER_RECORDS)
			{
				ParseRecordsData(TYPE_CONSULT_BY_PROVIDER, (string)_list[0]);
			}
			if (_nameEvent == EVENT_REQUEST_CALL_DELETE_RECORDS)
			{
				long idRequest = (long)_list[0];
				RemoveLocalRequests(idRequest);
				CommsHTTPConstants.DeleteRequest(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idRequest);
			}
			if (_nameEvent == EVENT_REQUEST_CALL_UPDATE_IMG_REF)
			{
				long idRequest = (long)_list[0];
				long idImageReference = (long)_list[1];
				RequestModel request = GetLocalRequest(idRequest);
				if (request != null)
				{
					request.Referenceimg = idImageReference;
				}
				CommsHTTPConstants.UpdateImageReferenceRequest(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idRequest, idImageReference);
			}
			if (_nameEvent == EVENT_REQUEST_CALL_CONSULT_IMAGES_REQUEST)
			{
				CommsHTTPConstants.ConsultImagesRequest(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, (long)_list[0], MenusScreenController.TABLE_REQUESTS);
			}
			if (_nameEvent == EVENT_REQUEST_RESULT_CONSULT_IMAGES_REQUEST)
			{
				if (_list == null)
				{
					return;
				}
				if (_list.Length == 0)
				{
					return;
				}

				string buf = (string)_list[0];
				string[] lines = buf.Split(new string[] { CommController.TOKEN_SEPARATOR_LINES }, StringSplitOptions.None);

				List<ImageModel> imagesForRequest = new List<ImageModel>();

				for (int k = 0; k < lines.Length; k++)
				{
					string[] tokens = lines[k].Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
					if (tokens.Length == 6)
					{
						ImageModel img = new ImageModel();
						img.Id = long.Parse(tokens[0]);
						img.Table = tokens[1];
						img.IdOrigin = long.Parse(tokens[2]);
						img.Size = int.Parse(tokens[3]);
						img.Type = int.Parse(tokens[4]);
						img.Url = tokens[5];
						imagesForRequest.Add(img);
					}
				}

				UIEventController.Instance.DispatchUIEvent(EVENT_REQUEST_RESULT_FORMATTED_IMAGES_REQUEST, imagesForRequest);
			}
			if (_nameEvent == ProposalsController.EVENT_PROPOSAL_CALL_UPDATE_PROPOSAL)
			{
				ProposalModel proposalData = (ProposalModel)_list[0];
				RequestModel request = GetLocalRequest(proposalData.Request);
				if (request != null)
				{
					request.Price = proposalData.Price;
					request.Deadline = proposalData.Deadline;
					if (proposalData.Accepted == ProposalModel.STATE_PROPOSAL_ACCEPTED_AND_FIXED)
					{
						request.Provider = (int)proposalData.User;
						MustReloadRequests = true;
					}
				}
			}
			if (_nameEvent == EVENT_REQUEST_CALL_SET_JOB_AS_FINISHED)
			{
				long idRequest = (long)_list[0];
				bool broken = (bool)_list[1];
				RequestModel localRequest = GetLocalRequest(idRequest);
				if (localRequest != null)
				{
					localRequest.IsDataFull = false;
					localRequest.Deliverydate = 0;
				}
				CommsHTTPConstants.SetRequestAsFinished(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idRequest, broken);
			}
			if (_nameEvent == EVENT_REQUEST_CALL_SCORE_AND_FEEDBACK_UPDATE)
			{
				long idRequest = (long)_list[0];
				RequestModel localRequest = GetLocalRequest(idRequest);
				int scoreConsumer = (int)_list[1];
				string feedbackConsumer = (string)_list[2];
				int scoreProvider = (int)_list[3];
				string feedbackProvider = (string)_list[4];
				string signatureCustomer = (string)_list[5];
				string signatureProvider = (string)_list[6];
				if (localRequest != null)
				{
					localRequest.IsDataFull = false;
					localRequest.ScoreCustomerGivesToTheProvider = scoreConsumer;
					localRequest.ScoreProviderGivesToTheCustomer = scoreProvider;
					localRequest.FeedbackCustomerGivesToTheProvider = feedbackConsumer;
					localRequest.FeedbackProviderGivesToTheCustomer = feedbackProvider;
				}
				CommsHTTPConstants.UpdateRequestScoreAndFeedback(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, idRequest, scoreConsumer, feedbackConsumer, scoreProvider, feedbackProvider, signatureCustomer, signatureProvider);
			}
			if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_INSERTED_PROPOSAL)
			{
				if ((bool)_list[0])
				{
					long proposalType = (int)_list[2];
					long requestID = (long)_list[3];
					string reportedByUsersID = (string)_list[4];

					if (proposalType == ProposalModel.TYPE_REPORT)
					{
						RequestModel localRequest = GetLocalRequest(requestID);
						if (localRequest != null)
						{
							localRequest.Reported = reportedByUsersID;
						}
					}
				}
			}
		}
	}

}