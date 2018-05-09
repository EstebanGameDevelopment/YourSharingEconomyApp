using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/******************************************
 * 
 * SoundsController
 * 
 * Class that handles the sounds played in the app
 * 
 * @author Esteban Gallardo
 */
public class SoundsController : StateManager  
{
    public const string SOUND_SELECTION_FX = "SOUND_FX_SELECTION";
    public const string SOUND_FX_SUB_SELECTION = "SOUND_FX_SUB_SELECTION";

    // ----------------------------------------------
    // SINGLETON
    // ----------------------------------------------
    private static SoundsController _instance;

    public static SoundsController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<SoundsController>();
            }
            return _instance;
        }
    }

    // ----------------------------------------------
    // CONSTANTS
    // ----------------------------------------------
    public const string SOUND_COOCKIE = "SOUND_COOCKIE";

    // ----------------------------------------------
    // PUBLIC MEMBERS
    // ----------------------------------------------
    public AudioClip[] Sounds;
    
    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------
    private AudioSource m_audio1;
    private AudioSource m_audio2;
    private bool m_enabled;

    public bool Enabled
    {
        get { return m_enabled; }
        set { m_enabled = value;
            PlayerPrefs.SetInt(SOUND_COOCKIE, (m_enabled?1:0));
        }
    }

	// ----------------------------------------------
	// CONSTRUCTOR
	// ----------------------------------------------	
	// -------------------------------------------
	/* 
	 * Constructor
	 */
    public SoundsController() 
	{
	}

    // ----------------------------------------------
    // INIT/DESTROY
    // ----------------------------------------------	

    // -------------------------------------------
    /* 
     * Destroy audio's gameObject		
     */
    public void Init()
    {
        AudioSource[] aSources = GetComponents<AudioSource>();
        m_audio1 = aSources[0];
        m_audio2 = aSources[1];

        m_enabled = (PlayerPrefs.GetInt(SOUND_COOCKIE, 1) == 1);
		m_enabled = false;
    }
	
	// -------------------------------------------
    /* 
     * Destroy audio's gameObject		
     */
    public void Destroy() 
	{
        Destroy(m_audio1);
        Destroy(m_audio2);
	}

    // -------------------------------------------
    /* 
     * StopAllSounds
     */
    public void StopAllSounds()
    {
        m_audio1.Stop();
        m_audio2.Stop();
    }

    // -------------------------------------------
    /* 
     * Play loop
     */
    public void PlaySoundLoop(AudioClip _audio)
    {
        if (_audio == null) return;
        if (!m_enabled) return;

        m_audio1.clip = _audio;
        m_audio1.loop = true;
        if (!m_audio1.isPlaying)
        {
            m_audio1.Play();
        }
    }

    // -------------------------------------------
    /* 
     * StopAllSounds
     */
    public void StopMainLoop()
    {
        m_audio1.clip = null;
        m_audio1.Stop();
    }

    // -------------------------------------------
    /* 
     * PlaySingleSound
     */
    public void PlaySingleSound(string _audioName)
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            if (Sounds[i].name == _audioName)
            {
                PlaySingleSound(Sounds[i]);
            }
        }
    }

    // -------------------------------------------
    /* 
     * PlaySingleSound
     */
    public void PlaySingleSound(AudioClip _audio)
    {
        if (!m_enabled) return;
        if (_audio != null)
        {
            m_audio2.PlayOneShot(_audio);
        }
    }

    // -------------------------------------------
    /* 
     * PlaySingleSound
     */
    public void PlayFxSelection()
    {
        PlaySingleSound(SOUND_SELECTION_FX);
    }

    // -------------------------------------------
    /* 
     * PlayFxSubSelection
     */
    public void PlayFxSubSelection()
    {
        PlaySingleSound(SOUND_FX_SUB_SELECTION);
    }
    
}