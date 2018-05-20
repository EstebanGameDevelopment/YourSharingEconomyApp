﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

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
	public class ScreenFileSystemNavitagorView : ScreenBaseView, IBasicScreenView
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
		public void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.navigator.file.system");

			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);
			m_container.Find("Button_Accept").GetComponent<Button>().onClick.AddListener(AcceptPressed);
			m_container.Find("Button_Accept/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.navigator.select.a.file");

			m_listItems = m_container.Find("ListItems");
			UpdateListItems(FileSystemController.Instance.PathLastSearch);

			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			if (m_listItems!=null) m_listItems.GetComponent<FileManagerView>().Destroy();
			m_listItems = null;

			BasicEventController.Instance.BasicEvent -= OnBasicEvent;
			GameObject.Destroy(this.gameObject);
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
			BasicEventController.Instance.DispatchBasicEvent(EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION, true, m_currentFileSelection);
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
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
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				Destroy();
			}
		}
	}
}