﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class ImageConsultAllHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "ImagesConsultAll.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&origin=" + (string)_list[2] + "&tabla=" + (string)_list[3];
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommsHTTPConstants.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_RESULT_CONSULT_IMAGES_REQUEST);
				return;
			}

			UIEventController.Instance.DispatchUIEvent(RequestsController.EVENT_REQUEST_RESULT_CONSULT_IMAGES_REQUEST, m_jsonResponse);
		}
	}

}