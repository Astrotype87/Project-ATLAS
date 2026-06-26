using System.Collections;
using UnityEngine;

using ProjectATLAS.System;

namespace ProjectATLAS
{
    public static class LoadingExtensions
    {
        public static IEnumerator ReportLoadingProgress(
            this AsyncOperation asyncOperation, string text, float start = 0.0f, float end = 1.0f)
        {
            LoadingScreen.Instance.SetLoadingText(text);
            
            while (!asyncOperation.isDone)
            {
                float progress = Mathf.Lerp(start, end, asyncOperation.progress / 0.9f);
                LoadingScreen.Instance.SetProgress(progress);
                yield return null;
            }
            
            LoadingScreen.Instance.SetProgress(end);
        }
    }
}
