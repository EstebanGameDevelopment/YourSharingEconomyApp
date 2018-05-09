using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public class RequestConsultByDistanceHTTP : BaseDataHTTP, IHTTPComms
{
    private string m_urlRequest = ScreenController.URL_BASE_PHP + "RequestConsultByDistance.php";

    public string UrlRequest
    {
        get { return m_urlRequest; }
    }

    public string Build(params object[] _list)
    {
        return "?id=" + (string)_list[0] 
               +"&password=" + (string)_list[1]
               +"&latitude=" + (string)_list[2]
               +"&longitude=" + (string)_list[3]
               +"&distance=" + (string)_list[4]
               +"&state=" + (string)_list[5];
    }

    public override void Response(string _response)
    {
        if (!ResponseCode(_response))
        {
            CommController.Instance.DisplayLog(m_jsonResponse);
            BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_CONSULT_DISTANCE_RECORDS);
            return;
        }

        BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_CONSULT_DISTANCE_RECORDS, m_jsonResponse);
    }
}

