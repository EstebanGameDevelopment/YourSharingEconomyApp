﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/******************************************
 * 
 * IBasicScreenView
 * 
 * Basic interface that force the programmer to 
 * initialize and free resources to avoid memory leaks
 * 
 * @author Esteban Gallardo
 */
public interface IBasicScreenView
{
    // FUNCTIONS
    void Initialize(params object[] _list);
    void Destroy();
    void SetActivation(bool _activation);
}