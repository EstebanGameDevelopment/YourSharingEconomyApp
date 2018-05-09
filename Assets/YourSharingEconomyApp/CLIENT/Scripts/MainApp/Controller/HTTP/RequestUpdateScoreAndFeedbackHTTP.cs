﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public class RequestUpdateScoreAndFeedbackHTTP : BaseDataHTTP, IHTTPComms
{
    private string m_urlRequest = ScreenController.URL_BASE_PHP + "RequestUpdateScore.php";

    public string UrlRequest
    {
        get { return m_urlRequest; }
    }

    public string Build(params object[] _list)
    {
        m_method = METHOD_POST;

        m_formPost = new WWWForm();
        m_formPost.AddField("id", (string)_list[0]);
        m_formPost.AddField("password", (string)_list[1]);

        m_formPost.AddField("request", (string)_list[2]);
        m_formPost.AddField("scorecustomer", (string)_list[3]);
        m_formPost.AddField("feedbackcustomer", (string)_list[4]);
        m_formPost.AddField("scoreprovider", (string)_list[5]);
        m_formPost.AddField("feedbackprovider", (string)_list[6]);

        return null;
    }

    public override void Response(byte[] _response)
    {
        if (!ResponseCode(_response))
        {
            CommController.Instance.DisplayLog(m_jsonResponse);
            BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_SCORE_AND_FEEDBACK_UPDATE, false);
            return;
        }

        string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
        if (bool.Parse(data[0]))
        {
            BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_SCORE_AND_FEEDBACK_UPDATE, true, long.Parse(data[1]));
        }
        else
        {
            BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_RESULT_SCORE_AND_FEEDBACK_UPDATE, false);
        }
    }
}
