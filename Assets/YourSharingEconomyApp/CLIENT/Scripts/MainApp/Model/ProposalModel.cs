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
	 * ProposalModel
	 * 
	 * Keeps all the information about the proposal
	 * 
	 * @author Esteban Gallardo
	 */
	public class ProposalModel
	{
		public const int STATE_PROPOSAL_ALL = -1;
		public const int STATE_PROPOSAL_OPEN = 0;
		public const int STATE_PROPOSAL_ACCEPTED_AND_KEEP_LOOKING = 1;
		public const int STATE_PROPOSAL_ACCEPTED_AND_FIXED = 2;

		public const int TYPE_INFO = 0;
		public const int TYPE_OFFER = 1;
		public const int TYPE_REPORT = 2;

		public const int ACTIVE_INVALID = 0;
		public const int ACTIVE_VALID = 1;

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------
		private long m_id;
		private long m_user;
		private long m_request;
		private int m_type;
		private string m_title;
		private string m_description;
		private int m_price;
		private long m_deadline;
		private int m_accepted;
		private int m_active;
		private long m_created;
		private string m_reported;
		private int m_confirmedReported = 0;

		public long Id
		{
			get { return m_id; }
			set { m_id = value; }
		}
		public long User
		{
			get { return m_user; }
			set { m_user = value; }
		}
		public long Request
		{
			get { return m_request; }
			set { m_request = value; }
		}
		public int Type
		{
			get { return m_type; }
			set { m_type = value; }
		}
		public string Title
		{
			get { return m_title; }
			set { m_title = value; }
		}
		public string Description
		{
			get { return m_description; }
			set { m_description = value; }
		}
		public int Price
		{
			get { return m_price; }
			set { m_price = value; }
		}
		public long Deadline
		{
			get { return m_deadline; }
			set { m_deadline = value; }
		}
		public int Accepted
		{
			get { return m_accepted; }
			set { m_accepted = value; }
		}
		public int Active
		{
			get { return m_active; }
			set { m_active = value; }
		}
		public long Created
		{
			get { return m_created; }
		}
		public string Reported
		{
			get { return m_reported; }
			set { m_reported = value; }
		}
		public int ConfirmedReported
		{
			get { return m_confirmedReported; }
		}

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public ProposalModel()
		{
			m_id = -1;
			m_user = -1;
			m_request = -1;
			m_type = TYPE_OFFER;
			m_title = "";
			m_description = "";
			m_price = 0;
			m_deadline = -1;
			m_accepted = STATE_PROPOSAL_OPEN;
			m_active = ACTIVE_VALID;
			m_created = -1;
			m_reported = "";
			m_confirmedReported = 0;
		}

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public ProposalModel(long _id,
							long _user,
							long _request,
							int _type,
							string _title,
							string _description,
							int _price,
							long _deadline,
							int _accepted,
							int _active,
							long _created,
							string _reported,
							int _confirmedReported)
		{
			m_id = _id;
			m_user = _user;
			m_request = _request;
			m_type = _type;
			m_title = _title;
			m_description = _description;
			m_price = _price;
			m_deadline = _deadline;
			m_accepted = _accepted;
			m_active = _active;
			m_created = _created;
			m_reported = _reported;
			m_confirmedReported = _confirmedReported;
		}

		// -------------------------------------------
		/* 
		 * Clone
		 */
		public ProposalModel Clone()
		{
			return new ProposalModel(m_id,
									m_user,
									m_request,
									m_type,
									m_title,
									m_description,
									m_price,
									m_deadline,
									m_accepted,
									m_active,
									m_created,
									m_reported,
									m_confirmedReported);
		}

		// -------------------------------------------
		/* 
		 * Copy
		 */
		public void Copy(ProposalModel _proposal)
		{
			m_id = _proposal.Id;
			m_user = _proposal.User;
			m_request = _proposal.Request;
			m_type = _proposal.Type;
			m_title = _proposal.Title;
			m_description = _proposal.Description;
			m_price = _proposal.Price;
			m_deadline = _proposal.Deadline;
			m_accepted = _proposal.Accepted;
			m_active = _proposal.Active;
			m_created = _proposal.Created;
			m_reported = _proposal.Reported;
			m_confirmedReported = _proposal.ConfirmedReported;
		}

		// -------------------------------------------
		/* 
		 * CheckDatesRight
		 */
		public bool CheckDatesRight()
		{
			long myCurrentTime = DateConverter.GetTimestamp();
			return (myCurrentTime < m_deadline);
		}

		// -------------------------------------------
		/* 
		 * AllDataFilled
		 */
		public bool AllDataFilled()
		{
			if ((m_type == TYPE_INFO) || (m_type == TYPE_REPORT))
			{
				return (m_user != -1) &&
						(m_request != -1) &&
						(m_title.Length > 0) &&
						(m_description.Length > 0);
			}
			else
			{
				return (m_user != -1) &&
						(m_request != -1) &&
						(m_title.Length > 0) &&
						(m_description.Length > 0) &&
						(m_price > 0) &&
						CheckDatesRight();
			}
		}

		// -------------------------------------------
		/* 
		 * IsSelected
		 */
		public bool IsSelected()
		{
			return (m_accepted == STATE_PROPOSAL_ACCEPTED_AND_KEEP_LOOKING) || (m_accepted == STATE_PROPOSAL_ACCEPTED_AND_FIXED);
		}

		// -------------------------------------------
		/* 
		 * IsActiveOffer
		 */
		public bool IsActiveOffer()
		{
			return (m_active == ACTIVE_VALID);
		}

		// -------------------------------------------
		/* 
		 * IsDisplayable
		 */
		public bool IsDisplayable()
		{
			return (m_type != TYPE_REPORT);
		}



		// -------------------------------------------
		/* 
		 * CanBeRemoved
		 */
		public bool CanBeRemoved()
		{
			return (m_reported.Length == 0);
		}


		// -------------------------------------------
		/* 
		 * CanBeReported
		 */
		public bool CanBeReported()
		{
			return ((m_confirmedReported == 0) && (m_reported.Length == 0));
		}

	}
}