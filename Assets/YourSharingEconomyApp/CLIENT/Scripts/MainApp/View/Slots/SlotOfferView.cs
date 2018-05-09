using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/******************************************
 * 
 * SlotOfferView
 * 
 * Item that will represent an offer that a
 * provider can offer to a customer for the request.
 * 
 * Sections:
 *      -Name
 *      -Price
 *      -Deadline
 *      
 * @author Esteban Gallardo
 */
public class SlotOfferView : Button
{
    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------	
    public const string EVENT_SLOTOFFER_SELECTED_PROPOSAL = "EVENT_SLOTOFFER_SELECTED_PROPOSAL";

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------	
    private Transform m_container;
    private ProposalModel m_proposal;

    private GameObject m_iconInfo;
    private GameObject m_iconProposed;
    private GameObject m_iconAccepted;

    private GameObject m_iconCalendar;

    public ProposalModel Proposal
    {
        get { return m_proposal; }
    }

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public void Initialize(ProposalModel _item)
    {
        m_container = this.gameObject.transform;

        m_proposal = _item.Clone();

        m_iconInfo = m_container.Find("IconStates/Info").gameObject;
        m_iconProposed = m_container.Find("IconStates/Proposed").gameObject;
        m_iconAccepted = m_container.Find("IconStates/Accepted").gameObject;

        m_iconInfo.SetActive(false);
        m_iconProposed.SetActive(false);
        m_iconAccepted.SetActive(false);

        m_iconCalendar = m_container.Find("Calendar").gameObject;
        m_iconCalendar.SetActive(false);

        m_container.Find("bgReported").gameObject.SetActive(false);
        m_container.Find("bgUnselected").gameObject.SetActive(false);
        m_container.Find("bgSelected").gameObject.SetActive(false);
        if (m_proposal.Reported.Length > 0)
        {
            m_container.Find("bgReported").gameObject.SetActive(true);
        }
        else
        {
            m_container.Find("bgUnselected").gameObject.SetActive(!m_proposal.IsSelected());
            m_container.Find("bgSelected").gameObject.SetActive(m_proposal.IsSelected());
        }

        LoadData(false);
    }

    // -------------------------------------------
    /* 
	 * LoadData
	 */
    public void LoadData(bool _refreshData)
    {
        Text title = m_container.Find("Title").GetComponent<Text>();
        Text price = m_container.Find("Price").GetComponent<Text>();
        Text deadline = m_container.Find("Deadline").GetComponent<Text>();
        Text currency = m_container.Find("Currency").GetComponent<Text>();
        
        title.text = m_proposal.Title;
        price.text = m_proposal.Price.ToString();
        deadline.text = DateConverter.TimeStampToDateTimeString(m_proposal.Deadline);

        RequestModel requestModel = RequestsController.Instance.GetLocalRequest(m_proposal.Request);
        if (requestModel != null)
        {
            currency.text = requestModel.Currency;
        }

        if ((m_proposal.Type == ProposalModel.TYPE_INFO) || (m_proposal.Type == ProposalModel.TYPE_REPORT))
        {
            price.gameObject.SetActive(false);
            deadline.gameObject.SetActive(false);
            currency.gameObject.SetActive(false);
        }

        if (m_proposal.Accepted != -1)
        {
            m_iconAccepted.SetActive(true);
        }
        else
        {
            switch (m_proposal.Type)
            {
                case ProposalModel.TYPE_OFFER:
                    m_iconProposed.SetActive(true);
                    break;

                case ProposalModel.TYPE_INFO:
                    m_iconInfo.SetActive(true);
                    break;

                case ProposalModel.TYPE_REPORT:
                    break;
            }
        }
    }

    // -------------------------------------------
    /* 
	 * Destroy
	 */
    public void Destroy()
    {
        m_proposal = null;
        GameObject.DestroyObject(this.gameObject);
    }

    // -------------------------------------------
    /* 
     * Constructor
     */
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        BasicEventController.Instance.DispatchBasicEvent(EVENT_SLOTOFFER_SELECTED_PROPOSAL, this.gameObject);
    }
}
