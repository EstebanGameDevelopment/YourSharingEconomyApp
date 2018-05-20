using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

namespace YourSharingEconomyApp
{

	public class ImageLoadHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = ScreenController.URL_BASE_PHP + "ImageLoad.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "?id=" + (string)_list[0];
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_LOAD_SERVER_DATA_RECEIVED, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				int sizeData = int.Parse(data[6]);
				int startingPos = _response.Length - sizeData;
				byte[] dataImage = new byte[sizeData];
				Array.Copy(_response, startingPos, dataImage, 0, sizeData);
				BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_LOAD_SERVER_DATA_RECEIVED, true, long.Parse(data[1]), data[2], long.Parse(data[3]), int.Parse(data[4]), data[5], int.Parse(data[6]), dataImage);
			}
			else
			{
				BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_LOAD_SERVER_DATA_RECEIVED, false);
			}
		}
	}

}