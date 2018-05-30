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
	public class ScreenPremiumPostView : ScreenBaseView, IBasicView
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
		public override void Initialize(params object[] _list)
		{
			int hoursToBeAbleToPost = (int)_list[0];
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.premium.post", ((int)(MenusScreenController.Instance.HoursToEnableANewProposal - hoursToBeAbleToPost) + 1));

			m_container.Find("Button_PurchasePost").GetComponent<Button>().onClick.AddListener(OnPremiumPostPressed);
			m_container.Find("Button_PurchasePost/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.premim.button.now");

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
			GameObject.Destroy(this.gameObject);

			return false;
		}

		// -------------------------------------------
		/* 
		 * OnRentPurchase
		 */
		private void OnPremiumPostPressed()
		{
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
			UIEventController.Instance.DispatchUIEvent(IAPController.EVENT_IAP_CALL_PURCHASE_POST_OFFER_NO_WAIT);
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
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					string title = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.iap.failure.premium.post");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_POST_DESTROY);
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_IAP_RESULT_PURCHASE_POST_OFFER)
			{
				if ((bool)_list[0])
				{
					string title = LanguageController.Instance.GetText("message.info");
					string description = LanguageController.Instance.GetText("message.iap.congratulations.premium.post");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_POST_CONFIRMATION);
				}
				else
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					string title = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.iap.failure.premium.post");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_POST_DESTROY);
				}
			}
			if (_nameEvent == ScreenController.EVENT_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				if (subEvent == SUB_EVENT_PREMIUM_POST_CONFIRMATION)
				{
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
					UIEventController.Instance.DelayUIEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, 0.01f, (long)UsersController.Instance.CurrentUser.Id);
					Destroy();
				}
				if (subEvent == SUB_EVENT_PREMIUM_POST_DESTROY)
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