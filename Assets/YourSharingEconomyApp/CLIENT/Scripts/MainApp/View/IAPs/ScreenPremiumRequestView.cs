﻿using System;
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
	 * ScreenPremiumRequestView
	 * 
	 * It asks the user for an IAP when he wants to create
	 * a new request.
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenPremiumRequestView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_PREMIUM_REQUEST = "SCREEN_PREMIUM_REQUEST";

		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_PREMIUM_REQUEST_CONFIRMATION = "SUB_EVENT_PREMIUM_REQUEST_CONFIRMATION";
		public const string SUB_EVENT_PREMIUM_REQUEST_DESTROY = "SUB_EVENT_PREMIUM_REQUEST_DESTROY";

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

			if (MenusScreenController.Instance.TotalNumberOfFreeRequests == 0)
			{
				m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.premium.no.free.request");
			}
			else
			{
				m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.premium.request", MenusScreenController.Instance.TotalNumberOfFreeRequests);
			}

			m_container.Find("Button_PurchaseRequest").GetComponent<Button>().onClick.AddListener(OnPremiumRequestPressed);
			if (MenusScreenController.Instance.TotalNumberOfFreeRequests == 0)
			{
				m_container.Find("Button_PurchaseRequest/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.premium.button.add.no.free.request");
			}
			else
			{
				m_container.Find("Button_PurchaseRequest/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("message.iap.premium.button.add.request");
			}

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
		private void OnPremiumRequestPressed()
		{
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
			UIEventController.Instance.DispatchUIEvent(IAPController.EVENT_IAP_CALL_PURCHASE_CREATE_NEW_REQUEST);
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
					string description = LanguageController.Instance.GetText("message.iap.failure.premium.request");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_REQUEST_DESTROY);
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_IAP_CALL_PURCHASE_NEW_REQUEST)
			{
				if (!(bool)_list[0])
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					string title = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.iap.failure.premium.request");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_REQUEST_DESTROY);
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_IAP_RESULT_PURCHASE_NEW_REQUEST)
			{
				if ((bool)_list[0])
				{
					string title = LanguageController.Instance.GetText("message.info");
					string description = LanguageController.Instance.GetText("message.iap.congratulations.premium.request");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_REQUEST_CONFIRMATION);
				}
				else
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					string title = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.iap.failure.premium.request");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, title, description, null, SUB_EVENT_PREMIUM_REQUEST_DESTROY);
				}
			}
			if (_nameEvent == ScreenController.EVENT_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				if (subEvent == SUB_EVENT_PREMIUM_REQUEST_CONFIRMATION)
				{
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
					UIEventController.Instance.DelayUIEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, 0.01f, (long)UsersController.Instance.CurrentUser.Id);
					Destroy();
				}
				if (subEvent == SUB_EVENT_PREMIUM_REQUEST_DESTROY)
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