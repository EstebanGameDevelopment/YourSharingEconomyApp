using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * RequestModel
 * 
 * It keeps all the information about a request
 * 
 * @author Esteban Gallardo
 */
public class RequestModel
{
    public const int STATE_REQUEST_ALL = -1;
    public const int STATE_REQUEST_OPEN = 0;
    public const int STATE_REQUEST_CLOSED = 1;
    public const int STATE_REQUEST_FINISHED = 2;

    public const int IMAGE_TYPE_REFERENCE = 0;
    public const int IMAGE_TYPE_FINISHED = 99;

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------
    private long m_id;
    private int m_customer;
    private int m_provider;
    private string m_title;
    private string m_description;
    private int m_images;
    private long m_referenceimg;

    private string m_village = "";
    private string m_mapdata;

    private float m_latitude;
    private float m_longitude;

    private float m_price;
    private string m_currency;
    private int m_distance;

    private string m_flags;
    private int m_travel;
    private int m_material;
    private int m_notifications;

    private long m_creationdate;
    private long m_deadline;

    private int m_score;
    private long m_deliverydate;
    private int m_workdays;

    private bool m_checkExistence;
    private bool m_isDataFull = false;

    private string m_feedbackcustomer;
    private int m_scorecustomer;
    private string m_feedbackprovider;
    private int m_scoreprovider;

    private string m_reported;
    private int m_flaged;

    private List<ImageModel> m_temporalImageReferences = new List<ImageModel>();

    private long m_proposal;

    public long Id
    {
        get { return m_id; }
        set { m_id = value; }
    }
    public int Customer
    {
        get { return m_customer; }
        set { m_customer = value; }
    }
    public int Provider
    {
        get { return m_provider; }
        set { m_provider = value; }
    }
    public string Title
    {
        get { return m_title; }
        set { m_title = value; }
    }
    public string Description
    {
        get { return m_description; }
        set { m_description = value; }
    }
    public int Images
    {
        get { return m_images; }
        set { m_images = value; }
    }
    public long Referenceimg
    {
        get { return m_referenceimg; }
        set { m_referenceimg = value; }
    }
    public string Village
    {
        get { return m_village; }
        set { m_village = value; }
    }
    public string Mapdata
    {
        get { return m_mapdata; }
        set
        {
            m_mapdata = value;
            string[] coordinates = m_mapdata.Split(',');
            m_latitude = float.Parse(coordinates[0]);
            m_longitude = float.Parse(coordinates[1]);
        }
    }
    public float Latitude
    {
        get { return m_latitude; }
        set { m_latitude = value; }
    }
    public float Longitude
    {
        get { return m_longitude; }
        set { m_longitude = value; }
    }
    public float Price
    {
        get { return m_price; }
        set { m_price = value; }
    }
    public string Currency
    {
        get { return m_currency; }
        set { m_currency = value; }
    }
    public int Distance
    {
        get { return m_distance; }
        set { m_distance = value; }
    }
    public string Flags
    {
        get { return m_flags; }
    }
    public int Travel
    {
        get { return m_travel; }
        set { m_travel = value; }
    }
    public int Material
    {
        get { return m_material; }
        set { m_material = value; }
    }
    public int Notifications
    {
        get { return m_notifications; }
        set { m_notifications = value; }
    }
    public long Creationdate
    {
        get { return m_creationdate; }
        set { m_creationdate = value; }
    }
    public long Deadline
    {
        get { return m_deadline; }
        set { m_deadline = value; }
    }
    public int Score
    {
        get { return m_score; }
        set { m_score = value; }
    }
    public long Deliverydate
    {
        get { return m_deliverydate; }
        set { m_deliverydate = value; }
    }
    public int Workdays
    {
        get { return m_workdays; }
        set { m_workdays = value; }
    }
    public bool CheckExistence
    {
        get { return m_checkExistence; }
        set { m_checkExistence = value; }
    }
    public bool IsDataFull
    {
        get { return m_isDataFull; }
        set { m_isDataFull = value; }
    }
    public List<ImageModel> TemporalImageReferences
    {
        get { return m_temporalImageReferences; }
    }
    public long Proposal
    {
        get { return m_proposal; }
        set { m_proposal = value; }
    }

    public string FeedbackCustomerGivesToTheProvider
    {
        get { return m_feedbackcustomer; }
        set { m_feedbackcustomer = value; }
    }
    public int ScoreCustomerGivesToTheProvider
    {
        get { return m_scorecustomer; }
        set { m_scorecustomer = value; }
    }

    public string FeedbackProviderGivesToTheCustomer
    {
        get { return m_feedbackprovider; }
        set { m_feedbackprovider = value; }
    }
    public int ScoreProviderGivesToTheCustomer
    {
        get { return m_scoreprovider; }
        set { m_scoreprovider = value; }
    }
    public string Reported
    {
        get { return m_reported; }
        set { m_reported = value; }
    }
    public int Flaged
    {
        get { return m_flaged; }
        set { m_flaged = value; }
    }

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public RequestModel()
    {
        m_id = -1;
        m_customer = -1;
        m_provider = -1;
        m_title = "";
        m_description = "";
        m_images = 0;
        m_referenceimg = -1;

        m_village = "";
        m_mapdata = "";

        m_latitude = -1;
        m_longitude = -1;

        m_price = 0;
        m_currency = "";
        m_distance = 0;
        m_travel = 1;
        m_material = 1;
        m_notifications = 1;

        m_creationdate = -1;
        m_deadline = DateConverter.GetTimestamp();

        m_score = -1;
        m_deliverydate = -1;
        m_workdays = -1;

        m_checkExistence = true;
        m_isDataFull = false;

        m_proposal = -1;

        m_feedbackcustomer = "";
        m_scorecustomer = -1;
        m_feedbackprovider = "";
        m_scoreprovider = -1;

        m_reported = "";
        m_flaged = 0;
    }

    // -------------------------------------------
    /* 
     * Constructor
     */
    public RequestModel(long _id,
                        int _customer,
                        int _provider,
                        string _title,
                        string _description,
                        int _images,
                        long _referenceimg,
                        string _village,
                        string _mapdata,
                        float _latitude,
                        float _longitude,
                        float _price,
                        string _currency,
                        int _distance,
                        string _flags,
                        int _notifications,
                        long _creationdate,
                        long _deadline,
                        int _score,
                        long _deliverydate,
                        int _workdays,
                        bool _checkExistence,
                        bool _isDataFull,
                        List<ImageModel> _temporalImageReferences,
                        long _proposal,
                        string _feedbackcustomer,
                        int _scorecustomer,
                        string _feedbackprovider,
                        int _scoreprovider,
                        string _reported,
                        int _flaged)
    {
        m_id = _id;
        m_customer = _customer;
        m_provider = _provider;
        m_title = _title;
        m_description = _description;
        m_images = _images;
        m_referenceimg = _referenceimg;

        m_village = _village;
        m_mapdata = _mapdata;

        m_latitude = _latitude;
        m_longitude = _longitude;

        m_price = _price;
        m_currency = _currency;
        m_distance = _distance;
        m_flags = _flags;
        SetFlags(_flags);
        m_notifications = _notifications;

        m_creationdate = _creationdate;
        m_deadline = _deadline;

        m_score = _score;
        m_deliverydate = _deliverydate;
        m_workdays = _workdays;

        m_isDataFull = _isDataFull;
        m_checkExistence = _checkExistence;

        if (_temporalImageReferences != null)
        {
            m_temporalImageReferences.Clear();
            for (int i = 0; i < _temporalImageReferences.Count; i++)
            {
                m_temporalImageReferences.Add(_temporalImageReferences[i].Clone());
            }
        }

        m_proposal = _proposal;

        m_feedbackcustomer = _feedbackcustomer;
        m_scorecustomer = _scorecustomer;
        m_feedbackprovider = _feedbackprovider;
        m_scoreprovider = _scoreprovider;

        m_reported = _reported;
        m_flaged = _flaged;
    }

    // -------------------------------------------
    /* 
     * Clone
     */
    public RequestModel Clone()
    {
        return new RequestModel(m_id,
                                m_customer,
                                m_provider,
                                m_title,
                                m_description,
                                m_images,
                                m_referenceimg,
                                m_village,
                                m_mapdata,
                                m_latitude,
                                m_longitude,
                                m_price,
                                m_currency,
                                m_distance,
                                m_flags,
                                m_notifications,
                                m_creationdate,
                                m_deadline,
                                m_score,
                                m_deliverydate,
                                m_workdays,
                                m_isDataFull,
                                m_checkExistence,
                                m_temporalImageReferences,
                                m_proposal,
                                m_feedbackcustomer,
                                m_scorecustomer,
                                m_feedbackprovider,
                                m_scoreprovider,
                                m_reported,
                                m_flaged);
    }

    // -------------------------------------------
    /* 
     * Clone
     */
    public RequestModel CloneNoImages()
    {
        return new RequestModel(m_id,
                                m_customer,
                                m_provider,
                                m_title,
                                m_description,
                                m_images,
                                m_referenceimg,
                                m_village,
                                m_mapdata,
                                m_latitude,
                                m_longitude,
                                m_price,
                                m_currency,
                                m_distance,
                                m_flags,
                                m_notifications,
                                m_creationdate,
                                m_deadline,
                                m_score,
                                m_deliverydate,
                                m_workdays,
                                m_isDataFull,
                                m_checkExistence,
                                null,
                                m_proposal,
                                m_feedbackcustomer,
                                m_scorecustomer,
                                m_feedbackprovider,
                                m_scoreprovider,
                                m_reported,
                                m_flaged);
    }

    // -------------------------------------------
    /* 
     * Copy
     */
    public void Copy(RequestModel _request, bool _images)
    {
        m_id = _request.Id;
        m_customer = _request.Customer;
        m_provider = _request.Provider;
        m_title = _request.Title;
        m_description = _request.Description;
        m_images = _request.Images;
        m_referenceimg = _request.Referenceimg;
        m_village = _request.Village;
        m_mapdata = _request.Mapdata;
        m_latitude = _request.Latitude;
        m_longitude = _request.Longitude;
        m_price = _request.Price;
        m_currency = _request.Currency;
        m_distance = _request.Distance;
        m_flags = _request.Flags;
        m_travel = _request.Travel;
        m_material = _request.Material;
        m_notifications = _request.Notifications;
        m_notifications = _request.Notifications;
        m_deadline = _request.Deadline;
        m_score = _request.Score;
        m_deliverydate = _request.Deliverydate;
        m_workdays = _request.Workdays;
        m_checkExistence = _request.CheckExistence;
        m_isDataFull = _request.IsDataFull;

        if (_request.TemporalImageReferences != null)
        {
            m_temporalImageReferences.Clear();
            for (int i = 0; i < _request.TemporalImageReferences.Count; i++)
            {
                m_temporalImageReferences.Add(_request.TemporalImageReferences[i].Clone());
            }
        }

        m_proposal = _request.Proposal;
        m_feedbackcustomer = _request.FeedbackCustomerGivesToTheProvider;
        m_scorecustomer = _request.ScoreCustomerGivesToTheProvider;
        m_feedbackprovider = _request.FeedbackProviderGivesToTheCustomer;
        m_scoreprovider = _request.ScoreProviderGivesToTheCustomer;

        m_reported = _request.Reported;
        m_flaged = _request.Flaged;
    }

    // -------------------------------------------
    /* 
     * AllDataFilled
     */
    public bool AllDataFilled()
    {
        return (m_customer != -1) &&
                (m_title.Length > 0) &&
                (m_description.Length > 0) &&
                (m_village.Length > 0) &&
                (m_mapdata.Length > 0) &&
                (m_price != -1) &&
                (m_images > 0) &&
                CheckDatesRight();
    }

    // -------------------------------------------
    /* 
     * CheckDatesRight
     */
    public bool CheckDatesRight()
    {
        long myCurrentTime = DateConverter.GetTimestamp();
        return (myCurrentTime <= m_deadline);
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
                m_temporalImageReferences.Clear();
                for (int i = 0; i < _data.Count; i++)
                {
                    m_temporalImageReferences.Add(_data[i].Clone());
                }
            }
        }
    }

    // -------------------------------------------
    /* 
    * ReportedByUser
    */
    public bool ReportedByUser(int _userId)
    {
        string sUserID = _userId.ToString();
        if (m_reported.IndexOf(sUserID) != -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // -------------------------------------------
    /* 
    * GetFlags
    */
    public string GetFlags()
    {
        return m_travel.ToString() + CommController.TOKEN_SEPARATOR_COMA + m_material.ToString();
    }

    // -------------------------------------------
    /* 
    * SetFlags
    */
    public void SetFlags(string _data)
    {
        if (_data != null)
        {
            string[] dataFlags = _data.Split(CommController.TOKEN_SEPARATOR_COMA);
            if (dataFlags.Length == 2)
            {
                m_travel = int.Parse(dataFlags[0]);
                m_material = int.Parse(dataFlags[1]);
            }
        }
    }

    // -------------------------------------------
    /* 
    * IsFlagged
    */
    public bool IsFlagged()
    {
        return m_flaged == 1;
    }

    // -------------------------------------------
    /* 
    * IsBrokenDeal
    */
    public bool IsBrokenDeal()
    {
        return ((m_deliverydate != -1) &&
                (m_scorecustomer == -1) && (m_scoreprovider == -1) &&
                ((m_feedbackcustomer.Length > 0) || (m_feedbackprovider.Length > 0)));
    }

    // -------------------------------------------
    /* 
    * CheckEnoughReportsToWarningForToxic
    */
    public bool CheckEnoughReportsToWarningForToxic()
    {
        if (m_reported.Length == 0) return false;

        string[] reports = m_reported.Split(',');
        return (reports.Length >= ScreenController.Instance.TotalReportsToWarnRequest);
    }
    
}
