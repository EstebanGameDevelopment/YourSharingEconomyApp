using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class IAPPremiumRequestHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "IAPPremiumRequest.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&code=" + (string)_list[2];
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_IAP_RESULT_PURCHASE_NEW_REQUEST, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_IAP_RESULT_PURCHASE_NEW_REQUEST, true);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_IAP_RESULT_PURCHASE_NEW_REQUEST, false);
			}
		}
	}

}