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
	 * ScreenPremiumPostView
	 * 
	 * It ask for a IAP when the users wants to create
	 * a proposal for an offer wihtout waiting X hours to make it for free.
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenPremiumPostView : ScreenBaseView, IBasicScreenView
	{
		public const string SCREEN_PREMIUM_POST = "SCREEN_PREMIUM_POST";

		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_PREMIUM_POST_CONFIRMATION = "SUB_EVENT_PREMIUM_POST_CONFIRMATION";
		public const string SUB_EVENT_PREMIUM_POST_DESTROY = "SUB_EVENT_PREMIUM_POST_DESTROY";

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
			int hoursToBeAbleToPost = (int)_list[0];
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.premium.post", ((int)(ScreenController.Instance.HoursToEnableANewProposal - hoursToBeAbleToPost) + 1));

			m_container.Find("Button_PurchasePost").GetComponent<Button>().onClick.AddListener(OnPremiumPostPressed);
			m_container.Find("Button_PurchasePost/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.premim.button.now");

			m_container.Find("Button_Cancel").GetComponent<Button>().onClick.AddListener(OnClickCancel);
			m_container.Find("Button_Cancel/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.cancel");

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
		 * OnRentPurchase
		 */
		private void OnPremiumPostPressed()
		{
			ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
			BasicEventController.Instance.DispatchBasicEvent(IAPController.EVENT_IAP_CALL_PURCHASE_POST_OFFER_NO_WAIT);
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
			if (_nameEvent == UsersController.EVENT_USER_IAP_CALL_PURCHASE_POST_OFFER)
			{
				if (!(bool)_list[0])
				{
					BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
					string title = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.iap.failure.premium.post");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_POST_DESTROY);
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_IAP_RESULT_PURCHASE_POST_OFFER)
			{
				if ((bool)_list[0])
				{
					string title = LanguageController.Instance.GetText("message.info");
					string description = LanguageController.Instance.GetText("message.iap.congratulations.premium.post");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_POST_CONFIRMATION);
				}
				else
				{
					BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
					string title = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.iap.failure.premium.post");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_POST_DESTROY);
				}
			}
			if (_nameEvent == ScreenInformationView.EVENT_SCREENINFORMATION_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				if (subEvent == SUB_EVENT_PREMIUM_POST_CONFIRMATION)
				{
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
					BasicEventController.Instance.DelayBasicEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, 0.01f, (long)UsersController.Instance.CurrentUser.Id);
					Destroy();
				}
				if (subEvent == SUB_EVENT_PREMIUM_POST_DESTROY)
				{
					Destroy();
				}
			}
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				OnClickCancel();
			}
		}
	}
}