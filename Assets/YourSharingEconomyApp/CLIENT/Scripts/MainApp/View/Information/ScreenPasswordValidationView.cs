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
	 * ScreenPasswordValidationView
	 * 
	 * Screen that manage the password validation
	 * when some sensitive data as the password needs
	 * to be changed by the user.
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenPasswordValidationView : ScreenBaseView, IBasicScreenView
	{
		public const string SCREEN_PASSWORD_VALIDATION = "SCREEN_PASSWORD_VALIDATION";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_VALIDATION_PASSWORD_RESULT = "EVENT_VALIDATION_PASSWORD_RESULT";
		public const string EVENT_VALIDATION_CANCELATION = "EVENT_VALIDATION_CANCELATION";

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
			m_container.Find("Button_Cancel").GetComponent<Button>().onClick.AddListener(CancelPressed);

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.validation.title");
			m_container.Find("PasswordTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.validation.enter.password");

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
			string passwordToCheck = m_container.Find("PasswordValue").GetComponent<InputField>().text.ToLower();

			if (passwordToCheck == UsersController.Instance.CurrentUser.PasswordPlain)
			{
				BasicEventController.Instance.DispatchBasicEvent(EVENT_VALIDATION_PASSWORD_RESULT, true);
			}
			else
			{
				BasicEventController.Instance.DispatchBasicEvent(EVENT_VALIDATION_PASSWORD_RESULT, false);
			}
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * ExitPressed
		 */
		private void CancelPressed()
		{
			BasicEventController.Instance.DispatchBasicEvent(EVENT_VALIDATION_CANCELATION);
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				CancelPressed();
			}
		}
	}
}