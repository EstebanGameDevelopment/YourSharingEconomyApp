using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GetConfigurationServerParametersHTTP : BaseDataHTTP, IHTTPComms
{
    private string m_urlRequest = ScreenController.URL_BASE_PHP + "GetConfigurationServerParameters.php";

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
            ScreenController.Instance.TotalReportsToWarnRequest = int.Parse(data[1]);
            ScreenController.Instance.HoursToEnableANewProposal = int.Parse(data[2]);
            ScreenController.Instance.TotalNumberOfFreeRequests = int.Parse(data[3]);
            ScreenController.Instance.TotalNumberImagesAsReference = int.Parse(data[4]);
            ScreenController.Instance.TotalNumberImagesAsFinished = int.Parse(data[5]);
            ScreenController.Instance.TotalNumberImagesAsProviderExperience = int.Parse(data[6]);
            ScreenController.Instance.SizeHeightAllowedImages = int.Parse(data[7]);
            ScreenController.Instance.ProviderSkills = data[8];
            if (ScreenController.Instance.DebugMode)
            {
                Debug.Log("TOTAL_REPORT_REQUEST_TO_WARN_USERS=" + ScreenController.Instance.TotalReportsToWarnRequest);
                Debug.Log("HOURS_TO_WAIT_FOR_NEW_PROPOSAL=" + ScreenController.Instance.HoursToEnableANewProposal);
                Debug.Log("FREE_REQUESTS_AVAILABLE_TO_CONSUMERS=" + ScreenController.Instance.TotalNumberOfFreeRequests);
                Debug.Log("TOTAL_NUMBER_IMAGES_AS_REFERENCE=" + ScreenController.Instance.TotalNumberImagesAsReference);
                Debug.Log("TOTAL_NUMBER_IMAGES_AS_FINISHED=" + ScreenController.Instance.TotalNumberImagesAsFinished);
                Debug.Log("TOTAL_NUMBER_IMAGES_PROVIDER_EXPERIENCE=" + ScreenController.Instance.TotalNumberImagesAsProviderExperience);
                Debug.Log("SIZE_HEIGHT_ALLOWED_IMAGES=" + ScreenController.Instance.SizeHeightAllowedImages);
                Debug.Log("PROVIDER_SKILLS=" + ScreenController.Instance.ProviderSkills);
            }
            BasicEventController.Instance.DispatchBasicEvent(ScreenInitialView.EVENT_CONFIGURATION_DATA_RECEIVED);
        }
    }
}
