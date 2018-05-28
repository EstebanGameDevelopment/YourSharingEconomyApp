using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	public class GetConfigurationServerParametersHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest = MenusScreenController.URL_BASE_PHP + "GetConfigurationServerParameters.php";

		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public string Build(params object[] _list)
		{
			return "";
		}

		public override void Response(byte[] _response)
		{
			if (!ResponseCode(_response))
			{
				CommController.Instance.DisplayLog(m_jsonResponse);
				return;
			}

			string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
			if (bool.Parse(data[0]))
			{
				MenusScreenController.Instance.TotalReportsToWarnRequest = int.Parse(data[1]);
				MenusScreenController.Instance.HoursToEnableANewProposal = int.Parse(data[2]);
				MenusScreenController.Instance.TotalNumberOfFreeRequests = int.Parse(data[3]);
				MenusScreenController.Instance.TotalNumberImagesAsReference = int.Parse(data[4]);
				MenusScreenController.Instance.TotalNumberImagesAsFinished = int.Parse(data[5]);
				MenusScreenController.Instance.TotalNumberImagesAsProviderExperience = int.Parse(data[6]);
				MenusScreenController.Instance.SizeHeightAllowedImages = int.Parse(data[7]);
				MenusScreenController.Instance.ProviderSkills = data[8];
				if (MenusScreenController.Instance.DebugMode)
				{
					Debug.Log("TOTAL_REPORT_REQUEST_TO_WARN_USERS=" + MenusScreenController.Instance.TotalReportsToWarnRequest);
					Debug.Log("HOURS_TO_WAIT_FOR_NEW_PROPOSAL=" + MenusScreenController.Instance.HoursToEnableANewProposal);
					Debug.Log("FREE_REQUESTS_AVAILABLE_TO_CONSUMERS=" + MenusScreenController.Instance.TotalNumberOfFreeRequests);
					Debug.Log("TOTAL_NUMBER_IMAGES_AS_REFERENCE=" + MenusScreenController.Instance.TotalNumberImagesAsReference);
					Debug.Log("TOTAL_NUMBER_IMAGES_AS_FINISHED=" + MenusScreenController.Instance.TotalNumberImagesAsFinished);
					Debug.Log("TOTAL_NUMBER_IMAGES_PROVIDER_EXPERIENCE=" + MenusScreenController.Instance.TotalNumberImagesAsProviderExperience);
					Debug.Log("SIZE_HEIGHT_ALLOWED_IMAGES=" + MenusScreenController.Instance.SizeHeightAllowedImages);
					Debug.Log("PROVIDER_SKILLS=" + MenusScreenController.Instance.ProviderSkills);
				}
				UIEventController.Instance.DispatchUIEvent(ScreenInitialView.EVENT_CONFIGURATION_DATA_RECEIVED);
			}
		}
	}
}