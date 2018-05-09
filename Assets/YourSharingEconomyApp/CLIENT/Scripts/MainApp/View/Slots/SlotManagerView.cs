using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/******************************************
 * 
 * SlotManagerView
 * 
 * Class that allows a pagination system for 
 * the requests' list to allow multiple pagination
 * to avoid load too much data at the same time.
 * 
 * @author Esteban Gallardo
 */
public class SlotManagerView: MonoBehaviour
{
    // ----------------------------------------------
    // CONSTANTS
    // ----------------------------------------------	
    public const int DEFAULT_ITEMS_EACH_PAGE = 4; 

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------	
    private GameObject m_content;
    private List<RequestModel> m_requests = new List<RequestModel>();
    private List<GameObject> m_gameObjects = new List<GameObject>();
    private int m_currentPage = 0;
    private int m_itemsEachPage = 0;
    private Transform m_imageLoading;
    private Transform m_textLoading;

    private Transform m_buttonNext;
    private Transform m_buttonPrevious;

    // -------------------------------------------
    /* 
	 * Initialize
	 */
    public void Initialize()
    {
        Initialize(DEFAULT_ITEMS_EACH_PAGE);
    }

    // -------------------------------------------
    /* 
	 * Initialize
	 */
    public void Initialize(int _itemsEachPage)
    {
        m_itemsEachPage = _itemsEachPage;

        m_content = this.gameObject.transform.Find("ScrollContent/Entries").gameObject;
        m_buttonNext = this.gameObject.transform.Find("Button_Next");
        m_buttonPrevious = this.gameObject.transform.Find("Button_Previous");

        if (m_buttonNext != null)
        {
            m_buttonNext.GetComponent<Button>().onClick.AddListener(OnNextPressed);
            m_buttonNext.gameObject.SetActive(false);
        }
        if (m_buttonPrevious != null)
        {
            m_buttonPrevious.GetComponent<Button>().onClick.AddListener(OnPreviousPressed);
            m_buttonPrevious.gameObject.SetActive(false);
        }

        m_imageLoading = this.gameObject.transform.Find("ImageLoading");
        m_textLoading = this.gameObject.transform.Find("TextLoading");
        if (m_textLoading!=null)
        {
            m_textLoading.GetComponent<Text>().text = LanguageController.Instance.GetText("message.loading");
        }
    }

    // -------------------------------------------
    /* 
     * Destroy
     */
    public void Destroy()
    {
        ClearCurrentRequests();
        ClearCurrentGameObject();
        m_requests = null;
        m_gameObjects = null;
    }

    // -------------------------------------------
    /* 
     * AddNewRequests
     */
    public void AddNewRequests(RequestModel _request)
    {
        m_requests.Add(_request);

        if (m_imageLoading != null) m_imageLoading.gameObject.SetActive(false);
        if (m_textLoading != null) m_textLoading.gameObject.SetActive(false);
    }

    // -------------------------------------------
    /* 
	 * ClearCurrentRequests
	 */
    public void ClearCurrentRequests()
    {
        m_currentPage = 0;
        m_requests.Clear();
    }

    // -------------------------------------------
    /* 
	 * ClearCurrentGameObject
	 */
    public void ClearCurrentGameObject()
    {
        for (int i = 0; i < m_gameObjects.Count; i++)
        {
            if (m_gameObjects[i] != null)
            {
                m_gameObjects[i].GetComponent<SlotRequestView>().Destroy();
                m_gameObjects[i] = null;
            }
        }
        m_gameObjects.Clear();

        if (m_imageLoading != null) m_imageLoading.gameObject.SetActive(true);
        if (m_textLoading != null)
        {
            m_textLoading.GetComponent<Text>().text = LanguageController.Instance.GetText("message.loading");
            m_textLoading.gameObject.SetActive(true);
        }
    }


    // -------------------------------------------
    /* 
	 * LoadCurrentPage
	 */
    public void LoadCurrentPage()
    {
        if ((m_buttonNext != null) && (m_buttonPrevious != null) && (m_requests.Count > m_itemsEachPage))
        {
            ClearCurrentGameObject();

            int initialItem = m_currentPage * m_itemsEachPage;
            int finalItem = initialItem + m_itemsEachPage;

            int i = initialItem;
            for (i = initialItem; i < finalItem; i++)
            {
                if (i < m_requests.Count)
                {
                    GameObject newSlotInfoRequest = Utilities.AddChild(m_content.transform, ScreenController.Instance.SlotInfoRequestImage);
                    newSlotInfoRequest.GetComponent<SlotRequestView>().Initialize(m_requests[i]);
                    m_gameObjects.Add(newSlotInfoRequest);
                }
            }
            bool endReached = (i >= m_requests.Count);

            if (m_buttonNext != null) m_buttonNext.gameObject.SetActive(false);
            if (m_buttonPrevious != null) m_buttonPrevious.gameObject.SetActive(false);

            if (initialItem == 0)
            {
                if (m_requests.Count > m_itemsEachPage)
                {
                    if (m_buttonNext != null)
                    {
                        m_buttonNext.gameObject.SetActive(true);
                    }
                }
                if (m_buttonPrevious != null)
                {
                    m_buttonPrevious.gameObject.SetActive(false);
                }
            }
            else
            {
                if (endReached)
                {
                    if ((m_buttonPrevious != null) && (initialItem != 0))
                    {
                        m_buttonPrevious.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (m_buttonNext != null) m_buttonNext.gameObject.SetActive(true);
                    if (m_buttonPrevious != null) m_buttonPrevious.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (m_buttonNext != null) m_buttonNext.gameObject.SetActive(false);
            if (m_buttonPrevious != null) m_buttonPrevious.gameObject.SetActive(false);

            ClearCurrentGameObject();
            for (int i = 0; i < m_requests.Count; i++)
            {                
                GameObject newSlotInfoRequest = Utilities.AddChild(m_content.transform, ScreenController.Instance.SlotInfoRequestImage);
                newSlotInfoRequest.GetComponent<SlotRequestView>().Initialize(m_requests[i]);
                m_gameObjects.Add(newSlotInfoRequest);
            }
        }

        if (m_requests.Count == 0)
        {
            if (m_imageLoading != null) m_imageLoading.gameObject.SetActive(true);
            if (m_textLoading != null) m_textLoading.GetComponent<Text>().text = LanguageController.Instance.GetText("message.no.records");
        }
        else
        {
            if (m_imageLoading != null) m_imageLoading.gameObject.SetActive(false);
            if (m_textLoading != null) m_textLoading.gameObject.SetActive(false);
        }
    }

    // -------------------------------------------
    /* 
     * OnPreviousPressed
     */
    public void OnPreviousPressed()
    {
        m_currentPage--;
        if (m_currentPage < 0) m_currentPage = 0;
        LoadCurrentPage();
    }

    // -------------------------------------------
    /* 
     * OnNextPressed
     */
    public void OnNextPressed()
    {
        m_currentPage++;
        if (m_currentPage * m_itemsEachPage >= m_requests.Count) m_currentPage--;
        LoadCurrentPage();
    }

    // -------------------------------------------
    /* 
     * CheckSlotExisting
     */
    public bool CheckSlotExisting(GameObject _slot)
    {
        if (_slot == null) return false;

        for (int i = 0; i < m_gameObjects.Count; i++)
        {
            if (_slot == m_gameObjects[i])
            {
                return true;
            }
        }
        return false;
    }
        
}
