using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * SearchModel
 * 
 * Data used to make a search request
 * 
 * @author Esteban Gallardo
 */
public class SearchModel
{
    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------
    private string m_village;
    private string m_mapData;
    private float m_longitude;
    private float m_latitude;
    private int m_distance;

    public string Village
    {
        get { return m_village; }
        set { m_village = value; }
    }
    public string MapData
    {
        get { return m_mapData; }
        set { m_mapData = value; }
    }
    public float Longitude
    {
        get { return m_longitude; }
        set { m_longitude = value; }
    }
    public float Latitude
    {
        get { return m_latitude; }
        set { m_latitude = value; }
    }
    public int Distance
    {
        get { return m_distance; }
        set { m_distance = value; }
    }

    // -------------------------------------------
    /* 
     * Constructor
     */
    public SearchModel(string _village, string _mapData, int _distance)
    {
        m_village = _village;
        m_mapData = _mapData;
        string[] coord = m_mapData.Split(',');
        m_latitude = float.Parse(coord[0]);
        m_longitude = float.Parse(coord[1]);        
        m_distance = _distance;
    }

    // -------------------------------------------
    /* 
     * Clone
     */
    public SearchModel Clone()
    {
        return new SearchModel(m_village, m_mapData, m_distance);
    }

    // -------------------------------------------
    /* 
     * Copy
     */
    public void Copy(SearchModel _request)
    {
        m_village = _request.Village;
        m_mapData = _request.MapData;
        m_longitude = _request.Longitude;
        m_latitude = _request.Latitude;
        m_distance = _request.Distance;
    }

    // -------------------------------------------
    /* 
     * ToString
     */
    public override string ToString()
    {
        return "(" + m_village + "," + m_latitude + "," + m_longitude + "," + m_distance + ")";
    }
}