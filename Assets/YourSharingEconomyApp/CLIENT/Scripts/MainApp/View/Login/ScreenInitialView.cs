using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * ScreenInitialView
	 * 
	 * Initial screen that will show the splash logo
	 * and it will try to login automatically if there is
	 * local encrypted information about the user and password.
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenInitialView : ScreenBaseView, IBasicScreenView
	{
		public const string SCREEN_INITIAL = "SCREEN_INITIAL";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_CONFIGURATION_DATA_RECEIVED = "EVENT_CONFIGURATION_DATA_RECEIVED";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;
		private Transform m_login;

		private int m_connectedFacebook = -1;
		private bool m_enableInteraction = true;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Button_Exit").GetComponent<Button>().onClick.AddListener(ExitPressed);

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.your.sharingeconomyapp.title");
			m_container.Find("Welcome").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.initial.welcome") + " " + LanguageController.Instance.GetText("message.your.sharingeconomyapp.title");

			m_container.Find("Button_Email").GetComponent<Button>().onClick.AddListener(EnterEmailPressed);
			m_container.Find("Button_Facebook").GetComponent<Button>().onClick.AddListener(EnterFacebookPressed);

			m_container.Find("Button_Email/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.initial.enter.mail");
			m_container.Find("Button_Facebook/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.initial.enter.facebook");

			// AUTO LOGIN
			m_login = m_root.transform.Find("Login");
			m_login.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.your.sharingeconomyapp.title");
			m_login.Find("Message").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.initial.login.welcome", LanguageController.Instance.GetText("message.your.sharingeconomyapp.title"));

			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);

			m_container.gameObject.SetActive(false);
			m_login.gameObject.SetActive(true);

			GetConfigurationServerData();

#if ENABLED_FACEBOOK
        m_connectedFacebook = 1;
#endif
		}

		// -------------------------------------------
		/* 
		 * GetConfigurationServerData
		 */
		private void GetConfigurationServerData()
		{
			CommController.Instance.GetServerConfigurationParameters();
		}

		// -------------------------------------------
		/* 
		 * AutoLogin
		 */
		private void AutoLogin()
		{
			// CHECK COOKIES CONNECTION
			m_connectedFacebook = PlayerPrefs.GetInt(ScreenController.USER_FACEBOOK_CONNECTED_COOCKIE, -1);
			if (m_connectedFacebook == -1)
			{
				m_container.gameObject.SetActive(true);
				m_login.gameObject.SetActive(false);
			}
			else
			{
				m_container.gameObject.SetActive(false);
				m_login.gameObject.SetActive(true);
			}

			// AUTOCONNECT
			ItemMultiTextEntry userData = ScreenController.Instance.LoadEmailLoginLocal();
			switch (m_connectedFacebook)
			{
				case 0:
					if (userData != null)
					{
						BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_LOGIN_REQUEST, userData.Items[0], userData.Items[1]);
					}
					else
					{
						m_container.gameObject.SetActive(true);
						m_login.gameObject.SetActive(false);
					}
					break;

				case 1:
					FacebookController.Instance.Initialitzation();
					break;

				default:
					break;
			}
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			BasicEventController.Instance.BasicEvent -= OnBasicEvent;
			GameObject.DestroyObject(this.gameObject);
		}

		// -------------------------------------------
		/* 
		 * EnterEmailPressed
		 */
		private void EnterEmailPressed()
		{
			if (!m_enableInteraction) return;
			m_enableInteraction = false;

			ScreenController.Instance.CreateNewScreenNoParameters(ScreenLoginView.SCREEN_LOGIN, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
		}

		// -------------------------------------------
		/* 
		 * RegisterPressed
		 */
		private void EnterFacebookPressed()
		{
			if (!m_enableInteraction) return;
			m_enableInteraction = false;

			FacebookController.Instance.Initialitzation();
		}

		// -------------------------------------------
		/* 
		 * ExitPressed
		 */
		private void ExitPressed()
		{
			Application.Quit();
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == BasicEventController.EVENT_BASICEVENT_DELAYED_CALL)
			{
				if (this.gameObject == ((GameObject)_list[0]))
				{
					Invoke((string)_list[1], 0);
				}
			}
			if (_nameEvent == EVENT_CONFIGURATION_DATA_RECEIVED)
			{
				UsersController.Instance.InitLocalUserSkills();
				BasicEventController.Instance.DelayBasicEvent(BasicEventController.EVENT_BASICEVENT_DELAYED_CALL, 2.4f, this.gameObject, "AutoLogin");
			}
			if (_nameEvent == FacebookController.EVENT_FACEBOOK_COMPLETE_INITIALITZATION)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if ((string)_list[0] != null)
				{
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
					CommController.Instance.RequestUserByFacebook(FacebookController.Instance.Id, FacebookController.Instance.NameHuman, FacebookController.Instance.Email, FacebookController.Instance.GetPackageFriends());
				}
				else
				{
					string titleInfoError = LanguageController.Instance.GetText("message.warning");
					string descriptionInfoError = LanguageController.Instance.GetText("message.operation.canceled");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_FACEBOOK_LOGIN_FORMATTED)
			{
				m_enableInteraction = true;
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if ((bool)_list[0])
				{
					ScreenController.Instance.CreateNewScreenNoParameters(ScreenMainMenuView.SCREEN_MAIN_MENU, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
				}
				else
				{
					string titleInfoError = LanguageController.Instance.GetText("message.error");
					string descriptionInfoError = LanguageController.Instance.GetText("screen.message.facebook.error.register");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_LOGIN_FORMATTED)
			{
				m_enableInteraction = true;
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if ((bool)_list[0])
				{
					UserModel userData = (UserModel)_list[1];
					if (userData.Validated)
					{
						ScreenController.Instance.CreateNewScreenNoParameters(ScreenMainMenuView.SCREEN_MAIN_MENU, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
					}
					else
					{
						ScreenController.Instance.CreateNewScreenNoParameters(ScreenValidationConfirmationView.SCREEN_VALIDATION_CONFIRMATION, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
					}
				}
				else
				{
					string titleInfoError = LanguageController.Instance.GetText("message.error");
					string descriptionInfoError = LanguageController.Instance.GetText("screen.message.logging.wrong.user");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
				}
			}
			if (_nameEvent == ScreenInformationView.EVENT_SCREENINFORMATION_CONFIRMATION_POPUP)
			{
				m_enableInteraction = true;
				m_container.gameObject.SetActive(true);
				m_login.gameObject.SetActive(false);
			}
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				ExitPressed();
			}
		}
	}
}