using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.PrimeTweenSequencer.Editor
{
    public static class IconDictionary
    {
        private static readonly Dictionary<Type, string> iconNames = new()
        {
            { typeof(TweenSequencer), "animation icon" },
            { typeof(TweenStep), "animationclip icon" },
            { typeof(SequenceStep), "animator icon" },
            { typeof(PlaySequencerStep), "animation icon" },
            { typeof(DelayStep), "unityeditor.animationwindow" },
            { typeof(CallbackStep), "eventsystem icon" },
            { typeof(GameObject), "gameobject icon" },
            { typeof(Transform), "transform icon" },
            { typeof(RectTransform), "recttransform icon" },
            // Add more type-icon mappings as needed
        };
        
        public static string GetIconName(Type type)
        {
            if (type == null) return "gameobject icon";
            
            if (iconNames.TryGetValue(type, out var iconName))
                return iconName;
            
            return "gameobject icon";
        }
        
        public static Texture2D GetIcon(Type type)
        {
            string iconName = GetIconName(type);
            return EditorGUIUtility.IconContent(iconName).image as Texture2D;
        }
    }
}
