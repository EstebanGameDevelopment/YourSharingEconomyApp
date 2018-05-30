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
	 * ScreenSearchRequestView
	 * 
	 * It allows the provider to look for request in their area of choice
	 * 
	 * Sections:
	 *      -Area defined by Google Maps
	 *      -Distance to consider for the search
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenSearchRequestView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_SEARCH_REQUEST = "SCREEN_SEARCH_REQUEST";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------
		private GameObject m_root;
		private Transform m_container;

		private string m_villageSearch;
		private string m_mapDataSearch;

		private int m_indexDistance;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public override void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.search.work");
			m_container.Find("Description").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.search.requests.choose.area");

			m_villageSearch = LanguageController.Instance.GetText("screen.profile.village.not.defined");
			if (UsersController.Instance.CurrentUser.Village.Length > 0)
			{
				m_villageSearch = UsersController.Instance.CurrentUser.Village;
			}
			if (UsersController.Instance.CurrentUser.Mapdata.Length > 0)
			{
				m_mapDataSearch = UsersController.Instance.CurrentUser.Mapdata;
			}
			else
			{
				m_mapDataSearch = "";
			}
			m_container.Find("Button_Maps/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.area.search") + '\n' + m_villageSearch;
			m_container.Find("Button_Maps").GetComponent<Button>().onClick.AddListener(OnGoogleMaps);

			m_indexDistance = 1;
			m_container.Find("RequestDistanceLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.distance.search");
			m_container.Find("RequestDisctanceDropdown").GetComponent<Dropdown>().onValueChanged.AddListener(OnRequestDistanceDropdown);
			SelectRequestDistanceByIndex(m_indexDistance);

			m_container.Find("Button_SearchRequest").GetComponent<Button>().onClick.AddListener(SearchRequestPressed);
			m_container.Find("Button_SearchRequest/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.search.requests.search.now");

			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);

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

			UIEventController.Instance.UIEvent -= OnBasicEvent;
			BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
			GameObject.Destroy(this.gameObject);

			return false;
		}

		// -------------------------------------------
		/* 
		 * OnGoogleMaps
		 */
		private void OnGoogleMaps()
		{
#if ENABLED_FACEBOOK
        string warning = LanguageController.Instance.GetText("message.warning");
        string description = LanguageController.Instance.GetText("message.map.not.available.in.facebook");
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, "");
#else
			MenusScreenController.Instance.CreateNewScreen(ScreenGoogleMapView.SCREEN_GOOGLEMAP, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, false, m_mapDataSearch, m_villageSearch);
#endif
		}

		// -------------------------------------------
		/* 
		 * OnRequestDistanceDropdown
		 */
		private void OnRequestDistanceDropdown(int _index)
		{
			m_indexDistance = _index;
		}

		// -------------------------------------------
		/* 
		 * SelectRequestCurrencyByName
		 */
		private void SelectRequestDistanceByIndex(int _index)
		{
			m_container.Find("RequestDisctanceDropdown").GetComponent<Dropdown>().value = _index;
		}

		// -------------------------------------------
		/* 
		 * CreateRequestPressed
		 */
		private void SearchRequestPressed()
		{
			if (m_mapDataSearch.Length == 0)
			{
				string warning = LanguageController.Instance.GetText("message.error");
				string description = LanguageController.Instance.GetText("message.search.requests.specify.location");
				MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, "");
			}
			else
			{
				MenusScreenController.Instance.CreateNewScreen(ScreenSearchResultView.SCREEN_SEARCH_RESULT, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS, true, new SearchModel(m_villageSearch, m_mapDataSearch, m_indexDistance));
			}
		}

		// -------------------------------------------
		/* 
		 * BackPressed
		 */
		private void BackPressed()
		{
			MenusScreenController.Instance.CreateNewScreenNoParameters(ScreenOffersSummaryView.SCREEN_OFFERS, UIScreenTypePreviousAction.DESTROY_ALL_SCREENS);
		}

		// -------------------------------------------
		/* 
		 * OnBasicSystemEvent
		 */
		private void OnBasicSystemEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == GoogleMap.EVENT_GOOGLEMAP_USER_UPDATE_VILLAGE)
			{
				m_villageSearch = (string)_list[0];
				m_mapDataSearch = (string)_list[1];
				m_container.Find("Button_Maps/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.area.search") + '\n' + m_villageSearch;
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (!this.gameObject.activeSelf) return;

			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				BackPressed();
			}
		}
	}
}