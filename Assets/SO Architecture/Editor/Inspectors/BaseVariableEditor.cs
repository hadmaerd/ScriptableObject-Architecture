﻿using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(BaseVariable<>), true)]
public class BaseVariableEditor : Editor
{
    private BaseVariable Target { get { return (BaseVariable)target; } }
    protected bool IsClamped { get { return Target is IClampedVariable; } }

    private SerializedProperty _valueProperty;
    private SerializedProperty _runtimeValueProperty;
    private SerializedProperty _developerDescription;
    private SerializedProperty _readOnly;
    private SerializedProperty _preserveInitValue;
    private SerializedProperty _raiseWarning;
    private SerializedProperty _minValueProperty;
    private SerializedProperty _maxValueProperty;
    private AnimBool _raiseWarningAnimation;

    private bool _valueChangedInInspector;

    private const string READONLY_TOOLTIP = "Should this value be changable during runtime? Will still be editable in the inspector regardless";
    
    protected virtual void OnEnable()
    {
        _valueProperty = serializedObject.FindProperty("_value");
        _runtimeValueProperty = serializedObject.FindProperty("_runtimeValue");
        _developerDescription = serializedObject.FindProperty("DeveloperDescription");
        _readOnly = serializedObject.FindProperty("_readOnly");
        _preserveInitValue = serializedObject.FindProperty ("_preserveInitValue");
        _raiseWarning = serializedObject.FindProperty("_raiseWarning");

        if (IsClamped)
        {
            _minValueProperty = serializedObject.FindProperty("_minClampedValue");
            _maxValueProperty = serializedObject.FindProperty("_maxClampedValue");
        }        

        _raiseWarningAnimation = new AnimBool(_readOnly.boolValue);
        _raiseWarningAnimation.valueChanged.AddListener(Repaint);
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        DrawValue(Target.PreserveInitValue && Application.isPlaying ? _runtimeValueProperty : _valueProperty);
        DrawClampedFields();
        DrawReadonlyField ();
        DrawPreserveInitValueField ();
        DrawDeveloperDescription();
    }
    protected void DrawValue(SerializedProperty valueProperty)
    {
        if (_valueChangedInInspector) {
            _valueChangedInInspector = false;
            Target.Raise ();
        }

        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            string content = "Cannot display value. No PropertyDrawer for (" + Target.Type + ") [" + Target.ToString() + "]";
            GenericPropertyDrawer.DrawPropertyDrawer(Target.Type, valueProperty, new GUIContent(content, content));

            if (scope.changed)
            {
                // Value changed, raise events
                //Target.Raise();
                _valueChangedInInspector = true;
            }
        }
    }
    protected void DrawClampedFields()
    {
        if (!IsClamped)
            return;

        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.PropertyField(_minValueProperty);
            EditorGUILayout.PropertyField(_maxValueProperty);
        }
    }
    protected void DrawReadonlyField()
    {
        if (IsClamped)
            return;

        EditorGUILayout.PropertyField(_readOnly, new GUIContent("Read Only", READONLY_TOOLTIP));

        _raiseWarningAnimation.target = _readOnly.boolValue;
        using (var fadeGroup = new EditorGUILayout.FadeGroupScope(_raiseWarningAnimation.faded))
        {
            if (fadeGroup.visible)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_raiseWarning);
                EditorGUI.indentLevel--;
            }
        }
    }
    protected void DrawPreserveInitValueField ()
    {
        EditorGUILayout.PropertyField (_preserveInitValue);
    }
    protected void DrawDeveloperDescription()
    {
        EditorGUILayout.PropertyField(_developerDescription);
    }
}