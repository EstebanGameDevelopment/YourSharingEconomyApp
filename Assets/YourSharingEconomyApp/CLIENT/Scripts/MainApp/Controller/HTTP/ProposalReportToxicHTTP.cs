using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class ProposalReportToxicHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "ProposalReportToxic.php";

		private string m_proposalID;
		private string m_reporterID;
		private string m_requestID;

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			m_proposalID = (string)_list[2];
			m_reporterID = (string)_list[3];
			m_requestID = (string)_list[4];

			return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&proposal=" + m_proposalID + "&reporter=" + m_reporterID + "&request=" + m_requestID;
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_REPORT_PROPOSAL, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_REPORT_PROPOSAL, true, long.Parse(m_proposalID), int.Parse(m_reporterID));
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_REPORT_PROPOSAL, false);
			}
		}
	}

}