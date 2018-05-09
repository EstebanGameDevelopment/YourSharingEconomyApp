using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * ScreenSystemNavigatorView
 * 
 * Creates the functionality to browse files in the filesystem
 * to select the images to load
 * 
 * @author Esteban Gallardo
 */
public class ScreenSystemNavigatorView : ScreenBaseView, IBasicScreenView
{
    public const string SCREEN_SYSTEM_NAVIGATOR = "SCREEN_SYSTEM_NAVIGATOR";

    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------	
    public const string EVENT_SCREENSYSTEMNAVIGATOR_SELECTED_FILE   = "EVENT_SCREENSYSTEMNAVIGATOR_SELECTED_FILE";
    public const string EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION = "EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION";

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public void Initialize(params object[] _list)
    {
        BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
    }


    // -------------------------------------------
    /* 
     * Destroy
     */
    public void Destroy()
    {
        BasicEventController.Instance.BasicEvent -= OnBasicEvent;
        GetComponent<SystemNavigator>().Destroy();
        GameObject.DestroyObject(this.gameObject);
    }

    // -------------------------------------------
    /* 
	 * OnBasicEvent
	 */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (_nameEvent == EVENT_SCREENSYSTEMNAVIGATOR_SELECTED_FILE)
        {
            if(_list.Length > 0)
            {
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
                BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION, 0.1f, true, (string)_list[0], (float)_list[1]);
            }
            else
            {
                BasicEventController.Instance.DispatchBasicEvent(EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION, false);
            }
            Destroy();
        }
        if (_nameEvent == ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP)
        {
            Destroy();
        }
        if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
        {
            Destroy();
        }
    }
}