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
	 * ScreenSearchResultView
	 * 
	 * It will show a list with the result of a search
	 * in an specific area selected by the provider.
	 * 
	 * Sections:
	 *      -List of SlotRequestsView
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenSearchResultView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_SEARCH_RESULT = "SCREEN_SEARCH_RESULT";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_SCREENSEARCHRESULTS_LOAD_DATA_SEARCHED = "EVENT_SCREENSEARCHRESULTS_LOAD_DATA_SEARCHED";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------
		private GameObject m_root;
		private Transform m_container;
		private Transform m_slotSearchResultInfoContainer;

		private SearchModel m_searchRequest;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public override void Initialize(params object[] _list)
		{
			m_searchRequest = (SearchModel)_list[0];
			MenusScreenController.Instance.LastSearchModel = m_searchRequest.Clone();

			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.search.work");

			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);
			m_container.Find("Button_Search").GetComponent<Button>().onClick.AddListener(NewSearchPressed);

			m_container.Find("YourSearchResultWorks/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.search.work.description");
			m_slotSearchResultInfoContainer = m_container.Find("YourSearchResultWorks");
			m_slotSearchResultInfoContainer.GetComponent<SlotManagerView>().Initialize(6);

			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);

			UIEventController.Instance.DelayUIEvent(EVENT_SCREENSEARCHRESULTS_LOAD_DATA_SEARCHED, 0.01f);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public override bool Destroy()
		{
			if (base.Destroy()) return true;

			m_slotSearchResultInfoContainer.GetComponent<SlotManagerView>().Destroy();

			UIEventController.Instance.UIEvent -= OnBasicEvent;
			UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);

			return false;
		}

		// -------------------------------------------
		/* 
		 * ExitPressed
		 */
		private void BackPressed()
		{
			MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenOffersSummaryView.SCREEN_OFFERS, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
		}

		// -------------------------------------------
		/* 
		 * NewSearchPressed
		 */
		private void NewSearchPressed()
		{
			MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenSearchRequestView.SCREEN_SEARCH_REQUEST, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (!this.gameObject.activeSelf) return;

			if (_nameEvent == EVENT_SCREENSEARCHRESULTS_LOAD_DATA_SEARCHED)
			{
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_BY_DISTANCE_RECORDS, m_searchRequest);
			}
			if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_RECORDS)
			{
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				List<RequestModel> data = (List<RequestModel>)_list[0];
				if ((int)_list[1] == RequestsController.TYPE_CONSULT_BY_DISTANCE)
				{
					if (data.Count > 0)
					{
						m_slotSearchResultInfoContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
						m_slotSearchResultInfoContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();

						for (int i = 0; i < data.Count; i++)
						{
							RequestModel item = data[i];
							if ((item.Customer != UsersController.Instance.CurrentUser.Id) && (item.Provider == -1))
							{
								m_slotSearchResultInfoContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
							}
						}
						m_slotSearchResultInfoContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
					}
				}
			}
			if (_nameEvent == SlotRequestView.EVENT_SLOTREQUEST_SELECTED_REQUEST)
			{
				GameObject slotClicked = (GameObject)_list[0];
				if (m_slotSearchResultInfoContainer.GetComponent<SlotManagerView>().CheckSlotExisting(slotClicked))
				{
					SlotRequestView slotSelected = slotClicked.GetComponent<SlotRequestView>();
					UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_SINGLE_RECORD, slotSelected.Request.Id);
				}
			}
			if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_SINGLE_RECORD)
			{
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				RequestModel request = (RequestModel)_list[0];
				if (request != null)
				{
					MenusScreenController.Instance.CreateNewScreen(ScreenCreateRequestView.SCREEN_DISPLAY_REQUEST, UIScreenTypePreviousAction.HIDE_CURRENT_SCREEN, true, request);
				}
				else
				{
					string warning = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.request.not.found");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, "");
				}
			}
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				NewSearchPressed();
			}
		}
	}
}