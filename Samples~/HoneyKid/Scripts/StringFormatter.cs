using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StringFormatter : MonoBehaviour
{
    [SerializeField] private UnityEventString stringResult;
    [SerializeField] private string formatString;

    public void Format(int value) => FormatTemplate(value);
    public void Format(float value) => FormatTemplate(value);
    public void Format(double value) => FormatTemplate(value);
    public void Format(string value) => FormatTemplate(value);
    public void Format(bool value) => FormatTemplate(value);

    public void FormatTemplate<T>(T value) =>
        stringResult?.Invoke(string.IsNullOrEmpty(formatString)
            ? value.ToString()
            : string.Format(formatString, value)
        );


    [Serializable]
    private class UnityEventString : UnityEvent<string>
    {
    }
}