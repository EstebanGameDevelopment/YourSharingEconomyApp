using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * UsersController
	 * 
	 * It manages all the operations related with the users' management
	 * 
	 * @author Esteban Gallardo
	 */
	public class UsersController : MonoBehaviour
	{
		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------
		public const string EVENT_USER_LOGIN_REQUEST = "EVENT_USER_LOGIN_REQUEST";
		public const string EVENT_USER_LOGIN_RESULT = "EVENT_USER_LOGIN_RESULT";
		public const string EVENT_USER_LOGIN_FORMATTED = "EVENT_USER_LOGIN_FORMATTED";

		public const string EVENT_USER_FACEBOOK_LOGIN_REQUEST = "EVENT_USER_FACEBOOK_LOGIN_REQUEST";
		public const string EVENT_USER_FACEBOOK_LOGIN_RESULT = "EVENT_USER_FACEBOOK_LOGIN_RESULT";
		public const string EVENT_USER_FACEBOOK_LOGIN_FORMATTED = "EVENT_USER_FACEBOOK_LOGIN_FORMATTED";

		public const string EVENT_USER_REGISTER_REQUEST = "EVENT_USER_REGISTER_REQUEST";
		public const string EVENT_USER_REGISTER_RESULT = "EVENT_USER_REGISTER_RESULT";

		public const string EVENT_USER_UPDATE_PROFILE_REQUEST = "EVENT_USER_UPDATE_PROFILE_REQUEST";
		public const string EVENT_USER_UPDATE_PROFILE_RESULT = "EVENT_USER_UPDATE_PROFILE_RESULT";

		public const string EVENT_USER_CHECK_VALIDATION_RESULT = "EVENT_USER_CHECK_VALIDATION_RESULT";

		public const string EVENT_USER_CALL_CONSULT_SINGLE_RECORD = "EVENT_USER_CALL_CONSULT_SINGLE_RECORD";
		public const string EVENT_USER_RESULT_CONSULT_SINGLE_RECORD = "EVENT_USER_RESULT_CONSULT_SINGLE_RECORD";
		public const string EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD = "EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD";

		public const string EVENT_USER_RESULT_RESETED_PASSWORD_BY_EMAIL = "EVENT_USER_RESULT_RESETED_PASSWORD_BY_EMAIL";

		public const string EVENT_USER_IAP_CALL_PURCHASE_RENT_PROVIDER = "EVENT_USER_IAP_CALL_PURCHASE_RENT_PROVIDER";
		public const string EVENT_USER_IAP_RESULT_PURCHASE_RENT_PROVIDER = "EVENT_USER_IAP_RESULT_PURCHASE_RENT_PROVIDER";

		public const string EVENT_USER_IAP_CALL_PURCHASE_NEW_REQUEST = "EVENT_USER_IAP_CALL_PURCHASE_NEW_REQUEST";
		public const string EVENT_USER_IAP_RESULT_PURCHASE_NEW_REQUEST = "EVENT_USER_IAP_RESULT_PURCHASE_NEW_REQUEST";

		public const string EVENT_USER_IAP_CALL_PURCHASE_POST_OFFER = "EVENT_USER_IAP_CALL_PURCHASE_POST_OFFER";
		public const string EVENT_USER_IAP_RESULT_PURCHASE_POST_OFFER = "EVENT_USER_IAP_RESULT_PURCHASE_POST_OFFER";

		// ----------------------------------------------
		// SINGLETON
		// ----------------------------------------------
		private static UsersController _instance;

		public static UsersController Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType(typeof(UsersController)) as UsersController;
					if (!_instance)
					{
						GameObject container = new GameObject();
						container.name = "UsersController";
						_instance = container.AddComponent(typeof(UsersController)) as UsersController;
					}
				}
				return _instance;
			}
		}

		// ----------------------------------------------
		// MEMBERS
		// ----------------------------------------------
		private UserModel m_currentUser;
		private List<UserModel> m_users = new List<UserModel>();
		private bool m_mustReloadUsers = false;


		// ----------------------------------------------
		// GETTERS/SETTERS
		// ----------------------------------------------
		public UserModel CurrentUser
		{
			get { return m_currentUser; }
		}
		public bool MustReloadUsers
		{
			get { return m_mustReloadUsers; }
			set { m_mustReloadUsers = value; }
		}

		// ----------------------------------------------
		// CONSTRUCTOR
		// ----------------------------------------------	
		// -------------------------------------------
		/* 
		 * Constructor
		 */
		private UsersController()
		{
		}

		// -------------------------------------------
		/* 
		 * Initialitzation
		 */
		public void Init()
		{
			m_currentUser = new UserModel("");

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
		 * GetLocalUser
		 */
		public UserModel GetLocalUser(long _idUser)
		{
			for (int i = 0; i < m_users.Count; i++)
			{
				if (m_users[i].Id == _idUser)
				{
					return m_users[i];
				}
			}
			return null;
		}

		// -------------------------------------------
		/* 
		 * InitLocalUserSkills
		 */
		public void InitLocalUserSkills()
		{
			m_currentUser.GetFormattedSkills();
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UsersController.EVENT_USER_LOGIN_REQUEST)
			{
				string email = (string)_list[0];
				string password = (string)_list[1];
				m_currentUser.UpdateBasicInfo(email, password);
				CommController.Instance.RequestUserByLogin(email, password);
			}
			if (_nameEvent == UsersController.EVENT_USER_LOGIN_RESULT)
			{
				if ((bool)_list[0])
				{
					m_currentUser.UpdateModel(false,
											  m_currentUser.Email,
											  (string)_list[1], //string _id,
											  (string)_list[2], //string _nickname,
											  (string)_list[3], //string _village,
											  (string)_list[4], //string _mapdata,
											  (string)_list[5], //string _registerdate,
											  (string)_list[6], //string _lastlogin,
											  (string)_list[7], //string _rentstart,
											  (string)_list[8], //string _rentdays,
											  (string)_list[9], //string _scoreuser,
											  (string)_list[10], //string _scoreprovider,
											  (string)_list[11], //string _votesuser,
											  (string)_list[12], //string _votesprovider
											  (string)_list[13], //string _validated
											  (string)_list[14], //string _skills
											  (string)_list[15], //string _description
											  (string)_list[16], //string _additionalrequest
											  (string)_list[17], //string _additionaloffer
											  (string)_list[18], //string _banned
											  (string)_list[19] //string _publickey
											  );
					if (ScreenController.Instance.DebugMode)
					{
						Debug.Log("UsersController::LOGIN SUCCESS");
					}
				}
				else
				{
					if (ScreenController.Instance.DebugMode)
					{
						Debug.Log("UsersController::LOGIN ERROR");
					}
				}
				BasicEventController.Instance.DispatchBasicEvent(EVENT_USER_LOGIN_FORMATTED, (bool)_list[0], m_currentUser);
			}
			if (_nameEvent == UsersController.EVENT_USER_REGISTER_REQUEST)
			{
				string email = (string)_list[0];
				string password = (string)_list[1];
				m_currentUser.UpdateBasicInfo(email, password);
				CommController.Instance.RequestUserRegister(email, password);
			}
			if (_nameEvent == UsersController.EVENT_USER_REGISTER_RESULT)
			{
				if ((bool)_list[0])
				{
					m_currentUser.UpdateModel(false,
											  m_currentUser.Email,
											  (string)_list[1], //string _id,
											  (string)_list[2], //string _nickname,
											  (string)_list[3], //string _village,
											  (string)_list[4], //string _mapdata,
											  (string)_list[5], //string _registerdate,
											  (string)_list[6], //string _lastlogin,
											  (string)_list[7], //string _rentstart,
											  (string)_list[8], //string _rentdays,
											  (string)_list[9], //string _scoreuser,
											  (string)_list[10], //string _scoreprovider,
											  (string)_list[11], //string _votesuser,
											  (string)_list[12], //string _votesprovider
											  (string)_list[13], //string _validated
											  (string)_list[14], //string _skills
											  (string)_list[15], //string _description
											  (string)_list[16], //string _additionalrequest
											  (string)_list[17], //string _additionaloffer
											  (string)_list[18], //string _banned
											  (string)_list[19] //string _publickey
											  );
					if (ScreenController.Instance.DebugMode)
					{
						Debug.Log("UsersController::LOGIN EMAIL SUCCESS");
					}
				}
				else
				{
					if (ScreenController.Instance.DebugMode)
					{
						Debug.Log("UsersController::LOGIN EMAIL ERROR");
					}
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_FACEBOOK_LOGIN_RESULT)
			{
				if ((bool)_list[0])
				{
					m_currentUser.UpdateModel(true,
											  (string)_list[1], //string _email,
											  (string)_list[3], //string _id,
											  (string)_list[4], //string _nickname,
											  (string)_list[5], //string _village,
											  (string)_list[6], //string _mapdata,
											  (string)_list[7], //string _registerdate,
											  (string)_list[8], //string _lastlogin,
											  (string)_list[9], //string _rentstart,
											  (string)_list[10], //string _rentdays,
											  (string)_list[11], //string _scoreuser,
											  (string)_list[12], //string _scoreprovider,
											  (string)_list[13], //string _votesuser,
											  (string)_list[14], //string _votesprovider
											  (string)_list[15], //string _validated
											  (string)_list[16], //string _skills
											  (string)_list[17], //string _description
											  (string)_list[18], //string _additionalrequest
											  (string)_list[19], //string _additionaloffer
											  (string)_list[20], //string _banned
											  (string)_list[21] //string _publicKey
											  );
					m_currentUser.SetPassword((string)_list[2]);
					if (ScreenController.Instance.DebugMode)
					{
						Debug.Log("UsersController::LOGIN FACEBOOK SUCCESS");
					}
				}
				else
				{
					if (ScreenController.Instance.DebugMode)
					{
						Debug.Log("UsersController::LOGIN FACEBOOK ERROR");
					}
				}
				BasicEventController.Instance.DispatchBasicEvent(EVENT_USER_FACEBOOK_LOGIN_FORMATTED, (bool)_list[0], m_currentUser);
			}
			if (_nameEvent == UsersController.EVENT_USER_UPDATE_PROFILE_REQUEST)
			{
				string idUser = (string)_list[0];
				string newPassword = (string)_list[1];
				string newEmail = (string)_list[2];
				string newNameUser = (string)_list[3];
				string newVillage = (string)_list[4];
				string newMapData = (string)_list[5];
				string newSkills = (string)_list[6];
				string newDescription = (string)_list[7];
				string publicKeyAddress = (string)_list[8];
				CommController.Instance.RequestUpdateProfile(idUser, UsersController.Instance.CurrentUser.PasswordPlain, newPassword, newEmail, newNameUser, newVillage, newMapData, newSkills, newDescription, publicKeyAddress);
			}
			if (_nameEvent == UsersController.EVENT_USER_UPDATE_PROFILE_RESULT)
			{
				if ((bool)_list[0])
				{
					m_currentUser.UpdateProfile((string)_list[1], //string _password,
											  (string)_list[2], //string _email,
											  (string)_list[3], //string _name,
											  (string)_list[4], //string _village,
											  (string)_list[5], //string _mapdata
											  (string)_list[6], //string _skills
											  (string)_list[7] //string _description
											  );
					if (ScreenController.Instance.DebugMode)
					{
						Debug.Log("UsersController::UPDATE PROFILE SUCCESS");
					}
				}
				else
				{
					if (ScreenController.Instance.DebugMode)
					{
						Debug.Log("UsersController::UPDATE PROFILE ERROR");
					}
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_CHECK_VALIDATION_RESULT)
			{
				if ((bool)_list[0])
				{
					m_currentUser.ValidateUser(true);
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD)
			{
				long idUserSearch = (long)_list[0];
				UserModel sUser = GetLocalUser(idUserSearch);
				if (sUser != null)
				{
					BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD, sUser);
				}
				else
				{
					CommController.Instance.RequestConsultUser(m_currentUser.Id, m_currentUser.Password, idUserSearch);
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_RESULT_CONSULT_SINGLE_RECORD)
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

				UserModel requestedUser = null;

				for (int k = 0; k < lines.Length; k++)
				{
					string[] tokens = lines[k].Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
					if (k == 0)
					{
						if (ScreenController.Instance.DebugMode)
						{
							Debug.Log("EVENT_USER_RESULT_CONSULT_SINGLE_RECORD::tokens[" + tokens.Length + "]=" + lines[k]);
						}
						if (tokens.Length == 20)
						{
							requestedUser = new UserModel((string)tokens[1], //string _id,
															(string)tokens[2], //string _nickname,
															(string)tokens[3], //string _village,
															(string)tokens[4], //string _mapdata,
															(string)tokens[5], //string _registerdate,
															(string)tokens[6], //string _lastlogin,
															(string)tokens[7], //string _rentstart,
															(string)tokens[8], //string _rentdays,
															(string)tokens[9], //string _scoreuser,
															(string)tokens[10], //string _scoreprovider,
															(string)tokens[11], //string _votesuser,
															(string)tokens[12], //string _votesprovider
															(string)tokens[13], //string _validated
															(string)tokens[14], //string _skills
															(string)tokens[15], //string _description
															(string)tokens[16], //string _additionalrequest
															(string)tokens[17], //string _additionaloffer
															(string)tokens[18], //string _banned
															(string)tokens[19] //string _publickey
															);
						}
					}
					else
					{
						if (ScreenController.Instance.DebugMode)
						{
							Debug.Log("EVENT_USER_RESULT_CONSULT_SINGLE_RECORD::((IMAGE)) tokens[" + tokens.Length + "]=" + lines[k]);
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
							requestedUser.ImageReferencesExperience.Add(img);
						}
					}
				}
				if (requestedUser != null)
				{
					if (requestedUser.Id == m_currentUser.Id)
					{
						m_currentUser.Copy(requestedUser);
					}
					else
					{
						m_users.Add(requestedUser);
					}
				}
				BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD, requestedUser);
			}
			if (_nameEvent == EVENT_USER_IAP_CALL_PURCHASE_RENT_PROVIDER)
			{
				bool success = (bool)_list[0];
				if (success)
				{
					int rentValue = (int)_list[1];
					string codeValidation = (string)_list[2];
					CommController.Instance.IAPRentTimeAsAProvider(m_currentUser.Id, m_currentUser.Password, rentValue, codeValidation);
				}
			}
			if (_nameEvent == EVENT_USER_IAP_CALL_PURCHASE_POST_OFFER)
			{
				bool success = (bool)_list[0];
				string codeValidation = (string)_list[1];
				if (success)
				{
					CommController.Instance.IAPPurchasePremiumOffer(m_currentUser.Id, m_currentUser.Password, codeValidation);
				}
			}
			if (_nameEvent == EVENT_USER_IAP_CALL_PURCHASE_NEW_REQUEST)
			{
				bool success = (bool)_list[0];
				string codeValidation = (string)_list[1];
				if (success)
				{
					CommController.Instance.IAPPurchasePremiumRequest(m_currentUser.Id, m_currentUser.Password, codeValidation);
				}
			}
		}
	}

}