﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class ProposalRemoveHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "ProposalDelete.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&proposal=" + (string)_list[2];
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommsHTTPConstants.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_DELETE_PROPOSAL, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_DELETE_PROPOSAL, true);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_DELETE_PROPOSAL, false);
			}
		}
	}

}