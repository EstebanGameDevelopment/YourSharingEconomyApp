using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * ScreenCreateRequestView
 * 
 * Complex class that displays the requests, along with all the possible
 * states it can take.
 * 
 * Sections:
 * 
 *      -Title
 *      -Description
 *      -Price
 *      -Deadline
 *      -Location
 *      -Reference Images (Image posted by the user to work as a reference for the provider)
 *      -Offers (only in display mode, they are the offers and questions providers can make)
 *      -Finished Images (Images posted by the provider to work as a proof of a finished work)
 *      -Score system (only after work completed, with the finished images posted)
 * 
 * @author Esteban Gallardo
 */
public class ScreenCreateRequestView : ScreenBaseView, IBasicScreenView
{
    public const string SCREEN_CREATE_REQUEST = "SCREEN_CREATE_REQUEST";
    public const string SCREEN_DISPLAY_REQUEST = "SCREEN_DISPLAY_REQUEST";

    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------
    public const string EVENT_SCREENCREATEREQUEST_DELAY_LOAD_IMAGE                  = "EVENT_SCREENCREATEREQUEST_DELAY_LOAD_IMAGE";
    
    // ----------------------------------------------
    // SUB
    // ----------------------------------------------	
    public const string SUB_EVENT_SCREENCREATEREQUEST_CONFIRMATION                      = "SUB_EVENT_SCREENCREATEREQUEST_CONFIRMATION";
    public const string SUB_EVENT_SCREENCREATEREQUEST_EXIT_WITHOUT_SAVING               = "SUB_EVENT_SCREENCREATEREQUEST_EXIT_WITHOUT_SAVING";
    public const string SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_EDIT                      = "SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_EDIT";
    public const string SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_REMOVE                    = "SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_REMOVE";
    public const string SUB_EVENT_SCREENCREATEREQUEST_REMOVE_WITH_PENALTY               = "SUB_EVENT_SCREENCREATEREQUEST_REMOVE_WITH_PENALTY";
    public const string SUB_EVENT_SCREENCREATEREQUEST_BECOME_A_PROVIDER                 = "SUB_EVENT_SCREENCREATEREQUEST_BECOME_A_PROVIDER";
    public const string SUB_EVENT_SCREENCREATEREQUEST_UPLOAD_FINAL_IMAGE                = "SUB_EVENT_SCREENCREATEREQUEST_UPLOAD_FINAL_IMAGE";
    public const string SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_FEEDBACK_CUSTOMER   = "SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_FEEDBACK_CUSTOMER";
    public const string SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_FEEDBACK_PROVIDER   = "SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_FEEDBACK_PROVIDER";
    public const string SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_SCORE_PROVIDER      = "SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_SCORE_PROVIDER";
    public const string SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_SCORE_CUSTOMER      = "SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_SCORE_CUSTOMER";
    public const string SUB_EVENT_SCREENCREATEREQUEST_DEAL_BROKEN_CONFIRMATION          = "SUB_EVENT_SCREENCREATEREQUEST_DEAL_BROKEN_CONFIRMATION";
    public const string SUB_EVENT_SCREENCREATEREQUEST_EXIT_CONFIRMATION                 = "SUB_EVENT_SCREENCREATEREQUEST_EXIT_CONFIRMATION";

    public const string SCOREEVENT_RATING_PROVIDER_TO_CUSTOMER = "SCOREEVENT_RATING_PROVIDER_TO_CUSTOMER";
    public const string SCOREEVENT_RATING_CUSTOMER_TO_PROVIDER = "SCOREEVENT_RATING_CUSTOMER_TO_PROVIDER";

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------	
    private GameObject m_root;
    private Transform m_container;
    private RequestModel m_requestData;
    private bool m_isReadyToPublish = false;
    private bool m_hasChanged = false;

    private Transform m_referenceImagesContainer;
    private Transform m_finishedImagesContainer;

    private GameObject m_buttonMakeOffer;
    private Transform m_labelTitleMyOffers;
    private Transform m_offerContainer;
    private List<GameObject> m_slotOfferList = new List<GameObject>();
    private GameObject m_offersTextLoading;

    private bool m_isDisplayInfo = false;

    private GameObject m_btnSave;
    private GameObject m_btnBack;

    private List<GameObject> m_imagesReferences = new List<GameObject>();
    private int m_commitImageIndex = -1;
    private int m_counterImagesLoaded = -1;

    private Transform m_labelFinishedImages;

    private Transform m_panelScoreConsumer;
    private Transform m_panelScoreProvider;

    private Transform m_editFieldFeedbackConsumer;
    private Transform m_scrollFieldFeedbackConsumer;
    private Transform m_editFieldFeedbackProvider;
    private Transform m_scrollFieldFeedbackProvider;

    private Transform m_buttonEdit;
    private Transform m_buttonRemove;

    private Transform m_buttonAddFinishedImages;
    private Transform m_buttonBreakDeal;

    private int m_typeImageToUpload = 0;
    private List<GameObject> m_imagesFinishedJob = new List<GameObject>();
    
    private bool m_flagResetAllProposal = true;

    private Transform m_emptyReferemceScore;
    private Transform m_containerScore;

    private Transform m_imageLoadingOffers;
    private Transform m_imageLoadingReference;
    private Transform m_imageLoadingFinished;

    private bool m_pressedCheckCustomerProfile = false;

    private bool m_loadedAllOffersData = false;

    public bool IsReadyToPublish
    {
        get { return m_isReadyToPublish; }
        set
        {
            if (!m_isDisplayInfo)
            {
                m_hasChanged = true;
                if (m_requestData.AllDataFilled())
                {
                    m_isReadyToPublish = value;
                    m_btnSave.SetActive(m_isReadyToPublish);
                }
            }
        }
    }

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public void Initialize(params object[] _list)
    {
        m_root = this.gameObject;
        m_container = m_root.transform.Find("Content/ScrollPage/Page");

        m_requestData = new RequestModel();
        m_requestData.Customer = UsersController.Instance.CurrentUser.Id;
        if (_list != null)
        {
            if (_list.Length > 0)
            {
                if (_list[0] != null)
                {
                    if (_list[0] is RequestModel)
                    {
                        m_requestData = ((RequestModel)_list[0]).Clone();
                    }
                }
            }
        }

        // ++++ ONLY IF THERE ARE THE ICON OF SAVE&BACK THE REQUEST BELONGS TO THE CURRENT USER ++++
        if (m_root.transform.Find("Content/Button_Save") != null)
        {
            m_isDisplayInfo = false;
            BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_CANCEL_LOADING_IMAGES);
        }
        else
        {
            m_isDisplayInfo = true;
        }


        if (!m_isDisplayInfo)
        {
            m_btnSave = m_root.transform.Find("Content/Button_Save").gameObject;
            m_btnBack = m_root.transform.Find("Content/Button_Back").gameObject;

            m_requestData.Customer = UsersController.Instance.CurrentUser.Id;
            m_btnSave.GetComponent<Button>().onClick.AddListener(SavePressed);
            m_btnBack.GetComponent<Button>().onClick.AddListener(BackEditionPressed);
            m_btnSave.SetActive(false);
        }
        else
        {
            m_btnBack = m_root.transform.Find("Content/Button_Back").gameObject;
            m_btnBack.GetComponent<Button>().onClick.AddListener(BackPressed);
        }

        IsReadyToPublish = false;

        // ++++ BUTTONS TO EDIT/DELETE ++++
        if (m_isDisplayInfo)
        {
            m_buttonEdit = m_root.transform.Find("Content/Button_Edit");
            if (m_buttonEdit != null)
            {
                m_buttonEdit.GetComponent<Button>().onClick.AddListener(EditPressed);
            }
            m_buttonRemove = m_root.transform.Find("Content/Button_Remove");
            if (m_buttonRemove != null)
            {
                m_buttonRemove.GetComponent<Button>().onClick.AddListener(RemovePressed);
            }
            if (m_requestData.Reported.Length > 0)
            {
                m_buttonEdit.gameObject.SetActive(false);
                m_buttonRemove.gameObject.SetActive(false);
            }
        }

        m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.title");

        if (m_container.Find("Button_CheckCustomerProfile") != null)
        {
            m_container.Find("Button_CheckCustomerProfile").GetComponent<Button>().onClick.AddListener(OnCheckCustomerProfile);
            m_container.Find("Button_CheckCustomerProfile/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.check.consumer.profile");
        }

        m_container.Find("RequestTitleLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.name.dress");
        if (!m_isDisplayInfo)
        {
            m_container.Find("RequestTitleValue").GetComponent<InputField>().onEndEdit.AddListener(OnRequestTitle);
        }

        m_container.Find("RequestDescriptionLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.description.dress");
        if (!m_isDisplayInfo)
        {
            m_container.Find("RequestDescriptionValue").GetComponent<InputField>().onEndEdit.AddListener(OnRequestDescription);
        }

        m_container.Find("RequestPriceLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.price");
        if (!m_isDisplayInfo)
        {
            m_container.Find("RequestPriceValue").GetComponent<InputField>().onEndEdit.AddListener(OnRequestPrice);
            m_container.Find("RequestPriceCurrency").GetComponent<Dropdown>().onValueChanged.AddListener(OnRequestCurrency);
            SelectRequestCurrencyByName(m_requestData.Currency);
        }

        m_container.Find("RequestDeadlineLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.description.dress");
        m_container.Find("Button_DeadlineCalendar").GetComponent<Button>().onClick.AddListener(OnCalendarClick);

        m_container.Find("CanTravelToProvider/Label").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.can.travel");
        m_container.Find("CanTravelToProvider").GetComponent<Toggle>().onValueChanged.AddListener(OnCanTravelClick);

        m_container.Find("ProvideMaterialToProvider/Label").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.provide.material");
        m_container.Find("ProvideMaterialToProvider").GetComponent<Toggle>().onValueChanged.AddListener(OnProvideMaterialClick);

        m_container.Find("Notifications/Label").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.notify.by.email");
        m_container.Find("Notifications").GetComponent<Toggle>().onValueChanged.AddListener(OnNotifyByEmailClick);

        m_container.Find("Button_Maps").GetComponent<Button>().onClick.AddListener(OnGoogleMaps);

        m_container.Find("RequestDistanceLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.distance.search");
        m_container.Find("RequestDisctanceDropdown").GetComponent<Dropdown>().onValueChanged.AddListener(OnRequestDistanceDropdown);
        SelectRequestDistanceByIndex(m_requestData.Distance);

        if (!m_isDisplayInfo)
        {
            m_container.Find("Button_Images").GetComponent<Button>().onClick.AddListener(OnAddNewImage);
            m_container.Find("Button_Images/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.add.new.image");
        }

        // ++++ IMAGES USED AS A REFERENCE ++++
        if (m_container.Find("RequestRefenceImagesLabel") != null)
        {
            m_container.Find("RequestRefenceImagesLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.referenced.images");
        }
        m_referenceImagesContainer = m_container.Find("ScrollImages/PicturesContainer");
        m_imageLoadingReference = m_container.Find("ImageLoadingReference");

        // ++++ POST NEW OFFERS ++++
        // THE OFFERS WILL DISAPPEAR ONCE THERE IS A DEAL WITH A PROVIDER (m_requestData.Provider != -1)
        m_buttonMakeOffer = null;
        m_offerContainer = null;
        m_labelTitleMyOffers = null;
        m_offerContainer = m_container.Find("ScrollOffers/Offers");
        if (m_offerContainer != null)
        {
            if (m_container.Find("Button_MakeOffer") != null)
            {
                m_buttonMakeOffer = m_container.Find("Button_MakeOffer").gameObject;
                if (m_buttonMakeOffer != null) m_buttonMakeOffer.GetComponent<Button>().onClick.AddListener(OnPostNewOffer);
            }

            m_labelTitleMyOffers = m_container.Find("OffersTitleLabel");
            if (m_labelTitleMyOffers != null)
            {
                m_labelTitleMyOffers.GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.offers.proposed");
                m_labelTitleMyOffers.gameObject.SetActive(false);
            }

            m_imageLoadingOffers = m_container.Find("ScrollOffers/Image");
            m_offersTextLoading = m_container.Find("ScrollOffers/Loading").gameObject;
            m_offersTextLoading.GetComponent<Text>().text = LanguageController.Instance.GetText("message.loading");            
        }

        // ++++ DRESS FINISHED ++++
        // 1º-THERE IS A DEAL DONE WITH A PROVIDER (m_requestData.Provider != -1)
        // 2º-PROVIDER SHOULD ADD A IMAGE (THEN THE DELIVERYDATE=time())
        // 3º-CUSTOMER SHOULD SCORE PROVIDER (m_requestData.Scoreprovider != -1)
        // 4º-PROVIDER SHOULD SCORE CUSTOMER (m_requestData.Scorecustomer != -1)
        m_finishedImagesContainer = m_container.Find("ScrollFinished/PicturesContainer");

        // PANELS SCORES
        m_emptyReferemceScore = m_container.Find("EmptyReferenceScore");
        m_containerScore = m_container.Find("ContentScore");

        // FINISHED IMAGES LABEL
        m_labelFinishedImages = m_container.Find("FinishedTitleLabel");

        // BUTTON ADD FINISHED IMAGES
        if (m_containerScore != null)
        {
            m_buttonAddFinishedImages = m_containerScore.Find("Button_AddFinished");
            if (m_buttonAddFinishedImages != null)
            {
                m_buttonAddFinishedImages.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.add.finished.image");
                m_buttonAddFinishedImages.GetComponent<Button>().onClick.AddListener(OnAddFinishedImage);
            }

            // BUTTON ADD FINISHED IMAGES
            m_buttonBreakDeal = m_containerScore.Find("Button_BreakDeal");
            if (m_buttonBreakDeal != null)
            {
                m_buttonBreakDeal.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.break.deal");
                m_buttonBreakDeal.GetComponent<Button>().onClick.AddListener(OnBreakDeal);
            }
        }

        m_imageLoadingFinished = m_container.Find("ImageLoadingFinished");

        if (m_imageLoadingOffers != null) m_imageLoadingOffers.gameObject.SetActive(m_isDisplayInfo);
        if (m_imageLoadingReference != null) m_imageLoadingReference.gameObject.SetActive(m_isDisplayInfo);
        if (m_imageLoadingFinished != null) m_imageLoadingFinished.gameObject.SetActive(m_isDisplayInfo);

        if (m_container.Find("RequestImagesLabel") != null)
        {
            m_container.Find("RequestImagesLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.referenced.images");
        }

        // ++++ LOADING THE DATA ++++
        LoadRequestData();

        BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
    }

    // -------------------------------------------
    /* 
     * Destroy
     */
    public void Destroy()
    {
        ClearAllReferenceImages();
        ClearAllFinishedImages();
        ClearAllProposals();
        m_requestData = null;
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

            if (m_imageLoadingOffers != null) m_imageLoadingOffers.gameObject.SetActive(m_isDisplayInfo);
            if (m_imageLoadingReference != null) m_imageLoadingReference.gameObject.SetActive(m_isDisplayInfo);
            if (m_imageLoadingFinished != null) m_imageLoadingFinished.gameObject.SetActive(m_isDisplayInfo);

            LoadRequestData();
        }
        else
        {
            this.gameObject.SetActive(_activation);
        }
    }

    // -------------------------------------------
    /* 
     * HideFlexibleComponents
     */
    private void HideFlexibleComponents()
    {
        if (m_containerScore != null) m_containerScore.gameObject.SetActive(false);

        // RESET VISIBILITY
        if (m_finishedImagesContainer != null) m_finishedImagesContainer.parent.gameObject.SetActive(false);
        if (m_labelFinishedImages != null) m_labelFinishedImages.gameObject.SetActive(false);
        SetFeedbackProvider(false);
        SetPanelScoreProvider(false);
        SetPanelScoreConsumer(false);
        SetFeedbackConsumer(false);
        if (m_buttonAddFinishedImages != null) m_buttonAddFinishedImages.gameObject.SetActive(false);
        if (m_buttonBreakDeal != null) m_buttonBreakDeal.gameObject.SetActive(false);
        if (m_imageLoadingFinished != null) m_imageLoadingFinished.gameObject.SetActive(false);
    }

    // -------------------------------------------
    /* 
     * LoadRequestData()
     */
    private void LoadRequestData()
    {
        m_loadedAllOffersData = false;

        HideFlexibleComponents();

        RequestModel requestData = RequestsController.Instance.GetLocalRequest(m_requestData.Id);
        if (requestData != null)
        {
            m_requestData = requestData.Clone();
        }

        if (m_requestData.Customer != UsersController.Instance.CurrentUser.Id)
        {
            if (m_buttonEdit != null) m_buttonEdit.gameObject.SetActive(false);
            if (m_buttonRemove != null) m_buttonRemove.gameObject.SetActive(false);
        }
        else
        {
            if (m_requestData.Provider != -1)
            {
                if (m_buttonEdit != null) m_buttonEdit.gameObject.SetActive(false);
                if (m_buttonRemove != null) m_buttonRemove.gameObject.SetActive(true);
            }
        }

        if (m_requestData.Deliverydate != -1)
        {
            if (m_buttonEdit != null) m_buttonEdit.gameObject.SetActive(false);
            if (m_buttonRemove != null) m_buttonRemove.gameObject.SetActive(false);
        }

        // GENERAL DESCRIPTION
        if (!m_isDisplayInfo)
        {
            m_container.Find("RequestTitleValue").GetComponent<InputField>().text = m_requestData.Title;
            m_container.Find("RequestDescriptionValue").GetComponent<InputField>().text = m_requestData.Description;
            m_container.Find("RequestPriceValue").GetComponent<InputField>().text = m_requestData.Price.ToString();
            SelectRequestCurrencyByName(m_requestData.Currency);
        }
        else
        {
            m_container.Find("RequestTitleValue").GetComponent<Text>().text = m_requestData.Title;
            m_container.Find("ScrollDescriptionValue/RequestDescriptionValue").GetComponent<Text>().text = m_requestData.Description;
            m_container.Find("RequestPriceValue").GetComponent<Text>().text = m_requestData.Price.ToString();
            m_container.Find("RequestPriceCurrency").GetComponent<Text>().text = m_requestData.Currency.ToString();
        }

        // FLAGS
        m_container.Find("Button_DeadlineCalendar/Title").GetComponent<Text>().text = DateConverter.TimeStampToDateTimeString(m_requestData.Deadline);
        m_container.Find("CanTravelToProvider").GetComponent<Toggle>().isOn = (m_requestData.Travel == 1);
        m_container.Find("ProvideMaterialToProvider").GetComponent<Toggle>().isOn = (m_requestData.Material == 1);
        m_container.Find("Notifications").GetComponent<Toggle>().isOn = (m_requestData.Notifications == 1);

        // LOCATION MAP
        string village = LanguageController.Instance.GetText("screen.profile.village.not.defined");
        if (m_requestData.Village.Length > 0)
        {
            village = m_requestData.Village;
        }
        else
        {
            if (UsersController.Instance.CurrentUser.Village.Length > 0)
            {
                village = UsersController.Instance.CurrentUser.Village;
                m_requestData.Village = village;
                if (UsersController.Instance.CurrentUser.Mapdata.Length > 0)
                {
                    m_requestData.Mapdata = UsersController.Instance.CurrentUser.Mapdata;
                    if (m_requestData.Mapdata.IndexOf(",") > 0)
                    {
                        string[] coordinates = m_requestData.Mapdata.Split(',');
                        m_requestData.Latitude = float.Parse(coordinates[0]);
                        m_requestData.Longitude = float.Parse(coordinates[1]);
                    }
                }
            }
        }
        m_container.Find("Button_Maps/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.area.search") + '\n' + village;

        // FILL PROPOSALS
        if (m_offerContainer != null)
        {
            ClearAllProposals();
            m_imageLoadingOffers.gameObject.SetActive(true);
            m_offersTextLoading.SetActive(true);

            // ENABLE/DISABLE OFFERS
            if (m_offerContainer != null) m_offerContainer.gameObject.SetActive(true);
            if (m_buttonMakeOffer != null)
            {
                m_buttonMakeOffer.SetActive(false);
                if (m_requestData.Provider == -1)
                {
                    m_buttonMakeOffer.SetActive(true);
                    if (m_requestData.Customer != UsersController.Instance.CurrentUser.Id)
                    {
                        m_buttonMakeOffer.transform.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.make.offer");
                    }
                    else
                    {
                        m_buttonMakeOffer.transform.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.reply.to.offers");
                    }
                }
            }
            if ((m_labelTitleMyOffers != null) && (m_buttonMakeOffer != null))
            {
                m_labelTitleMyOffers.gameObject.SetActive(!m_buttonMakeOffer.activeSelf);
            }

            m_imageLoadingOffers.gameObject.SetActive(true);
            m_offersTextLoading.SetActive(true);

            BasicEventController.Instance.DispatchBasicEvent(ProposalsController.EVENT_PROPOSAL_CALL_CONSULT_PROPOSALS, m_requestData.Id);
        }

        // FINISHED LABEL
        if (m_labelFinishedImages != null)
        {
            if (m_requestData.Provider != -1)
            {
                if (m_requestData.Provider == UsersController.Instance.CurrentUser.Id)
                {
                    m_labelFinishedImages.gameObject.SetActive(false);
                }
                else
                {
                    m_labelFinishedImages.gameObject.SetActive(true);
                }
                m_labelFinishedImages.GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.finished.images");
            }
            else
            {
                m_labelFinishedImages.gameObject.SetActive(false);
            }
        }

        // CONSUMER SCORE
        if (m_panelScoreConsumer != null)
        {
            SetPanelScoreConsumer(false);
            bool isInteractable = (m_requestData.Customer == UsersController.Instance.CurrentUser.Id);
            m_panelScoreConsumer.GetComponent<PanelRatingView>().Initialize(m_requestData.ScoreCustomerGivesToTheProvider,
                                                                                1,
                                                                                LanguageController.Instance.GetText("message.consumer.rating.to.provider"),
                                                                                true,
                                                                                SCOREEVENT_RATING_CUSTOMER_TO_PROVIDER,
                                                                                isInteractable && (m_requestData.ScoreCustomerGivesToTheProvider <= 0));

            // FEEDBACK COSTUMER
            if (m_editFieldFeedbackConsumer != null)
            {
                SetFeedbackConsumer(false);
            }

            if (m_requestData.Deliverydate != -1)
            {
                if (isInteractable)
                {
                    SetPanelScoreConsumer(true);
                }
                else
                {
                    if ((m_requestData.Deliverydate != -1) && (m_requestData.ScoreCustomerGivesToTheProvider != -1) && (m_requestData.ScoreProviderGivesToTheCustomer != -1))
                    {
                        SetPanelScoreConsumer(true);
                    }
                    else
                    {
                        SetPanelScoreConsumer(false);
                    }
                }
                if (m_editFieldFeedbackConsumer != null)
                {
                    if ((m_requestData.ScoreCustomerGivesToTheProvider > 0) && (m_panelScoreConsumer.gameObject.activeSelf))
                    {
                        SetFeedbackConsumer(true);
                    }
                    m_editFieldFeedbackConsumer.GetComponent<InputField>().text = m_requestData.FeedbackCustomerGivesToTheProvider;
                    m_scrollFieldFeedbackConsumer.GetComponent<Text>().text = m_requestData.FeedbackCustomerGivesToTheProvider;
                }
            }
            else
            {
                SetPanelScoreConsumer(false);
            }
        }

        // PROVIDER SCORE
        if (m_panelScoreProvider != null)
        {
            SetPanelScoreProvider(false);
            bool isInteractable = (m_requestData.Provider == UsersController.Instance.CurrentUser.Id);
            m_panelScoreProvider.GetComponent<PanelRatingView>().Initialize(m_requestData.ScoreProviderGivesToTheCustomer,
                                                                                        1,
                                                                                        LanguageController.Instance.GetText("message.consumer.rating.to.consumer."),
                                                                                        true,
                                                                                        SCOREEVENT_RATING_PROVIDER_TO_CUSTOMER,
                                                                                        isInteractable && (m_requestData.ScoreProviderGivesToTheCustomer <= 0));

            // FEEDBACK PROVIDER
            if (m_editFieldFeedbackProvider != null)
            {
                SetFeedbackProvider(false);
            }

            if (m_requestData.Deliverydate != -1)
            {
                if (isInteractable)
                {
                    if (m_requestData.ScoreCustomerGivesToTheProvider != -1)
                    {
                        SetPanelScoreProvider(true);
                    }
                    else
                    {
                        SetPanelScoreProvider(false);
                    }
                }
                else
                {
                    if ((m_requestData.Deliverydate != -1) && (m_requestData.ScoreCustomerGivesToTheProvider != -1) && (m_requestData.ScoreProviderGivesToTheCustomer != -1))
                    {
                        SetPanelScoreProvider(true);
                    }
                    else
                    {
                        SetPanelScoreProvider(false);
                    }
                }
                if (m_editFieldFeedbackProvider != null)
                {
                    if ((m_requestData.ScoreProviderGivesToTheCustomer > 0) && m_panelScoreProvider.gameObject.activeSelf)
                    {
                        SetFeedbackProvider(true);
                    }
                    m_editFieldFeedbackProvider.GetComponent<InputField>().text = m_requestData.FeedbackProviderGivesToTheCustomer;
                    m_scrollFieldFeedbackProvider.GetComponent<Text>().text = m_requestData.FeedbackProviderGivesToTheCustomer;
                }
            }
            else
            {
                SetPanelScoreProvider(false);
            }
        }

        // FINISHED IMAGES CONTAINER
        if (m_finishedImagesContainer != null)
        {
            if (m_requestData.Provider != -1)
            {
                if (m_requestData.Deliverydate != 0)
                {
                    m_finishedImagesContainer.parent.gameObject.SetActive(true);
                    m_imageLoadingFinished.gameObject.SetActive(true);
                }
            }
            else
            {
                m_finishedImagesContainer.parent.gameObject.SetActive(false);
                m_imageLoadingFinished.gameObject.SetActive(false);
            }
        }

        // ADD BUTTON FINISHED IMAGES
        if (m_buttonAddFinishedImages != null)
        {
            m_buttonAddFinishedImages.gameObject.SetActive(false);
            m_buttonBreakDeal.gameObject.SetActive(false);
            if (m_isDisplayInfo)
            {
                if (m_requestData.Provider == UsersController.Instance.CurrentUser.Id)
                {
                    if (m_requestData.ScoreCustomerGivesToTheProvider == -1)
                    {
                        if (m_requestData.Deliverydate != 0)
                        {
                            m_buttonAddFinishedImages.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (m_labelFinishedImages != null) m_labelFinishedImages.gameObject.SetActive(true);
                    }
                    if (m_requestData.Deliverydate == -1)
                    {
                        m_buttonBreakDeal.gameObject.SetActive(true);
                    }
                }
            }
        }

        // LOAD REFERENCE IMAGES
        ClearAllFinishedImages();
        if (!ClearAllReferenceImages())
        {
            InitializeImagesContainer();
        }
        else
        {
            BasicEventController.Instance.DelayBasicEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_IMAGES_REQUEST, 0.4f, m_requestData.Id);
        }
    }

    // -------------------------------------------
    /* 
     * OnChangeConsumerFeedback
     */
    private void OnChangeConsumerFeedback(string _text)
    {
        if ((m_requestData.FeedbackCustomerGivesToTheProvider.Length == 0)
            && (m_requestData.Customer == UsersController.Instance.CurrentUser.Id))
        {
            m_requestData.FeedbackCustomerGivesToTheProvider = _text;
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.are.you.sure.feedback"), null, SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_FEEDBACK_CUSTOMER);
        }
        else
        {
            m_editFieldFeedbackConsumer.GetComponent<InputField>().text = m_requestData.FeedbackCustomerGivesToTheProvider;
            m_scrollFieldFeedbackConsumer.GetComponent<Text>().text = m_requestData.FeedbackCustomerGivesToTheProvider;
        }
    }

    // -------------------------------------------
    /* 
     * OnChangeProviderFeedback
     */
    private void OnChangeProviderFeedback(string _text)
    {
        if ((m_requestData.FeedbackProviderGivesToTheCustomer.Length == 0)
            && (m_requestData.Provider == UsersController.Instance.CurrentUser.Id))
        {
            m_requestData.FeedbackProviderGivesToTheCustomer = _text;
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.are.you.sure.feedback"), null, SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_FEEDBACK_PROVIDER);
        }
        else
        {
            m_editFieldFeedbackProvider.GetComponent<InputField>().text = m_requestData.FeedbackProviderGivesToTheCustomer;
            m_scrollFieldFeedbackProvider.GetComponent<Text>().text = m_requestData.FeedbackProviderGivesToTheCustomer;
        }
    }

    // -------------------------------------------
    /* 
     * ClearAllReferenceImages
     */
    private bool ClearAllReferenceImages()
    {
        if (m_imagesReferences.Count > 0)
        {
            for (int i = 0; i < m_imagesReferences.Count; i++)
            {
                if (m_imagesReferences[i] != null)
                {
                    if (m_imagesReferences[i].GetComponent<SlotImageView>() != null)
                    {
                        m_imagesReferences[i].GetComponent<SlotImageView>().Destroy();
                    }
                }
            }
            m_imagesReferences.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }

    // -------------------------------------------
    /* 
     * ClearAllFinishedImages
     */
    private bool ClearAllFinishedImages()
    {
        if (m_imagesFinishedJob.Count > 0)
        {
            for (int i = 0; i < m_imagesFinishedJob.Count; i++)
            {
                if (m_imagesFinishedJob[i] != null)
                {
                    if (m_imagesFinishedJob[i].GetComponent<SlotImageView>() != null)
                    {
                        m_imagesFinishedJob[i].GetComponent<SlotImageView>().Destroy();
                    }
                }
            }
            m_imagesFinishedJob.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }

    // -------------------------------------------
    /* 
     * ClearAllProposals
     */
    private bool ClearAllProposals()
    {
        if (m_slotOfferList.Count > 0)
        {
            for (int i = 0; i < m_slotOfferList.Count; i++)
            {
                m_slotOfferList[i].GetComponent<SlotOfferView>().Destroy();
            }
            m_slotOfferList.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }


    // -------------------------------------------
    /* 
     * CheckAnyProposalAccepted
     */
    private bool CheckAnyProposalAccepted()
    {
        if (m_slotOfferList.Count > 0)
        {
            for (int i = 0; i < m_slotOfferList.Count; i++)
            {
                if (m_slotOfferList[i].GetComponent<SlotOfferView>().Proposal.Type == ProposalModel.TYPE_OFFER)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }


    // -------------------------------------------
    /* 
     * InitializeImagesContainer
     */
    private void InitializeImagesContainer()
    {
        m_requestData.Images = m_requestData.TemporalImageReferences.Count;
        m_counterImagesLoaded = m_requestData.Images;
        if (m_requestData.TemporalImageReferences.Count > 0)
        {
            BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENCREATEREQUEST_DELAY_LOAD_IMAGE, 0.4f);
        }
    }

    // -------------------------------------------
    /* 
     * LoadPopImages
     */
    private void LoadPopImages()
    {
        // LOAD IMAGES
        m_requestData.Images = m_requestData.TemporalImageReferences.Count;

        // ADD FIRST THE REFERENCE IMAGE
        for (int i = 0; i < m_requestData.TemporalImageReferences.Count; i++)
        {
            if ((m_requestData.TemporalImageReferences[i].Id == m_requestData.Referenceimg) && (m_requestData.TemporalImageReferences[i].Type != RequestModel.IMAGE_TYPE_FINISHED))
            {
                GameObject newImageRequest = Utilities.AddChild(m_referenceImagesContainer, ScreenController.Instance.SlotImage);
                long idImageReference = m_requestData.TemporalImageReferences[i].Id;
                string urlReference = m_requestData.TemporalImageReferences[i].Url;
                m_imagesReferences.Add(newImageRequest);
                newImageRequest.GetComponent<SlotImageView>().InitializeFromServerData(m_requestData.Id, idImageReference, m_isDisplayInfo, (m_requestData.Referenceimg == idImageReference), (m_requestData.Customer == UsersController.Instance.CurrentUser.Id), RequestModel.IMAGE_TYPE_REFERENCE, urlReference, m_requestData.IsFlagged(), false);
                if (m_imageLoadingReference != null) m_imageLoadingReference.gameObject.SetActive(false);
            }
        }

        // ADD FIRST THE REST OF IMAGES
        for (int i = 0; i < m_requestData.TemporalImageReferences.Count; i++)
        {
            if ((m_requestData.TemporalImageReferences[i].Id != m_requestData.Referenceimg) && (m_requestData.TemporalImageReferences[i].Type != RequestModel.IMAGE_TYPE_FINISHED))
            {
                GameObject newImageRequest = Utilities.AddChild(m_referenceImagesContainer, ScreenController.Instance.SlotImage);
                long idImageReference = m_requestData.TemporalImageReferences[i].Id;
                string urlReference = m_requestData.TemporalImageReferences[i].Url;
                m_imagesReferences.Add(newImageRequest);
                newImageRequest.GetComponent<SlotImageView>().InitializeFromServerData(m_requestData.Id, idImageReference, m_isDisplayInfo, (m_requestData.Referenceimg == idImageReference), (m_requestData.Customer == UsersController.Instance.CurrentUser.Id), RequestModel.IMAGE_TYPE_REFERENCE, urlReference, m_requestData.IsFlagged(), false);
                if (m_imageLoadingReference != null) m_imageLoadingReference.gameObject.SetActive(false);
            }
        }

        // ADD FINISHED IMAGES
        for (int i = 0; i < m_requestData.TemporalImageReferences.Count; i++)
        {
            if (m_requestData.TemporalImageReferences[i].Type == RequestModel.IMAGE_TYPE_FINISHED)
            {
                GameObject newFinishedImage = Utilities.AddChild(m_finishedImagesContainer, ScreenController.Instance.SlotImage);
                long idImageReference = m_requestData.TemporalImageReferences[i].Id;
                string urlReference = m_requestData.TemporalImageReferences[i].Url;
                m_imagesFinishedJob.Add(newFinishedImage);
                newFinishedImage.GetComponent<SlotImageView>().InitializeFromServerData(m_requestData.Id, idImageReference, m_isDisplayInfo, false, false, RequestModel.IMAGE_TYPE_FINISHED, urlReference, m_requestData.IsFlagged(), false);
                if (m_imageLoadingFinished != null) m_imageLoadingFinished.gameObject.SetActive(false);
            }
        }

        UpdateFeedbackElements();

        UpdateScoreElements(false);
    }

    // -------------------------------------------
    /* 
	 * UpdateScoreElements
	 */
    private void UpdateScoreElements(bool _forceDeliveryDate)
    {
        if ((m_requestData.Deliverydate != -1) || _forceDeliveryDate)
        {
            m_buttonBreakDeal.gameObject.SetActive(false);

            if (m_imagesFinishedJob.Count == 0)
            {
                m_buttonAddFinishedImages.gameObject.SetActive(false);
                m_labelFinishedImages.gameObject.SetActive(true);
                m_labelFinishedImages.GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.deal.broken");

                m_containerScore.position = m_emptyReferemceScore.position;
                m_finishedImagesContainer.parent.gameObject.SetActive(false);
                m_imageLoadingFinished.gameObject.SetActive(false);

                m_panelScoreProvider.GetComponent<PanelRatingView>().SetTitle(LanguageController.Instance.GetText("screen.create.request.no.score.provider.deal.broken"));
                m_panelScoreProvider.GetComponent<PanelRatingView>().SetTextScore("");

                m_panelScoreConsumer.GetComponent<PanelRatingView>().SetTitle(LanguageController.Instance.GetText("screen.create.request.no.score.consumer.deal.broken"));
                m_panelScoreConsumer.GetComponent<PanelRatingView>().SetTextScore("");

                m_panelScoreConsumer.GetComponent<PanelRatingView>().DisableInteraction();
                m_panelScoreProvider.GetComponent<PanelRatingView>().DisableInteraction();

                if ((m_requestData.FeedbackCustomerGivesToTheProvider.Length > 0) &&
                    (m_requestData.FeedbackProviderGivesToTheCustomer.Length > 0))
                {
                    SetPanelScoreProvider(true);
                    SetFeedbackProvider(true);
                    SetPanelScoreConsumer(true);
                    SetFeedbackConsumer(true);
                }
                else
                {
                    if (UsersController.Instance.CurrentUser.Id == m_requestData.Provider)
                    {
                        SetPanelScoreProvider(true);
                        SetFeedbackProvider(true);
                        if ((m_requestData.FeedbackCustomerGivesToTheProvider.Length > 0) &&
                            (m_requestData.FeedbackProviderGivesToTheCustomer.Length > 0))
                        {
                            SetPanelScoreConsumer(true);
                            SetFeedbackConsumer(true);
                        }
                    }
                    else
                    {
                        if (UsersController.Instance.CurrentUser.Id == m_requestData.Customer)
                        {
                            SetPanelScoreConsumer(true);
                            SetFeedbackConsumer(true);
                            if ((m_requestData.FeedbackCustomerGivesToTheProvider.Length > 0) &&
                                (m_requestData.FeedbackProviderGivesToTheCustomer.Length > 0))
                            {
                                SetPanelScoreProvider(true);
                                SetFeedbackProvider(true);
                            }
                        }
                    }
                }
            }
            else
            {
                if ((m_requestData.ScoreCustomerGivesToTheProvider != -1) &&
                    (m_requestData.ScoreProviderGivesToTheCustomer != -1) &&
                    (m_requestData.FeedbackCustomerGivesToTheProvider.Length > 0) &&
                    (m_requestData.FeedbackProviderGivesToTheCustomer.Length > 0))
                {
                    SetPanelScoreProvider(true);
                    SetFeedbackProvider(true);
                    SetPanelScoreConsumer(true);
                    SetFeedbackConsumer(true);
                }
            }
        }

        if (m_imagesFinishedJob.Count > 0)
        {
            m_imageLoadingFinished.gameObject.SetActive(false);
        }

        if (m_containerScore != null) m_containerScore.gameObject.SetActive(true);
    }

    // -------------------------------------------
    /* 
	 * ExitPressed
	 */
    private void BackPressed()
    {
        BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
    }

    // -------------------------------------------
    /* 
	 * ExitPressed
	 */
    private void SavePressed()
    {
        string warning = LanguageController.Instance.GetText("message.warning");
        if (m_requestData.Id == -1)
        {
            string description = LanguageController.Instance.GetText("message.create.request.confirmation");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_CONFIRMATION);
        }
        else
        {
            string description = LanguageController.Instance.GetText("message.create.request.save.confirmation");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_CONFIRMATION);
        }
    }

    // -------------------------------------------
    /* 
	 * BackEditionPressed
	 */
    private void BackEditionPressed()
    {
        if (m_hasChanged)
        {
            string warning = LanguageController.Instance.GetText("message.warning");
            string description = LanguageController.Instance.GetText("message.create.request.exit.without.saving");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_EXIT_WITHOUT_SAVING);
        }
        else
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
        }
    }

    // -------------------------------------------
    /* 
	 * ExitPressed
	 */
    private void EditPressed()
    {
        if (!m_loadedAllOffersData)
        {
            string warning = LanguageController.Instance.GetText("message.warning");
            string description = LanguageController.Instance.GetText("screen.create.request.wait.until.loaded.information");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
        }
        else
        {
            if (CheckAnyProposalAccepted())
            {
                m_flagResetAllProposal = true;
                string warning = LanguageController.Instance.GetText("message.warning");
                string description = LanguageController.Instance.GetText("message.create.request.invalidate.if.edit");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_EDIT);
            }
            else
            {
                string warning = LanguageController.Instance.GetText("message.info");
                string description = LanguageController.Instance.GetText("message.create.request.want.to.edit");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_EDIT);
            }
        }
    }


    // -------------------------------------------
    /* 
	 * RemovePressed
	 */
    private void RemovePressed()
    {
        if (!m_loadedAllOffersData)
        {
            string warning = LanguageController.Instance.GetText("message.warning");
            string description = LanguageController.Instance.GetText("screen.create.request.wait.until.loaded.information");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
        }
        else
        {
            if (m_requestData.Provider != -1)
            {
                string warning = LanguageController.Instance.GetText("message.warning");
                string description = LanguageController.Instance.GetText("message.create.request.remove.with.penalty");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_REMOVE_WITH_PENALTY);
            }
            else
            {
                string warning = LanguageController.Instance.GetText("message.warning");
                string description = LanguageController.Instance.GetText("message.create.request.want.to.remove");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_REMOVE);
            }
        }
    }

    // -------------------------------------------
    /* 
	 * OnCheckCustomerProfile
	 */
    private void OnCheckCustomerProfile()
    {
        m_pressedCheckCustomerProfile = true;
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
        BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, (long)m_requestData.Customer);
    }

    // -------------------------------------------
    /* 
	 * OnRequestTitle
	 */
    private void OnRequestTitle(string _data)
    {
        m_requestData.Title = _data;
        IsReadyToPublish = true;
    }

    // -------------------------------------------
    /* 
	 * OnRequestDescription
	 */
    private void OnRequestDescription(string _data)
    {
        m_requestData.Description = _data;
        IsReadyToPublish = true;
    }

    // -------------------------------------------
    /* 
	 * OnCanTravelClick
	 */
    private void OnCanTravelClick(bool _value)
    {
        m_requestData.Travel = (_value ? 1 : 0);
        IsReadyToPublish = true;
    }

    // -------------------------------------------
    /* 
	 * OnProvideMaterialClick
	 */
    private void OnProvideMaterialClick(bool _value)
    {
        m_requestData.Material = (_value ? 1 : 0);
        IsReadyToPublish = true;
    }

    // -------------------------------------------
    /* 
	 * OnNotifyByEmailClick
	 */
    private void OnNotifyByEmailClick(bool _value)
    {
        m_requestData.Notifications = (_value ? 1 : 0);
        IsReadyToPublish = true;
    }

    // -------------------------------------------
    /* 
	 * OnRequestPrice
	 */
    private void OnRequestPrice(string _data)
    {
        int priceRequest = -1;
        if (!int.TryParse(_data, out priceRequest))
        {
            priceRequest = -1;
        }
        m_requestData.Price = priceRequest;
        IsReadyToPublish = true;
    }

    // -------------------------------------------
    /* 
	 * OnRequestCurrency
	 */
    private void OnRequestCurrency(int _index)
    {
        m_requestData.Currency = m_container.Find("RequestPriceCurrency").GetComponent<Dropdown>().options[_index].text;
        IsReadyToPublish = true;
    }

    // -------------------------------------------
    /* 
	 * SelectRequestCurrencyByName
	 */
    private void SelectRequestCurrencyByName(string _currencyName)
    {
        if ((_currencyName == null) || (_currencyName.Length == 0))
        {
            m_container.Find("RequestPriceCurrency").GetComponent<Dropdown>().value = 0;
            OnRequestCurrency(0);
            return;
        }

        List<Dropdown.OptionData> options = m_container.Find("RequestPriceCurrency").GetComponent<Dropdown>().options;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].text.ToUpper() == _currencyName.ToUpper())
            {
                m_container.Find("RequestPriceCurrency").GetComponent<Dropdown>().value = i;
                return;
            }
        }
    }

    // -------------------------------------------
    /* 
	 * OnRequestDistanceDropdown
	 */
    private void OnRequestDistanceDropdown(int _index)
    {
        m_requestData.Distance = _index;
        IsReadyToPublish = true;
    }

    // -------------------------------------------
    /* 
	 * SelectRequestCurrencyByName
	 */
    private void SelectRequestDistanceByIndex(int _index)
    {
        m_container.Find("RequestDisctanceDropdown").GetComponent<Dropdown>().value = _index;
    }

    // -------------------------------------------
    /* 
	 * OnCalendarClick
	 */
    private void OnCalendarClick()
    {
        ScreenController.Instance.CreateNewScreen(ScreenCalendarView.SCREEN_CALENDAR, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, false, m_requestData.Deadline.ToString());
    }

    // -------------------------------------------
    /* 
	 * OnGoogleMaps
	 */
    private void OnGoogleMaps()
    {
        ScreenController.Instance.CreateNewScreen(ScreenGoogleMapView.SCREEN_GOOGLEMAP, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, false, m_requestData.Mapdata, m_requestData.Village);
    }

    // -------------------------------------------
    /* 
	 * OnAddNewImage
	 */
    private void OnAddNewImage()
    {
#if ENABLED_FACEBOOK
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.no.add.media.on.web"), null, "");
#else
        m_typeImageToUpload = RequestModel.IMAGE_TYPE_REFERENCE;
        if (m_imagesReferences.Count >= ScreenController.Instance.TotalNumberImagesAsReference)
        {
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.maximum.number.images", ScreenController.Instance.TotalNumberImagesAsReference), null, "");
        }
        else
        {
            ScreenController.Instance.CreateNewScreenNoParameters(ScreenTypeMediaView.SCREEN_TYPE_MEDIA, false, TypePreviousActionEnum.KEEP_CURRENT_SCREEN);
        }
#endif
    }

    // -------------------------------------------
    /* 
	 * OnAddFinishedImage
	 */
    private void OnAddFinishedImage()
    {
#if ENABLED_FACEBOOK
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.no.add.media.on.web"), null, "");
#else
        if (!m_loadedAllOffersData)
        {
            string warning = LanguageController.Instance.GetText("message.warning");
            string description = LanguageController.Instance.GetText("screen.create.request.wait.until.loaded.information");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
        }
        else
        {
            if (m_imagesFinishedJob.Count == 0)
            {
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.add.final.image"), null, SUB_EVENT_SCREENCREATEREQUEST_UPLOAD_FINAL_IMAGE);
            }
            else
            {
                m_typeImageToUpload = RequestModel.IMAGE_TYPE_FINISHED;
                if (m_imagesFinishedJob.Count >= ScreenController.Instance.TotalNumberImagesAsFinished)
                {
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.maximum.number.images", ScreenController.Instance.TotalNumberImagesAsFinished), null, "");
                }
                else
                {
                    ScreenController.Instance.CreateNewScreenNoParameters(ScreenTypeMediaView.SCREEN_TYPE_MEDIA, false, TypePreviousActionEnum.KEEP_CURRENT_SCREEN);
                }
            }
        }
#endif
    }

    // -------------------------------------------
    /* 
	 * OnBreakDeal
	 */
    private void OnBreakDeal()
    {
#if ENABLED_FACEBOOK
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.no.add.media.on.web"), null, "");
#else
        if (!m_loadedAllOffersData)
        {
            string warning = LanguageController.Instance.GetText("message.warning");
            string description = LanguageController.Instance.GetText("screen.create.request.wait.until.loaded.information");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
        }
        else
        {
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.confirmation.break.deal"), null, SUB_EVENT_SCREENCREATEREQUEST_DEAL_BROKEN_CONFIRMATION);
        }
#endif
    }

    // -------------------------------------------
    /* 
     * CheckCheckTimeLastOfferProposed
     */
    private bool CheckTimeLastOfferProposed()
    {
        int indexProposal = -1;
        long maxDate = -1;
        for (int i = 0; i < m_slotOfferList.Count; i++)
        {
            if (m_slotOfferList[i].GetComponent<SlotOfferView>().Proposal.User == UsersController.Instance.CurrentUser.Id)
            {
                if (maxDate < m_slotOfferList[i].GetComponent<SlotOfferView>().Proposal.Created)
                {
                    indexProposal = i;
                    maxDate = m_slotOfferList[indexProposal].GetComponent<SlotOfferView>().Proposal.Created;
                }
            }
        }

        if (indexProposal != -1)
        {
            int hoursToBeAbleToPost = DateConverter.GetTimeDifferenceInHours(maxDate);
            if (hoursToBeAbleToPost > ScreenController.Instance.HoursToEnableANewProposal)
            {
                return true;
            }
            else
            {
                if (UsersController.Instance.CurrentUser.Additionaloffer <= 0)
                {
                    ScreenController.Instance.CreateNewScreen(ScreenPremiumPostView.SCREEN_PREMIUM_POST, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, false, hoursToBeAbleToPost);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        else
        {
            return true;
        }
    }

    // -------------------------------------------
    /* 
     * OnPostNewOffer
     */
    private void OnPostNewOffer()
    {
        if (!m_loadedAllOffersData)
        {
            string warning = LanguageController.Instance.GetText("message.warning");
            string description = LanguageController.Instance.GetText("screen.create.request.wait.until.loaded.information");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
        }
        else
        {
            if (m_requestData.ReportedByUser(UsersController.Instance.CurrentUser.Id))
            {
                string warning = LanguageController.Instance.GetText("message.warning");
                string description = LanguageController.Instance.GetText("screen.create.request.you.already.reported");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
            }
            else
            {
                if (m_requestData.IsFlagged())
                {
                    string warning = LanguageController.Instance.GetText("message.warning");
                    string description = LanguageController.Instance.GetText("screen.create.request.has.been.flagged");
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
                }
                else
                {
                    if (!UsersController.Instance.CurrentUser.Validated)
                    {
                        string warning = LanguageController.Instance.GetText("message.warning");
                        string description = LanguageController.Instance.GetText("message.user.not.validated");
                        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
                    }
                    else
                    {
                        if (CheckTimeLastOfferProposed())
                        {
                            if (m_requestData.Customer == UsersController.Instance.CurrentUser.Id)
                            {
                                ScreenController.Instance.CreateNewScreen(ScreenProposalView.SCREEN_CREATE_PROPOSAL, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, m_requestData.Clone(), true);
                            }
                            else
                            {
                                if (!UsersController.Instance.CurrentUser.IsProvider())
                                {
                                    ScreenController.Instance.CreateNewScreen(ScreenBecomeProviderView.SCREEN_BECOME_PROVIDER, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, false);
                                }
                                else
                                {
                                    ScreenController.Instance.CreateNewScreen(ScreenProposalView.SCREEN_CREATE_PROPOSAL, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, m_requestData.Clone());
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // -------------------------------------------
    /* 
	 * UpdateReferenceImageSelected
	 */
    private void UpdateReferenceImageSelected()
    {
        long finalIndexImageReference = 0;
        for (int i = 0; i < m_imagesReferences.Count; i++)
        {
            if (m_imagesReferences[i].GetComponent<SlotImageView>().Selected)
            {
                finalIndexImageReference = m_imagesReferences[i].GetComponent<SlotImageView>().Id;
            }
        }
        BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_UPDATE_IMG_REF, m_requestData.Id, finalIndexImageReference);
    }

    // -------------------------------------------
    /* 
	 * MessageConfirmationServer
	 */
    private void MessageConfirmationServer()
    {
        RequestsController.Instance.MustReloadRequests = true;
        BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
        string warning = LanguageController.Instance.GetText("message.info");
        string description;
        if (m_requestData.Id == -1)
        {
            description = LanguageController.Instance.GetText("message.create.request.server.confirmation");
        }
        else
        {
            description = LanguageController.Instance.GetText("message.create.request.server.changes.confirmation");
        }

        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_EXIT_CONFIRMATION);
    }

    // -------------------------------------------
    /* 
     * SetPanelScoreConsumer
     */
    private void SetPanelScoreConsumer(bool _value)
    {
        if ((m_panelScoreConsumer == null) && (m_containerScore != null))
        {
            m_panelScoreConsumer = m_containerScore.Find("StarsScoreConsumer");
        }

        if (m_panelScoreConsumer != null)
        {
            m_panelScoreConsumer.gameObject.SetActive(_value);
        }
    }

    // -------------------------------------------
    /* 
     * SetFeedbackConsumer
     */
    private void SetFeedbackConsumer(bool _value)
    {
        if ((m_editFieldFeedbackConsumer == null) && (m_containerScore != null))
        {
            m_editFieldFeedbackConsumer = m_containerScore.Find("FeedbackCustomerValue");
            m_editFieldFeedbackConsumer.GetComponent<InputField>().onEndEdit.AddListener(OnChangeConsumerFeedback);
            m_editFieldFeedbackConsumer.gameObject.SetActive(false);
            m_scrollFieldFeedbackConsumer = m_containerScore.Find("ScrollFeedbackCustomerValue/DescriptionValue");
            m_scrollFieldFeedbackConsumer.parent.gameObject.SetActive(false);
        }

        if (m_editFieldFeedbackConsumer != null)
        {
            m_editFieldFeedbackConsumer.gameObject.SetActive(false);
            m_scrollFieldFeedbackConsumer.parent.gameObject.SetActive(false);
            if (m_requestData.FeedbackCustomerGivesToTheProvider.Length == 0)
            {
                m_editFieldFeedbackConsumer.GetComponent<InputField>().text = m_requestData.FeedbackCustomerGivesToTheProvider;
                m_editFieldFeedbackConsumer.gameObject.SetActive(_value);
            }
            else
            {
                m_scrollFieldFeedbackConsumer.GetComponent<Text>().text = m_requestData.FeedbackCustomerGivesToTheProvider;
                m_scrollFieldFeedbackConsumer.parent.gameObject.SetActive(_value);
            }
        }
    }

    // -------------------------------------------
    /* 
     * SetPanelScoreProvider
     */
    private void SetPanelScoreProvider(bool _value)
    {
        if ((m_panelScoreProvider == null) && (m_containerScore != null))
        {
            m_panelScoreProvider = m_containerScore.Find("StarsScoreProvider");
        }

        if (m_panelScoreProvider != null)
        {
            m_panelScoreProvider.gameObject.SetActive(_value);
        }
    }

    // -------------------------------------------
    /* 
     * SetFeedbackProvider
     */
    private void SetFeedbackProvider(bool _value)
    {
        if ((m_editFieldFeedbackProvider == null) && (m_containerScore != null))
        {
            m_editFieldFeedbackProvider = m_containerScore.Find("FeedbackProviderValue");
            m_editFieldFeedbackProvider.GetComponent<InputField>().onEndEdit.AddListener(OnChangeProviderFeedback);
            m_editFieldFeedbackProvider.gameObject.SetActive(false);
            m_scrollFieldFeedbackProvider = m_containerScore.Find("ScrollFeedbackProviderValue/DescriptionValue");
            m_scrollFieldFeedbackProvider.parent.gameObject.SetActive(false);
        }

        if (m_editFieldFeedbackProvider != null)
        {
            m_editFieldFeedbackProvider.gameObject.SetActive(false);
            m_scrollFieldFeedbackProvider.parent.gameObject.SetActive(false);
            if (m_requestData.FeedbackProviderGivesToTheCustomer.Length == 0)
            {
                m_editFieldFeedbackProvider.GetComponent<InputField>().text = m_requestData.FeedbackProviderGivesToTheCustomer;
                m_editFieldFeedbackProvider.gameObject.SetActive(_value);
            }
            else
            {
                m_scrollFieldFeedbackProvider.GetComponent<Text>().text = m_requestData.FeedbackProviderGivesToTheCustomer;
                m_scrollFieldFeedbackProvider.parent.gameObject.SetActive(_value);
            }
        }
    }

    // -------------------------------------------
    /* 
     * UpdateFeedbackElements
     */
    private void UpdateFeedbackElements()
    {
        SetPanelScoreConsumer(false);
        SetFeedbackConsumer(false);
        SetPanelScoreProvider(false);
        SetFeedbackProvider(false);

        if (m_imagesFinishedJob.Count > 0)
        {
            if (m_requestData.Customer == UsersController.Instance.CurrentUser.Id)
            {
                if ((m_requestData.Deliverydate != -1)
                    && (m_requestData.ScoreCustomerGivesToTheProvider != -1)
                    && (m_requestData.ScoreProviderGivesToTheCustomer != -1)
                    && (m_requestData.FeedbackProviderGivesToTheCustomer.Length > 0)
                    && (m_requestData.FeedbackCustomerGivesToTheProvider.Length > 0))
                {
                    SetPanelScoreConsumer(true);
                    SetFeedbackConsumer(true);
                    SetPanelScoreProvider(true);
                    SetFeedbackProvider(true);
                }
                else
                {
                    if ((m_requestData.Deliverydate != -1)
                        && (m_requestData.ScoreCustomerGivesToTheProvider != -1))
                    {
                        SetPanelScoreConsumer(true);
                        SetFeedbackConsumer(true);
                    }
                    else
                    {
                        if (m_requestData.Deliverydate != -1)
                        {
                            SetPanelScoreConsumer(true);
                        }
                    }
                }
            }
            else
            {
                if (m_requestData.Provider == UsersController.Instance.CurrentUser.Id)
                {
                    if ((m_requestData.Deliverydate != -1)
                        && (m_requestData.ScoreCustomerGivesToTheProvider != -1)
                        && (m_requestData.ScoreProviderGivesToTheCustomer != -1)
                        && (m_requestData.FeedbackProviderGivesToTheCustomer.Length > 0)
                        && (m_requestData.FeedbackCustomerGivesToTheProvider.Length > 0))
                    {
                        SetPanelScoreConsumer(true);
                        SetFeedbackConsumer(true);
                        SetPanelScoreProvider(true);
                        SetFeedbackProvider(true);
                    }
                    else
                    {
                        if ((m_requestData.Deliverydate != -1)
                            && (m_requestData.ScoreCustomerGivesToTheProvider != -1)
                            && (m_requestData.ScoreProviderGivesToTheCustomer != -1))
                        {
                            SetPanelScoreProvider(true);
                            SetFeedbackProvider(true);
                        }
                        else
                        {
                            if ((m_requestData.Deliverydate != -1)
                                && (m_requestData.ScoreCustomerGivesToTheProvider != -1))
                            {
                                SetPanelScoreProvider(true);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (m_requestData.Customer == UsersController.Instance.CurrentUser.Id)
            {
                if ((m_requestData.Deliverydate != -1)
                    && (m_requestData.FeedbackProviderGivesToTheCustomer.Length > 0)
                    && (m_requestData.FeedbackCustomerGivesToTheProvider.Length > 0))
                {
                    SetFeedbackProvider(true);
                    SetFeedbackConsumer(true);
                }
                else
                {
                    if (m_requestData.Deliverydate != -1)
                    {
                        SetFeedbackConsumer(true);
                    }
                }
            }
            else
            {
                if ((m_requestData.Deliverydate != -1)
                    && (m_requestData.FeedbackProviderGivesToTheCustomer.Length > 0)
                    && (m_requestData.FeedbackCustomerGivesToTheProvider.Length > 0))
                {
                    SetFeedbackProvider(true);
                    SetFeedbackConsumer(true);
                }
                else
                {
                    if (m_requestData.Deliverydate != -1)
                    {
                        SetFeedbackProvider(true);
                    }
                }
            }
        }
    }

    // -------------------------------------------
    /* 
     * OnBasicEvent
     */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (!this.gameObject.activeSelf)
        {
            return;
        }

        if (_nameEvent == UsersController.EVENT_USER_UPDATE_VILLAGE)
        {
            m_requestData.Village = (string)_list[0];
            m_requestData.Mapdata = (string)_list[1];
            IsReadyToPublish = true;

            m_container.Find("Button_Maps/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.area.search") + '\n' + m_requestData.Village;
        }
        if (_nameEvent == ScreenCalendarView.EVENT_SCREENCALENDAR_SELECT_DAY)
        {
            m_requestData.Deadline = (long)_list[0];
            m_container.Find("Button_DeadlineCalendar/Title").GetComponent<Text>().text = DateConverter.TimeStampToDateTimeString(m_requestData.Deadline);
            IsReadyToPublish = true;
        }
        if (_nameEvent == ScreenInformationView.EVENT_SCREENINFORMATION_CONFIRMATION_POPUP)
        {
            string subEvent = (string)_list[2];
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_CONFIRMATION)
            {
                if ((bool)_list[1])
                {
                    m_hasChanged = false;
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_CREATE_OR_UPDATE_REQUEST, m_requestData);
                }
                else
                {
                    string warning = LanguageController.Instance.GetText("message.warning");
                    string description = LanguageController.Instance.GetText("message.operation.canceled");
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_EXIT_WITHOUT_SAVING)
            {
                if ((bool)_list[1])
                {
                    BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_EXIT_CONFIRMATION)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_EDIT)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    if (m_flagResetAllProposal)
                    {
                        BasicEventController.Instance.DispatchBasicEvent(ProposalsController.EVENT_PROPOSAL_CALL_RESET_ALL_PROPOSALS, m_requestData.Id);
                    }
                    else
                    {
                        ScreenController.Instance.CreateNewScreen(ScreenCreateRequestView.SCREEN_CREATE_REQUEST, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, m_requestData);
                    }
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_WANT_TO_REMOVE)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_DELETE_RECORDS, m_requestData.Id);
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_REMOVE_WITH_PENALTY)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_DELETE_RECORDS, m_requestData.Id);
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_BECOME_A_PROVIDER)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    ScreenController.Instance.CreateNewScreenNoParameters(ScreenProfileView.SCREEN_PROFILE, TypePreviousActionEnum.DESTROY_ALL_SCREENS);
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_UPLOAD_FINAL_IMAGE)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    m_typeImageToUpload = RequestModel.IMAGE_TYPE_FINISHED;
                    ScreenController.Instance.CreateNewScreenNoParameters(ScreenTypeMediaView.SCREEN_TYPE_MEDIA, false, TypePreviousActionEnum.KEEP_CURRENT_SCREEN);
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_FEEDBACK_CUSTOMER)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_SCORE_AND_FEEDBACK_UPDATE, m_requestData.Id, m_requestData.ScoreCustomerGivesToTheProvider, m_requestData.FeedbackCustomerGivesToTheProvider, m_requestData.ScoreProviderGivesToTheCustomer, m_requestData.FeedbackProviderGivesToTheCustomer);
                }
                else
                {
                    m_requestData.FeedbackCustomerGivesToTheProvider = "";
                    m_editFieldFeedbackConsumer.GetComponent<InputField>().text = m_requestData.FeedbackCustomerGivesToTheProvider;
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_FEEDBACK_PROVIDER)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_SCORE_AND_FEEDBACK_UPDATE, m_requestData.Id, m_requestData.ScoreCustomerGivesToTheProvider, m_requestData.FeedbackCustomerGivesToTheProvider, m_requestData.ScoreProviderGivesToTheCustomer, m_requestData.FeedbackProviderGivesToTheCustomer);
                }
                else
                {
                    m_requestData.FeedbackProviderGivesToTheCustomer = "";
                    m_editFieldFeedbackProvider.GetComponent<InputField>().text = m_requestData.FeedbackProviderGivesToTheCustomer;
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_SCORE_PROVIDER)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    SetFeedbackConsumer(true);
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    m_panelScoreConsumer.GetComponent<PanelRatingView>().SetScore(m_requestData.ScoreCustomerGivesToTheProvider, 1);
                    m_panelScoreConsumer.GetComponent<PanelRatingView>().IsInteractable = false;
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_SCORE_AND_FEEDBACK_UPDATE, m_requestData.Id, m_requestData.ScoreCustomerGivesToTheProvider, m_requestData.FeedbackCustomerGivesToTheProvider, m_requestData.ScoreProviderGivesToTheCustomer, m_requestData.FeedbackProviderGivesToTheCustomer);
                }
                else
                {
                    m_requestData.ScoreCustomerGivesToTheProvider = -1;
                    m_panelScoreConsumer.GetComponent<PanelRatingView>().SetScore(0, 1);
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_SCORE_CUSTOMER)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    m_editFieldFeedbackProvider.gameObject.SetActive(true);
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    m_panelScoreProvider.GetComponent<PanelRatingView>().SetScore(m_requestData.ScoreProviderGivesToTheCustomer, 1);
                    m_panelScoreProvider.GetComponent<PanelRatingView>().IsInteractable = false;
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_SCORE_AND_FEEDBACK_UPDATE, m_requestData.Id, m_requestData.ScoreCustomerGivesToTheProvider, m_requestData.FeedbackCustomerGivesToTheProvider, m_requestData.ScoreProviderGivesToTheCustomer, m_requestData.FeedbackProviderGivesToTheCustomer);
                }
                else
                {
                    m_requestData.ScoreProviderGivesToTheCustomer = -1;
                    m_panelScoreProvider.GetComponent<PanelRatingView>().SetScore(0, 1);
                }
            }
            if (subEvent == SUB_EVENT_SCREENCREATEREQUEST_DEAL_BROKEN_CONFIRMATION)
            {
                if ((bool)_list[1])
                {
                    if (m_buttonAddFinishedImages != null) m_buttonAddFinishedImages.gameObject.SetActive(false);
                    if (m_buttonBreakDeal != null) m_buttonBreakDeal.gameObject.SetActive(false);
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_SET_JOB_AS_FINISHED, m_requestData.Id, true);
                }
            }
        }
        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_DELETE_RECORDS)
        {
            RequestsController.Instance.MustReloadRequests = true;
            if ((bool)_list[0])
            {
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("screen.create.request.delete.success"), null, SUB_EVENT_SCREENCREATEREQUEST_EXIT_WITHOUT_SAVING);
            }
            else
            {
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.error"), LanguageController.Instance.GetText("screen.create.request.delete.failure"), null, SUB_EVENT_SCREENCREATEREQUEST_EXIT_WITHOUT_SAVING);
            }
        }
        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_CREATED_RECORD_CONFIRMATION)
        {
            if ((bool)_list[0])
            {
                m_requestData.Id = (long)_list[1];
                if (m_imagesReferences.Count > 0)
                {
                    BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION);
                }
                else
                {
                    UpdateReferenceImageSelected();
                }
            }
            else
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                string warning = LanguageController.Instance.GetText("message.error");
                string description = LanguageController.Instance.GetText("message.create.request.server.error");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENCREATEREQUEST_EXIT_CONFIRMATION);
            }
        }
        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_UPDATE_IMG_REF)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            if (m_isDisplayInfo)
            {
                string info = LanguageController.Instance.GetText("message.info");
                string description = LanguageController.Instance.GetText("message.create.request.server.changes.confirmation");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, info, description, null, "");
            }
            else
            {
                if ((bool)_list[0])
                {
                    MessageConfirmationServer();
                }
                else
                {
                    BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                    string warning = LanguageController.Instance.GetText("message.error");
                    string description = LanguageController.Instance.GetText("message.create.request.server.error");
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
                }
            }
        }
        if (_nameEvent == ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION)
        {
            if (m_requestData.Provider == -1)
            {
                m_commitImageIndex++;
                if (_list.Length > 0)
                {
                    if ((bool)_list[0])
                    {
                        long indexImageAssigned = (long)_list[1];
                        m_imagesReferences[m_commitImageIndex - 1].GetComponent<SlotImageView>().Id = indexImageAssigned;
                    }
                }
                if (m_commitImageIndex < m_imagesReferences.Count)
                {
                    m_imagesReferences[m_commitImageIndex].GetComponent<SlotImageView>().UploadImages(ScreenController.TABLE_REQUESTS, m_requestData.Id, m_commitImageIndex);
                }
                else
                {
                    UpdateReferenceImageSelected();
                }
            }
            else
            {
                if ((bool)_list[0])
                {
                    RequestsController.Instance.MustReloadRequests = true;
                    if (m_imagesFinishedJob.Count == 1)
                    {
                        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                        BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_SET_JOB_AS_FINISHED, m_requestData.Id, false);
                    }
                    else
                    {
                        BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                    }
                }
            }
        }
        if (_nameEvent == ScreenSystemNavigatorView.EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION)
        {
            if ((bool)_list[0])
            {
                string filePathImage = (string)_list[1];
                float scrollPosition = (float)_list[2];
                if (m_typeImageToUpload == RequestModel.IMAGE_TYPE_REFERENCE)
                {
                    GameObject newImageRequest = Utilities.AddChild(m_referenceImagesContainer, ScreenController.Instance.SlotImage);
                    newImageRequest.GetComponent<SlotImageView>().Initialize(m_requestData.Id, -1, filePathImage, (m_requestData.Images == 0), (m_requestData.Customer == UsersController.Instance.CurrentUser.Id), RequestModel.IMAGE_TYPE_REFERENCE, scrollPosition);
                    m_imagesReferences.Add(newImageRequest);
                    m_requestData.Images++;
                    if (m_imageLoadingReference != null) m_imageLoadingReference.gameObject.SetActive(false);
                    IsReadyToPublish = true;
                }
                else
                {
                    RequestsController.Instance.MustReloadRequests = true;
                    GameObject newFinishedRequest = Utilities.AddChild(m_finishedImagesContainer, ScreenController.Instance.SlotImage);
                    newFinishedRequest.GetComponent<SlotImageView>().Initialize(m_requestData.Id, -1, filePathImage, false, false, RequestModel.IMAGE_TYPE_FINISHED, scrollPosition);
                    m_imagesFinishedJob.Add(newFinishedRequest);
                    if (m_imageLoadingFinished != null) m_imageLoadingFinished.gameObject.SetActive(false);
                    if (m_buttonBreakDeal != null) m_buttonBreakDeal.gameObject.SetActive(false);
                }
            }
            else
            {
                if (_list.Length > 1)
                {
                    string urlImage = (string)_list[1];
                    if (m_typeImageToUpload == RequestModel.IMAGE_TYPE_REFERENCE)
                    {
                        GameObject newImageRequest = Utilities.AddChild(m_referenceImagesContainer, ScreenController.Instance.SlotImage);
                        newImageRequest.GetComponent<SlotImageView>().InitializeWithURL(m_requestData.Id, -1, ScreenController.Instance.ImageReferenceLink, (m_requestData.Images == 0), (m_requestData.Customer == UsersController.Instance.CurrentUser.Id), RequestModel.IMAGE_TYPE_REFERENCE, urlImage);
                        m_imagesReferences.Add(newImageRequest);
                        m_requestData.Images++;
                    }
                    else
                    {
                        RequestsController.Instance.MustReloadRequests = true;
                        GameObject newImageRequest = Utilities.AddChild(m_finishedImagesContainer, ScreenController.Instance.SlotImage);
                        newImageRequest.GetComponent<SlotImageView>().InitializeWithURL(m_requestData.Id, -1, ScreenController.Instance.ImageReferenceLink, (m_requestData.Images == 0), (m_requestData.Customer == UsersController.Instance.CurrentUser.Id), RequestModel.IMAGE_TYPE_FINISHED, urlImage);
                        m_imagesFinishedJob.Add(newImageRequest);
                        if (m_imageLoadingFinished != null) m_imageLoadingFinished.gameObject.SetActive(false);
                        if (m_buttonBreakDeal != null) m_buttonBreakDeal.gameObject.SetActive(false);
                    }
                }
            }
        }
        if (_nameEvent == EVENT_SCREENCREATEREQUEST_DELAY_LOAD_IMAGE)
        {
            LoadPopImages();
        }
        if (_nameEvent == ImagesController.EVENT_IMAGES_LOAD_SERVER_DATA_RECEIVED)
        {
            if (_list.Length > 0)
            {
                m_counterImagesLoaded--;
                if (m_counterImagesLoaded <= 0)
                {
                    // BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                }
            }
        }
        if (_nameEvent == ImagesController.EVENT_IMAGES_LOAD_SERVER_LOCAL_DATA_LOADED)
        {
            m_counterImagesLoaded--;
            if (m_counterImagesLoaded <= 0)
            {
                // BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            }
        }
        if (_nameEvent == ImagesController.EVENT_IMAGES_CALL_DELETE_IMAGE)
        {
            GameObject slotImage = (GameObject)_list[0];
            for (int i = 0; i < m_imagesReferences.Count; i++)
            {
                if (slotImage == m_imagesReferences[i].gameObject)
                {
                    m_imagesReferences[i].GetComponent<SlotImageView>().Destroy();
                    m_imagesReferences.RemoveAt(i);
                    m_requestData.Images--;
                    break;
                }
            }
        }
        if (_nameEvent == ImagesController.EVENT_IMAGES_SELECTED_THUMBNAIL_REFERENCE)
        {
            GameObject slotImage = (GameObject)_list[0];
            long idImage = (long)_list[1];
            bool isOnlyDisplay = (bool)_list[2];
            m_requestData.Referenceimg = idImage;
            for (int i = 0; i < m_imagesReferences.Count; i++)
            {
                if (slotImage != m_imagesReferences[i].gameObject)
                {
                    m_imagesReferences[i].GetComponent<SlotImageView>().Selected = false;
                }
                else
                {
                    m_imagesReferences[i].GetComponent<SlotImageView>().Selected = true;
                }
            }
            if (isOnlyDisplay)
            {
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_UPDATE_IMG_REF, m_requestData.Id, m_requestData.Referenceimg);
            }
            IsReadyToPublish = true;
        }
        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_IMAGES_REQUEST)
        {
            List<ImageModel> imagesForRequest = (List<ImageModel>)_list[0];
            if (imagesForRequest.Count > 0)
            {
                m_requestData.CopyImages(imagesForRequest);
                InitializeImagesContainer();
            }
        }
        if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_FORMATTED_PROPOSALS)
        {
            m_loadedAllOffersData = true;
            List<ProposalModel> proposals = (List<ProposalModel>)_list[0];
            if (proposals != null)
            {
                if (proposals.Count > 0)
                {
                    m_imageLoadingOffers.gameObject.SetActive(false);
                    m_offersTextLoading.SetActive(false);

                    ClearAllProposals();
                    for (int i = 0; i < proposals.Count; i++)
                    {
                        ProposalModel item = proposals[i];
                        if (item.IsSelected() && item.IsDisplayable())
                        {
                            GameObject newSlotInfoRequest = Utilities.AddChild(m_offerContainer, ScreenController.Instance.SlotOffer);
                            newSlotInfoRequest.GetComponent<SlotOfferView>().Initialize(item);
                            m_slotOfferList.Add(newSlotInfoRequest);
                        }
                    }
                    for (int i = 0; i < proposals.Count; i++)
                    {
                        ProposalModel item = proposals[i];
                        if (!item.IsSelected() && item.IsDisplayable())
                        {
                            GameObject newSlotInfoRequest = Utilities.AddChild(m_offerContainer, ScreenController.Instance.SlotOffer);
                            newSlotInfoRequest.GetComponent<SlotOfferView>().Initialize(item);
                            m_slotOfferList.Add(newSlotInfoRequest);
                        }
                    }
                }
                else
                {
                    m_imageLoadingOffers.gameObject.SetActive(true);
                    m_offersTextLoading.SetActive(true);
                    m_offersTextLoading.GetComponent<Text>().text = LanguageController.Instance.GetText("message.no.records");
                }
            }
        }
        if (_nameEvent == SlotOfferView.EVENT_SLOTOFFER_SELECTED_PROPOSAL)
        {
            GameObject slotProposal = (GameObject)_list[0];
            for (int i = 0; i < m_slotOfferList.Count; i++)
            {
                if (slotProposal == m_slotOfferList[i].gameObject)
                {
                    switch (slotProposal.GetComponent<SlotOfferView>().Proposal.Type)
                    {
                        case ProposalModel.TYPE_INFO:
                            ScreenController.Instance.CreateNewScreen(ScreenProposalView.SCREEN_QUESTION_PROPOSAL, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, m_slotOfferList[i].GetComponent<SlotOfferView>().Proposal, m_requestData);
                            break;

                        case ProposalModel.TYPE_OFFER:
                            ScreenController.Instance.CreateNewScreen(ScreenProposalView.SCREEN_DISPLAY_PROPOSAL, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, m_slotOfferList[i].GetComponent<SlotOfferView>().Proposal, m_requestData);
                            break;
                    }
                    return;
                }
            }
        }
        if (_nameEvent == SCOREEVENT_RATING_CUSTOMER_TO_PROVIDER)
        {
            m_requestData.ScoreCustomerGivesToTheProvider = (int)_list[1] + 1;
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.are.you.sure.score", m_requestData.ScoreCustomerGivesToTheProvider), null, SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_SCORE_PROVIDER);
        }
        if (_nameEvent == SCOREEVENT_RATING_PROVIDER_TO_CUSTOMER)
        {
            m_requestData.ScoreProviderGivesToTheCustomer = (int)_list[1] + 1;
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.are.you.sure.score", m_requestData.ScoreProviderGivesToTheCustomer), null, SUB_EVENT_SCREENCREATEREQUEST_ARE_YOUR_SURE_SCORE_CUSTOMER);
        }
        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_SCORE_AND_FEEDBACK_UPDATE)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            if ((bool)_list[0])
            {
                string info = LanguageController.Instance.GetText("message.info");
                string description = LanguageController.Instance.GetText("message.create.request.updated.success.score.feedback");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, info, description, null, "");
                UpdateFeedbackElements();
                UpdateScoreElements(true);
            }
            else
            {
                string info = LanguageController.Instance.GetText("message.info");
                string description = LanguageController.Instance.GetText("message.create.request.updated.failure.score.feedback");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, info, description, null, "");
            }
        }
        if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_RESET_ALL_PROPOSALS)
        {
            if (m_flagResetAllProposal)
            {
                m_flagResetAllProposal = false;
                if ((bool)_list[0])
                {
                    // PROCEED TO EDIT
                    ScreenController.Instance.CreateNewScreen(ScreenCreateRequestView.SCREEN_CREATE_REQUEST, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, m_requestData);
                }
                else
                {
                    string info = LanguageController.Instance.GetText("message.error");
                    string description = LanguageController.Instance.GetText("message.create.request.failure.reset.proposals");
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, info, description, null, "");
                }
            }
        }
        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_SET_JOB_AS_FINISHED)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            if (m_imagesFinishedJob.Count > 0)
            {
                if ((bool)_list[0])
                {
                    RequestsController.Instance.MustReloadRequests = true;
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("screen.create.request.success.finish.job"), null, "");
                }
                else
                {
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("screen.create.request.failure.finish.job"), null, "");
                }
            }
            else
            {
                if ((bool)_list[0])
                {
                    RequestsController.Instance.MustReloadRequests = true;
                    UpdateScoreElements(true);
                    if (m_requestData.Customer == UsersController.Instance.CurrentUser.Id)
                    {
                        SetPanelScoreConsumer(true);
                        SetFeedbackConsumer(true);
                    }
                    else
                    {
                        SetPanelScoreProvider(true);
                        SetFeedbackProvider(true);
                    }
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("screen.create.request.success.broken.job"), null, "");
                }
                else
                {
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("screen.create.request.failure.broken.job"), null, "");
                }
            }
        }
        if (_nameEvent == UsersController.EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            if (m_pressedCheckCustomerProfile)
            {
                m_pressedCheckCustomerProfile = false;
                if (_list == null) return;
                if (_list.Length == 0) return;
                UserModel consumer = (UserModel)_list[0];
                if (consumer != null)
                {
                    ScreenController.Instance.CreateNewScreen(ScreenCustomerProfileView.SCREEN_CUSTOMER_PROFILE, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, consumer);
                }
            }
            else
            {
                OnPostNewOffer();
            }
        }
        if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
        {
            if (!m_isDisplayInfo)
            {
                BackEditionPressed();
            }
            else
            {
                BackPressed();
            }
        }
    }
}