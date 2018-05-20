﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * FileItemView
	 * 
	 * Slot that will be used in all the lists of the system 
	 * 
	 * @author Esteban Gallardo
	 */
	public class FileItemView : Button
	{
		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_FILE_ITEM_SELECTED	= "EVENT_FILE_ITEM_SELECTED";
		public const string EVENT_FILE_FOLDER_SELECTED	= "EVENT_FILE_FOLDER_SELECTED";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private Transform m_container;
		private GameObject m_selectedBackground;
		private ItemMultiObjectEntry m_data;
		private bool m_isSelectable = false;
		private GameObject[] m_icons;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(params object[] _list)
		{
			m_data = (ItemMultiObjectEntry)_list[0];

			m_container = this.gameObject.transform;
			m_icons = new GameObject[4];
			m_icons[FileSystemController.ITEM_BACK] = m_container.Find("Icons/BACK").gameObject;
			m_icons[FileSystemController.ITEM_DRIVE] = m_container.Find("Icons/DRIVE").gameObject;
			m_icons[FileSystemController.ITEM_FOLDER] = m_container.Find("Icons/FOLDER").gameObject;
			m_icons[FileSystemController.ITEM_FILE] = m_container.Find("Icons/FILE").gameObject;
			for (int i = 0; i < m_icons.Length; i++)
			{
				m_icons[i].SetActive(false);
			}

			int type = (int)(m_data.Objects[0]);
			string nameItem = "";
			switch (type)
			{
				case FileSystemController.ITEM_BACK:
					m_isSelectable = false;
					nameItem = ((DirectoryInfo)m_data.Objects[1]).Name;
					m_icons[FileSystemController.ITEM_BACK].SetActive(true);
					break;

				case FileSystemController.ITEM_DRIVE:
					m_isSelectable = false;
					nameItem = ((DirectoryInfo)m_data.Objects[1]).Name;
					m_icons[FileSystemController.ITEM_DRIVE].SetActive(true);
					break;

				case FileSystemController.ITEM_FOLDER:
					m_isSelectable = false;
					nameItem = ((DirectoryInfo)m_data.Objects[1]).Name;
					m_icons[FileSystemController.ITEM_FOLDER].SetActive(true);
					break;

				case FileSystemController.ITEM_FILE:
					m_isSelectable = true;
					nameItem = ((FileInfo)m_data.Objects[1]).Name;
					m_icons[FileSystemController.ITEM_FILE].SetActive(true);
					break;
			}

			m_container.Find("Name").GetComponent<Text>().text = nameItem;
			m_selectedBackground = m_container.Find("Selected").gameObject;
			m_selectedBackground.SetActive(false);

			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			GameObject.Destroy(this.gameObject);
			BasicEventController.Instance.BasicEvent -= OnBasicEvent;
		}

		// -------------------------------------------
		/* 
		 * OnPointerClick
		 */
		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			if (m_isSelectable)
			{
				BasicEventController.Instance.DispatchBasicEvent(EVENT_FILE_ITEM_SELECTED, m_data);
			}
			else
			{
				BasicEventController.Instance.DispatchBasicEvent(EVENT_FILE_FOLDER_SELECTED, m_data);
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == EVENT_FILE_ITEM_SELECTED)
			{
				m_selectedBackground.SetActive((m_data == (ItemMultiObjectEntry)_list[0]));
				if (m_selectedBackground.activeSelf)
				{
					string pathToFile = ((FileInfo)m_data.Objects[1]).FullName;
					Texture2D loadedTexture = ImageUtils.LoadTexture2D(pathToFile, 600);
					byte[] dataImage = loadedTexture.EncodeToJPG(75);
					ScreenController.Instance.CreateNewScreen(ScreenImageView.SCREEN_IMAGE, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, false, (long)-1, dataImage);
				}				
			}
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				Destroy();
			}
		}
	}
}