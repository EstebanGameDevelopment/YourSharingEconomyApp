using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class UserLoginByEmailHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "UserLoginByEmail.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?email=" + (string)_list[0] + "&password=" + (string)_list[1];
		}

		public override void Response(string _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_LOGIN_RESULT, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_LOGIN_RESULT, true, data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19]);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_LOGIN_RESULT, false);
			}
		}
	}
}