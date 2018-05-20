using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

namespace YourSharingEconomyApp
{

	public class RequestUpdateImageRefHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = ScreenController.URL_BASE_PHP + "RequestUpdateImageReference.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&request=" + (string)_list[2] + "&imgreference=" + (string)_list[3];
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_UPDATE_IMG_REF, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_UPDATE_IMG_REF, true);
			}
			else
			{
				BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_UPDATE_IMG_REF, false);
			}
		}
	}

}