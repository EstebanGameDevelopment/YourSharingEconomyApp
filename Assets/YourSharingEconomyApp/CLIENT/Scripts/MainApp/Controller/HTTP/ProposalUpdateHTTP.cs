using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class ProposalUpdateHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "ProposalUpdate.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			m_method = METHOD_POST;

			m_formPost = new WWWForm();
			m_formPost.AddField("language", LanguageController.Instance.CodeLanguage);
			m_formPost.AddField("id", (string)_list[0]);
			m_formPost.AddField("password", (string)_list[1]);

			m_formPost.AddField("proposal", (string)_list[2]);
			m_formPost.AddField("request", (string)_list[3]);
			m_formPost.AddField("price", (string)_list[4]);
			m_formPost.AddField("deadline", (string)_list[5]);
			m_formPost.AddField("provider", (string)_list[6]);
			m_formPost.AddField("accepted", (string)_list[7]);

			return null;
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommsHTTPConstants.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_UPDATE_PROPOSAL, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_UPDATE_PROPOSAL, true);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_UPDATE_PROPOSAL, false);
			}
		}
	}

}