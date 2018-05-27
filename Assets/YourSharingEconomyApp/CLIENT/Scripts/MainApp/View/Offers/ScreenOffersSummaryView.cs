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
	 * ScreenOffersSummaryView
	 * 
	 * It's the summary of all the request the provider is working on or interested to do.
	 * 
	 * Sections:
	 * 
	 *      -List of requests the provider has made any offer or question
	 *      -List of request the provider is working on
	 *      -List of finished requests
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenOffersSummaryView : ScreenBaseView, IBasicScreenView
	{
		public const string SCREEN_OFFERS = "SCREEN_OFFERS";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_SCREENOFFERS_LOAD_CURRENT_WORK_ACCEPTED = "EVENT_SCREENOFFERS_LOAD_CURRENT_WORK_ACCEPTED";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------
		private GameObject m_root;
		private Transform m_container;
		private Transform m_slotWorksFollowingInfoContainer;
		private Transform m_slotWorksAcceptedInfoContainer;
		private Transform m_slotWorksFinishedInfoContainer;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content/ScrollPage/Page");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.search.work");

			m_container.Find("Button_SearchWorks").GetComponent<Button>().onClick.AddListener(CreateRequestPressed);
			m_container.Find("Button_SearchWorks/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.offers.search.request");

			m_container.Find("YourFollowingWorks/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.offers.following.works");
			m_slotWorksFollowingInfoContainer = m_container.Find("YourFollowingWorks");
			m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().Initialize();

			m_container.Find("YourActiveWorks/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.offers.active.works.in.progress");
			m_slotWorksAcceptedInfoContainer = m_container.Find("YourActiveWorks");
			m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().Initialize();

			m_container.Find("YourHistoryWorks/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.offers.active.works.finished");
			m_slotWorksFinishedInfoContainer = m_container.Find("YourHistoryWorks");
			m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().Initialize();

			m_root.transform.Find("Content/Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);
			m_root.transform.Find("Content/Button_Refresh").GetComponent<Button>().onClick.AddListener(RefreshDataPressed);

			BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);

			BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENOFFERS_LOAD_CURRENT_WORK_ACCEPTED, 0.1f);
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

				m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
				m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
				m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();

				m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
				m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
				m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();

				BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENOFFERS_LOAD_CURRENT_WORK_ACCEPTED, 0.1f);
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
			m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().Destroy();
			m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().Destroy();
			m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().Destroy();

			m_slotWorksFollowingInfoContainer = null;
			m_slotWorksAcceptedInfoContainer = null;
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
			ScreenController.Instance.CreateNewScreenNoParameters(ScreenSearchRequestView.SCREEN_SEARCH_REQUEST, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
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
		 * CheckSlotRequestExisting
		 */
		private bool CheckSlotRequestExisting(GameObject _slot)
		{
			return m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().CheckSlotExisting(_slot) ||
				m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().CheckSlotExisting(_slot) ||
				m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().CheckSlotExisting(_slot);
		}

		// -------------------------------------------
		/* 
		 * RefreshDataPressed
		 */
		private void RefreshDataPressed()
		{
			RequestsController.Instance.MustReloadRequests = true;
			RequestsController.Instance.ClearProviderRequests();
			ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
			BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENOFFERS_LOAD_CURRENT_WORK_ACCEPTED, 0.1f);
		}


		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (!this.gameObject.activeSelf) return;

			if (_nameEvent == EVENT_SCREENOFFERS_LOAD_CURRENT_WORK_ACCEPTED)
			{
				BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_BY_PROVIDER, UsersController.Instance.CurrentUser.Id, false);
			}
			if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_RECORDS)
			{
				BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
				List<RequestModel> data = (List<RequestModel>)_list[0];
				if ((int)_list[1] == RequestsController.TYPE_CONSULT_BY_PROVIDER)
				{
					m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
					m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
					m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();

					m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
					m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
					m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();

					if (data.Count > 0)
					{
						for (int i = 0; i < data.Count; i++)
						{
							RequestModel item = data[i];
							if (!item.IsFlagged())
							{
								if (item.Provider == -1)
								{
									m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
								}
								else
								{
									if (item.Deliverydate == -1)
									{
										m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
									}
									else
									{
										m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
									}
								}
							}
						}
					}
					m_slotWorksFollowingInfoContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
					m_slotWorksAcceptedInfoContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
					m_slotWorksFinishedInfoContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
				}
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
			if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				BackPressed();
			}
		}
	}
}