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
	 * SlotImageView
	 * 
	 * Loads and displays an image. Also it has
	 * the behavior to commit the data to the server's database.
	 * 
	 * @author Esteban Gallardo
	 */
	public class SlotImageView : MonoBehaviour
	{
		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_IMAGESLOT_CONFIRMATION_DELETE = "SUB_EVENT_IMAGESLOT_CONFIRMATION_DELETE";
		public const string SUB_EVENT_IMAGESLOT_CONFIRMATION_SEE_TOXIC_CONTENT = "SUB_EVENT_IMAGESLOT_CONFIRMATION_SEE_TOXIC_CONTENT";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private long m_originId;
		private long m_id;
		private Transform m_container;
		private Image m_background;
		private Image m_image;
		private int m_type;
		private string m_url = "";
		private bool m_selected = false;
		private bool m_isDisplayOnly = false;
		private bool m_isCurrentUser = true;
		private bool m_imageLoaded = false;

		private bool m_requestedToDelete = false;
		private bool m_isUploadingFinalImage = false;

		private GameObject m_selectedIcon = null;
		private GameObject m_unSelectedIcon = null;
		private GameObject m_loadingText = null;

		private Transform m_buttonDelete;
		private Transform m_buttonReference;

		private string m_pathFile = "";

		private bool m_isPossibleImageURL = false;
		private Sprite m_urlImage;

		private bool m_isFlagged = false;
		private bool m_isBanned = false;
		private bool m_requestedToSeeFlagged = false;

		public long Id
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public bool Selected
		{
			get { return m_selected; }
			set
			{
				m_selected = value;
				m_selectedIcon.SetActive(m_selected);
				m_unSelectedIcon.SetActive(!m_selected);
			}
		}

		public bool DisplayStar
		{
			set { m_buttonReference.gameObject.SetActive(value); }
		}

		public bool ImageLoaded
		{
			get { return m_imageLoaded; }
		}


		// -------------------------------------------
		/* 
		 * Constructor
		 */
		private void InitCommon(long _originId, long _idImage, bool _selected, bool _isCurrentUser, int _type)
		{
			m_id = _idImage;
			m_originId = _originId;
			m_background = this.gameObject.GetComponent<Image>();
			m_container = this.gameObject.transform.Find("Dress");
			m_image = m_container.GetComponent<Image>();
			m_type = _type;

			m_buttonDelete = this.gameObject.transform.Find("Button_Delete");
			m_buttonReference = this.gameObject.transform.Find("Button_Reference");
			m_buttonDelete.GetComponent<Button>().onClick.AddListener(OnDeleteImage);
			m_buttonReference.GetComponent<Button>().onClick.AddListener(OnSelectedImage);

			this.gameObject.GetComponent<Button>().onClick.AddListener(OnClickOnPicture);

			if (m_type == RequestModel.IMAGE_TYPE_FINISHED)
			{
				this.gameObject.transform.Find("Button_Delete").gameObject.SetActive(false);
				this.gameObject.transform.Find("Button_Reference").gameObject.SetActive(false);
			}

			m_selectedIcon = this.gameObject.transform.Find("Button_Reference/Selected").gameObject;
			m_unSelectedIcon = this.gameObject.transform.Find("Button_Reference/Unselected").gameObject;
			Selected = _selected;
			m_isCurrentUser = _isCurrentUser;

			m_loadingText = this.gameObject.transform.Find("Loading").gameObject;
			m_loadingText.GetComponent<Text>().text = LanguageController.Instance.GetText("message.loading");

			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);
			BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			m_urlImage = null;
			UIEventController.Instance.UIEvent -= OnBasicEvent;
			BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
			GameObject.Destroy(this.gameObject);
		}

		// -------------------------------------------
		/* 
		 * Initialize
		 */
		public void Initialize(long _originId, int _idImage, string _pathFile, bool _selected, bool _isCurrentUser, int _type, float _scrollPosition)
		{
			InitCommon(_originId, _idImage, _selected, _isCurrentUser, _type);
			m_pathFile = _pathFile;
			m_url = "";

			UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_FROM_SYSTEM_IMAGE, _pathFile, _scrollPosition, m_image, (int)m_background.GetComponent<RectTransform>().sizeDelta.y, m_type);
		}

		// -------------------------------------------
		/* 
		 * InitializeWithURL
		 */
		public void InitializeWithURL(long _originId, int _idImage, Sprite _urlImage, bool _selected, bool _isCurrentUser, int _type, string _url)
		{
			InitCommon(_originId, _idImage, _selected, _isCurrentUser, _type);
			m_pathFile = _url;
			m_url = _url;
			m_urlImage = _urlImage;

			if ((m_url.IndexOf(".png") != -1) || (m_url.IndexOf(".jpg") != -1) || (m_url.IndexOf(".jpeg") != -1) ||
				(m_url.IndexOf(".PNG") != -1) || (m_url.IndexOf(".JPG") != -1) || (m_url.IndexOf(".JPEG") != -1))
			{
				m_url = "";
				m_isPossibleImageURL = true;
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_IMAGE_URL, m_pathFile, m_image, (int)m_background.GetComponent<RectTransform>().sizeDelta.y, ImageUtils.GetBytesJPG(_urlImage));
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_URL, m_pathFile, m_image, (int)m_background.GetComponent<RectTransform>().sizeDelta.y, ImageUtils.GetBytesJPG(_urlImage), m_url);
			}
		}

		// -------------------------------------------
		/* 
		 * InitializeFromServerData
		 */
		public void InitializeFromServerData(long _originId, long _idImage, bool _isDisplayOnly, bool _selected, bool _isCurrentUser, int _type, string _url, bool _isFlagged, bool _isBanned)
		{
			InitCommon(_originId, _idImage, _selected, _isCurrentUser, _type);
			m_url = _url;
			m_isFlagged = _isFlagged;
			m_isBanned = _isBanned;

			m_isDisplayOnly = _isDisplayOnly;
			if (m_isDisplayOnly)
			{
				this.gameObject.transform.Find("Button_Delete").gameObject.SetActive(false);
			}

			if (m_isFlagged || m_isBanned)
			{
				if (m_isBanned)
				{
					UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_TOXIC_IMAGE, m_image, (int)(m_background.GetComponent<RectTransform>().sizeDelta.y), ImageUtils.GetBytesJPG(MenusScreenController.Instance.ImageToxicConfirmed));
				}
				else
				{
					UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_TOXIC_IMAGE, m_image, (int)(m_background.GetComponent<RectTransform>().sizeDelta.y), ImageUtils.GetBytesJPG(MenusScreenController.Instance.ImageToxicPossible));
				}
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_FROM_ID, this.gameObject, _idImage, m_image, (int)m_background.GetComponent<RectTransform>().sizeDelta.y, false);
			}
		}

		// -------------------------------------------
		/* 
		 * OnDeleteImage
		 */
		public void OnDeleteImage()
		{
			m_requestedToDelete = true;
			string warning = LanguageController.Instance.GetText("message.warning");
			string description = LanguageController.Instance.GetText("message.slot.image.delete.confirmation");
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_IMAGESLOT_CONFIRMATION_DELETE);
		}

		// -------------------------------------------
		/* 
		 * OnSelectedImage
		 */
		public void OnSelectedImage()
		{
			if (m_isCurrentUser)
			{
				Selected = true;
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_SELECTED_THUMBNAIL_REFERENCE, this.gameObject, m_id, m_isDisplayOnly);
			}
		}

		// -------------------------------------------
		/* 
		 * OnClickOnPicture
		 */
		public void OnClickOnPicture()
		{
			if (m_isFlagged || m_isBanned)
			{
				if (m_isBanned)
				{
					string warning = LanguageController.Instance.GetText("message.warning");
					string description = LanguageController.Instance.GetText("message.show.nothing.of.user.banned");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, "");
				}
				else
				{
					string warning = LanguageController.Instance.GetText("message.warning");
					string description = LanguageController.Instance.GetText("message.request.declared.toxic");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_IMAGESLOT_CONFIRMATION_SEE_TOXIC_CONTENT);
					m_requestedToSeeFlagged = true;
				}
			}
			else
			{
				if (m_url.Length > 0)
				{
					Application.OpenURL(m_url);
				}
				else
				{
					MenusScreenController.Instance.CreateNewScreen(ScreenImageView.SCREEN_IMAGE, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, false, m_id, GetByteImageJPG());
				}
			}
		}

		// -------------------------------------------
		/* 
		 * GetByteImagePNG
		 */
		private byte[] GetByteImagePNG()
		{
			if (!m_imageLoaded)
			{
				return null;
			}
			else
			{
				return ImageUtils.GetBytesPNG(m_image);
			}
		}

		// -------------------------------------------
		/* 
		 * GetByteImageJPG
		 */
		private byte[] GetByteImageJPG()
		{
			if (!m_imageLoaded)
			{
				return null;
			}
			else
			{
				return ImageUtils.GetBytesJPG(m_image);
			}
		}

		// -------------------------------------------
		/* 
		 * GetByteImageJPG
		 */
		public void UploadImages(string _table, long _originId, int _indexUpload)
		{
			if (m_id == -1)
			{
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_NEW_IMAGE, m_id, _table, _originId, m_type, GetByteImageJPG(), m_url);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION);
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicSystemEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == ImageUtils.EVENT_IMAGES_LOAD_CONFIRMATION_FROM_SYSTEM)
			{
				string pathFile = (string)_list[0];
				if (m_pathFile == pathFile)
				{
					m_imageLoaded = true;
					if (_list.Length == 3)
					{
						int widthTexture = (int)_list[1];
						int heightTexture = (int)_list[2];
						if (m_isPossibleImageURL)
						{
							m_isPossibleImageURL = false;
							if ((widthTexture < 100) || (heightTexture < 100))
							{
								m_url = m_pathFile;
								UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_REFERENCE_IMG_WITH_URL, m_pathFile, m_image, (int)m_background.GetComponent<RectTransform>().sizeDelta.y, ImageUtils.GetBytesJPG(m_urlImage), m_url);
							}
						}
					}
					if (m_type == RequestModel.IMAGE_TYPE_FINISHED)
					{
						m_isUploadingFinalImage = true;
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
						UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_NEW_IMAGE, m_id, MenusScreenController.TABLE_REQUESTS, m_originId, m_type, GetByteImageJPG(), m_url);
					}
				}
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == ScreenController.EVENT_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				if (subEvent == SUB_EVENT_IMAGESLOT_CONFIRMATION_DELETE)
				{
					if (m_requestedToDelete)
					{
						m_requestedToDelete = false;
						UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
						if ((bool)_list[1])
						{
							UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_CALL_DELETE_IMAGE, this.gameObject, m_id);
						}
					}
				}
				if (subEvent == SUB_EVENT_IMAGESLOT_CONFIRMATION_SEE_TOXIC_CONTENT)
				{
					if (m_requestedToSeeFlagged)
					{
						m_requestedToSeeFlagged = false;
						if ((bool)_list[1])
						{
							MenusScreenController.Instance.CreateNewScreen(ScreenImageView.SCREEN_IMAGE, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, false, m_id, null);
						}
					}
				}
			}
			if (_nameEvent == ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION)
			{
				if (m_isUploadingFinalImage)
				{
					if (m_type != RequestModel.IMAGE_TYPE_FINISHED)
					{
						UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					}
					if ((bool)_list[0])
					{
						m_isUploadingFinalImage = false;
					}
				}
			}
			if (_nameEvent == ImagesController.EVENT_IMAGE_LOADED_REPORT_SYSTEM)
			{
				Image sImage = (Image)_list[0];
				if (sImage == m_image)
				{
					m_loadingText.SetActive(false);
					m_imageLoaded = true;
				}
			}
		}
	}
}