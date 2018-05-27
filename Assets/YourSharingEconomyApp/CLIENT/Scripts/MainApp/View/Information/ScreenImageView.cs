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
	 * ScreenImageView
	 * 
	 * Just display an image to fullscreen
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenImageView : ScreenBaseView, IBasicScreenView
	{
		public const string SCREEN_IMAGE = "SCREEN_IMAGE";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;
		private Image m_imageContent;
		private long m_idImage;
		private byte[] m_binaryDataImage;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(params object[] _list)
		{
			m_idImage = (long)_list[0];
			m_binaryDataImage = (byte[])_list[1];

			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");
			m_container.GetComponent<Button>().onClick.AddListener(OkPressed);

			if (m_container.Find("Image") != null)
			{
				m_imageContent = m_container.Find("Image").GetComponent<Image>();
				m_imageContent.gameObject.SetActive(false);
			}

			Transform loading = m_container.Find("Loading");
			if (loading != null)
			{
				loading.GetComponent<Text>().text = LanguageController.Instance.GetText("message.loading");
			}

			if (m_binaryDataImage == null)
			{
				BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_LOAD_PRIORITY_FROM_ID, this.gameObject, m_idImage, m_imageContent, (int)m_imageContent.GetComponent<RectTransform>().sizeDelta.y, false);
			}
			else
			{
				ImageUtils.LoadBytesImage(m_imageContent, m_binaryDataImage, (int)m_imageContent.GetComponent<RectTransform>().sizeDelta.y, ScreenController.Instance.SizeHeightAllowedImages);
			}

			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
		}


		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			BasicEventController.Instance.BasicEvent -= OnBasicEvent;
			GameObject.Destroy(this.gameObject);
		}

		// -------------------------------------------
		/* 
		 * OkPressed
		 */
		private void OkPressed()
		{
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				OkPressed();
			}
		}
	}
}