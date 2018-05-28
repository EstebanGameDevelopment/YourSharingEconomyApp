using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using YourCommonTools;

namespace YourSharingEconomyApp
{
	/******************************************
	 * 
	 * YourCollaborativeEconomyApp
	 * 
	 * ScreenManager controller that handles all the screens's creation and disposal
	 * 
	 * @author Esteban Gallardo
	 */
	public class MenusScreenController : ScreenController
	{
		// ----------------------------------------------
		// CONFIGURATION
		// ----------------------------------------------	
		public const string URL_BASE_PHP = "http://localhost:8080/yoursharingeconomyapp/";

		public const string KYRJEncryption = "sK1rwpD1p+5e#bvt31CK13z77n=ES8jR"; //32 chr shared ascii string (32 * 8 = 256 bit)

		// COOKIES
		public const string USER_EMAIL_COOCKIE = "USER_EMAIL_COOCKIE";
		public const string USER_NAME_COOCKIE = "USER_NAME_COOCKIE";
		public const string USER_PASSWORD_COOCKIE = "USER_PASSWORD_COOCKIE";
		public const string USER_FACEBOOK_CONNECTED_COOCKIE = "USER_FACEBOOK_CONNECTED_COOCKIE";

		// TABLES
		public const string TABLE_REQUESTS = "requests";
		public const string TABLE_USERS = "users";

		// ----------------------------------------------
		// SINGLETON
		// ----------------------------------------------	
		private static MenusScreenController _instance;

		public static MenusScreenController Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType(typeof(MenusScreenController)) as MenusScreenController;
				}
				return _instance;
			}
		}

		// ----------------------------------------------
		// PUBLIC MEMBERS
		// ----------------------------------------------	
		public TextAsset ReadMeFile;
		[Tooltip("It allows the debug communications")]
		public bool DebugComms = true;
		[Tooltip("It allows the iaps")]
		public bool DebugIAPs = true;

		// ----------------------------------------------
		// DRESSMAKERS PUBLIC VARIABLES
		// ----------------------------------------------	
		[Tooltip("Slot request with image")]
		public GameObject SlotInfoRequestImage;

		[Tooltip("Slot offer")]
		public GameObject SlotOffer;

		[Tooltip("Slot image")]
		public GameObject SlotImage;

		[Tooltip("Image reference link")]
		public Sprite ImageReferenceLink;

		[Tooltip("Image Toxic Confirmed")]
		public Sprite ImageToxicConfirmed;

		[Tooltip("Image Toxic Possible")]
		public Sprite ImageToxicPossible;

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private SearchModel m_lastSearchModel;

		// HOURS BE ABLE TO MAKE A NEW PROPOSAL
		private int m_hoursToEnableANewProposal = 3;

		// NUMBER OF REPORTS FROM USERS TO SHOW A WARNING MESSAGE ON THE REQUEST
		private int m_totalReportsToWarnRequest = 3;

		// TOTAL NUMBER OF FREE REQUESTS AVAILABLE AT THE SAME TIME
		private int m_totalNumberOfFreeRequests = 1;

		// TOTAL NUMBER OF IMAGES THAT CAN BE USED AS A REFERENCE
		private int m_totalNumberImagesAsReference = 4;

		// TOTAL NUMBER OF IMAGES THAT CAN BE USED AS A FINISHED JOB
		private int m_totalNumberImagesAsFinished = 4;

		// TOTAL NUMBER OF IMAGES THAT THE PROVIDER CAN POST TO SHOW HIS EXPERIENCE
		private int m_totalNumberImagesAsProviderExperience = 10;

		// SIZE TO SCALE THE IMAGES SELECTED BY THE USER
		private int m_sizeHeightAllowedImages = 600;

		// SKILLS OF THE PROVIDERS
		private string m_providerSkills;

		// ----------------------------------------------
		// GETTERS/SETTERS
		// ----------------------------------------------	
		public SearchModel LastSearchModel
		{
			get { return m_lastSearchModel; }
			set { m_lastSearchModel = value; }
		}
		public int HoursToEnableANewProposal
		{
			get { return m_hoursToEnableANewProposal; }
			set { m_hoursToEnableANewProposal = value; }
		}
		public int TotalReportsToWarnRequest
		{
			get { return m_totalReportsToWarnRequest; }
			set { m_totalReportsToWarnRequest = value; }
		}
		public int TotalNumberOfFreeRequests
		{
			get { return m_totalNumberOfFreeRequests; }
			set { m_totalNumberOfFreeRequests = value; }
		}
		public int TotalNumberImagesAsReference
		{
			get { return m_totalNumberImagesAsReference; }
			set { m_totalNumberImagesAsReference = value; }
		}
		public int TotalNumberImagesAsFinished
		{
			get { return m_totalNumberImagesAsFinished; }
			set { m_totalNumberImagesAsFinished = value; }
		}
		public int TotalNumberImagesAsProviderExperience
		{
			get { return m_totalNumberImagesAsProviderExperience; }
			set { m_totalNumberImagesAsProviderExperience = value; }
		}
		public int SizeHeightAllowedImages
		{
			get { return m_sizeHeightAllowedImages; }
			set { m_sizeHeightAllowedImages = value; }
		}
		public string ProviderSkills
		{
			get { return m_providerSkills; }
			set { m_providerSkills = value; }
		}

		// -------------------------------------------
		/* 
		 * Initialitzation listener
		 */
		public override void Start()
		{
			base.Start();

			if (DebugMode)
			{
				Debug.Log("YourVRUIScreenController::Start::First class to initialize for the whole system to work");
			}

			UIEventController.Instance.UIEvent += new UIEventHandler(OnUIEvent);

			LanguageController.Instance.Initialize();
			UsersController.Instance.Init();
			CommController.Instance.Init();
			RequestsController.Instance.Init();
			ProposalsController.Instance.Init();
			ImagesController.Instance.Init();
			SoundsController.Instance.Initialize();
			IAPController.Instance.Init();
			CreateNewScreenNoParameters(ScreenInitialView.SCREEN_INITIAL, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);

#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = true;
#endif
		}

		// -------------------------------------------
		/* 
		 * Destroy all references
		 */
		public override void Destroy()
		{
			base.Destroy();

			if (_instance != null)
			{
				UIEventController.Instance.UIEvent -= OnUIEvent;
				LanguageController.Instance.Destroy();
				UsersController.Instance.Destroy();
				CommController.Instance.Destroy();
				RequestsController.Instance.Destroy();
				ProposalsController.Instance.Destroy();
				ImagesController.Instance.Destroy();
				SoundsController.Instance.Destroy();
				IAPController.Instance.Destroy();
				Destroy(_instance);
				_instance = null;
			}
		}


		// -------------------------------------------
		/* 
		 * LoadEmailLoginLocal
		 */
		public ItemMultiTextEntry LoadEmailLoginLocal()
		{
			try
			{
				string email = RJEncryptor.DecryptStringWithKey(PlayerPrefs.GetString(USER_EMAIL_COOCKIE, ""), KYRJEncryption);
				string password = RJEncryptor.DecryptStringWithKey(PlayerPrefs.GetString(USER_PASSWORD_COOCKIE, ""), KYRJEncryption);
				string nameUser = PlayerPrefs.GetString(USER_NAME_COOCKIE, "");

				if (email.Length > 0)
				{
					return new ItemMultiTextEntry(email, password, nameUser);
				}
				else
				{
					return null;
				}
			}
			catch (Exception err)
			{
				if (DebugMode)
				{
					Debug.Log(err.StackTrace);
				}
				return null;
			}
		}



		// -------------------------------------------
		/* 
		 * Manager of global events
		 */
		protected override void OnUIEvent(string _nameEvent, params object[] _list)
		{
			base.OnUIEvent(_nameEvent, _list);

			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN)
			{
				SoundsConstants.PlayFxSelection();
			}		
		}
	}
}