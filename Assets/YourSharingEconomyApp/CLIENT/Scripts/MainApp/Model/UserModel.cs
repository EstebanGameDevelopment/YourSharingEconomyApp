using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * UserModel
 * 
 * It keeps all the information about the user/provider
 * 
 * @author Esteban Gallardo
 */
public class UserModel
{
    // CONSTANTS
    public const char TOKEN_SEPARATOR_SKILL_PARAMETER = ';';
    public const string TOKEN_SEPARATOR_SKILL_LINE = "<skill>";

    private int m_id;
    private bool m_validated;
    private string m_email = "";
    private string m_facebook;
    private string m_nickname;
    private string m_password;
    private string m_passwordEncrypted;

    private string m_village = "";
    private string m_mapdata;

    private long m_registerdate;
    private long m_lastlogin;
    private long m_rentstart;
    private int m_rentdays;
    private int m_scoreuser;
    private int m_scoreprovider;
    private int m_votesuser;
    private int m_votesprovider;

    private string m_skills = "";
    private string m_description = "";

    private int m_additionalrequest = 0;
    private int m_additionaloffer = 0;

    private int m_banned = 0;

    private List<SkillModel> m_skillsList = null;

    private List<ImageModel> m_imageReferencesExperience = new List<ImageModel>();

    public int Id
    {
        get { return m_id; }
    }
    public bool Validated
    {
        get { return m_validated; }
    }
    public string Email
    {
        get { return m_email; }
        set {
            if (value.Length > 0)
            {
                m_email = value;
            }
        }
    }
    public string Password
    {
        get { return m_passwordEncrypted; }
    }
    public string PasswordPlain
    {
        get { return m_password; }
    }
    public string Facebook
    {
        get { return m_facebook; }
    }
    public string Nickname
    {
        get { return m_nickname; }
    }
    public string Village
    {
        get { return m_village; }
    }
    public string Mapdata
    {
        get { return m_mapdata; }
    }
    public long Registerdate
    {
        get { return m_registerdate; }
    }
    public long Lastlogin
    {
        get { return m_lastlogin; }
    }
    public long Rentstart
    {
        get { return m_rentstart; }
    }
    public int Rentdays
    {
        get { return m_rentdays; }
    }
    public int Scoreuser
    {
        get { return m_scoreuser; }
    }
    public int Scoreprovider
    {
        get { return m_scoreprovider; }
    }
    public int Votesuser
    {
        get { return m_votesuser; }
    }
    public int Votesprovider
    {
        get { return m_votesprovider; }
    }
    public string Skills
    {
        get { return m_skills; }
    }
    public List<SkillModel> SkillsList
    {
        get { return m_skillsList; }
    }
    public string Description
    {
        get { return m_description; }
        set { m_description = value; }
    }
    public List<ImageModel> ImageReferencesExperience
    {
        get { return m_imageReferencesExperience; }
    }
    public int Additionalrequest
    {
        get { return m_additionalrequest; }
        set { m_additionalrequest = value; }
    }
    public int Additionaloffer
    {
        get { return m_additionaloffer; }
        set { m_additionaloffer = value; }
    }
    public int Banned
    {
        get { return m_banned; }
    }

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public UserModel(string _email)
    {
        Email = _email;
        m_id = -1;
        m_nickname = "";
        m_facebook = "";
        m_village = "";
        m_mapdata = "";
        m_description = "";
        m_skills = "";
        m_banned = 0;
    }

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public UserModel(string _id,
                    string _nickname,
                    string _village,
                    string _mapdata,
                    string _registerdate,
                    string _lastlogin,
                    string _rentstart,
                    string _rentdays,
                    string _scoreuser,
                    string _scoreprovider,
                    string _votesuser,
                    string _votesprovider,
                    string _validated,
                    string _skills,
                    string _description,
                    string _additionalrequest,
                    string _additionaloffer,
                    string _banned)

    {
        m_id = int.Parse(_id);
        m_nickname = _nickname;
        m_village = _village;
        m_mapdata = _mapdata;
        m_registerdate = long.Parse(_registerdate);
        m_lastlogin = long.Parse(_lastlogin);
        m_rentstart = long.Parse(_rentstart);
        m_rentdays = int.Parse(_rentdays);
        m_scoreuser = int.Parse(_scoreuser);
        m_scoreprovider = int.Parse(_scoreprovider);
        m_votesuser = int.Parse(_votesuser);
        m_votesprovider = int.Parse(_votesprovider);
        m_validated = (int.Parse(_validated) == 1);
        m_skills = _skills;
        m_description = _description;
        m_additionalrequest = int.Parse(_additionalrequest);
        m_additionaloffer = int.Parse(_additionaloffer);
        m_banned = int.Parse(_banned);

        GetFormattedSkills();
    }

    // -------------------------------------------
    /* 
	 * SetPassword
	 */
    public void SetPassword(string _password)
    {
        m_password = _password;
        m_passwordEncrypted = RJEncryptor.Encrypt(_password, false);
        PlayerPrefs.SetString(ScreenController.USER_PASSWORD_COOCKIE, RJEncryptor.Encrypt(_password, false));
    }

    // -------------------------------------------
    /* 
	 * UpdateBasicInfo
	 */
    public void UpdateBasicInfo(string _email, string _password)
    {
        Email = _email;
        m_password = _password;
        m_passwordEncrypted = RJEncryptor.Encrypt(_password, false);

        PlayerPrefs.SetString(ScreenController.USER_EMAIL_COOCKIE, Email.ToLower());
        PlayerPrefs.SetString(ScreenController.USER_PASSWORD_COOCKIE, RJEncryptor.Encrypt(_password, false));
    }

    // -------------------------------------------
    /* 
	 * UpdateBasicInfo
	 */
    public void ValidateUser(bool _validated)
    {
        m_validated = _validated;
    }

    // -------------------------------------------
    /* 
	 * SetLocation
	 */
    public void SetLocation(string _village, string _mapdata)
    {
        m_village = _village;
        m_mapdata = _mapdata;
    }

    // -------------------------------------------
    /* 
     * UpdateModel
     */
    public void UpdateModel(bool _enableFacebook,
                            string _email,
                            string _id,
                            string _nickname,
                            string _village,
                            string _mapdata,
                            string _registerdate,
                            string _lastlogin,
                            string _rentstart,
                            string _rentdays,
                            string _scoreuser,
                            string _scoreprovider,
                            string _votesuser,
                            string _votesprovider,
                            string _validated,
                            string _skills,
                            string _description,
                            string _additionalrequest,
                            string _additionaloffer,
                            string _banned)
    {
        Email = _email;
        m_id = int.Parse(_id);
        m_nickname = _nickname;
        m_village = _village;
        m_mapdata = _mapdata;
        m_registerdate = long.Parse(_registerdate);
        m_lastlogin = long.Parse(_lastlogin);
        m_rentstart = long.Parse(_rentstart);
        m_rentdays = int.Parse(_rentdays);
        m_scoreuser = int.Parse(_scoreuser);
        m_scoreprovider = int.Parse(_scoreprovider);
        m_votesuser = int.Parse(_votesuser);
        m_votesprovider = int.Parse(_votesprovider);
        m_validated = (int.Parse(_validated) == 1);
        m_skills = _skills;
        m_description = _description;
        m_additionalrequest = int.Parse(_additionalrequest);
        m_additionaloffer = int.Parse(_additionaloffer);
        m_banned = int.Parse(_banned);

        GetFormattedSkills();

        if (_enableFacebook)
        {
            SaveLocalFacebookLogin(FacebookController.Instance.Id, m_nickname, Email);
        }
        else
        {
            SaveLocalEmailLogin(Email, m_password);
        }
    }

    // -------------------------------------------
    /* 
     * UpdateProfile
     */
    public void UpdateProfile(string _password,
                            string _email,
                            string _name,
                            string _village,
                            string _mapdata,
                            string _skills,
                            string _description)
    {
        m_password = _password;
        m_passwordEncrypted = RJEncryptor.Encrypt(_password, false);
        Email = _email;
        m_nickname = _name;
        m_village = _village;
        m_mapdata = _mapdata;
        m_skills = _skills;
        m_description = _description;
        
        GetFormattedSkills();

        UpdateLocalProfile(m_password, Email, m_nickname);
    }


    // -------------------------------------------
    /* 
	 * SaveEmailLoginLocal
	 */
    private void SaveLocalEmailLogin(string _email, string _password)
    {
        PlayerPrefs.SetString(ScreenController.USER_EMAIL_COOCKIE, RJEncryptor.Encrypt(_email.ToLower(), false));
        PlayerPrefs.SetString(ScreenController.USER_PASSWORD_COOCKIE, RJEncryptor.Encrypt(_password, false));
        PlayerPrefs.SetInt(ScreenController.USER_FACEBOOK_CONNECTED_COOCKIE, 0);
    }

    // -------------------------------------------
    /* 
     * SaveFacebookLoginLocal
     */
    private void SaveLocalFacebookLogin(string _facebookId, string _name, string _email)
    {
        PlayerPrefs.SetString(ScreenController.USER_EMAIL_COOCKIE, RJEncryptor.Encrypt(_email.ToLower(), false));
        PlayerPrefs.SetString(ScreenController.USER_NAME_COOCKIE, _name);
        PlayerPrefs.SetInt(ScreenController.USER_FACEBOOK_CONNECTED_COOCKIE, 1);
    }

    // -------------------------------------------
    /* 
     * SaveFacebookLoginLocal
     */
    private void UpdateLocalProfile(string _password, string _email, string _name)
    {
        PlayerPrefs.SetString(ScreenController.USER_PASSWORD_COOCKIE, RJEncryptor.Encrypt(_password, false));
        PlayerPrefs.SetString(ScreenController.USER_EMAIL_COOCKIE, RJEncryptor.Encrypt(_email.ToLower(), false));
        PlayerPrefs.SetString(ScreenController.USER_NAME_COOCKIE, _name);
    }

    // -------------------------------------------
    /* 
     * IsProvider
     */
    public bool IsProvider()
    {
        return (m_skills.Length > 0) && IsEnabledAsProvider();
    }

    // -------------------------------------------
    /* 
     * GetFormattedSkills
     */
    public void GetFormattedSkills()
    {
        m_skillsList = new List<SkillModel>();

        if (m_skills.Length == 0)
        {
            m_skills = ScreenController.Instance.ProviderSkills;
        }

        string[] skillItem = m_skills.Split(new string[] { TOKEN_SEPARATOR_SKILL_LINE }, StringSplitOptions.None);

        for (int i = 0; i < skillItem.Length; i++)
        {
            string[] dataItem = skillItem[i].Split(TOKEN_SEPARATOR_SKILL_PARAMETER);
            SkillModel newSkill = new SkillModel(dataItem[0], int.Parse(dataItem[1]));
            m_skillsList.Add(newSkill);
        }
    }

    // -------------------------------------------
    /* 
     * SetFormattedSkills
     */
    private void SetFormattedSkills()
    {
        m_skills = "";
        for (int i = 0; i < m_skillsList.Count; i++)
        {
            SkillModel dataItem = m_skillsList[i];
            m_skills += dataItem.Name + TOKEN_SEPARATOR_SKILL_PARAMETER + dataItem.Value;
            if (i + 1 < m_skillsList.Count)
            {
                m_skills += TOKEN_SEPARATOR_SKILL_LINE;
            }
        }
    }

    // -------------------------------------------
    /* 
     * UpdateSingleSkill
     */
    public void UpdateSingleSkill(string _nameSkill, int _value)
    {
        for (int i = 0; i < m_skillsList.Count; i++)
        {
            SkillModel dataItem = m_skillsList[i];
            if (dataItem.Name == _nameSkill)
            {
                dataItem.Value = _value;
            }
        }
        SetFormattedSkills();
    }

    // -------------------------------------------
    /* 
    * Copy
    */
    public void Copy(UserModel _user)
    {
        Email = _user.Email;
        m_id = _user.Id;
        m_nickname = _user.Nickname;
        m_village = _user.Village;
        m_mapdata = _user.Mapdata;
        m_registerdate = _user.Registerdate;
        m_lastlogin = _user.Lastlogin;
        m_rentstart = _user.Rentstart;
        m_rentdays = _user.Rentdays;
        m_scoreuser = _user.Scoreuser;
        m_scoreprovider = _user.Scoreprovider;
        m_votesuser = _user.Votesuser;
        m_votesprovider = _user.Votesprovider;
        m_validated = _user.Validated;
        m_skills = _user.Skills;
        m_description = _user.Description;
        m_additionalrequest = _user.Additionalrequest;
        m_additionaloffer = _user.Additionaloffer;
        m_banned = _user.Banned;

        GetFormattedSkills();

        CopyImages(_user.ImageReferencesExperience);
    }

    // -------------------------------------------
    /* 
    * CopyImages
    */
    public void CopyImages(List<ImageModel> _data)
    {
        if (_data != null)
        {
            if (_data.Count > 0)
            {
                m_imageReferencesExperience.Clear();
                for (int i = 0; i < _data.Count; i++)
                {
                    m_imageReferencesExperience.Add(_data[i].Clone());
                }
            }
        }
    }

    // -------------------------------------------
    /* 
    * IsEnabledAsProvider
    */
    private bool IsEnabledAsProvider()
    {
        if (m_rentstart != -1)
        {
            int daysConsumed = DateConverter.GetDaysFromSeconds(DateConverter.GetTimestamp() - m_rentstart);
            if (m_rentdays != -1)
            {
                int daysLeft = m_rentdays - daysConsumed;
                if (daysLeft > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // -------------------------------------------
    /* 
    * IsBanned
    */
    public bool IsBanned()
    {
        return (m_banned == 1);
    }
}