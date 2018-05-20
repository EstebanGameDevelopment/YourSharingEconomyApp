using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YourSharingEconomyApp
{

	public class UserRegisterWithEmailHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = ScreenController.URL_BASE_PHP + "UserRegisterByEmail.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?language=" + LanguageController.Instance.CodeLanguage + "&email=" + (string)_list[0] + "&password=" + (string)_list[1];
		}

		public override void Response(string _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_REGISTER_RESULT, false);
				return;
			}



			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_REGISTER_RESULT, true, data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19]);
			}
			else
			{
				BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_REGISTER_RESULT, false);
			}
		}
	}
}