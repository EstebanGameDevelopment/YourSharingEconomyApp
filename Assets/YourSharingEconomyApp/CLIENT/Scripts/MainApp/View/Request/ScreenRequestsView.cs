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
	 * ScreenRequestsView
	 * 
	 * It shows a general overview of the requests:
	 * 
	 * Sections:
	 * 
	 *      -Requests without provider assigned
	 *      -Requests in progress
	 *      -Requests finished
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenRequestsView : ScreenBaseView, IBasicScreenView
	{
		public const string SCREEN_REQUESTS = "SCREEN_REQUESTS";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_SCREENREQUESTS_REQUEST_LOAD_DATA = "EVENT_SCREENREQUESTS_REQUEST_LOAD_DATA";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;
		private Transform m_slotRequestSearchingContainer;
		private Transform m_slotRequestOnGoingContainer;
		private Transform m_slotWorksFinishedInfoContainer;
		private bool m_allInformationLoaded = false;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content/ScrollPage/Page");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.main.request.title");

			m_container.Find("Button_Create").GetComponent<Button>().onClick.AddListener(CreateRequestPressed);
			m_container.Find("Button_Create/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.main.request.new.dress");

			m_container.Find("YourActiveRequests/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.main.request.active.jobs");
			m_slotRequestSearchingContainer = m_container.Find("YourActiveRequests");
			m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().Initialize();

			m_container.Find("YourOnGoingRequests/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.main.request.on.going.jobs");
			m_slotRequestOnGoingContainer = m_container.Find("YourOnGoingRequests");
			m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().Initialize();

			m_container.Find("YourCompletedRequests/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.offers.active.works.finished");
			m_slotWorksFinishedInfoContainer = m_container.Find("YourCompletedRequests");
			m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().Initialize();

			m_root.transform.Find("Content/Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);
			m_root.transform.Find("Content/Button_Refresh").GetComponent<Button>().onClick.AddListener(RefreshDataPressed);

			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);

			BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENREQUESTS_REQUEST_LOAD_DATA, 0.1f);
		}

		// -------------------------------------------
		/* 
		 * SetActivation
		 */
		public override void SetActivation(bool _activation)
		{
			if (_activation && !this.gameObject.activeSelf)
			{
				this.gameObject.SetActive(_activation);

				m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
				m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
				m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();

				m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
				m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
				m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();

				BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENREQUESTS_REQUEST_LOAD_DATA, 0.1f);
			}
			else
			{
				this.gameObject.SetActive(_activation);
			}
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().Destroy();
			m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().Destroy();
			m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().Destroy();

			m_slotRequestSearchingContainer = null;
			m_slotRequestOnGoingContainer = null;
			m_slotWorksFinishedInfoContainer = null;

			BasicEventController.Instance.BasicEvent -= OnBasicEvent;
			GameObject.DestroyObject(this.gameObject);
		}

		// -------------------------------------------
		/* 
		 * CreateRequestPressed
		 */
		private void CreateRequestPressed()
		{
#if ENABLED_FACEBOOK
        string warning = LanguageController.Instance.GetText("message.warning");
        string description = LanguageController.Instance.GetText("screen.create.request.only.create.in.mobile");
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
#else
			if (!m_allInformationLoaded)
			{
				string warning = LanguageController.Instance.GetText("message.warning");
				string description = LanguageController.Instance.GetText("screen.create.request.wait.until.loaded.information");
				ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
			}
			else
			{
				if (UsersController.Instance.CurrentUser.IsBanned())
				{
					string warning = LanguageController.Instance.GetText("message.warning");
					string description = LanguageController.Instance.GetText("message.user.banned");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
				}
				else
				{
					if (!UsersController.Instance.CurrentUser.Validated)
					{
						string warning = LanguageController.Instance.GetText("message.warning");
						string description = LanguageController.Instance.GetText("message.user.not.validated");
						ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
					}
					else
					{
						if (RequestsController.Instance.CountActiveRequestByUser(UsersController.Instance.CurrentUser.Id) < ScreenController.Instance.TotalNumberOfFreeRequests)
						{
							ScreenController.Instance.CreateNewScreen(ScreenCreateRequestView.SCREEN_CREATE_REQUEST, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, null);
						}
						else
						{
							if (UsersController.Instance.CurrentUser.Additionalrequest > 0)
							{
								ScreenController.Instance.CreateNewScreen(ScreenCreateRequestView.SCREEN_CREATE_REQUEST, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, null);
							}
							else
							{
								ScreenController.Instance.CreateNewScreen(ScreenPremiumRequestView.SCREEN_PREMIUM_REQUEST, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, false);
							}
						}
					}
				}
			}
#endif
		}

		// -------------------------------------------
		/* 
		 * ExitPressed
		 */
		private void BackPressed()
		{
			ScreenController.Instance.CreateNewScreenNoParameters(ScreenMainMenuView.SCREEN_MAIN_MENU, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
		}


		// -------------------------------------------
		/* 
		 * RefreshDataPressed
		 */
		private void RefreshDataPressed()
		{
			RequestsController.Instance.MustReloadRequests = true;
			RequestsController.Instance.ClearUserRequests();
			ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
			BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENREQUESTS_REQUEST_LOAD_DATA, 0.1f);
		}

		// -------------------------------------------
		/* 
		 * CheckSlotRequestExisting
		 */
		private bool CheckSlotRequestExisting(GameObject _slot)
		{
			return m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().CheckSlotExisting(_slot) ||
				m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().CheckSlotExisting(_slot) ||
				m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().CheckSlotExisting(_slot);
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (!this.gameObject.activeSelf) return;

			if (_nameEvent == EVENT_SCREENREQUESTS_REQUEST_LOAD_DATA)
			{
				BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_RECORDS_BY_USER);
			}
			if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_RECORDS)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				List<RequestModel> data = (List<RequestModel>)_list[0];
				if ((int)_list[1] == RequestsController.TYPE_CONSULT_BY_USER)
				{
					m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
					m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
					m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();

					m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
					m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
					m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();

					if (data.Count > 0)
					{
						for (int i = 0; i < data.Count; i++)
						{
							RequestModel item = data[i];
							if (item.IsFlagged())
							{
								m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
							}
							else
							{
								if (item.Deliverydate == -1)
								{
									if (item.Provider == -1)
									{
										m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
									}
									else
									{
										m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
									}
								}
								else
								{
									m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
								}
							}
						}
					}
					m_slotRequestSearchingContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
					m_slotRequestOnGoingContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
					m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
				}
				m_allInformationLoaded = true;
			}
			if (_nameEvent == SlotRequestView.EVENT_SLOTREQUEST_SELECTED_REQUEST)
			{
				GameObject slotClicked = (GameObject)_list[0];
				if (CheckSlotRequestExisting(slotClicked))
				{
					SlotRequestView slotSelected = slotClicked.GetComponent<SlotRequestView>();
					BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_SINGLE_RECORD, slotSelected.Request.Id);
				}
			}
			if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_SINGLE_RECORD)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				RequestModel request = (RequestModel)_list[0];
				if (request != null)
				{
					ScreenController.Instance.CreateNewScreen(ScreenCreateRequestView.SCREEN_DISPLAY_REQUEST, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, request);
				}
				else
				{
					string warning = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.request.not.found");
					ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				CreateRequestPressed();
			}
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				BackPressed();
			}
		}
	}
}