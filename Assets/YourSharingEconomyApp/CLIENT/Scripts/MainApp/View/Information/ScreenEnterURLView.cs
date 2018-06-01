using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * ScreenEnterURLView
	 * 
	 * It allows the user to enter an URL
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenEnterURLView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_ENTER_URL = "SCREEN_ENTER_URL";

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

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.enter.url.title");

			m_container.Find("Button_Save").GetComponent<Button>().onClick.AddListener(OnSaveURL);
			m_container.Find("Button_Cancel").GetComponent<Button>().onClick.AddListener(OnCancelURL);
			m_container.Find("Button_Clipboard").GetComponent<Button>().onClick.AddListener(OnCopyClipboard);

#if ENABLED_FACEBOOK
        m_container.Find("Button_Clipboard").gameObject.SetActive(false);
#endif

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
		 * OnSaveURL
		 */
		private void OnSaveURL()
		{
			string urlToSave = m_container.Find("URLValue").GetComponent<InputField>().text;

			if (urlToSave.Length == 0)
			{
				string titleInfoError = LanguageController.Instance.GetText("message.error");
				string descriptionInfoError = LanguageController.Instance.GetText("screen.enter.url.empty.data");
				MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ScreenFileSystemNavitagorView.EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION, false, urlToSave);
			}
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnCancelURL
		 */
		private void OnCancelURL()
		{
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnCopyClipboard
		 */
		private void OnCopyClipboard()
		{
			m_container.Find("URLValue").GetComponent<InputField>().text = Utilities.Clipboard;
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				OnCancelURL();
			}
		}
	}
}