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
 * It displays allows to edit the provider profile.
 * 
 * Sections:
 *      -Description
 *      -Provider Skills
 *      -Images of previous works
 *      -History of works done in this app
 * 
 * @author Esteban Gallardo
 */
public class ScreenProviderProfileView : ScreenBaseView, IBasicScreenView
{
    public const string SCREEN_PROVIDER_PROFILE_DISPLAY = "SCREEN_PROVIDER_PROFILE_DISPLAY";
    public const string SCREEN_PROVIDER_PROFILE_EDITION = "SCREEN_PROVIDER_PROFILE_EDITION";

    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------	
    public const string EVENT_SCREENPROVIDERPROFILE_CLICK_PROPERTY              = "EVENT_SCREENPROVIDERPROFILE_CLICK_PROPERTY";
    public const string EVENT_SCREENPROVIDERPROFILE_LOAD_USER_DATA_EXPERIENCE   = "EVENT_SCREENPROVIDERPROFILE_LOAD_USER_DATA_EXPERIENCE";

    // ----------------------------------------------
    // SUB
    // ----------------------------------------------	
    public const string SUB_EVENT_SCREENPROVIDERPROFILE_SAVE_CONFIRMATION       = "SUB_EVENT_SCREENPROVIDERPROFILE_SAVE_CONFIRMATION";
    public const string SUB_EVENT_SCREENPROVIDERPROFILE_EXIT_WITHOUT_SAVING     = "SUB_EVENT_SCREENPROVIDERPROFILE_EXIT_WITHOUT_SAVING";

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------	
    private GameObject m_root;
    private Transform m_container;
    private UserModel m_userData;
    private bool m_isDisplayInfo;
    private Transform m_requestsContainer;

    private Transform m_imagesContainer;
    private List<GameObject> m_imagesExperience = new List<GameObject>();
    private Transform m_imageLoadingExperience;

    private Transform m_btnExit;
    private Transform m_btnSave;
    private bool m_hasBeenModified = false;

    private int m_imagesLeftToConfirmFromUpload = 0;

    private List<GameObject> m_skillsPanels = new List<GameObject>();

    // ----------------------------------------------
    // GETTERS/SETTERS
    // ----------------------------------------------	
    public bool HasBeenModified
    {
        get { return m_hasBeenModified; }
        set { m_hasBeenModified = value;
            if (m_hasBeenModified)
            {
                UsersController.Instance.MustReloadUsers = true;
                if (m_btnSave != null) m_btnSave.gameObject.SetActive(true);
            }
            else
            {
                if (m_btnSave != null) m_btnSave.gameObject.SetActive(false);
            }
        }
    }

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public void Initialize(params object[] _list)
    {
        m_userData = (UserModel)_list[0];

        m_root = this.gameObject;
        m_container = m_root.transform.Find("Content/ScrollPage/Page");

        Transform buttonEdit = m_root.transform.Find("Content/Button_Edit");
        if (buttonEdit != null)
        {
            buttonEdit.gameObject.SetActive(false);
            buttonEdit.GetComponent<Button>().onClick.AddListener(EditPressed);
            m_isDisplayInfo = true;
            if (m_userData.Id == UsersController.Instance.CurrentUser.Id)
            {
                buttonEdit.gameObject.SetActive(true);
            }
            if (UsersController.Instance.CurrentUser.IsBanned())
            {
                buttonEdit.gameObject.SetActive(false);
            }
        }
        else
        {
            m_isDisplayInfo = false;
            BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_CANCEL_LOADING_IMAGES);
        }

        m_btnExit = m_root.transform.Find("Content/Button_Exit");
        if (m_btnExit != null)
        {
            if (m_isDisplayInfo)
            {
                m_btnExit.GetComponent<Button>().onClick.AddListener(ExitPressed);
            }
            else
            {
                m_btnExit.GetComponent<Button>().onClick.AddListener(ExitEditionPressed);
                HasBeenModified = false;
            }
        }

        m_btnSave = m_root.transform.Find("Content/Button_Save");
        if (m_btnSave != null)
        {
            m_btnSave.GetComponent<Button>().onClick.AddListener(SavePressed);
        }

        // DESCRIPTION
        if (!m_isDisplayInfo)
        {
            m_container.Find("DescriptionValue").GetComponent<InputField>().onEndEdit.AddListener(OnDescriptionProviderProfile);
        }

        // CONTAINER REQUESTS DONE
        m_requestsContainer = m_container.Find("YourHistoryWorks");
        if (m_requestsContainer != null)
        {
            m_requestsContainer.GetComponent<SlotManagerView>().Initialize();
        }

        // CONTAINER IMAGES EXPERIENCE
        m_imagesContainer = m_container.Find("YourImagesExperience/ScrollContent/Entries");
        m_imageLoadingExperience = m_container.Find("YourImagesExperience/ImageLoading");

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

        LoadUserData();
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
            if (UsersController.Instance.MustReloadUsers)
            {
                UsersController.Instance.MustReloadUsers = false;
                if (m_imageLoadingExperience != null) m_imageLoadingExperience.gameObject.SetActive(true);
                ClearAllImages();
                BasicEventController.Instance.DelayBasicEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, 0.1f, (long)m_userData.Id);
            }            
        }
        else
        {
            this.gameObject.SetActive(_activation);
        }
    }

    // -------------------------------------------
    /* 
	 * Destroy
	 */
    public void Destroy()
    {
        ClearAllImages();
        if (m_requestsContainer!=null)
        {
            m_requestsContainer.GetComponent<SlotManagerView>().Destroy();
        }        
        BasicEventController.Instance.BasicEvent -= OnBasicEvent;
        GameObject.DestroyObject(this.gameObject);
    }

    // -------------------------------------------
    /* 
	 * LoadUserData
	 */
    public void LoadUserData()
    {
        if (!m_userData.IsBanned())
        {
            m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.provider.profile");
        }
        m_container.Find("Name").GetComponent<Text>().text = m_userData.Nickname;
        m_container.Find("Description").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.provider.a.few.words.about");

        if (m_container.Find("StarsScoreGlobal") != null)
        {
            m_container.Find("StarsScoreGlobal").GetComponent<PanelRatingView>().Initialize(m_userData.Scoreprovider,
                                                                                            m_userData.Votesprovider,
                                                                                            LanguageController.Instance.GetText("message.provider.rating"),
                                                                                            true,
                                                                                            "",
                                                                                            !m_isDisplayInfo);
        }

        // SKILLS
        List<Transform> skillTransforms = new List<Transform>();
        Transform panelSkillValue = null;
        int counterSkill = 0;
        do
        {
            panelSkillValue = m_container.Find("StarsScore_" + counterSkill);
            if (panelSkillValue  != null)
            {
                panelSkillValue.gameObject.SetActive(false);
                skillTransforms.Add(panelSkillValue);
            }
            counterSkill++;
        } while (panelSkillValue != null);

        // FILL DATA SKILLS
        m_skillsPanels.Clear();
        for (int i = 0; i < skillTransforms.Count; i++)
        {
            if (i < m_userData.SkillsList.Count)
            {
                SkillModel itemSkill = m_userData.SkillsList[i];
                string nameSkill = itemSkill.Name;
                int valueSkill = itemSkill.Value;
                panelSkillValue = skillTransforms[i];
                if (panelSkillValue != null)
                {
                    panelSkillValue.GetComponent<PanelRatingView>().Initialize(valueSkill + 1,
                                                                                1,
                                                                                LanguageController.Instance.GetText("screen.profile.skill.name." + nameSkill),
                                                                                true,
                                                                                EVENT_SCREENPROVIDERPROFILE_CLICK_PROPERTY,
                                                                                !m_isDisplayInfo);
                    panelSkillValue.GetComponent<PanelRatingView>().Property = nameSkill;
                    m_skillsPanels.Add(panelSkillValue.gameObject);
                    panelSkillValue.gameObject.SetActive(true);
                }
            }
        }

        // DESCRIPTION
        if (m_isDisplayInfo)
        {
            m_container.Find("ScrollDescriptionValue/DescriptionValue").GetComponent<Text>().supportRichText = true;
            m_container.Find("ScrollDescriptionValue/DescriptionValue").GetComponent<Text>().text = m_userData.Description;
        }
        else
        {
            m_container.Find("DescriptionValue").GetComponent<InputField>().text = m_userData.Description;
        }

        // ADD NEW IMAGE EXPERIENCE
        Transform btnAddImage = m_container.Find("Button_Images");
        if (btnAddImage != null)
        {
            btnAddImage.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.provider.add.image.experience");
            btnAddImage.GetComponent<Button>().onClick.AddListener(OnAddNewImageExperience);
        }

        if (m_container.Find("YourImagesExperience/Title") != null) m_container.Find("YourImagesExperience/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.provider.images.experience");
        if (m_container.Find("YourHistoryWorks/Title") != null) m_container.Find("YourHistoryWorks/Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.profile.provider.related.works");        

        BasicEventController.Instance.DelayBasicEvent(EVENT_SCREENPROVIDERPROFILE_LOAD_USER_DATA_EXPERIENCE, 0.1f);
    }

    // -------------------------------------------
    /* 
     * ClearAllImages
     */
    private bool ClearAllImages()
    {
        if (m_imagesExperience.Count > 0)
        {
            for (int i = 0; i < m_imagesExperience.Count; i++)
            {
                if (m_imagesExperience[i] != null)
                {
                    if (m_imagesExperience[i].GetComponent<SlotImageView>() != null)
                    {
                        m_imagesExperience[i].GetComponent<SlotImageView>().Destroy();
                    }
                }
            }
            m_imagesExperience.Clear();
            return true;
        }
        else
        {
            return false;
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
	 * ExitEditionPressed
	 */
    private void ExitEditionPressed()
    {
        if (!m_hasBeenModified)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
        }
        else
        {
            string warning = LanguageController.Instance.GetText("message.warning");
            string description = LanguageController.Instance.GetText("message.profile.provider.exit.without.saving");
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROVIDERPROFILE_EXIT_WITHOUT_SAVING);
        }
    }

    // -------------------------------------------
    /* 
	 * SavePressed
	 */
    private void SavePressed()
    {
        string warning = LanguageController.Instance.GetText("message.warning");
        string description = LanguageController.Instance.GetText("message.profile.provider.save.confirmation");
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROVIDERPROFILE_SAVE_CONFIRMATION);
    }

    // -------------------------------------------
    /* 
	 * EditPressed
	 */
    private void EditPressed()
    {
        ScreenController.Instance.CreateNewScreen(ScreenProviderProfileView.SCREEN_PROVIDER_PROFILE_EDITION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, true, m_userData);
    }

    // -------------------------------------------
    /* 
	 * OnDescriptionProviderProfile
	 */
    private void OnDescriptionProviderProfile(string _newValue)
    {
        m_userData.Description = _newValue;
        HasBeenModified = true;
    }

    // -------------------------------------------
    /* 
	 * OnAddNewImageExperience
	 */
    private void OnAddNewImageExperience()
    {
#if ENABLED_FACEBOOK
        ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.create.request.no.add.media.on.web"), null, "");
#else
        if (m_imagesExperience.Count >= ScreenController.Instance.TotalNumberImagesAsProviderExperience)
        {
            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.warning"), LanguageController.Instance.GetText("screen.profile.provider.maximum.number.images", ScreenController.Instance.TotalNumberImagesAsProviderExperience), null, "");
        }
        else
        {
            ScreenController.Instance.CreateNewScreenNoParameters(ScreenTypeMediaView.SCREEN_TYPE_MEDIA, false, TypePreviousActionEnum.KEEP_CURRENT_SCREEN);
        }
#endif
    }


    // -------------------------------------------
    /* 
	 * OnBasicEvent
	 */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (!this.gameObject.activeSelf) return;

        if (_nameEvent == EVENT_SCREENPROVIDERPROFILE_CLICK_PROPERTY)
        {
            GameObject panelClicked = (GameObject)_list[0];
            int newValue = (int)_list[1];
            string nameProperty = (string)_list[2];
            m_userData.UpdateSingleSkill(nameProperty, newValue);
            for (int i = 0; i < m_skillsPanels.Count; i++)
            {
                if (panelClicked == m_skillsPanels[i])
                {
                    m_skillsPanels[i].GetComponent<PanelRatingView>().SetScore(newValue + 1, 1);
                }
            }
            HasBeenModified = true;
        }
        if (_nameEvent == UsersController.EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD)
        {
            UserModel sUser = (UserModel)_list[0];
            if (sUser != null)
            {
                m_userData.Copy(sUser);
                LoadUserData();
            }
        }
        if (_nameEvent == EVENT_SCREENPROVIDERPROFILE_LOAD_USER_DATA_EXPERIENCE)
        {
            // CREATE THE LIST OF EXPERIENCE IMAGES
            SlotRequestView.DestroySlots(m_imagesExperience);
            for (int i = 0; i < m_userData.ImageReferencesExperience.Count; i++)
            {
                GameObject newImageRequest = Utilities.AddChild(m_imagesContainer, ScreenController.Instance.SlotImage);
                long idImageReference = m_userData.ImageReferencesExperience[i].Id;
                string urlReference = m_userData.ImageReferencesExperience[i].Url;
                m_imagesExperience.Add(newImageRequest);
                if (m_imageLoadingExperience != null) m_imageLoadingExperience.gameObject.SetActive(false);
                newImageRequest.GetComponent<SlotImageView>().InitializeFromServerData(m_userData.Id, idImageReference, m_isDisplayInfo, false, false, RequestModel.IMAGE_TYPE_REFERENCE, urlReference, false, (m_userData.IsBanned()));
                newImageRequest.GetComponent<SlotImageView>().DisplayStar = false;
            }

            // LOAD WORKS DONE
            if (m_isDisplayInfo)
            {
                BasicEventController.Instance.DispatchBasicEvent(RequestsController.EVENT_REQUEST_CALL_CONSULT_BY_PROVIDER, m_userData.Id, true);
            }
            else
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            }
        }
        if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_FORMATTED_RECORDS)
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            List<RequestModel> data = (List<RequestModel>)_list[0];
            if (m_requestsContainer != null)
            {
                if ((int)_list[1] == RequestsController.TYPE_CONSULT_BY_PROVIDER)
                {
                    m_requestsContainer.GetComponent<SlotManagerView>().ClearCurrentGameObject();
                    m_requestsContainer.GetComponent<SlotManagerView>().ClearCurrentRequests();

                    if (data.Count > 0)
                    {
                        // ADD TOXIC AND BROKEN FIRST
                        for (int i = 0; i < data.Count; i++)
                        {
                            RequestModel item = data[i];
                            if ((item.Provider != -1) && item.IsBrokenDeal())
                            {
                                m_requestsContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
                            }
                        }

                        // ADD NORMAL REST
                        for (int i = 0; i < data.Count; i++)
                        {
                            RequestModel item = data[i];
                            if ((item.Provider != -1) && !item.IsBrokenDeal())
                            {
                                m_requestsContainer.GetComponent<SlotManagerView>().AddNewRequests(item);
                            }
                        }
                    }
                    m_requestsContainer.GetComponent<SlotManagerView>().LoadCurrentPage();
                }
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
        if (_nameEvent == ScreenSystemNavigatorView.EVENT_SCREENSYSTEMNAVIGATOR_FINAL_SELECTION)
        {
            if (_list.Length > 0)
            {
                if ((bool)_list[0])
                {
                    string filePathImage = (string)_list[1];
                    float scrollPosition = (float)_list[2];
                    GameObject newImageRequest = Utilities.AddChild(m_imagesContainer, ScreenController.Instance.SlotImage);
                    newImageRequest.GetComponent<SlotImageView>().Initialize(m_userData.Id, -1, filePathImage, false, false, RequestModel.IMAGE_TYPE_REFERENCE, scrollPosition);
                    newImageRequest.GetComponent<SlotImageView>().DisplayStar = false;
                    m_imagesExperience.Add(newImageRequest);
                }
                else
                {
                    if (_list.Length > 1)
                    {
                        string urlImage = (string)_list[1];
                        GameObject newImageRequest = Utilities.AddChild(m_imagesContainer, ScreenController.Instance.SlotImage);
                        newImageRequest.GetComponent<SlotImageView>().InitializeWithURL(m_userData.Id, -1, ScreenController.Instance.ImageReferenceLink, false, false, RequestModel.IMAGE_TYPE_REFERENCE, urlImage);
                        newImageRequest.GetComponent<SlotImageView>().DisplayStar = false;
                        m_imagesExperience.Add(newImageRequest);
                    }
                }
                HasBeenModified = true;
            }
        }
        if (_nameEvent == ImagesController.EVENT_IMAGES_CALL_DELETE_IMAGE)
        {
            GameObject slotImage = (GameObject)_list[0];
            for (int i = 0; i < m_imagesExperience.Count; i++)
            {
                if (slotImage == m_imagesExperience[i].gameObject)
                {
                    m_imagesExperience[i].GetComponent<SlotImageView>().Destroy();
                    m_imagesExperience.RemoveAt(i);
                    HasBeenModified = true;
                    break;
                }
            }
        }
        if (_nameEvent == ScreenInformationView.EVENT_SCREENINFORMATION_CONFIRMATION_POPUP)
        {
            string subEvent = (string)_list[2];
            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
            if (subEvent == SUB_EVENT_SCREENPROVIDERPROFILE_SAVE_CONFIRMATION)
            {
                if ((bool)_list[1])
                {
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
                    BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_UPDATE_PROFILE_REQUEST,
                                                                     UsersController.Instance.CurrentUser.Id.ToString(),
                                                                     UsersController.Instance.CurrentUser.PasswordPlain,
                                                                     UsersController.Instance.CurrentUser.Email,
                                                                     UsersController.Instance.CurrentUser.Nickname,
                                                                     UsersController.Instance.CurrentUser.Village,
                                                                     UsersController.Instance.CurrentUser.Mapdata,
                                                                     m_userData.Skills,
                                                                     m_userData.Description);

                }
                else
                {
                    string warning = LanguageController.Instance.GetText("message.warning");
                    string description = LanguageController.Instance.GetText("message.operation.canceled");
                    ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
                }
            }
            if (subEvent == SUB_EVENT_SCREENPROVIDERPROFILE_EXIT_WITHOUT_SAVING)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                if ((bool)_list[1])
                {
                    BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
                }
            }
        }
        if (_nameEvent == UsersController.EVENT_USER_UPDATE_PROFILE_RESULT)
        {
            if ((bool)_list[0])
            {
                if (m_imagesExperience.Count > 0)
                {
                    m_imagesLeftToConfirmFromUpload = 0;
                    m_imagesExperience[m_imagesLeftToConfirmFromUpload].GetComponent<SlotImageView>().UploadImages(ScreenController.TABLE_USERS, (long)m_userData.Id, m_imagesLeftToConfirmFromUpload);
                }
                else
                {
                    BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                    BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
                }
            }
            else
            {
                string warning = LanguageController.Instance.GetText("message.error");
                string description = LanguageController.Instance.GetText("message.operation.canceled");
                ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, warning, description, null, "");
            }
        }
        if (_nameEvent == ImagesController.EVENT_IMAGES_UPLOAD_TO_SERVER_CONFIRMATION)
        {
            m_imagesLeftToConfirmFromUpload++;
            if (m_imagesLeftToConfirmFromUpload < m_imagesExperience.Count)
            {
                m_imagesExperience[m_imagesLeftToConfirmFromUpload].GetComponent<SlotImageView>().UploadImages(ScreenController.TABLE_USERS, (long)m_userData.Id, m_imagesLeftToConfirmFromUpload);
            }
            else
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                BasicEventController.Instance.DispatchBasicEvent(ScreenController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
            }
        }
        if (_nameEvent == ScreenController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
        {
            if (m_isDisplayInfo)
            {
                ExitPressed();
            }
            else
            {
                ExitEditionPressed();
            }
        }
    }
}