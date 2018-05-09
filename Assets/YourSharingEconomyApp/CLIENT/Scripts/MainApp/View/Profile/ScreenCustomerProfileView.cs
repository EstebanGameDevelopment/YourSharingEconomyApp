using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * ScreenCustomerProfileView
 * 
 * It displays the customer profile view.
 * 
 * Sections:
 *      -Name
 *      -User's Score
 *      -History of works
 * 
 * @author Esteban Gallardo
 */
public class ScreenCustomerProfileView : ScreenBaseView, IBasicScreenView
{
    public const string SCREEN_CUSTOMER_PROFILE = "SCREEN_CUSTOMER_PROFILE";

    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------
    public const string EVENT_SCREENCUSTOMERPROFILE_LOAD_USER_DATA = "EVENT_SCREENCUSTOMERPROFILE_LOAD_USER_DATA";

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------	
    private GameObject m_root;
    private Transform m_container;
    private UserModel m_userData;

    private Transform m_requestsContainer;

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public void Initialize(params object[] _list)
    {
        m_userData = (UserModel)_list[0];

        m_root = this.gameObject;
        m_container = m_root.transform.Find("Content");

        m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.consumer.profile");
        m_container.Find("Name").GetComponent<Text>().text = m_userData.Nickname;

        m_container.Find("Button_Exit").GetComponent<Button>().onClick.AddListener(ExitPressed);
        m_container.Find("StarsScorePrefab").GetComponent<PanelRatingView>().Initialize(m_userData.Scoreuser,
                                                                                        m_userData.Votesuser,
                                                                                        LanguageController.Instance.GetText("message.consumer.rating"),
                                                                                        true,
                                                                                        "",
                                                                                        false);

        // CONTAINER REQUESTS DONE
        m_requestsContainer = m_container.Find("YourHistoryRequests");
        m_requestsContainer.GetComponent<SlotManagerView>().Initialize();
        m_container.Find("YourHistoryRequests/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.profile.users.requested.works");

        // TOXIC BANNED USER
        Transform iconToxic = m_container.Find("IconToxic");
        if (iconToxic != null)
        {
            iconToxic.gameObject.SetActive(false);
            if (m_userData.IsBanned())
            {
                iconToxic.gameObject.SetActive(true);
                m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.title.toxic.banner.user");
            }
        }

        BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);

        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
        BasicEventController.Instance.DelayBasicEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_RECORDS_BY_USER, 0.1f, m_userData.Id);
    }

    // -------------------------------------------
    /* 
	 * Destroy
	 */
    public void Destroy()
    {
        m_requestsContainer.GetComponent<SlotManagerView>().Destroy();
        BasicEventController.Instance.BasicEvent -= OnBasicEvent;
        GameObject.DestroyObject(this.gameObject);
    }

    // -------------------------------------------
    /* 
     * SetActivation
     */
    public override void SetActivation(bool _activation)
    {
        if (_activation && !this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(_activation);

            m_requestsContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
            m_requestsContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();

            BasicEventController.Instance.DelayBasicEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_RECORDS_BY_USER, 0.1f, m_userData.Id);
        }
        else
        {
            this.gameObject.SetActive(_activation);
        }
    }

    // -------------------------------------------
    /* 
	 * ExitPressed
	 */
    private void ExitPressed()
    {
        BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
    }

    // -------------------------------------------
    /* 
	 * OnBasicEvent
	 */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (!this.gameObject.activeSelf) return;

        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_RECORDS)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            List<RequestModel> data = (List<RequestModel>)_list[0];
            if ((int)_list[1] == RequestsController.TYPE_CONSULT_BY_USER)
            {
                m_requestsContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
                m_requestsContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();
                if (data.Count > 0)
                {
                    // ADD TOXIC AND BROKEN FIRST
                    for (int i = 0; i < data.Count; i++)
                    {
                        RequestModel item = data[i];
                        if ((item.Customer == m_userData.Id) && (item.IsFlagged() || item.IsBrokenDeal()))
                        {
                            m_requestsContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
                        }
                    }

                    // REST OF REQUESTS
                    for (int i = 0; i < data.Count; i++)
                    {
                        RequestModel item = data[i];
                        if ((item.Customer == m_userData.Id) && (!(item.IsFlagged() || item.IsBrokenDeal())))
                        {
                            m_requestsContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
                        }
                    }
                }
                m_requestsContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
            }
        }
        if (_nameEvent == SlotRequestView.EVENT_SLOTREQUEST_SELECTED_REQUEST)
        {
            GameObject slotClicked = (GameObject)_list[0];
            if (m_userData.IsBanned())
            {
                string warning = LanguageController.Instance.GetText("message.warning");
                string description = LanguageController.Instance.GetText("message.show.nothing.of.user.banned");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
            }
            else
            {
                if (m_requestsContainer.GetComponent<SlotManagerView>().CheckSlotExisting(slotClicked))
                {
                    SlotRequestView slotSelected = slotClicked.GetComponent<SlotRequestView>();
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_SINGLE_RECORD, slotSelected.Request.Id);
                }
            }
        }
        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_SINGLE_RECORD)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            RequestModel request = (RequestModel)_list[0];
            if (request != null)
            {
                ScreenController.Instance.CreateNewScreen(ScreenCreateRequestView.SCREEN_DISPLAY_REQUEST, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, request);
            }
            else
            {
                string warning = LanguageController.Instance.GetText("message.error");
                string description = LanguageController.Instance.GetText("message.request.not.found");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
            }
        }
        if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
        {
            ExitPressed();
        }
    }
}