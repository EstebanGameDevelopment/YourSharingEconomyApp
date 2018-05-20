using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * PanelRatingView
	 * 
	 * Rating panel that displays a score and also allows 
	 * the user to rate other members
	 * 
	 * @author Esteban Gallardo
	 */
	public class PanelRatingView : MonoBehaviour
	{
		// ----------------------------------------------
		// CONSTANTS
		// ----------------------------------------------	
		public const float MAXIMUM_VALUE_VOTE = 5;

		// ----------------------------------------------
		// PUBLIC MEMBERS
		// ----------------------------------------------	
		public Sprite EmptyStar;
		public Sprite FullStar;
		public Sprite HalfStar;

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private Transform m_container;
		private string m_eventData = "";
		private List<GameObject> m_stars = new List<GameObject>();
		private bool m_showScore = false;
		private bool m_isInteractable = true;

		private string m_property = "";

		public bool IsInteractable
		{
			get { return m_isInteractable; }
			set { m_isInteractable = value; }
		}
		public string Property
		{
			get { return m_property; }
			set { m_property = value; }
		}

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public void Initialize(int _score, int _totalVotes, string _title, bool _showScore, string _eventData, bool _isInteractable)
		{
			m_eventData = _eventData;
			m_container = this.gameObject.transform;

			if (m_stars.Count == 0)
			{
				GameObject star0 = m_container.Find("Star0").gameObject;
				star0.GetComponent<Button>().onClick.AddListener(OnClickStar0);
				m_stars.Add(star0);

				GameObject star1 = m_container.Find("Star1").gameObject;
				star1.GetComponent<Button>().onClick.AddListener(OnClickStar1);
				m_stars.Add(star1);

				GameObject star2 = m_container.Find("Star2").gameObject;
				star2.GetComponent<Button>().onClick.AddListener(OnClickStar2);
				m_stars.Add(star2);

				GameObject star3 = m_container.Find("Star3").gameObject;
				star3.GetComponent<Button>().onClick.AddListener(OnClickStar3);
				m_stars.Add(star3);

				GameObject star4 = m_container.Find("Star4").gameObject;
				star4.GetComponent<Button>().onClick.AddListener(OnClickStar4);
				m_stars.Add(star4);
			}

			for (int i = 0; i < m_stars.Count; i++)
			{
				m_stars[i].GetComponent<Image>().overrideSprite = EmptyStar;
			}

			m_showScore = _showScore;
			SetScore(_score, _totalVotes);

			m_container.Find("Title").GetComponent<Text>().text = _title;

			m_isInteractable = _isInteractable;
		}

		// -------------------------------------------
		/* 
		 * SetScore
		 */
		public void SetScore(int _score, int _totalVotes)
		{
			int counter = 0;
			float starScore = ((float)_score / (float)_totalVotes);
			for (int i = 0; i < m_stars.Count; i++)
			{
				float checkValue = i + 1;
				if (starScore >= checkValue)
				{
					counter++;
					m_stars[i].GetComponent<Image>().overrideSprite = FullStar;
				}
				else
				{
					float segment = checkValue - starScore;
					if (segment < 0.5)
					{
						m_stars[i].GetComponent<Image>().overrideSprite = HalfStar;
					}
					else
					{
						m_stars[i].GetComponent<Image>().overrideSprite = EmptyStar;
					}
				}
			}

			if (m_showScore)
			{
				string scoreGrade = LanguageController.Instance.GetText("message.score." + counter);
				if (_score > 0)
				{
					m_container.Find("Score").GetComponent<Text>().text = scoreGrade + " : " + ((starScore / MAXIMUM_VALUE_VOTE) * 100) + "%";
				}
				else
				{
					m_container.Find("Score").GetComponent<Text>().text = "";
				}
			}
		}

		// -------------------------------------------
		/* 
		 * OnClickStar0
		 */
		private void OnClickStar(int _index)
		{
			if (m_isInteractable)
			{
				if (m_eventData.Length > 0)
				{
					BasicEventController.Instance.DispatchBasicEvent(m_eventData, this.gameObject, _index, m_property);
				}
			}
		}

		// -------------------------------------------
		/* 
		 * OnClickStar0
		 */
		private void OnClickStar0()
		{
			OnClickStar(0);
		}

		// -------------------------------------------
		/* 
		 * OnClickStar1
		 */
		private void OnClickStar1()
		{
			OnClickStar(1);
		}

		// -------------------------------------------
		/* 
		 * OnClickStar2
		 */
		private void OnClickStar2()
		{
			OnClickStar(2);
		}

		// -------------------------------------------
		/* 
		 * OnClickStar3
		 */
		private void OnClickStar3()
		{
			OnClickStar(3);
		}

		// -------------------------------------------
		/* 
		 * OnClickStar4
		 */
		private void OnClickStar4()
		{
			OnClickStar(4);
		}

		// -------------------------------------------
		/* 
		 * DisableInteraction
		 */
		public void DisableInteraction()
		{
			m_isInteractable = false;
		}


		// -------------------------------------------
		/* 
		 * DisableInteraction
		 */
		public void SetTitle(string _text)
		{
			m_container.Find("Title").GetComponent<Text>().text = _text;
		}

		// -------------------------------------------
		/* 
		 * DisableInteraction
		 */
		public void SetTextScore(string _text)
		{
			m_container.Find("Score").GetComponent<Text>().text = _text;
		}
	}
}