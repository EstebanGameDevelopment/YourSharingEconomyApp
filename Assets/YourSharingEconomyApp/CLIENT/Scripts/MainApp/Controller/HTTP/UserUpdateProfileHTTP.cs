using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public class UserUpdateProfileHTTP : BaseDataHTTP, IHTTPComms
{
    private string m_urlRequest = ScreenController.URL_BASE_PHP + "UserUpdateProfile.php";

    private string m_newPassword;
    private string m_newEmail;
    private string m_newName;
    private string m_newVillage;
    private string m_newMapData;
    private string m_newSkills;
    private string m_newDescription;

    public string UrlRequest
    {
        get { return m_urlRequest; }
    }

    public string Build(params object[] _list)
    {
        m_method = METHOD_POST;

        m_formPost = new WWWForm();
        m_formPost.AddField("id", (string)_list[0]);
        m_formPost.AddField("currpass", (string)_list[1]);
        m_formPost.AddField("password", (string)_list[2]);
        m_formPost.AddField("email", (string)_list[3]);
        m_formPost.AddField("name", (string)_list[4]);
        m_formPost.AddField("village", (string)_list[5]);
        m_formPost.AddField("mapdata", (string)_list[6]);
        m_formPost.AddField("skills", (string)_list[7]);
        m_formPost.AddField("description", CommController.FilterSpecialTokens(((string)_list[8])));
        
        m_newPassword = (string)_list[2];
        m_newEmail = (string)_list[3];
        m_newName = (string)_list[4];
        m_newVillage = (string)_list[5];
        m_newMapData = (string)_list[6];
        m_newSkills = (string)_list[7];
        m_newDescription = (string)_list[8];

        return null;
    }

    public override void Response(byte[] _response)
    {
        if (!ResponseCode(_response))
        {
            CommController.Instance.DisplayLog(m_jsonResponse);
            BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_UPDATE_PROFILE_RESULT, false);
            return;
        }

        string[] data = m_jsonResponse.Split(new string[] { CommController.TOKEN_SEPARATOR_EVENTS }, StringSplitOptions.None);
        if (bool.Parse(data[0]))
        {
            BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_UPDATE_PROFILE_RESULT, true, m_newPassword, m_newEmail, m_newName, m_newVillage, m_newMapData, m_newSkills, m_newDescription);
        }
        else
        {
            BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_UPDATE_PROFILE_RESULT, false);
        }
    }
}

