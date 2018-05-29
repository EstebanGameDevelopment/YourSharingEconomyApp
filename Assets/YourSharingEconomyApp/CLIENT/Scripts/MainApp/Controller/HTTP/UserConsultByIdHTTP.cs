using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class UserConsultByIdHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "UserConsult.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		static string DecodeToString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

		public string Build(params object[] _list)
		{
			return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&user=" + (string)_list[2];
		}

		public override void Response(string _response)
		{
			if (!ResponseCode(_response))
			{
				CommsHTTPConstants.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_RESULT_CONSULT_SINGLE_RECORD);
				return;
			}

			UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_RESULT_CONSULT_SINGLE_RECORD, m_jsonResponse);
		}
	}
}