using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class ProposalCreateNewHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "ProposalCreate.php";

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
			m_formPost.AddField("user", (string)_list[3]);
			m_formPost.AddField("request", (string)_list[4]);
			m_formPost.AddField("type", (string)_list[5]);
			m_formPost.AddField("title", CommController.FilterSpecialTokens((string)_list[6]));
			m_formPost.AddField("description", CommController.FilterSpecialTokens((string)_list[7]));
			m_formPost.AddField("price", (string)_list[8]);
			m_formPost.AddField("deadline", (string)_list[9]);
			m_formPost.AddField("accepted", (string)_list[10]);

			return null;
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_INSERTED_PROPOSAL, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_INSERTED_PROPOSAL, true, long.Parse(data[1]), int.Parse(data[2]), long.Parse(data[3]), data[4]);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_RESULT_INSERTED_PROPOSAL, false);
			}
		}
	}
}