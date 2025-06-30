using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEditor.AddressableAssets;

[CustomEditor(typeof(FontManager))]
public class FontManagerEditor : Editor
{
    private SerializedProperty uiBaseFontProp;
    private SerializedProperty contentBaseFontProp;
    private SerializedProperty uiFontLabelMappingsProp;
    private SerializedProperty contentFontLabelsProp;

    private string[] availableLocales;
    private string[] fontLabels;

    private void OnEnable()
    {
        uiBaseFontProp = serializedObject.FindProperty("uiBaseFont");
        contentBaseFontProp = serializedObject.FindProperty("contentBaseFont");
        uiFontLabelMappingsProp = serializedObject.FindProperty("uiFontLabelMappings");
        contentFontLabelsProp = serializedObject.FindProperty("contentFontLabels");

        RefreshLocales();
        RefreshFontLabels();
        SyncLocalesWithMappings();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Font Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawBaseFonts();
        EditorGUILayout.Space(10);

        DrawUIFonts();
        EditorGUILayout.Space(10);

        DrawContentFonts();
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Refresh Label & Locale List"))
        {
            RefreshLocales();
            RefreshFontLabels();
            SyncLocalesWithMappings();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBaseFonts()
    {
        EditorGUILayout.PropertyField(uiBaseFontProp, new GUIContent("Base UI Font (Empty)"));
        EditorGUILayout.PropertyField(contentBaseFontProp, new GUIContent("Base Content Font (Empty)"));
    }

    private void DrawUIFonts()
    {
        EditorGUILayout.LabelField("UI Font Label Mappings", EditorStyles.boldLabel);

        for (int i = 0; i < uiFontLabelMappingsProp.arraySize; i++)
        {
            var element = uiFontLabelMappingsProp.GetArrayElementAtIndex(i);

            var localeProp = element.FindPropertyRelative("locale");
            var labelListProp = element.FindPropertyRelative("addressableLabels");

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Locale: {localeProp.stringValue}");

            EditorGUILayout.LabelField("Font Labels (Fallback 순서)");

            for (int j = 0; j < labelListProp.arraySize; j++)
            {
                var labelProp = labelListProp.GetArrayElementAtIndex(j);

                EditorGUILayout.BeginHorizontal();
                int labelIndex = Mathf.Max(0, fontLabels.ToList().FindIndex(x => x == labelProp.stringValue));
                labelIndex = EditorGUILayout.Popup(labelIndex, fontLabels);
                labelProp.stringValue = fontLabels[labelIndex];

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    labelListProp.DeleteArrayElementAtIndex(j);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Label"))
            {
                labelListProp.InsertArrayElementAtIndex(labelListProp.arraySize);
                var newLabel = labelListProp.GetArrayElementAtIndex(labelListProp.arraySize - 1);
                newLabel.stringValue = fontLabels.FirstOrDefault() ?? "font-none";
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawContentFonts()
    {
        EditorGUILayout.LabelField("Content Font Labels (Global)", EditorStyles.boldLabel);

        for (int i = 0; i < contentFontLabelsProp.arraySize; i++)
        {
            var labelProp = contentFontLabelsProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            int labelIndex = Mathf.Max(0, fontLabels.ToList().FindIndex(x => x == labelProp.stringValue));
            labelIndex = EditorGUILayout.Popup(labelIndex, fontLabels);
            labelProp.stringValue = fontLabels[labelIndex];

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                contentFontLabelsProp.DeleteArrayElementAtIndex(i);
                EditorGUILayout.EndHorizontal();
                continue;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Add Label"))
        {
            contentFontLabelsProp.InsertArrayElementAtIndex(contentFontLabelsProp.arraySize);
            var newLabel = contentFontLabelsProp.GetArrayElementAtIndex(contentFontLabelsProp.arraySize - 1);
            newLabel.stringValue = fontLabels.FirstOrDefault() ?? "font-none";
        }
    }

    private void SyncLocalesWithMappings()
    {
        var currentLocales = availableLocales.ToList();

        List<string> existingLocales = new();
        for (int i = 0; i < uiFontLabelMappingsProp.arraySize; i++)
        {
            var element = uiFontLabelMappingsProp.GetArrayElementAtIndex(i);
            existingLocales.Add(element.FindPropertyRelative("locale").stringValue);
        }

        foreach (var locale in currentLocales)
        {
            if (!existingLocales.Contains(locale))
            {
                uiFontLabelMappingsProp.InsertArrayElementAtIndex(uiFontLabelMappingsProp.arraySize);
                var newElement = uiFontLabelMappingsProp.GetArrayElementAtIndex(uiFontLabelMappingsProp.arraySize - 1);
                newElement.FindPropertyRelative("locale").stringValue = locale;
                var labels = newElement.FindPropertyRelative("addressableLabels");
                labels.ClearArray();
                labels.InsertArrayElementAtIndex(0);
                labels.GetArrayElementAtIndex(0).stringValue = $"font-{locale}";
            }
        }
    }

    private void RefreshLocales()
    {
        availableLocales = LocalizationSettings.AvailableLocales?.Locales
            .Select(l => l.Identifier.ToString()).ToArray();

        if (availableLocales == null || availableLocales.Length == 0)
            availableLocales = new[] { "en" };
    }

    private void RefreshFontLabels()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null)
        {
            var labelSet = new HashSet<string>();

            foreach (var group in settings.groups)
            {
                foreach (var entry in group.entries)
                {
                    foreach (var label in entry.labels)
                    {
                        if (label.StartsWith("font-"))
                            labelSet.Add(label);
                    }
                }
            }

            fontLabels = labelSet.ToArray();
        }

        if (fontLabels == null || fontLabels.Length == 0)
        {
            fontLabels = new[] { "font-none" };
        }
    }
}
