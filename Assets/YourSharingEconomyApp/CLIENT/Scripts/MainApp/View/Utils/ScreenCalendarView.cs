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
	 * ScreenCalendarView
	 * 
	 * Uses the plugin FlatCalendar to select a deadline date for the requests
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenCalendarView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_CALENDAR = "SCREEN_CALENDAR";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_SCREENCALENDAR_SELECT_DAY = "EVENT_SCREENCALENDAR_SELECT_DAY";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private FlatCalendar m_flatCalendar;
		private DateTime m_myDateTime;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public override void Initialize(params object[] _list)
		{
			double timestampDate = DateConverter.GetTimestamp();

			if (_list.Length > 0)
			{
				if (_list[0] is string)
				{
					string bufferData = (string)_list[0];
					if (bufferData.Length > 0)
					{
						if (!double.TryParse(bufferData, out timestampDate))
						{
							timestampDate = DateConverter.GetTimestamp();
						}
					}
				}
			}

			// INIT CALENDAR
			m_flatCalendar = GameObject.Find("FlatCalendar").GetComponent<FlatCalendar>();
			m_myDateTime = DateConverter.TimeStampToDateTime(timestampDate);
			m_flatCalendar.initFlatCalendar();
			m_flatCalendar.setCallback_OnDaySelected(DayUpdated);
			m_flatCalendar.setUIStyle(0);
			UIEventController.Instance.DelayUIEvent(UIEventController.EVENT_BASICEVENT_DELAYED_CALL, 0.01f, this.gameObject, "SetRequestDate");

			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * SetRequestDate
		 */
		public void SetRequestDate()
		{
			GameObject.FindObjectOfType<FlatCalendar>().SetMyTime(m_myDateTime);
		}

		// -------------------------------------------
		/* 
		 * DayUpdated
		 */
		public void DayUpdated(FlatCalendar.TimeObj _time)
		{
			DateTime newDate = new DateTime(_time.year, _time.month, _time.day, 0, 0, 0, 0, System.DateTimeKind.Utc);
			UIEventController.Instance.DispatchUIEvent(EVENT_SCREENCALENDAR_SELECT_DAY, (long)DateConverter.DateTimeToTimeStamp(newDate));
			Destroy();
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public override bool Destroy()
		{
			if (base.Destroy()) return true;

			UIEventController.Instance.UIEvent -= OnBasicEvent;
			UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);

			return false;
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UIEventController.EVENT_BASICEVENT_DELAYED_CALL)
			{
				if (this.gameObject == ((GameObject)_list[0]))
				{
					Invoke((string)_list[1], 0);
				}
			}
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				Destroy();
			}
		}
	}
}