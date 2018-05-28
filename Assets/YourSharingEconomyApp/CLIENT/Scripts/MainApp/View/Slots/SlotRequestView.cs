using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * SlotRequesView
	 * 
	 * Slot that will be used in all the lists of the system 
	 * to display a request.
	 * 
	 * Section:
	 * 
	 *      -Name Request
	 *      -Price
	 *      -Location
	 *      -Deadline
	 *      -State (Without provider, with provider, finished without score, finished with score, flagged, banned)
	 * 
	 * @author Esteban Gallardo
	 */
	public class SlotRequestView : Button
	{
		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_SLOTREQUEST_SELECTED_REQUEST = "EVENT_SLOTREQUEST_SELECTED_REQUEST";

		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_SLOTREQUEST_SEE_TOXIC = "SUB_EVENT_SLOTREQUEST_SEE_TOXIC";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private Transform m_container;
		private RequestModel m_request;

		private GameObject m_iconOpen;
		private GameObject m_iconClosed;
		private GameObject m_iconFinished;
		private GameObject m_iconBroken;
		private GameObject m_iconToxic;

		private GameObject m_iconCalendar;

		private GameObject m_miniScore;

		private Image m_thumbnail;

		private bool m_flagToxicConfirmation = false;

		public RequestModel Request
		{
			get { return m_request; }
		}

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(RequestModel _item)
		{
			m_container = this.gameObject.transform;

			m_request = _item.Clone();

			m_iconOpen = m_container.Find("IconStates/Open").gameObject;
			m_iconClosed = m_container.Find("IconStates/Closed").gameObject;
			m_iconFinished = m_container.Find("IconStates/Finished").gameObject;
			m_iconBroken = m_container.Find("IconStates/Broken").gameObject;
			m_iconToxic = m_container.Find("IconStates/Toxic").gameObject;
			m_miniScore = m_container.Find("MiniScore").gameObject;

			m_iconOpen.SetActive(false);
			m_iconClosed.SetActive(false);
			m_iconFinished.SetActive(false);
			m_iconBroken.SetActive(false);
			m_miniScore.SetActive(false);
			m_iconToxic.SetActive(false);

			m_iconCalendar = m_container.Find("Calendar").gameObject;
			m_iconCalendar.SetActive(false);

			if (m_container.Find("Loading") != null)
			{
				m_container.Find("Loading").GetComponent<Text>().text = LanguageController.Instance.GetText("message.loading");
			}



			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);

			LoadData(false);
		}

		// -------------------------------------------
		/* 
		 * LoadData
		 */
		public void LoadData(bool _refreshData)
		{
			if (_refreshData)
			{
				RequestModel requestData = RequestsController.Instance.GetLocalRequest(m_request.Id);
				if (requestData != null)
				{
					m_request = requestData.Clone();
				}
			}

			m_container.Find("Title").GetComponent<Text>().text = m_request.Title;
			m_container.Find("Price").GetComponent<Text>().text = m_request.Price.ToString();
			m_container.Find("Deadline").GetComponent<Text>().text = DateConverter.TimeStampToDateTimeString(m_request.Deadline);

			if (m_container.Find("Village") != null)
			{
				m_container.Find("Village").GetComponent<Text>().text = m_request.Village;
			}

			if (m_container.Find("Thumbnail") != null)
			{
				m_thumbnail = m_container.Find("Thumbnail/Image").GetComponent<Image>();
				if ((!m_request.CheckEnoughReportsToWarningForToxic()) && (m_request.Flaged != 1))
				{
					UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_FROM_ID, this.gameObject, m_request.Referenceimg, m_thumbnail, (int)(m_thumbnail.gameObject.GetComponent<RectTransform>().sizeDelta.y), false);
				}
				else
				{
					HideLoading();
					if (m_request.IsFlagged())
					{
						UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_TOXIC_IMAGE, m_thumbnail, (int)(m_thumbnail.gameObject.GetComponent<RectTransform>().sizeDelta.y), ImageUtils.GetBytesJPG(MenusScreenController.Instance.ImageToxicConfirmed));
					}
					else
					{
						UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGES_LOAD_TOXIC_IMAGE, m_thumbnail, (int)(m_thumbnail.gameObject.GetComponent<RectTransform>().sizeDelta.y), ImageUtils.GetBytesJPG(MenusScreenController.Instance.ImageToxicPossible));
					}
				}
			}

			if (m_request.IsFlagged())
			{
				m_iconToxic.SetActive(true);
				m_container.Find("Deadline").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.toxic");
			}
			else
			{
				if ((m_request.Deliverydate != -1) &&
					(m_request.ScoreCustomerGivesToTheProvider == -1) && (m_request.ScoreProviderGivesToTheCustomer == -1) &&
					((m_request.FeedbackCustomerGivesToTheProvider.Length > 0) || (m_request.FeedbackProviderGivesToTheCustomer.Length > 0)))
				{
					m_iconBroken.SetActive(true);
					m_container.Find("Deadline").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.broken");
				}
				else
				{
					if ((m_request.ScoreCustomerGivesToTheProvider != -1) && (m_request.ScoreProviderGivesToTheCustomer != -1))
					{
						m_iconFinished.SetActive(true);
						m_miniScore.SetActive(true);
						m_miniScore.GetComponent<Text>().text = m_request.ScoreCustomerGivesToTheProvider + "/" + m_request.ScoreProviderGivesToTheCustomer;
						m_container.Find("Deadline").GetComponent<Text>().text = LanguageController.Instance.GetText("message.work.finished");
					}
					else
					{
						m_iconCalendar.SetActive(true);
						if (m_request.Provider == -1)
						{
							m_iconOpen.SetActive(true);
						}
						else
						{
							m_iconClosed.SetActive(true);
						}
					}
				}
			}
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			m_request = null;
			UIEventController.Instance.UIEvent -= OnBasicEvent;
			GameObject.DestroyObject(this.gameObject);
		}

		// -------------------------------------------
		/* 
		 * OnPointerClick
		 */
		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);

			if (UsersController.Instance.CurrentUser.IsBanned())
			{
				string warning = LanguageController.Instance.GetText("message.warning");
				string description = LanguageController.Instance.GetText("message.user.banned");
				MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, "");
			}
			else
			{
				if (m_request.IsFlagged())
				{
					m_flagToxicConfirmation = true;
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("message.request.declared.toxic"), null, SUB_EVENT_SLOTREQUEST_SEE_TOXIC);
				}
				else
				{
					if (m_request.CheckEnoughReportsToWarningForToxic())
					{
						m_flagToxicConfirmation = true;
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("message.request.possible.toxic"), null, SUB_EVENT_SLOTREQUEST_SEE_TOXIC);
					}
					else
					{
						UIEventController.Instance.DispatchUIEvent(EVENT_SLOTREQUEST_SELECTED_REQUEST, this.gameObject);
					}
				}
			}
		}

		// -------------------------------------------
		/* 
		 * DestroySlots
		 */
		public static void DestroySlots(List<GameObject> _list)
		{
			if (_list != null)
			{
				for (int i = 0; i < _list.Count; i++)
				{
					if (_list[i] != null)
					{
						SlotRequestView sView = _list[i].GetComponent<SlotRequestView>();
						if (sView != null)
						{
							sView.Destroy();
						}
					}
				}
				_list.Clear();
			}
		}

		// -------------------------------------------
		/* 
		 * HideLoading
		 */
		private void HideLoading()
		{
			if (m_container != null)
			{
				if (m_container.Find("Loading") != null)
				{
					m_container.Find("Loading").gameObject.SetActive(false);
				}
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == ImagesController.EVENT_IMAGE_LOADED_REPORT_SYSTEM)
			{
				Image sImage = (Image)_list[0];
				if (sImage == m_thumbnail)
				{
					HideLoading();
				}
			}
			if (_nameEvent == ScreenController.EVENT_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				if (subEvent == SUB_EVENT_SLOTREQUEST_SEE_TOXIC)
				{
					if (m_flagToxicConfirmation)
					{
						UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
						m_flagToxicConfirmation = false;
						if ((bool)_list[1])
						{
							UIEventController.Instance.DispatchUIEvent(EVENT_SLOTREQUEST_SELECTED_REQUEST, this.gameObject);
						}
					}
				}
			}
		}

	}
}