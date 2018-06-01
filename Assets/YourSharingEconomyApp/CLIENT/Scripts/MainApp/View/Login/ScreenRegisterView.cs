using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * ScreenRegisterView
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenRegisterView : ScreenBaseView, IBasicView
	{
		// ----------------------------------------------
		// SCREEN ID
		// ----------------------------------------------	
		public const string SCREEN_REGISTER = "SCREEN_REGISTER";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public override void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Button_Apply").GetComponent<Button>().onClick.AddListener(ApplyRegisterPressed);
			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.word");
			m_container.Find("EmailTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.email");
			m_container.Find("PasswordTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.password");
			m_container.Find("PasswordConfirmationTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.confirm");
			m_container.Find("Button_Apply/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.word");

			if (MenusScreenController.Instance.DebugMode)
			{
				m_container.Find("EmailValue").GetComponent<InputField>().text = "YOUR_EMAIL_ADDRESS@YOUR_OWN_DOMAIN.COM";
				m_container.Find("PasswordValue").GetComponent<InputField>().text = "YOUR_PASSWORD";
				m_container.Find("PasswordConfirmationValue").GetComponent<InputField>().text = "YOUR_PASSWORD";
			}

			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public override bool Destroy()
		{
			if (base.Destroy()) return true;

			UIEventController.Instance.UIEvent -= OnBasicEvent;
			UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);

			return false;
		}

		// -------------------------------------------
		/* 
		 * ApplyRegisterPressed
		 */
		private void ApplyRegisterPressed()
		{
			string emailToCheck = m_container.Find("EmailValue").GetComponent<InputField>().text.ToLower();
			string passwordToCheck = m_container.Find("PasswordValue").GetComponent<InputField>().text.ToLower();
			string confirmationToCheck = m_container.Find("PasswordConfirmationValue").GetComponent<InputField>().text.ToLower();

			if ((emailToCheck.Length == 0) || (passwordToCheck.Length == 0) || (confirmationToCheck.Length == 0))
			{
				string titleInfoError = LanguageController.Instance.GetText("message.error");
				string descriptionInfoError = LanguageController.Instance.GetText("screen.message.logging.error");
				MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
			}
			else
			{
				if (passwordToCheck != confirmationToCheck)
				{
					string titleInfoError = LanguageController.Instance.GetText("message.error");
					string descriptionInfoError = LanguageController.Instance.GetText("screen.register.mistmatch.password");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
				}
				else
				{
					string titleWait = LanguageController.Instance.GetText("screen.wait.register.title");
					string descriptionWait = LanguageController.Instance.GetText("screen.wait.register.description");
					UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_REGISTER_REQUEST, emailToCheck, passwordToCheck);
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, titleWait, descriptionWait, null, "");
				}
			}
		}

		// -------------------------------------------
		/* 
		 * BackPressed
		 */
		private void BackPressed()
		{
			MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenLoginView.SCREEN_LOGIN, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
		}

		// -------------------------------------------
		/* 
		 * RegisterPressed
		 */
		private void RegisterPressed()
		{
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UsersController.EVENT_USER_REGISTER_RESULT)
			{
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				if ((bool)_list[0])
				{
					MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenValidationConfirmationView.SCREEN_VALIDATION_CONFIRMATION, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
				}
				else
				{
					string titleInfoError = LanguageController.Instance.GetText("message.error");
					string descriptionInfoError = LanguageController.Instance.GetText("screen.register.wrong.register");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
				}
			}
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				BackPressed();
			}
		}
	}
}