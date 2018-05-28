using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class RequestUpdateTransactionHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "RequestUpdateTransaction.php";

		private string m_transactionID;

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			m_transactionID = (string)_list[3];
			return "?id=" + (string)_list[0]
					+ "&password=" + (string)_list[1]
					+ "&request=" + (string)_list[2]
					+ "&transaction=" + (string)_list[3];
		}		

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_TRANSACTION_REGISTERED_RESPONSE, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_TRANSACTION_REGISTERED_RESPONSE, true, m_transactionID);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_TRANSACTION_REGISTERED_RESPONSE, false);
			}
		}
	}

}