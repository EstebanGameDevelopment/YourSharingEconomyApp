using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YourCommonTools;

namespace YourSharingEconomyApp
{
	/******************************************
	 * 
	 * ScreenFileSystemNavitagorView
	 * 
	 * It will show a list with the available keys
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenFileSystemNavitagorView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_NAME = "SCREEN_FILESYSTEM_NAVIGATOR";

		// ----------------------------------------------
		// PUBLIC MEMBERS
		// ----------------------------------------------
		public const string EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION = "EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------
		public const string SEARCH_IMAGES = "*.png|*.jpg|*.jpeg";

		// ----------------------------------------------
		// PUBLIC MEMBERS
		// ----------------------------------------------
		public GameObject FileItemSlot;

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------
		private GameObject m_root;
		private Transform m_container;
		private Transform m_listItems;

		private string m_currentFileSelection;
		
		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public override void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.navigator.file.system");

			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);
			m_container.Find("Button_Accept").GetComponent<Button>().onClick.AddListener(AcceptPressed);
			m_container.Find("Button_Accept/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.navigator.select.a.file");

			m_listItems = m_container.Find("ListItems");
			UpdateListItems(FileSystemController.Instance.PathLastSearch);

			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);
			BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public override bool Destroy()
		{
			if (base.Destroy()) return true;

			if (m_listItems!=null) m_listItems.GetComponent<FileManagerView>().Destroy();
			m_listItems = null;

			BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
			UIEventController.Instance.UIEvent -= OnBasicEvent;
			UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);

			return false;
		}

		// -------------------------------------------
		/* 
		 * UpdateListItems
		 */
		private void UpdateListItems(DirectoryInfo _path)
		{
			m_listItems.GetComponent<FileManagerView>().ClearCurrentGameObject(true);
			List<ItemMultiObjectEntry> items = FileSystemController.Instance.GetFileList(_path, SEARCH_IMAGES);
			m_listItems.GetComponent<FileManagerView>().Initialize(15, items, FileItemSlot);
		}

		// -------------------------------------------
		/* 
		* BackPressed
		*/
		private void BackPressed()
		{
			Destroy();
		}

		// -------------------------------------------
		/* 
		* RefreshPressed
		*/
		private void AcceptPressed()
		{
			UIEventController.Instance.DispatchUIEvent(EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION, true, m_currentFileSelection);
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnBasicSystemEvent
		 */
		private void OnBasicSystemEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == BasicSystemEventController.EVENT_BASICSYSTEMEVENT_OPEN_INFO_IMAGE_SCREEN)
			{
				MenusScreenController.Instance.CreateNewScreen(ScreenImageView.SCREEN_IMAGE, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, false, _list);
			}
			if (_nameEvent == FileItemView.EVENT_FILE_ITEM_SELECTED)
			{
				ItemMultiObjectEntry item = (ItemMultiObjectEntry)_list[0];
				m_currentFileSelection = ((FileInfo)item.Objects[1]).FullName;
			}
			if (_nameEvent == FileItemView.EVENT_FILE_FOLDER_SELECTED)
			{
				ItemMultiObjectEntry item = (ItemMultiObjectEntry)_list[0];
				UpdateListItems((DirectoryInfo)item.Objects[1]);
			}
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