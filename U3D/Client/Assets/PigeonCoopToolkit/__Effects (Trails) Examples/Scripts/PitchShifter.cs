﻿using PigeonCoopToolkit.Utillities;
using UnityEngine;
using System.Collections;

public class PitchShifter : MonoBehaviour
{

    public Range pitchRange;
    public AudioSource src;
	// Use this for initialization
	void Start ()
	{
	    src.pitch = Random.Range(pitchRange.Min, pitchRange.Max);
	}
}
