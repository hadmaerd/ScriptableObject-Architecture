﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFillSetter : MonoBehaviour
{
    [SerializeField]
    private FloatReference _variable = default(FloatReference);
    [SerializeField]
    private FloatReference _maxValue = default(FloatReference);
    [SerializeField]
    private Image _imageTarget = default(Image);

    private void Update()
    {
        //_imageTarget.fillAmount = Mathf.Clamp01(_variable.Value / _maxValue.Value);
    }
    private void Start () {
        _variable.AddListener (delegate {
            _imageTarget.fillAmount = Mathf.Clamp01 (_variable.Value / _maxValue.Value);
        });
    }
}