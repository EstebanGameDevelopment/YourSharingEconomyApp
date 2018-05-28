using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * ProposalsController
	 * 
	 * It manages all the operations related with the proposals' management
	 * 
	 * @author Esteban Gallardo
	 */
	public class ProposalsController : MonoBehaviour
	{
		// ----------------------------------------------
		// EVENTS
		// ----------------------------------------------
		public const string EVENT_PROPOSAL_CALL_CONSULT_PROPOSALS = "EVENT_PROPOSAL_CALL_CONSULT_PROPOSALS";
		public const string EVENT_PROPOSAL_RESULT_CONSULTED_PROPOSALS = "EVENT_PROPOSAL_RESULT_CONSULTED_PROPOSALS";
		public const string EVENT_PROPOSAL_RESULT_FORMATTED_PROPOSALS = "EVENT_PROPOSAL_RESULT_FORMATTED_PROPOSALS";

		public const string EVENT_PROPOSAL_CALL_INSERT_NEW_PROPOSAL = "EVENT_PROPOSAL_CALL_INSERT_NEW_PROPOSAL";
		public const string EVENT_PROPOSAL_RESULT_INSERTED_PROPOSAL = "EVENT_PROPOSAL_RESULT_INSERTED_PROPOSAL";

		public const string EVENT_PROPOSAL_CALL_DELETE_PROPOSAL = "EVENT_PROPOSAL_CALL_DELETE_PROPOSAL";
		public const string EVENT_PROPOSAL_RESULT_DELETE_PROPOSAL = "EVENT_PROPOSAL_RESULT_DELETE_PROPOSAL";

		public const string EVENT_PROPOSAL_CALL_UPDATE_PROPOSAL = "EVENT_PROPOSAL_CALL_UPDATE_PROPOSAL";
		public const string EVENT_PROPOSAL_RESULT_UPDATE_PROPOSAL = "EVENT_PROPOSAL_RESULT_UPDATE_PROPOSAL";

		public const string EVENT_PROPOSAL_CALL_RESET_ALL_PROPOSALS = "EVENT_PROPOSAL_CALL_RESET_ALL_PROPOSALS";
		public const string EVENT_PROPOSAL_RESULT_RESET_ALL_PROPOSALS = "EVENT_PROPOSAL_RESULT_RESET_ALL_PROPOSALS";

		public const string EVENT_PROPOSAL_CALL_REACTIVATE_PROPOSAL = "EVENT_PROPOSAL_CALL_REACTIVATE_PROPOSAL";
		public const string EVENT_PROPOSAL_RESULT_REACTIVATE_PROPOSAL = "EVENT_PROPOSAL_RESULT_REACTIVATE_PROPOSAL";

		public const string EVENT_PROPOSAL_CALL_REPORT_PROPOSAL = "EVENT_PROPOSAL_CALL_REPORT_PROPOSAL";
		public const string EVENT_PROPOSAL_RESULT_REPORT_PROPOSAL = "EVENT_PROPOSAL_RESULT_REPORT_PROPOSAL";

		// ----------------------------------------------
		// CONSTANTS
		// ----------------------------------------------

		// ----------------------------------------------
		// SINGLETON
		// ----------------------------------------------
		private static ProposalsController _instance;

		public static ProposalsController Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType(typeof(ProposalsController)) as ProposalsController;
					if (!_instance)
					{
						GameObject container = new GameObject();
						container.name = "ProposalsController";
						_instance = container.AddComponent(typeof(ProposalsController)) as ProposalsController;
					}
				}
				return _instance;
			}
		}

		// ----------------------------------------------
		// MEMBERS
		// ----------------------------------------------
		private List<ProposalModel> m_proposals = new List<ProposalModel>();


		// ----------------------------------------------
		// GETTERS/SETTERS
		// ----------------------------------------------

		// ----------------------------------------------
		// CONSTRUCTOR
		// ----------------------------------------------	
		// -------------------------------------------
		/* 
		 * Constructor
		 */
		private ProposalsController()
		{
		}

		// -------------------------------------------
		/* 
		 * Initialitzation
		 */
		public void Init()
		{
			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			UIEventController.Instance.UIEvent -= OnBasicEvent;
			Destroy(_instance.gameObject);
			_instance = null;
		}

		// -------------------------------------------
		/* 
		 * GetLocalProposal
		 */
		public ProposalModel GetLocalProposal(long _idProposal)
		{
			for (int i = 0; i < m_proposals.Count; i++)
			{
				if (m_proposals[i].Id == _idProposal)
				{
					return m_proposals[i];
				}
			}
			return null;
		}

		// -------------------------------------------
		/* 
		 * AddConsultDistanceRequests
		 */
		private void AddConsultProposal(long _id,
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
			ProposalModel proposal = new ProposalModel(_id,
														_user,
														_request,
														_type,
														_title,
														_description,
														_price,
														_deadline,
														_accepted,
														_active,
														_created,
														_reported,
														_confirmedReported);
			m_proposals.Add(proposal);
		}

		// -------------------------------------------
		/* 
		 * ParseRecordsData
		 */
		private void ParseRecordsData(string _buf)
		{
			m_proposals.Clear();

			string data = _buf;
			if (data.Length > 5)
			{

				string[] lines = data.Split(new string[] { CommController.TOKEN_SEPARATOR_LINES }, StringSplitOptions.None);
				for (int i = 0; i < lines.Length; i++)
				{
					string[] tokens = lines[i].Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
					if (tokens.Length == 13)
					{
						long id = long.Parse(tokens[0]);
						long user = long.Parse(tokens[1]);
						long request = long.Parse(tokens[2]);
						int type = int.Parse(tokens[3]);
						string title = tokens[4];
						string description = tokens[5];
						int price = int.Parse(tokens[6]);
						long deadline = long.Parse(tokens[7]);
						int accepted = int.Parse(tokens[8]);
						int active = int.Parse(tokens[9]);
						long created = long.Parse(tokens[10]);
						string reported = tokens[11];
						int confirmedReported = int.Parse(tokens[12]);

						AddConsultProposal(id, user, request, type, title, description, price, deadline, accepted, active, created, reported, confirmedReported);
					}
				}
			}
			UIEventController.Instance.DispatchUIEvent(EVENT_PROPOSAL_RESULT_FORMATTED_PROPOSALS, m_proposals);
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (_nameEvent == EVENT_PROPOSAL_CALL_INSERT_NEW_PROPOSAL)
			{
				ProposalModel proposal = (ProposalModel)_list[0];
				RequestsController.Instance.MustReloadRequests = true;
				CommController.Instance.CreateNewProposal(UsersController.Instance.CurrentUser.Id.ToString(), UsersController.Instance.CurrentUser.Password, proposal);
			}
			if (_nameEvent == EVENT_PROPOSAL_CALL_CONSULT_PROPOSALS)
			{
				CommController.Instance.ConsultAllProposalsByRequest(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, (long)_list[0]);
			}
			if (_nameEvent == EVENT_PROPOSAL_RESULT_CONSULTED_PROPOSALS)
			{
				if (_list == null)
				{
					return;
				}
				if (_list.Length == 0)
				{
					return;
				}

				ParseRecordsData((string)_list[0]);
			}
			if (_nameEvent == EVENT_PROPOSAL_CALL_DELETE_PROPOSAL)
			{
				long idProposal = (long)_list[0];
				RequestsController.Instance.MustReloadRequests = true;
				CommController.Instance.RemoveProposal(UsersController.Instance.CurrentUser.Id.ToString(), UsersController.Instance.CurrentUser.Password, idProposal);
			}
			if (_nameEvent == EVENT_PROPOSAL_CALL_UPDATE_PROPOSAL)
			{
				ProposalModel proposal = (ProposalModel)_list[0];
				CommController.Instance.UpdateProposal(UsersController.Instance.CurrentUser.Id.ToString(), UsersController.Instance.CurrentUser.Password, proposal);
			}
			if (_nameEvent == EVENT_PROPOSAL_CALL_RESET_ALL_PROPOSALS)
			{
				long requestID = (long)_list[0];
				CommController.Instance.ResetProposalsForRequest(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, requestID);
			}
			if (_nameEvent == EVENT_PROPOSAL_CALL_REACTIVATE_PROPOSAL)
			{
				long proposalID = (long)_list[0];
				CommController.Instance.ReactivateProposal(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, proposalID);
			}
			if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_REACTIVATE_PROPOSAL)
			{
				bool reactivated = (bool)_list[0];
				if (reactivated)
				{
					long proposalID = (long)_list[1];
					ProposalModel proposal = GetLocalProposal(proposalID);
					if (proposal != null)
					{
						proposal.Active = 1;
					}
				}
			}
			if (_nameEvent == EVENT_PROPOSAL_CALL_REPORT_PROPOSAL)
			{
				long proposalID = (long)_list[0];
				int reporterID = (int)_list[1];
				long requestID = (long)_list[2];
				CommController.Instance.ReportProposal(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, proposalID, reporterID, requestID);
			}
			if (_nameEvent == EVENT_PROPOSAL_RESULT_REPORT_PROPOSAL)
			{
				bool success = (bool)_list[0];
				if (success)
				{
					long proposalID = (long)_list[1];
					int reporterID = (int)_list[2];
					ProposalModel proposal = GetLocalProposal(proposalID);
					if (proposal != null)
					{
						proposal.Reported = reporterID.ToString();
					}
				}
			}
		}
	}

}