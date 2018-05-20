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
	 * ScreenLoginView
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenLoginView : ScreenBaseView, IBasicScreenView
	{
		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_SCREENLOGIN_CONFIRMATION_EXIT_APP = "SUB_EVENT_SCREENLOGIN_CONFIRMATION_EXIT_APP";

		// ----------------------------------------------
		// SCREEN ID
		// ----------------------------------------------	
		public const string SCREEN_LOGIN = "SCREEN_LOGIN";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Button_OK").GetComponent<Button>().onClick.AddListener(OkPressed);
			m_container.Find("Button_Exit").GetComponent<Button>().onClick.AddListener(ExitPressed);
			m_container.Find("Button_Register").GetComponent<Button>().onClick.AddListener(RegisterPressed);
			m_container.Find("Button_Forget").GetComponent<Button>().onClick.AddListener(ForgetPressed);

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.login.title");
			m_container.Find("EmailTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.login.email");
			m_container.Find("PasswordTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.login.password");
			m_container.Find("Button_Register/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.login.register");
			m_container.Find("Button_Forget/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.login.forget.mail");

			if (ScreenController.Instance.DebugMode)
			{
				m_container.Find("EmailValue").GetComponent<InputField>().text = "YOUR_EMAIL_ADDRESS@YOUR_OWN_DOMAIN.COM";
				m_container.Find("PasswordValue").GetComponent<InputField>().text = "YOUR_PASSWORD";
			}

			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
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
		 * OkPressed
		 */
		private void OkPressed()
		{
			string emailToCheck = m_container.Find("EmailValue").GetComponent<InputField>().text.ToLower();
			string passwordToCheck = m_container.Find("PasswordValue").GetComponent<InputField>().text.ToLower();

			if ((emailToCheck.Length == 0) || (passwordToCheck.Length == 0))
			{
				string titleInfoError = LanguageController.Instance.GetText("message.error");
				string descriptionInfoError = LanguageController.Instance.GetText("screen.message.logging.error");
				ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
			}
			else
			{
				string titleWait = LanguageController.Instance.GetText("screen.wait.logging.title");
				string descriptionWait = LanguageController.Instance.GetText("screen.wait.logging.description");
				BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_LOGIN_REQUEST, emailToCheck, passwordToCheck);
				ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleWait, descriptionWait, null, "");
			}
		}

		// -------------------------------------------
		/* 
		 * ExitPressed
		 */
		private void ExitPressed()
		{
			string warning = LanguageController.Instance.GetText("message.warning");
			string description = LanguageController.Instance.GetText("message.do.you.want.exit");
			ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENLOGIN_CONFIRMATION_EXIT_APP);
		}

		// -------------------------------------------
		/* 
		 * RegisterPressed
		 */
		private void RegisterPressed()
		{
			ScreenController.Instance.CreateNewScreenNoParameters(ScreenRegisterView.SCREEN_REGISTER, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
		}

		// -------------------------------------------
		/* 
		 * ForgetPressed
		 */
		private void ForgetPressed()
		{
			string emailToCheck = m_container.Find("EmailValue").GetComponent<InputField>().text.ToLower();
			if (emailToCheck.Length == 0)
			{
				string titleInfoError = LanguageController.Instance.GetText("message.error");
				string descriptionInfoError = LanguageController.Instance.GetText("screen.login.email");
				ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
			}
			else
			{
				ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
				CommController.Instance.RequestResetPasswordByEmail(emailToCheck);
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UsersController.EVENT_USER_LOGIN_FORMATTED)
			{
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
			if (_nameEvent == UsersController.EVENT_USER_RESULT_RESETED_PASSWORD_BY_EMAIL)
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
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				ExitPressed();
			}
			if (_nameEvent == ScreenInformationView.EVENT_SCREENINFORMATION_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				if (subEvent == SUB_EVENT_SCREENLOGIN_CONFIRMATION_EXIT_APP)
				{
					if ((bool)_list[1])
					{
						Application.Quit();
					}
				}
			}
		}
	}
}