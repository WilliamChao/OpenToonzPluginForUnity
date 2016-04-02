using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace UTJ
{
    [CustomEditor(typeof(OpenToonzFx))]
    public class OpenToonzFxEditor : Editor
    {

        void OnEnable()
        {
        }


        static string MakePathRelative(string path)
        {
            Uri pathToAssets = new Uri(Application.streamingAssetsPath + "/");
            return pathToAssets.MakeRelativeUri(new Uri(path)).ToString();
        }

        void SelectPlugin()
        {
            var t = target as OpenToonzFx;
            var path = EditorUtility.OpenFilePanel("Select OpenToonz plugin", Application.streamingAssetsPath, "plugin");

            OpenToonzFx.PluginPath ppath;
            if (path.IndexOf(Application.streamingAssetsPath) == -1)
            {
                ppath = new OpenToonzFx.PluginPath
                {
                    m_root = OpenToonzFx.PluginPath.Root.Absolute,
                    m_leaf = path,
                };
            }
            else
            {
                ppath = new OpenToonzFx.PluginPath
                {
                    m_root = OpenToonzFx.PluginPath.Root.StreamingAssetsPath,
                    m_leaf = MakePathRelative(path),
                };
            }

            Undo.RecordObject(target, "Changed Plugin");

            t.pluginPath = ppath;
            t.pluginIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            //return;

            var t = target as OpenToonzFx;
            const float width = 100.0f;

            if(GUILayout.Button("Select Plugin", GUILayout.MinWidth(width)))
            {
                SelectPlugin();
            }

            var plugin_list = t.pluginList;
            if(plugin_list.Length == 0)
            {
                return;
            }

            {
                GUILayout.Label(t.pluginPath.GetFileName());

                EditorGUI.BeginChangeCheck();
                int v = EditorGUILayout.Popup(t.pluginIndex, plugin_list);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed Plugin Index");
                    t.pluginIndex = v;
                }
            }

            EditorGUILayout.Space();

            {
                GUILayout.Label(t.pluginName + " by " + t.pluginVendor);

                var note = t.pluginNote;
                if (note.Length > 0)
                {
                    GUILayout.Label(t.pluginNote);
                }
            }

            EditorGUILayout.Space();

            {
                EditorGUI.BeginChangeCheck();
                t.m_preview = EditorGUILayout.Toggle(
                                "Preview", t.m_preview, GUILayout.MinWidth(width));
                if (EditorGUI.EndChangeCheck())
                {
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }
            }

            EditorGUILayout.Space();

            GUILayout.Label("Params:");

            var cparams = t.pluginParams;
            if (cparams != null)
            {
                foreach (var p in cparams)
                {
                    var type = p.GetType();
                    if (type == typeof(ToonzDoubleParam))
                    {
                        var vp = p as ToonzDoubleParam;
                        EditorGUI.BeginChangeCheck();
                        var v = EditorGUILayout.DoubleField(
                            new GUIContent(p.name, p.note), vp.value.value, GUILayout.MinWidth(width));
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(target, "Changed Param Value");
                            vp.value.value = v;
                        }
                    }
                    else if (type == typeof(ToonzIntParam))
                    {
                        var vp = p as ToonzIntParam;
                        EditorGUI.BeginChangeCheck();
                        var v = EditorGUILayout.IntField(
                            new GUIContent(p.name, p.note), vp.value.value, GUILayout.MinWidth(width));
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(target, "Changed Param Value");
                            vp.value.value = v;
                        }
                    }
                    else if (type == typeof(ToonzBoolParam))
                    {
                        var vp = p as ToonzBoolParam;
                        EditorGUI.BeginChangeCheck();
                        var v = EditorGUILayout.IntField(
                            new GUIContent(p.name, p.note), vp.value.value, GUILayout.MinWidth(width));
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(target, "Changed Param Value");
                            vp.value.value = v;
                        }
                    }
                    else if (type == typeof(ToonzEnumParam))
                    {
                        var vp = p as ToonzEnumParam;
                        var v = EditorGUILayout.IntField(
                            new GUIContent(p.name, p.note), vp.value.value, GUILayout.MinWidth(width));
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(target, "Changed Param Value");
                            vp.value.value = v;
                        }
                    }
                    else if (type == typeof(ToonzRangeParam))
                    {
                        var vp = p as ToonzRangeParam;
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.LabelField(p.name, p.note);
                        EditorGUILayout.BeginHorizontal();
                        var vmin = EditorGUILayout.DoubleField("", vp.value.min, GUILayout.MinWidth(width));
                        var vmax = EditorGUILayout.DoubleField("", vp.value.max, GUILayout.MinWidth(width));
                        EditorGUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(target, "Changed Param Value");
                            vp.value.min = vmin;
                            vp.value.max = vmax;
                        }
                    }
                }
            }


            EditorGUILayout.Space();

            GUILayout.Label("Inputs:");
            var ports = t.pluginPorts;
            if(ports != null)
            {
                foreach (var port in ports)
                {
                    EditorGUI.BeginChangeCheck();
                    var newinput = EditorGUILayout.ObjectField(
                        port.name, port.input, typeof(Texture), true, GUILayout.MinWidth(width)) as Texture;
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Changed Input Texture");
                        port.input = newinput;
                    }
                }
                GUILayout.Label("  (None is treated as frame buffer)");
            }

        }
    }
}
