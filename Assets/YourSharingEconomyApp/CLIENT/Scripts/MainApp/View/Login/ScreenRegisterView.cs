using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * ScreenRegisterView
 * 
 * @author Esteban Gallardo
 */
public class ScreenRegisterView : ScreenBaseView, IBasicScreenView
{
    // ----------------------------------------------
    // SCREEN ID
    // ----------------------------------------------	
    public const string SCREEN_REGISTER = "SCREEN_REGISTER";

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------	
    private GameObject m_root;
    private Transform m_container;

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public void Initialize(params object[] _list)
    {
        m_root = this.gameObject;
        m_container = m_root.transform.Find("Content");

        m_container.Find("Button_Apply").GetComponent<Button>().onClick.AddListener(ApplyRegisterPressed);
        m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);

        m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.word");
        m_container.Find("EmailTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.email");
        m_container.Find("PasswordTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.password");
        m_container.Find("PasswordConfirmationTitle").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.confirm");
        m_container.Find("Button_Apply/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.register.word");

        if (ScreenController.Instance.DebugMode)
        {
            m_container.Find("EmailValue").GetComponent<InputField>().text = "YOUR_EMAIL_ADDRESS@YOUR_OWN_DOMAIN.COM";
            m_container.Find("PasswordValue").GetComponent<InputField>().text = "YOUR_PASSWORD";
            m_container.Find("PasswordConfirmationValue").GetComponent<InputField>().text = "YOUR_PASSWORD";
        }

        BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
    }

    // -------------------------------------------
    /* 
	 * Destroy
	 */
    public void Destroy()
    {
        BasicEventController.Instance.BasicEvent -= OnBasicEvent;
        GameObject.DestroyObject(this.gameObject);
    }

    // -------------------------------------------
    /* 
	 * ApplyRegisterPressed
	 */
    private void ApplyRegisterPressed()
    {
        string emailToCheck = m_container.Find("EmailValue").GetComponent<InputField>().text.ToLower();
        string passwordToCheck = m_container.Find("PasswordValue").GetComponent<InputField>().text.ToLower();
        string confirmationToCheck = m_container.Find("PasswordConfirmationValue").GetComponent<InputField>().text.ToLower();

        if ((emailToCheck.Length == 0) || (passwordToCheck.Length == 0) || (confirmationToCheck.Length == 0))
        {
            string titleInfoError = LanguageController.Instance.GetText("message.error");
            string descriptionInfoError = LanguageController.Instance.GetText("screen.message.logging.error");            
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
        }
        else
        {
            if (passwordToCheck != confirmationToCheck)
            {
                string titleInfoError = LanguageController.Instance.GetText("message.error");
                string descriptionInfoError = LanguageController.Instance.GetText("screen.register.mistmatch.password");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
            }
            else
            {
                string titleWait = LanguageController.Instance.GetText("screen.wait.register.title");
                string descriptionWait = LanguageController.Instance.GetText("screen.wait.register.description");
                BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_REGISTER_REQUEST, emailToCheck, passwordToCheck);
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleWait, descriptionWait, null, "");
            }
        }
    }

    // -------------------------------------------
    /* 
	 * BackPressed
	 */
    private void BackPressed()
    {
        ScreenController.Instance.CreateNewScreenNoParameters(ScreenLoginView.SCREEN_LOGIN, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
    }

    // -------------------------------------------
    /* 
	 * RegisterPressed
	 */
    private void RegisterPressed()
    {
        Destroy();
    }

    // -------------------------------------------
    /* 
	 * OnBasicEvent
	 */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (_nameEvent == UsersController.EVENT_USER_REGISTER_RESULT)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            if ((bool)_list[0])
            {
                ScreenController.Instance.CreateNewScreenNoParameters(ScreenValidationConfirmationView.SCREEN_VALIDATION_CONFIRMATION, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
            }
            else
            {
                string titleInfoError = LanguageController.Instance.GetText("message.error");
                string descriptionInfoError = LanguageController.Instance.GetText("screen.register.wrong.register");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
            }
        }
        if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
        {
            BackPressed();
        }
    }
}