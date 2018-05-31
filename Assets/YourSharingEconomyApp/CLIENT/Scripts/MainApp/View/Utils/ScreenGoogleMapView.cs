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
	 * ScreenGoogleMapView
	 * 
	 * Interface class that gets the user's input to modify the Google maps
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenGoogleMapView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_GOOGLEMAP = "SCREEN_GOOGLEMAP";

		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------	
		public const string EVENT_SCREENGOOGLEMAP_SELECTED_VILLAGE = "EVENT_SCREENGOOGLEMAP_SELECTED_VILLAGE";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;
		private Transform m_map;
		private Text m_village;

		private Vector2 m_anchor;
		private bool m_isPressed = false;

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public override void Initialize(params object[] _list)
		{
			string coordinateData = (string)_list[0];

			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");
			m_map = m_container.Find("MapScroller/Map");
			m_map.GetComponent<GoogleMap>().Initialization(coordinateData);

			m_container.Find("MapScroller/loading").GetComponent<Text>().text = LanguageController.Instance.GetText("message.loading");
			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.googlemap.title");
			m_container.Find("Description").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.googlemap.description");
			m_village = m_container.Find("Village").GetComponent<Text>();
			m_village.text = (string)_list[1];

			m_container.transform.Find("Button_Back").GetComponent<Button>().onClick.AddListener(OnBackButton);

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

			if (m_map != null)
			{
				m_map.GetComponent<GoogleMap>().Destroy();
				m_map = null;
			}

			UIEventController.Instance.UIEvent -= OnBasicEvent;
			BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
			UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);

			return false;
		}

		// -------------------------------------------
		/* 
		 * OnBackButton
		 */
		private void OnBackButton()
		{
			Destroy();
		}

		// -------------------------------------------
		/*
		 * OnFingerDown
		 */
		private void OnFingerDown(Vector2 _position)
		{
			if (m_map == null) return;

			m_anchor = new Vector2(_position.x, _position.y);
			m_isPressed = true;
		}

		// -------------------------------------------
		/*
		 * OnFingerMove
		 */
		private void OnFingerMove(Vector2 _position)
		{
			if (m_map == null) return;

			if (m_isPressed)
			{
				Vector2 currPos = new Vector2(_position.x, _position.y);
				m_map.transform.localPosition = Vector2.zero + (currPos - m_anchor);
			}
		}

		// -------------------------------------------
		/*
		 * OnFingerUp
		 */
		private void OnFingerUp(Vector2 _position)
		{
			if (m_map == null) return;

			if (m_isPressed)
			{
				m_isPressed = false;
				Vector2 distanceMoved = (new Vector2(_position.x, _position.y)) - m_anchor;
				distanceMoved = new Vector2(distanceMoved.x / UnityEngine.Screen.width, distanceMoved.y / UnityEngine.Screen.height);
				m_map.transform.localPosition = Vector2.zero;
				UIEventController.Instance.DispatchUIEvent(GoogleMap.EVENT_GOOGLEMAP_SHIFT_POSITION, distanceMoved);
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicSystemEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == GoogleMap.EVENT_GOOGLEMAP_USER_UPDATE_VILLAGE)
			{
				m_village.text = (string)_list[0];
			}
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				OnBackButton();
			}
		}

		// -------------------------------------------
		/*
		 * Update
		 */
		void Update()
		{
			if (!m_isPressed)
			{
				if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0))
				{
					Vector2 posDown;
					if (Input.GetMouseButtonDown(0))
					{
						posDown = Input.mousePosition;
					}
					else
					{
						posDown = Input.GetTouch(0).position;
					}

					OnFingerDown(posDown);
				}
			}
			else
			{
				if (Input.GetMouseButtonUp(0) || ((Input.touchCount > 0) && ((Input.GetTouch(0).phase == TouchPhase.Ended))))
				{
					Vector2 posUp;
					if (Input.GetMouseButtonUp(0))
					{
						posUp = Input.mousePosition;
					}
					else
					{
						posUp = Input.GetTouch(0).position;
					}

					OnFingerUp(posUp);
				}
				else
				{
					Vector2 posMoved = Input.mousePosition;
					if (!Input.GetMouseButton(0))
					{
						if (Input.touchCount > 0) posMoved = Input.GetTouch(0).position;
					}

					OnFingerMove(posMoved);
				}
			}
		}
	}
}