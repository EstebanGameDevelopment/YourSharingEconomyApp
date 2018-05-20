﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NBitcoin;
using UnityEngine;
using UnityEngine.UI;
using YourBitcoinController;

namespace YourBitcoinManager
{
	/******************************************
	 * 
	 * ScreenBitcoinAddFundsKeyView
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenBitcoinAddFundsKeyView : ScreenBaseView, IBasicScreenView
	{
		public const string SCREEN_NAME = "SCREEN_ADD_FUNDS";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private string SUBEVENT_CONFIRMATION_OPEN_URL_TO_ADD_BITCOINS = "SUBEVENT_CONFIRMATION_OPEN_URL_TO_ADD_BITCOINS";
		private string SUBEVENT_CONFIRMATION_OPEN_URL_BITCOINS_TO_PAYPAL = "SUBEVENT_CONFIRMATION_OPEN_URL_BITCOINS_TO_PAYPAL";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;

		private string m_publicKey = "";

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(params object[] _list)
		{
			m_publicKey = (string)_list[0];

			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(OnBackButton);

			m_container.Find("Enter").GetComponent<Button>().onClick.AddListener(OnEnterBitcoins);
			m_container.Find("Enter/Text").GetComponent<Text>().text = YourSharingEconomyApp.LanguageController.Instance.GetText("screen.bitcoin.enter.bitcoins");

			m_container.Find("Withdraw").GetComponent<Button>().onClick.AddListener(OnWithdrawBitcoins);
			m_container.Find("Withdraw/Text").GetComponent<Text>().text = YourSharingEconomyApp.LanguageController.Instance.GetText("screen.bitcoin.withdraw.bitcoins");

			m_container.Find("PublicKeyLabel").GetComponent<Text>().text = YourSharingEconomyApp.LanguageController.Instance.GetText("screen.bitcoin.copy.paste.public.address");
			m_container.Find("PublicKeyInput").GetComponent<InputField>().text = BitCoinController.Instance.CurrentPublicKey;

			BitcoinManagerEventController.Instance.BitcoinManagerEvent += new BitcoinManagerEventHandler(OnBasicEvent);

			m_container.Find("Network").GetComponent<Text>().text = YourSharingEconomyApp.LanguageController.Instance.GetText("text.network") + BitCoinController.Instance.Network.ToString();
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public override bool Destroy()
		{
			if (base.Destroy()) return true;

			BitcoinManagerEventController.Instance.BitcoinManagerEvent -= OnBasicEvent;
			BitcoinManagerEventController.Instance.DispatchBasicEvent(ScreenBitcoinController.EVENT_SCREENMANAGER_DESTROY_OVERLAY_SCREEN, this.gameObject);

			return false;
		}


		// -------------------------------------------
		/* 
		 * OnBackButton
		 */
		private void OnBackButton()
		{
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnEnterBitcoins
		 */
		private void OnEnterBitcoins()
		{
			Utilities.Clipboard = m_publicKey;
			string title = YourSharingEconomyApp.LanguageController.Instance.GetText("message.info");
			string description = YourSharingEconomyApp.LanguageController.Instance.GetText("screen.bitcoin.copied.public.key.clipboard");
			ScreenBitcoinController.Instance.CreateNewInformationScreen(ScreenBitcoinInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, title, description, null, SUBEVENT_CONFIRMATION_OPEN_URL_TO_ADD_BITCOINS);
		}

		// -------------------------------------------
		/* 
		 * OnWithdrawBitcoins
		 */
		private void OnWithdrawBitcoins()
		{
			string title = YourSharingEconomyApp.LanguageController.Instance.GetText("message.info");
			List<PageInformation> pages = new List<PageInformation>();
			pages.Add(new PageInformation(title, YourSharingEconomyApp.LanguageController.Instance.GetText("screen.bitcoin.choose.your.own.method.bitcoins.to.paypal"), null, ""));
			if (BitCoinController.Instance.IsMainNetwork)
			{
				pages.Add(new PageInformation(title, YourSharingEconomyApp.LanguageController.Instance.GetText("screen.bitcoin.choose.your.own.method.bitcoins.to.paypal.2"), null, SUBEVENT_CONFIRMATION_OPEN_URL_BITCOINS_TO_PAYPAL));
			}
			else
			{
				pages.Add(new PageInformation(title, YourSharingEconomyApp.LanguageController.Instance.GetText("screen.bitcoin.choose.your.own.method.bitcoins.to.paypal.2"), null, ""));
				pages.Add(new PageInformation(title, YourSharingEconomyApp.LanguageController.Instance.GetText("screen.bitcoin.choose.your.own.method.bitcoins.to.paypal.3"), null, SUBEVENT_CONFIRMATION_OPEN_URL_BITCOINS_TO_PAYPAL));
			}
			ScreenBitcoinController.Instance.CreateNewInformationScreen(ScreenBitcoinInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, pages);			
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (!this.gameObject.activeSelf) return;

			if (_nameEvent == ScreenBitcoinInformationView.EVENT_SCREENINFORMATION_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				if (subEvent == SUBEVENT_CONFIRMATION_OPEN_URL_TO_ADD_BITCOINS)
				{
					if (BitCoinController.Instance.IsMainNetwork)
					{
						Application.OpenURL("https://buy.blockexplorer.com/");
					}
					else
					{
						Application.OpenURL("https://testnet.manu.backend.hamburg/faucet");
					}
				}
				if (subEvent == SUBEVENT_CONFIRMATION_OPEN_URL_BITCOINS_TO_PAYPAL)
				{
					Application.OpenURL("https://www.yourvrexperience.com/?page_id=4247");
				}				
			}
			if (_nameEvent == ScreenBitcoinController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				OnBackButton();
			}
		}
	}
}