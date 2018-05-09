using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * PageInformation
 * 
 * It keeps information to be displayed by the information page
 * 
 * @author Esteban Gallardo
 */
[System.Serializable]
public class PageInformation
{
    public string MyTitle;
    public string MyText;
    public Sprite MySprite;
    public string EventData;
    public GameObject Reference;

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public PageInformation(string _title, string _text, Sprite _sprite, string _eventData, GameObject _reference)
    {
        MyTitle = _title;
        MyText = _text;
        MySprite = _sprite;
        EventData = _eventData;
        Reference = _reference;
    }

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    public PageInformation(string _title, string _text, Sprite _sprite, string _eventData)
    {
        MyTitle = _title;
        MyText = _text;
        MySprite = _sprite;
        EventData = _eventData;
        Reference = null;
    }

    // -------------------------------------------
    /* 
	 * Clone
	 */
    public PageInformation Clone()
    {
        return new PageInformation(MyTitle, MyText, MySprite, EventData, Reference);
    }
}
