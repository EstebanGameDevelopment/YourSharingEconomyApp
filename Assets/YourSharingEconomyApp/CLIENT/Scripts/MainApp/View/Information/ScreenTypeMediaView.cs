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
	 * ScreenTypeMediaView
	 * 
	 * It allows the users to select between select a file or enter an URL for the reference images
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenTypeMediaView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_TYPE_MEDIA = "SCREEN_TYPE_MEDIA";

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

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.type.media.title");

			m_container.Find("Button_AddImage").GetComponent<Button>().onClick.AddListener(OnAddTypeImage);
			m_container.Find("Button_AddImage/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.type.media.add.image");
			m_container.Find("Button_AddLink").GetComponent<Button>().onClick.AddListener(OnAddTypeLink);
			m_container.Find("Button_AddLink/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.type.media.add.url");

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
		 * OnAddTypeImage
		 */
		private void OnAddTypeImage()
		{
			bool displayFilebrowser = true;
#if ENABLED_FACEBOOK
        displayFilebrowser = false;
#endif

			if (displayFilebrowser)
			{
				MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenFileSystemNavitagorView.SCREEN_NAME, false, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN);
				Destroy();
			}
			else
			{
				string titleInfoError = LanguageController.Instance.GetText("message.error");
				string descriptionInfoError = LanguageController.Instance.GetText("screen.media.type.no.filesystem.available");
				MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
			}
		}

		// -------------------------------------------
		/* 
		 * OnAddTypeLink
		 */
		private void OnAddTypeLink()
		{
			MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenEnterURLView.SCREEN_ENTER_URL, false, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN);
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				Destroy();
			}
		}
	}
}