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
	 * ScreenBecomeProviderView
	 * 
	 * It ask the user for a IAP when he wants
	 * to become a provider.
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenBecomeProviderView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_BECOME_PROVIDER = "SCREEN_BECOME_PROVIDER";

		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_SCREENBECOMEPROVIDER_CONFIRMATION = "SUB_EVENT_SCREENBECOMEPROVIDER_CONFIRMATION";
		public const string SUB_EVENT_SCREENBECOMEPROVIDER_DESTROY = "SUB_EVENT_SCREENBECOMEPROVIDER_DESTROY";

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

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.become.proovider");

			m_container.Find("Button_Rent1").GetComponent<Button>().onClick.AddListener(OnRent1Pressed);
			m_container.Find("Button_Rent1/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.rent.proovider.1");

			m_container.Find("Button_Rent2").GetComponent<Button>().onClick.AddListener(OnRent2Pressed);
			m_container.Find("Button_Rent2/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.rent.proovider.2");

			m_container.Find("Button_Rent3").GetComponent<Button>().onClick.AddListener(OnRent3Pressed);
			m_container.Find("Button_Rent3/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.rent.proovider.3");

			m_container.Find("Button_Rent4").GetComponent<Button>().onClick.AddListener(OnRent4Pressed);
			m_container.Find("Button_Rent4/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.rent.proovider.4");

			m_container.Find("Button_Cancel").GetComponent<Button>().onClick.AddListener(OnClickCancel);
			m_container.Find("Button_Cancel/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.cancel");

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
		 * OnRentPurchase
		 */
		private void OnRentPurchase(string _rentIAP)
		{
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
			UIEventController.Instance.DispatchUIEvent(IAPController.EVENT_IAP_CALL_PURCHASE_RENT_PROVIDER, _rentIAP);
		}

		// -------------------------------------------
		/* 
		 * OnRent1Pressed
		 */
		private void OnRent1Pressed()
		{
			OnRentPurchase(IAPController.IAP_ENERGY_PACK_1);
		}

		// -------------------------------------------
		/* 
		 * OnRent2Pressed
		 */
		private void OnRent2Pressed()
		{
			OnRentPurchase(IAPController.IAP_ENERGY_PACK_2);
		}

		// -------------------------------------------
		/* 
		 * OnRent3Pressed
		 */
		private void OnRent3Pressed()
		{
			OnRentPurchase(IAPController.IAP_ENERGY_PACK_3);
		}

		// -------------------------------------------
		/* 
		 * OnRent4Pressed
		 */
		private void OnRent4Pressed()
		{
			OnRentPurchase(IAPController.IAP_ENERGY_PACK_4);
		}


		// -------------------------------------------
		/* 
		 * OnClickCancel
		 */
		private void OnClickCancel()
		{
			Destroy();
		}


		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UsersController.EVENT_USER_IAP_CALL_PURCHASE_RENT_PROVIDER)
			{
				if (!(bool)_list[0])
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					string title = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.iap.failure.become.proovider");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_SCREENBECOMEPROVIDER_DESTROY);
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_IAP_RESULT_PURCHASE_RENT_PROVIDER)
			{
				if ((bool)_list[0])
				{
					string title = LanguageController.Instance.GetText("message.info");
					string description = LanguageController.Instance.GetText("message.iap.congratulations.proovider");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_SCREENBECOMEPROVIDER_CONFIRMATION);
				}
				else
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					string title = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.iap.failure.become.proovider");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_SCREENBECOMEPROVIDER_DESTROY);
				}
			}			
			if (_nameEvent == ScreenController.EVENT_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				if (subEvent == SUB_EVENT_SCREENBECOMEPROVIDER_CONFIRMATION)
				{
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
					UIEventController.Instance.DelayUIEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, 0.01f, (long)UsersController.Instance.CurrentUser.Id);
					Destroy();					
				}
				if (subEvent == SUB_EVENT_SCREENBECOMEPROVIDER_DESTROY)
				{
					Destroy();
				}
			}
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				OnClickCancel();
			}
		}
	}
}