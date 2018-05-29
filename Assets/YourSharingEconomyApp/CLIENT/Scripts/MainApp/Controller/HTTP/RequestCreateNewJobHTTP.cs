using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class RequestCreateNewJobHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "RequestCreate.php";

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

			m_formPost.AddField("request", (string)_list[2]);
			m_formPost.AddField("customer", (string)_list[3]);
			m_formPost.AddField("provider", (string)_list[4]);
			m_formPost.AddField("title", CommController.FilterSpecialTokens((string)_list[5]));
			m_formPost.AddField("description", CommController.FilterSpecialTokens((string)_list[6]));
			m_formPost.AddField("images", (string)_list[7]);
			m_formPost.AddField("referenceimg", (string)_list[8]);
			m_formPost.AddField("village", (string)_list[9]);
			m_formPost.AddField("mapdata", (string)_list[10]);
			m_formPost.AddField("latitude", (string)_list[11]);
			m_formPost.AddField("longitude", (string)_list[12]);
			m_formPost.AddField("price", (string)_list[13]);
			m_formPost.AddField("currency", (string)_list[14]);
			m_formPost.AddField("distance", (string)_list[15]);
			m_formPost.AddField("flags", (string)_list[16]);
			m_formPost.AddField("notifications", (string)_list[17]);
			m_formPost.AddField("creationdate", (string)_list[18]);
			m_formPost.AddField("deadline", (string)_list[19]);
			m_formPost.AddField("feedbackcustomer", (string)_list[20]);
			m_formPost.AddField("scorecustomer", (string)_list[21]);
			m_formPost.AddField("feedbackprovider", (string)_list[22]);
			m_formPost.AddField("scoreprovider", (string)_list[23]);

			return null;
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommsHTTPConstants.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_RESULT_CREATED_RECORD_CONFIRMATION, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_RESULT_CREATED_RECORD_CONFIRMATION, true, long.Parse(data[1]));
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_RESULT_CREATED_RECORD_CONFIRMATION, false);
			}
		}
	}

}