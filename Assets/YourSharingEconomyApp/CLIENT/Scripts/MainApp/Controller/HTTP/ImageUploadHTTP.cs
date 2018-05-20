using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

namespace YourSharingEconomyApp
{

	public class ImageUploadHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = ScreenController.URL_BASE_PHP + "ImageUpload.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			m_method = METHOD_POST;

			m_formPost = new WWWForm();
			m_formPost.AddField("id", (string)_list[0]);
			m_formPost.AddField("table", (string)_list[1]);
			m_formPost.AddField("idorigin", (string)_list[2]);
			m_formPost.AddField("type", (string)_list[3]);
			m_formPost.AddField("url", (string)_list[4]);
			m_formPost.AddField("user", UsersController.Instance.CurrentUser.Id.ToString());
			m_formPost.AddField("password", UsersController.Instance.CurrentUser.Password);

			byte[] imageData = (byte[])_list[5];
			m_formPost.AddField("size", imageData.Length);
			m_formPost.AddBinaryData("data", imageData);

			return null;
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION, false);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION, true, long.Parse(data[1]), data[2], long.Parse(data[3]));
			}
			else
			{
				BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION, false);
			}
		}
	}

}