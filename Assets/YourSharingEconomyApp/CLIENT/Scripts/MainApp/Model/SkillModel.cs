using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * SkillModel
 * 
 * It keeps the information about a skill of the provider
 * 
 * @author Esteban Gallardo
 */
public class SkillModel
{
    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------
    private string m_name;
    private int m_value;

    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
    public int Value
    {
        get { return m_value; }
        set { m_value = value; }
    }

    // -------------------------------------------
    /* 
     * Constructor
     */
    public SkillModel(string _name, int _value)
    {
        m_name = _name;
        m_value = _value;
    }

    // -------------------------------------------
    /* 
     * Clone
     */
    public SkillModel Clone()
    {
        return new SkillModel(m_name, m_value);
    }

    // -------------------------------------------
    /* 
     * Copy
     */
    public void Copy(SkillModel _skill)
    {
        m_name = _skill.Name;
        m_value = _skill.Value;
    }

    // -------------------------------------------
    /* 
     * ToString
     */
    public override string ToString()
    {
        return "(" + m_name + "," + m_value + ")";
    }
}