using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class RequestSetAsFinishedHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = ScreenController.URL_BASE_PHP + "RequestUpdateAsFinished.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{

			return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&request=" + (string)_list[2] + "&broken=" + (string)_list[3] + "&language=" + LanguageController.Instance.CodeLanguage;
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_SET_JOB_AS_FINISHED, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_SET_JOB_AS_FINISHED, true, long.Parse(data[1]));
			}
			else
			{
				BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_SET_JOB_AS_FINISHED, false);
			}
		}
	}

}