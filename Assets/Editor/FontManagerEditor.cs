using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FontManager))]
public class FontManagerEditor : Editor
{
    SerializedProperty uiBaseFont;
    SerializedProperty contentBaseFont;
    SerializedProperty localeFontMappings;

    private void OnEnable()
    {
        uiBaseFont = serializedObject.FindProperty("uiBaseFont");
        contentBaseFont = serializedObject.FindProperty("contentBaseFont");
        localeFontMappings = serializedObject.FindProperty("localeFontMappings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Font Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 기본 폰트 설정
        EditorGUILayout.PropertyField(uiBaseFont, new GUIContent("UI Base Font"));
        EditorGUILayout.PropertyField(contentBaseFont, new GUIContent("Content Base Font"));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Fallback Fonts Per Locale", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Locale별 Fallback 폰트 설정
        for (int i = 0; i < localeFontMappings.arraySize; i++)
        {
            SerializedProperty mapping = localeFontMappings.GetArrayElementAtIndex(i);
            SerializedProperty localeCode = mapping.FindPropertyRelative("localeCode");
            SerializedProperty fallbackFonts = mapping.FindPropertyRelative("fallbackFonts");

            EditorGUILayout.BeginVertical("box");

            // Locale Code와 삭제 버튼
            EditorGUILayout.BeginHorizontal();
            localeCode.stringValue = EditorGUILayout.TextField("Locale Code", localeCode.stringValue);

            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                localeFontMappings.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            // 폰트 리스트
            EditorGUILayout.LabelField("Fallback Fonts", EditorStyles.miniBoldLabel);

            for (int j = 0; j < fallbackFonts.arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(fallbackFonts.GetArrayElementAtIndex(j), GUIContent.none);

                if (GUILayout.Button("-", GUILayout.Width(24)))
                {
                    fallbackFonts.DeleteArrayElementAtIndex(j);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Font"))
            {
                fallbackFonts.InsertArrayElementAtIndex(fallbackFonts.arraySize);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        if (GUILayout.Button("+ Add Locale"))
        {
            localeFontMappings.InsertArrayElementAtIndex(localeFontMappings.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
