﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class RequestConsultByProviderHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "RequestConsultByProvider.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&provider=" + (string)_list[2] + "&all=" + (string)_list[3];
		}

		public override void Response(string _response)
		{
			if (!ResponseCode(_response))
			{
				CommsHTTPConstants.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_RESULT_CONSULT_BY_PROVIDER_RECORDS);
				return;
			}

			UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_RESULT_CONSULT_BY_PROVIDER_RECORDS, m_jsonResponse);
		}
	}

}