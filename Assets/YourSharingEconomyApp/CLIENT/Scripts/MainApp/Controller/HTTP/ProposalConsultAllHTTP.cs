﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public class ProposalConsultAllHTTP : BaseDataHTTP, IHTTPComms
{
    private string m_urlRequest = ScreenController.URL_BASE_PHP + "ProposalsConsultRequestAll.php";

    public string UrlRequest
    {
        get { return m_urlRequest; }
    }

    public string Build(params object[] _list)
    {
        return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&request=" + (string)_list[2];
    }

    public override void Response(string _response)
    {
        if (!ResponseCode(_response))
        {
            CommController.Instance.DisplayLog(m_jsonResponse);
            BasicEventController.Instance.DispatchBasicEvent(ProposalsController.EVENT_PROPOSAL_RESULT_CONSULTED_PROPOSALS);
            return;
        }

        BasicEventController.Instance.DispatchBasicEvent(ProposalsController.EVENT_PROPOSAL_RESULT_CONSULTED_PROPOSALS, m_jsonResponse);
    }
}
