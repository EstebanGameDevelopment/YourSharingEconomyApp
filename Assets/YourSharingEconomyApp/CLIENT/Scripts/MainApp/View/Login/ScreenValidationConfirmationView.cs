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
	 * ScreenValidationConfirmationView
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenValidationConfirmationView : ScreenBaseView, IBasicView
	{
		// ----------------------------------------------
		// SCREEN ID
		// ----------------------------------------------	
		public const string SCREEN_VALIDATION_CONFIRMATION = "SCREEN_VALIDATION_CONFIRMATION";

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

			m_container.Find("Button_Validation").GetComponent<Button>().onClick.AddListener(OnCheckValidatedAccount);
			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.validation.confirmation.title");
			m_container.Find("Message").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.validation.confirmation.description");

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
		 * OnCheckValidatedAccount
		 */
		private void OnCheckValidatedAccount()
		{
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
			CommsHTTPConstants.CheckValidationUser(UsersController.Instance.CurrentUser.Id.ToString());
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
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UsersController.EVENT_USER_CHECK_VALIDATION_RESULT)
			{
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				if ((bool)_list[0])
				{
					MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenMainMenuView.SCREEN_MAIN_MENU, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
				}
				else
				{
					string warning = LanguageController.Instance.GetText("message.warning");
					string description = LanguageController.Instance.GetText("message.user.not.validated");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, "");
				}
			}
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				BackPressed();
			}
		}
	}
}