using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public class IAPPremiumPostOfferHTTP : BaseDataHTTP, IHTTPComms
{
    private string m_urlRequest = ScreenController.URL_BASE_PHP + "IAPPremiumPostOffer.php";

    public string UrlRequest
    {
        get { return m_urlRequest; }
    }

    public string Build(params object[] _list)
    {
        return "?id=" + (string)_list[0] + "&password=" + (string)_list[1] + "&code=" + (string)_list[2];
    }

    public override void Response(byte[] _response)
    {
        if (!ResponseCode(_response))
        {
            CommController.Instance.DisplayLog(m_jsonResponse);
            BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_IAP_RESULT_PURCHASE_POST_OFFER, false);
            return;
        }

        string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
        if (bool.Parse(data[0]))
        {
            BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_IAP_RESULT_PURCHASE_POST_OFFER, true);
        }
        else
        {
            BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_IAP_RESULT_PURCHASE_POST_OFFER, false);
        }
    }
}

