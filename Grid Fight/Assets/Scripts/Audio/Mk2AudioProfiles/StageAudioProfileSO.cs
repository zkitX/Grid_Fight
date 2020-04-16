﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage Audio Profile", menuName = "ScriptableObjects/Audio/Profiles/Stage")]
public class StageAudioProfileSO : ScriptableObject
{
    [Header("General")]
    public AudioClipInfoClass music;
    public AudioClipInfoClass ambience;
}