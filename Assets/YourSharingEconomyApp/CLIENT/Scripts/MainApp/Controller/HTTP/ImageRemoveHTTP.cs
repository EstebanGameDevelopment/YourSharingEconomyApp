using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class ImageRemoveHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "ImageRemove.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?id=" + (string)_list[0] + "&user=" + UsersController.Instance.CurrentUser.Id.ToString() + "&password=" + UsersController.Instance.CurrentUser.Password;
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommsHTTPConstants.DisplayLog(m_jsonResponse);
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGE_REMOVED_SERVER_CONFIRMATION, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGE_REMOVED_SERVER_CONFIRMATION, true, long.Parse(data[1]));
			}
			else
			{
				UIEventController.Instance.DispatchUIEvent(ImagesController.EVENT_IMAGE_REMOVED_SERVER_CONFIRMATION, false);
			}
		}
	}

}