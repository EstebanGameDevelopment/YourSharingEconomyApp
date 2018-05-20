using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YourBitcoinManager;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * ScreenProfileView
	 * 
	 * It display and allows to edit the most basic information of the user:
	 * 
	 * Sections:
	 *      -Email
	 *      -Name
	 *      -Password
	 *      -Location
	 *      -Facebook connection
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenProfileView : ScreenBaseView, IBasicScreenView
	{
		public const string SCREEN_PROFILE = "SCREEN_PROFILE";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_SCREENPROFILE_SERVER_REQUEST_RESET_PASSWORD_CONFIRMATION = "EVENT_SCREENPROFILE_SERVER_REQUEST_RESET_PASSWORD_CONFIRMATION";

		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_SCREENPROFILE_CONFIRMATION_SAVE_CONFIRMATION = "SUB_EVENT_SCREENPROFILE_CONFIRMATION_SAVE_CONFIRMATION";
		public const string SUB_EVENT_SCREENPROFILE_CONFIRMATION_RESET_PASSWORD = "SUB_EVENT_SCREENPROFILE_CONFIRMATION_RESET_PASSWORD";
		public const string SUB_EVENT_SCREENPROFILE_CONFIRMATION_EXIT_WITHOUT_SAVE = "SUB_EVENT_SCREENPROFILE_CONFIRMATION_EXIT_WITHOUT_SAVE";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;

		private string m_newEmail;
		private string m_newName;
		private string m_newPassword;
		private string m_newVillage;
		private string m_newMapData;
		private bool m_hasChanged = false;

		private GameObject m_buttonSave;

		public bool HasChanged
		{
			get { return m_hasChanged; }
			set
			{
				m_hasChanged = value;
				m_buttonSave.SetActive(m_hasChanged);
			}
		}

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.title");

			m_container.Find("EmailTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.your.email");
			m_container.Find("EmailValue").GetComponent<InputField>().text = UsersController.Instance.CurrentUser.Email;
			m_container.Find("EmailValue").GetComponent<InputField>().onEndEdit.AddListener(OnChangeEmail);

			m_container.Find("NameTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.your.name");
			m_container.Find("NameValue").GetComponent<InputField>().text = UsersController.Instance.CurrentUser.Nickname;
			m_container.Find("NameValue").GetComponent<InputField>().onEndEdit.AddListener(OnChangeName);

			m_container.Find("PasswordTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.your.password");
			m_container.Find("PasswordValue").GetComponent<InputField>().text = UsersController.Instance.CurrentUser.PasswordPlain;
			m_container.Find("PasswordValue").GetComponent<InputField>().onEndEdit.AddListener(OnChangePassword);

			m_container.Find("Button_CheckCustomer").GetComponent<Button>().onClick.AddListener(OnCustomerProfile);
			m_container.Find("Button_CheckCustomer/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.consumer.profile");

			m_container.Find("Button_CheckProvider").GetComponent<Button>().onClick.AddListener(OnProviderProfile);
			m_container.Find("Button_CheckProvider/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.proovider.profile");

			string village = LanguageController.Instance.GetText("screen.profile.village.not.defined");
			if (UsersController.Instance.CurrentUser.Village.Length > 0)
			{
				village = UsersController.Instance.CurrentUser.Village;
			}
			m_container.Find("Button_Maps/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.area.maps") + '\n' + village;
			m_container.Find("Button_Maps").GetComponent<Button>().onClick.AddListener(OnGoogleMaps);

			m_container.Find("Button_ResetPassword").GetComponent<Button>().onClick.AddListener(OnResetPasswordButton);

			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(OnBackButton);
			
			m_container.Find("Button_Blockchain").GetComponent<Button>().onClick.AddListener(OnBitcoinManager);
			m_container.Find("Button_Blockchain/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.blockchain.identity");
			
			m_buttonSave = m_container.Find("Button_Save").gameObject;
			m_buttonSave.GetComponent<Button>().onClick.AddListener(OnSaveButton);
			HasChanged = false;

			GameObject btnFacebook = m_container.Find("Button_Facebook").gameObject;
			if (FacebookController.Instance.IsInited)
			{
				btnFacebook.SetActive(false);
			}
			else
			{
				btnFacebook.GetComponent<Button>().onClick.AddListener(OnConnectWithFacebook);
				btnFacebook.transform.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.connect.with.facebook");
			}

			m_newEmail = UsersController.Instance.CurrentUser.Email;
			m_newName = UsersController.Instance.CurrentUser.Nickname;
			m_newPassword = UsersController.Instance.CurrentUser.PasswordPlain;
			m_newVillage = UsersController.Instance.CurrentUser.Village;
			m_newMapData = UsersController.Instance.CurrentUser.Mapdata;

			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			BasicEventController.Instance.BasicEvent -= OnBasicEvent;
			GameObject.Destroy(this.gameObject);
		}

		// -------------------------------------------
		/* 
		 * OnChangeEmail
		 */
		private void OnChangeEmail(string _data)
		{
			HasChanged = true;
			m_newEmail = _data.ToLower();
		}

		// -------------------------------------------
		/* 
		 * OnChangeName
		 */
		private void OnChangeName(string _data)
		{
			HasChanged = true;
			m_newName = _data;
		}

		// -------------------------------------------
		/* 
		 * OnChangePassword
		 */
		private void OnChangePassword(string _data)
		{
			HasChanged = true;
			m_newPassword = _data.ToLower();
			if (m_newPassword != UsersController.Instance.CurrentUser.PasswordPlain)
			{
				ScreenController.Instance.CreateNewScreenNoParameters(ScreenPasswordValidationView.SCREEN_PASSWORD_VALIDATION, false, TypePreviousActionEnum.KEEP_CURRENT_SCREEN);
			}
		}

		// -------------------------------------------
		/* 
		 * OnResetPasswordButton
		 */
		private void OnResetPasswordButton()
		{
			string warning = LanguageController.Instance.GetText("message.warning");
			string description = LanguageController.Instance.GetText("message.profile.reset.password");
			ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROFILE_CONFIRMATION_RESET_PASSWORD);
		}

		// -------------------------------------------
		/* 
		 * OnGoogleMaps
		 */
		private void OnGoogleMaps()
		{
#if ENABLED_FACEBOOK
        string warning = LanguageController.Instance.GetText("message.warning");
        string description = LanguageController.Instance.GetText("message.map.not.available.in.facebook");
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
#else
			ScreenController.Instance.CreateNewScreen(ScreenGoogleMapView.SCREEN_GOOGLEMAP, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, false, m_newMapData, m_newVillage);
#endif
		}

		// -------------------------------------------
		/* 
		 * OnBackButton
		 */
		private void OnBackButton()
		{
			if (!UsersController.Instance.CurrentUser.Validated)
			{
				ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, SUB_EVENT_SCREENPROFILE_CONFIRMATION_EXIT_WITHOUT_SAVE);
				CommController.Instance.CheckValidationUser(UsersController.Instance.CurrentUser.Id.ToString());
			}
			else
			{
				if (HasChanged)
				{
					string warning = LanguageController.Instance.GetText("message.warning");
					string description = LanguageController.Instance.GetText("message.exit.without.apply.changes");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROFILE_CONFIRMATION_EXIT_WITHOUT_SAVE);
				}
				else
				{
					ScreenController.Instance.CreateNewScreenNoParameters(ScreenMainMenuView.SCREEN_MAIN_MENU, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
				}
			}
		}

		// -------------------------------------------
		/* 
		 * OnSaveButton
		 */
		private void OnSaveButton()
		{
			if (HasChanged)
			{
				string warning = LanguageController.Instance.GetText("message.warning");
				string description = LanguageController.Instance.GetText("message.apply.changes");
				ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROFILE_CONFIRMATION_SAVE_CONFIRMATION);
			}
		}

		// -------------------------------------------
		/* 
		 * OnCustomerProfile
		 */
		private void OnCustomerProfile()
		{
			ScreenController.Instance.CreateNewScreen(ScreenCustomerProfileView.SCREEN_CUSTOMER_PROFILE, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, UsersController.Instance.CurrentUser);
		}

		// -------------------------------------------
		/* 
		 * OnProviderProfile
		 */
		private void OnProviderProfile()
		{
			if (!UsersController.Instance.CurrentUser.IsProvider())
			{
				ScreenController.Instance.CreateNewScreen(ScreenBecomeProviderView.SCREEN_BECOME_PROVIDER, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, false);
			}
			else
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
				BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, (long)UsersController.Instance.CurrentUser.Id);
			}
		}

		// -------------------------------------------
		/* 
		 * OnConnectWithFacebook
		 */
		private void OnConnectWithFacebook()
		{
			ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
			FacebookController.Instance.Initialitzation();
		}

		// -------------------------------------------
		/* 
		 * OnBitcoinManager
		 */
		private void OnBitcoinManager()
		{
			ScreenBitcoinController.Instance.InitializeBitcoin(ScreenBitcoinPrivateKeyView.SCREEN_NAME);
		}

		// -------------------------------------------
		/* 
		 * Update
		 */
		void Update()
		{
			if (ScreenBitcoinController.Instance.ScreensEnabled > 0)
			{
				if (m_container.gameObject.activeSelf)
				{
					m_container.gameObject.SetActive(false);
				}					
			}
			else
			{
				if (!m_container.gameObject.activeSelf)
				{
					m_container.gameObject.SetActive(true);
				}				
			}
		}
		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (!this.gameObject.activeSelf) return;

			if (_nameEvent == UsersController.EVENT_USER_CHECK_VALIDATION_RESULT)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if (!(bool)_list[0])
				{
					string warning = LanguageController.Instance.GetText("message.warning");
					string description = LanguageController.Instance.GetText("message.user.not.validated");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
				}
			}
			if (_nameEvent == ScreenPasswordValidationView.EVENT_VALIDATION_PASSWORD_RESULT)
			{
				if (!((bool)_list[0]))
				{
					m_newPassword = UsersController.Instance.CurrentUser.PasswordPlain;
				}
				else
				{
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.profile.changed.password.success"), null, "");
				}
			}
			if (_nameEvent == ScreenPasswordValidationView.EVENT_VALIDATION_CANCELATION)
			{
				m_newPassword = UsersController.Instance.CurrentUser.PasswordPlain;
			}
			if (_nameEvent == ScreenInformationView.EVENT_SCREENINFORMATION_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				if (subEvent == SUB_EVENT_SCREENPROFILE_CONFIRMATION_SAVE_CONFIRMATION)
				{
					if (HasChanged)
					{
						if ((bool)_list[1])
						{
							ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
							BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_UPDATE_PROFILE_REQUEST,
																			 UsersController.Instance.CurrentUser.Id.ToString(),
																			 m_newPassword,
																			 m_newEmail,
																			 m_newName,
																			 m_newVillage,
																			 m_newMapData,
																			 UsersController.Instance.CurrentUser.Skills,
																			 UsersController.Instance.CurrentUser.Description,
																			 UsersController.Instance.CurrentUser.PublicKey);
						}
					}
				}
				if (subEvent == SUB_EVENT_SCREENPROFILE_CONFIRMATION_RESET_PASSWORD)
				{
					if ((bool)_list[1])
					{
						ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
						CommController.Instance.RequestResetPassword(UsersController.Instance.CurrentUser.Id.ToString());
					}
				}
				if (subEvent == SUB_EVENT_SCREENPROFILE_CONFIRMATION_EXIT_WITHOUT_SAVE)
				{
					if ((bool)_list[1])
					{
						ScreenController.Instance.CreateNewScreenNoParameters(ScreenMainMenuView.SCREEN_MAIN_MENU, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
					}
				}
			}
			if (_nameEvent == EVENT_SCREENPROFILE_SERVER_REQUEST_RESET_PASSWORD_CONFIRMATION)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if ((bool)_list[0])
				{
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.profile.check.email.to.reset"), null, "");
				}
				else
				{
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.error.server"), null, "");
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_UPDATE_PROFILE_RESULT)
			{
				HasChanged = false;
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if ((bool)_list[0])
				{
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("screen.profile.update.confirmation"), null, "");
				}
				else
				{
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.error"), LanguageController.Instance.GetText("screen.profile.update.failed"), null, "");
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_UPDATE_VILLAGE)
			{
				HasChanged = true;
				m_newVillage = (string)_list[0];
				m_newMapData = (string)_list[1];
				m_container.Find("Button_Maps/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.area.maps") + '\n' + m_newVillage;
			}
			if (_nameEvent == FacebookController.EVENT_FACEBOOK_COMPLETE_INITIALITZATION)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if (((string)_list[0]) != null)
				{
					CommController.Instance.RequestUserByFacebook(FacebookController.Instance.Id, FacebookController.Instance.NameHuman, UsersController.Instance.CurrentUser.Email, FacebookController.Instance.GetPackageFriends());
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
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if ((bool)_list[0])
				{
					m_container.Find("Button_Facebook").gameObject.SetActive(false);
					string titleInfoError = LanguageController.Instance.GetText("message.info");
					string descriptionInfoError = LanguageController.Instance.GetText("message.connected.facebook.success");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
				}
				else
				{
					string titleInfoError = LanguageController.Instance.GetText("message.error");
					string descriptionInfoError = LanguageController.Instance.GetText("screen.message.facebook.error.register");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				UserModel sUser = (UserModel)_list[0];
				if (sUser != null)
				{
					ScreenController.Instance.CreateNewScreen(ScreenProviderProfileView.SCREEN_PROVIDER_PROFILE_DISPLAY, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, sUser);
				}
			}
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				OnBackButton();
			}
		}
	}
}