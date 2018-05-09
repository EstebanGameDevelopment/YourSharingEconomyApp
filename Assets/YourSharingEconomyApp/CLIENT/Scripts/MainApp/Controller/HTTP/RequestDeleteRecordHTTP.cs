using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public class RequestDeleteRecordHTTP : BaseDataHTTP, IHTTPComms
{
    private string m_urlRequest = ScreenController.URL_BASE_PHP + "RequestDelete.php";

    public string UrlRequest
    {
        get { return m_urlRequest; }
    }

    public string Build(params object[] _list)
    {
        return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&request=" + (string)_list[2];
    }

    public override void Response(byte[] _response)
    {
        if (!ResponseCode(_response))
        {
            CommController.Instance.DisplayLog(m_jsonResponse);
            BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_DELETE_RECORDS, false);
            return;
        }

        string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
        if (bool.Parse(data[0]))
        {
            BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_DELETE_RECORDS, true, long.Parse(data[1]));
        }
        else
        {
            BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_DELETE_RECORDS, false);
        }
    }
}

