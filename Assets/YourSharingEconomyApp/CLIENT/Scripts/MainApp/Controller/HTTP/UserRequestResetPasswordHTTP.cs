﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class UserRequestResetPasswordHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "UserRequestResetPassword.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?language=" + LanguageController.Instance.CodeLanguage + "&id=" + (string)_list[0];
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(ScreenProfileView.EVENT_SCREENPROFILE_SERVER_REQUEST_RESET_PASSWORD_CONFIRMATION, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(ScreenProfileView.EVENT_SCREENPROFILE_SERVER_REQUEST_RESET_PASSWORD_CONFIRMATION, true);
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ScreenProfileView.EVENT_SCREENPROFILE_SERVER_REQUEST_RESET_PASSWORD_CONFIRMATION, false);
			}
		}
	}

}