using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class UserRequestResetByEmailPasswordHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "UserRequestResetByEmailPassword.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?language=" + LanguageController.Instance.CodeLanguage + "&email=" + (string)_list[0];
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommsHTTPConstants.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_RESULT_RESETED_PASSWORD_BY_EMAIL, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_RESULT_RESETED_PASSWORD_BY_EMAIL, true);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_RESULT_RESETED_PASSWORD_BY_EMAIL, false);
			}
		}
	}

}