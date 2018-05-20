using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine.UI;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * ImagesController
	 * 
	 * It manages all the images of the system. Specially critical to load images in the background.
	 * 
	 * @author Esteban Gallardo
	 */
	public class ImagesController : MonoBehaviour
	{
		// ----------------------------------------------
		// CONSTANTS
		// ----------------------------------------------
		public const int LIMIT_IMAGES_IN_MEMORY_TO_CLEAR = 20;

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------
		public const string EVENT_IMAGES_LOAD_FROM_SYSTEM_IMAGE = "EVENT_IMAGES_LOAD_FROM_SYSTEM_IMAGE";
		public const string EVENT_IMAGES_LOAD_CONFIRMATION_FROM_SYSTEM = "EVENT_IMAGES_LOAD_CONFIRMATION_FROM_SYSTEM";
		public const string EVENT_IMAGES_LOAD_FROM_BYTE_ARRAY = "EVENT_IMAGES_LOAD_FROM_BYTE_ARRAY";

		public const string EVENT_IMAGES_LOAD_FROM_ID = "EVENT_IMAGES_LOAD_FROM_ID";
		public const string EVENT_IMAGES_LOAD_PRIORITY_FROM_ID = "EVENT_IMAGES_LOAD_PRIORITY_FROM_ID";
		public const string EVENT_IMAGES_LOAD_SERVER_DATA_RECEIVED = "EVENT_IMAGES_LOAD_SERVER_DATA_RECEIVED";
		public const string EVENT_IMAGES_LOAD_SERVER_LOCAL_DATA_LOADED = "EVENT_IMAGES_LOAD_SERVER_LOCAL_DATA_LOADED";
		public const string EVENT_IMAGES_LOAD_TOXIC_IMAGE = "EVENT_IMAGES_LOAD_TOXIC_IMAGE";

		public const string EVENT_IMAGES_UPLOAD_TO_SERVER_NEW_IMAGE = "EVENT_IMAGES_UPLOAD_TO_SERVER_NEW_IMAGE";
		public const string EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION = "EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION";

		public const string EVENT_IMAGES_CALL_DELETE_IMAGE = "EVENT_IMAGES_CALL_DELETE_IMAGE";
		public const string EVENT_IMAGE_REMOVED_SERVER_CONFIRMATION = "EVENT_IMAGE_REMOVED_SERVER_CONFIRMATION";

		public const string EVENT_IMAGES_SELECTED_THUMBNAIL_REFERENCE = "EVENT_IMAGES_SELECTED_THUMBNAIL_REFERENCE";
		public const string EVENT_IMAGE_LOADED_REPORT_SYSTEM = "EVENT_IMAGE_LOADED_REPORT_SYSTEM";

		public const string EVENT_IMAGES_CANCEL_LOADING_IMAGES = "EVENT_IMAGES_CANCEL_LOADING_IMAGES";

		public const string EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_URL = "EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_URL";
		public const string EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_IMAGE_URL = "EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_IMAGE_URL";

		public const string EVENT_IMAGES_LOAD_THUMBNAIL_FILE_BROWSER = "EVENT_IMAGES_LOAD_THUMBNAIL_FILE_BROWSER";
		public const string EVENT_IMAGES_CLEAR_THUMBNAIL_CACHE = "EVENT_IMAGES_CLEAR_THUMBNAIL_CACHE";

		// ----------------------------------------------
		// SINGLETON
		// ----------------------------------------------
		private static ImagesController _instance;

		public static ImagesController Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType(typeof(ImagesController)) as ImagesController;
					if (!_instance)
					{
						GameObject container = new GameObject();
						container.name = "ImagesController";
						_instance = container.AddComponent(typeof(ImagesController)) as ImagesController;
					}
				}
				return _instance;
			}
		}

		// ----------------------------------------------
		// MEMBERS
		// ----------------------------------------------
		private List<ImageModel> m_images = new List<ImageModel>();
		private ImageModel m_imageTemp;

		private List<ImageRequestedInfo> m_queuedRequests = new List<ImageRequestedInfo>();
		private List<ThumbnailRequestedInfo> m_thumbnailRequests = new List<ThumbnailRequestedInfo>();

		private ImageRequestedInfo m_imageReferenceTmp = null;

		private float m_timeAcumImages = 0;

		private float m_timeAcumThumbnail = 0;
		private int m_counterThumnail = 0;
		private int m_heightThumbnail = 0;

		private string m_pathLastFileBrowsed = "";
		private float m_lastScrollPosition = 0;

		// ----------------------------------------------
		// GETTERS/SETTERS
		// ----------------------------------------------

		public string PathLastFileBrowsed
		{
			get { return m_pathLastFileBrowsed; }
		}
		public float LastScrollPosition
		{
			get { return m_lastScrollPosition; }
		}

		// ----------------------------------------------
		// CONSTRUCTOR
		// ----------------------------------------------	
		// -------------------------------------------
		/* 
		 * Constructor
		 */
		private ImagesController()
		{
		}

		// -------------------------------------------
		/* 
		 * Initialitzation
		 */
		public void Init()
		{
			m_heightThumbnail = (int)((ThumbnailRequestedInfo.DEFAULT_ICON_HEIGHT / ThumbnailRequestedInfo.DEFAULT_SCREEN_HEIGHT) * Screen.height);
			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			BasicEventController.Instance.BasicEvent -= OnBasicEvent;
			Destroy(_instance.gameObject);
			_instance = null;
		}

		// -------------------------------------------
		/* 
		 * GetImageByID
		 */
		public ImageModel GetImageByID(long _id)
		{
			for (int i = 0; i < m_images.Count; i++)
			{
				if (m_images[i].Id == _id)
				{
					return m_images[i];
				}
			}
			return null;
		}

		// -------------------------------------------
		/* 
		 * RemoveImageByID
		 */
		private bool RemoveImageByID(long _id)
		{
			for (int i = 0; i < m_images.Count; i++)
			{
				if (m_images[i].Id == _id)
				{
					m_images.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		// -------------------------------------------
		/* 
		 * GetImageByOrigin
		 */
		public List<ImageModel> GetImageByOrigin(long _idOrigin, string _table)
		{
			List<ImageModel> imagesOrigin = new List<ImageModel>();
			for (int i = 0; i < m_images.Count; i++)
			{
				if ((m_images[i].IdOrigin == _idOrigin) && (m_images[i].Table == _table))
				{
					imagesOrigin.Add(m_images[i]);					
				}
			}
			return imagesOrigin;
		}

		// -------------------------------------------
		/* 
		 * RemoveImageByOrigin
		 */
		private bool RemoveImageByOrigin(long _idOrigin, string _table)
		{
			for (int i = 0; i < m_images.Count; i++)
			{
				if ((m_images[i].IdOrigin == _idOrigin) && (m_images[i].Table == _table))
				{
					m_images.RemoveAt(i);
					i--;
				}
			}
			return false;
		}

		// -------------------------------------------
		/* 
		 * LoadNewImage
		 */
		private bool LoadNewImage(GameObject _origin, long _id, Image _image, int _height, bool _showLoadingMessage)
		{
			m_imageReferenceTmp = new ImageRequestedInfo(_origin, _id, _image, _height, _showLoadingMessage);

			ImageModel imageModel = GetImageByID(m_imageReferenceTmp.Id);
			if (imageModel != null)
			{
				BasicEventController.Instance.DispatchBasicEvent(EVENT_IMAGE_LOADED_REPORT_SYSTEM, _image);
				try
				{
					ImageUtils.LoadBytesImage(m_imageReferenceTmp.Image, imageModel.Data, m_imageReferenceTmp.Height, ScreenController.Instance.SizeHeightAllowedImages);
				}
				catch (Exception err)
				{
					if (ScreenController.Instance.DebugMode) Debug.Log(err.StackTrace);
				};
				BasicEventController.Instance.DispatchBasicEvent(EVENT_IMAGES_LOAD_SERVER_LOCAL_DATA_LOADED);
				return true;
			}
			else
			{
				if (_showLoadingMessage)
				{
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
				}
				CommController.Instance.LoadImage(m_imageReferenceTmp.Id);
			}

			if (m_images.Count > LIMIT_IMAGES_IN_MEMORY_TO_CLEAR)
			{
				m_images.Clear();
			}
			return false;
		}


		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == EVENT_IMAGES_LOAD_FROM_SYSTEM_IMAGE)
			{
				BasicEventController.Instance.DispatchBasicEvent(EVENT_IMAGES_CLEAR_THUMBNAIL_CACHE);
				string fullPath = (string)_list[0];
				m_lastScrollPosition = (float)_list[1];
				if (fullPath.LastIndexOf('/') != -1)
				{
					m_pathLastFileBrowsed = fullPath.Substring(0, fullPath.LastIndexOf('/'));
				}
				if (fullPath.LastIndexOf('\\') != -1)
				{
					m_pathLastFileBrowsed = fullPath.Substring(0, fullPath.LastIndexOf('\\'));
				}
				Image imageContainer = (Image)_list[2];
				int heightTarget = (int)_list[3];
				int typeTarget = (int)_list[4];
				ImageUtils.LoadImage(fullPath, imageContainer, heightTarget, ScreenController.Instance.SizeHeightAllowedImages);
				if (typeTarget != RequestModel.IMAGE_TYPE_FINISHED)
				{
					BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				}
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_URL)
			{
				string pathFileURL = (string)_list[0];
				Image imgContainer = (Image)_list[1];
				int heightImage = (int)_list[2];
				byte[] dataImage = (byte[])_list[3];
				ImageUtils.LoadBytesImage(imgContainer, dataImage, heightImage, ScreenController.Instance.SizeHeightAllowedImages);
				BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_LOAD_CONFIRMATION_FROM_SYSTEM, pathFileURL);
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_IMAGE_URL)
			{
				string pathFileURL = (string)_list[0];
				Image imgContainer = (Image)_list[1];
				int heightImage = (int)_list[2];
				ImageURLLoader.Instance.LoadURLImage(pathFileURL, imgContainer, heightImage, ScreenController.Instance.SizeHeightAllowedImages);
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_FROM_ID)
			{
				m_timeAcumImages = ((GetImageByID((long)_list[1]) != null) ? 1 : 0);
				m_queuedRequests.Add(new ImageRequestedInfo((GameObject)_list[0], (long)_list[1], (Image)_list[2], (int)_list[3], (bool)_list[4]));
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_TOXIC_IMAGE)
			{
				Image imgContainer = (Image)_list[0];
				int heightImage = (int)_list[1];
				byte[] dataImage = (byte[])_list[2];
				ImageUtils.LoadBytesImage(imgContainer, dataImage, heightImage, ScreenController.Instance.SizeHeightAllowedImages);
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_PRIORITY_FROM_ID)
			{
				m_timeAcumImages = ((GetImageByID((long)_list[1]) != null) ? 1 : 0);
				m_queuedRequests.Insert(0, new ImageRequestedInfo((GameObject)_list[0], (long)_list[1], (Image)_list[2], (int)_list[3], (bool)_list[4]));
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_FROM_BYTE_ARRAY)
			{
				ImageUtils.LoadBytesImage((Image)_list[1], (byte[])_list[0], (int)_list[2], ScreenController.Instance.SizeHeightAllowedImages);
			}
			if (_nameEvent == EVENT_IMAGES_UPLOAD_TO_SERVER_NEW_IMAGE)
			{
				byte[] dataImg = (byte[])_list[4];

				m_imageTemp = new ImageModel();
				m_imageTemp.Table = (string)_list[1];
				m_imageTemp.IdOrigin = (long)_list[2];
				m_imageTemp.Type = (int)_list[3];
				m_imageTemp.Size = dataImg.Length;
				m_imageTemp.CopyData(dataImg);
				m_imageTemp.Url = (string)_list[5];

				CommController.Instance.UploadImage((long)_list[0], m_imageTemp.Table, m_imageTemp.IdOrigin, m_imageTemp.Type, dataImg, m_imageTemp.Url);
			}
			if (_nameEvent == EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION)
			{
				if (_list.Length > 0)
				{
					if ((bool)_list[0])
					{
						if (m_imageTemp != null)
						{
							m_imageTemp.Id = (long)_list[1];
							m_imageTemp.Table = (string)_list[2];
							m_imageTemp.IdOrigin = (long)_list[3];
							m_images.Add(m_imageTemp.Clone());
							m_imageTemp = null;
						}
					}
				}
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_SERVER_DATA_RECEIVED)
			{
				if (m_imageReferenceTmp != null)
				{
					if ((bool)_list[0])
					{
						ImageModel imageNew = new ImageModel();
						imageNew.Id = (long)_list[1];
						imageNew.Table = (string)_list[2];
						imageNew.IdOrigin = (long)_list[3];
						imageNew.Type = (int)_list[4];
						imageNew.Url = (string)_list[5];
						imageNew.Size = (int)_list[6];
						imageNew.CopyData((byte[])_list[7]);
						m_images.Add(imageNew);

						ImageModel imageModel = GetImageByID(imageNew.Id);
						if (imageModel != null)
						{
							if (m_imageReferenceTmp != null)
							{
								if (m_imageReferenceTmp.Image != null)
								{
									ImageUtils.LoadBytesImage(m_imageReferenceTmp.Image, imageModel.Data, m_imageReferenceTmp.Height, ScreenController.Instance.SizeHeightAllowedImages);
									BasicEventController.Instance.DispatchBasicEvent(EVENT_IMAGE_LOADED_REPORT_SYSTEM, m_imageReferenceTmp.Image, imageNew.Url);
								}
							}
						}
					}
				}
				m_imageReferenceTmp = null;
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_SERVER_LOCAL_DATA_LOADED)
			{
				m_imageReferenceTmp = null;
			}
			if (_nameEvent == EVENT_IMAGES_CALL_DELETE_IMAGE)
			{
				long idImage = (long)_list[1];
				if (idImage != -1)
				{
					RemoveImageByID(idImage);
					CommController.Instance.RemoveImage(idImage);
				}
			}
			if (_nameEvent == EVENT_IMAGES_CANCEL_LOADING_IMAGES)
			{
				m_queuedRequests.Clear();
				// m_imageReferenceTmp = null;
			}
			if (_nameEvent == RequestsController.EVENT_REQUEST_CALL_DELETE_RECORDS)
			{
				m_images.Clear();
				m_queuedRequests.Clear();
				m_imageReferenceTmp = null;
			}
			if (_nameEvent == EVENT_IMAGES_LOAD_THUMBNAIL_FILE_BROWSER)
			{
				if (m_thumbnailRequests.Count < 50)
				{
					m_thumbnailRequests.Add(new ThumbnailRequestedInfo((GUIContent)_list[0], (string)_list[1], m_heightThumbnail));
				}
			}
			if (_nameEvent == EVENT_IMAGES_CLEAR_THUMBNAIL_CACHE)
			{
				for (int i = 0; i < m_thumbnailRequests.Count; i++)
				{
					m_thumbnailRequests[i].Destroy();
				}
				m_thumbnailRequests.Clear();
				m_counterThumnail = 0;
				m_timeAcumImages = 0;
			}
		}

		// -------------------------------------------
		/* 
		 * Process other images requests
		 */
		void Update()
		{
			// LOAD IMAGES
			if (m_imageReferenceTmp == null)
			{
				m_timeAcumImages += Time.deltaTime;
				if (m_timeAcumImages > 1)
				{
					m_timeAcumImages = 0;
					if (m_queuedRequests.Count > 0)
					{
						ImageRequestedInfo reqImage = m_queuedRequests[0];
						m_queuedRequests.RemoveAt(0);
						if (LoadNewImage(reqImage.Origin, reqImage.Id, reqImage.Image, reqImage.Height, reqImage.ShowLoadingMessage))
						{
							m_timeAcumImages = 0.8f;
						}
					}
				}
			}

			// LOAD THUMNAILS
			if (m_thumbnailRequests.Count > 0)
			{
				m_timeAcumThumbnail += Time.deltaTime;
				if (m_timeAcumImages > 0.2)
				{
					m_timeAcumImages = 0;
					if (m_counterThumnail < m_thumbnailRequests.Count)
					{
						m_thumbnailRequests[m_counterThumnail].LoadTexture();
						m_counterThumnail++;
					}
				}
			}
		}
	}

}