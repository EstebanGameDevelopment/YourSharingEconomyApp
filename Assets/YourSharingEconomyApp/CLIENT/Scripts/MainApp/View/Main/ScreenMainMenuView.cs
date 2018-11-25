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
	 * ScreenMainMenuView
	 * 
	 * Main screen of the system
	 * 
	 * Sections:
	 * 
	 *      -Access to provider's section
	 *      -Access to customer's section
	 *      -Access to profile's section
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenMainMenuView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_MAIN_MENU = "SCREEN_MAIN_MENU";

		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_SCREENMAIN_CONFIRMATION_EXIT_APP = "SUB_EVENT_SCREENMAIN_CONFIRMATION_EXIT_APP";

		// ----------------------------------------------
		// CONSTANTS
		// ----------------------------------------------	
		public const int CONSULT_TYPE_CUSTOMER = 0;
		public const int CONSULT_TYPE_PROVIDER = 1;

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;
		private Transform m_iconOn;
		private Transform m_iconOff;
		private int m_consultType = -1;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public override void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Button_Provider").GetComponent<Button>().onClick.AddListener(ProviderPressed);
			m_container.Find("Button_Customer").GetComponent<Button>().onClick.AddListener(CustomerPressed);
			m_container.Find("Button_Profile").GetComponent<Button>().onClick.AddListener(ProfilePressed);

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.your.sharingeconomyapp.title");
			m_container.Find("Button_Provider/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.main.proovider");
			m_container.Find("Button_Customer/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.main.consumer.");
			m_container.Find("Button_Profile/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.main.profile");

			m_container.Find("Button_Exit").GetComponent<Button>().onClick.AddListener(ExitPressed);

			m_container.Find("Button_Sound").GetComponent<Button>().onClick.AddListener(OnSoundPressed);
			m_iconOn = m_container.Find("Button_Sound/IconOn");
			m_iconOff = m_container.Find("Button_Sound/IconOff");

			m_iconOn.gameObject.SetActive(SoundsController.Instance.Enabled);
			m_iconOff.gameObject.SetActive(!SoundsController.Instance.Enabled);

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
		 * ExitPressed
		 */
		private void ExitPressed()
		{
			string warning = LanguageController.Instance.GetText("message.warning");
			string description = LanguageController.Instance.GetText("message.do.you.want.exit");
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENMAIN_CONFIRMATION_EXIT_APP);
		}

		// -------------------------------------------
		/* 
		 * OnSoundPressed
		 */
		private void OnSoundPressed()
		{
			SoundsController.Instance.Enabled = !SoundsController.Instance.Enabled;
			m_iconOn.gameObject.SetActive(SoundsController.Instance.Enabled);
			m_iconOff.gameObject.SetActive(!SoundsController.Instance.Enabled);
		}

		// -------------------------------------------
		/* 
		 * ProviderPressed
		 */
		private void ProviderPressed()
		{
			m_consultType = CONSULT_TYPE_PROVIDER;
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
			UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, (long)UsersController.Instance.CurrentUser.Id, true);
		}

		// -------------------------------------------
		/* 
		 * CustomerPressed
		 */
		private void CustomerPressed()
		{
			m_consultType = CONSULT_TYPE_CUSTOMER;
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
			UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, (long)UsersController.Instance.CurrentUser.Id, true);
		}

		// -------------------------------------------
		/* 
		 * ProfilePressed
		 */
		private void ProfilePressed()
		{
			MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenProfileView.SCREEN_PROFILE, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == ScreenController.EVENT_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				if (subEvent == SUB_EVENT_SCREENMAIN_CONFIRMATION_EXIT_APP)
				{
					if ((bool)_list[1])
					{
						Application.Quit();
					}
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD)
			{
                UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
                if (m_consultType == CONSULT_TYPE_CUSTOMER)
				{
					MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenRequestsView.SCREEN_REQUESTS, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
				}
				else
				{
					MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenOffersSummaryView.SCREEN_OFFERS, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
				}
			}
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				ExitPressed();
			}
		}
	}
}