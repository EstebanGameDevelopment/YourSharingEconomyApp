using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * ScreenTypeMediaView
 * 
 * It allows the users to select between select a file or enter an URL for the reference images
 * 
 * @author Esteban Gallardo
 */
public class ScreenTypeMediaView : ScreenBaseView, IBasicScreenView
{
    public const string SCREEN_TYPE_MEDIA = "SCREEN_TYPE_MEDIA";

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

        m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.type.media.title");

        m_container.Find("Button_AddImage").GetComponent<Button>().onClick.AddListener(OnAddTypeImage);
        m_container.Find("Button_AddImage/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.type.media.add.image");
        m_container.Find("Button_AddLink").GetComponent<Button>().onClick.AddListener(OnAddTypeLink);
        m_container.Find("Button_AddLink/Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.type.media.add.url");

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
	 * OnAddTypeImage
	 */
    private void OnAddTypeImage()
    {
        bool displayFilebrowser = true;
#if ENABLED_FACEBOOK
        displayFilebrowser = false;
#endif

        if (displayFilebrowser)
        {
            ScreenController.Instance.CreateNewScreenNoParameters(ScreenSystemNavigatorView.SCREEN_SYSTEM_NAVIGATOR, false, TypePreviousActionEnum.KEEP_CURRENT_SCREEN);
            Destroy();
        }
        else
        {
            string titleInfoError = LanguageController.Instance.GetText("message.error");
            string descriptionInfoError = LanguageController.Instance.GetText("screen.media.type.no.filesystem.available");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, titleInfoError, descriptionInfoError, null, "");
        }
    }

    // -------------------------------------------
    /* 
	 * OnAddTypeLink
	 */
    private void OnAddTypeLink()
    {
        ScreenController.Instance.CreateNewScreenNoParameters(ScreenEnterURLView.SCREEN_ENTER_URL, false, TypePreviousActionEnum.KEEP_CURRENT_SCREEN);
        Destroy();
    }

    // -------------------------------------------
    /* 
	 * OnBasicEvent
	 */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
        {
            Destroy();
        }
    }
}
