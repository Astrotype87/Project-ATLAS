using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.TTS
{
    public class TTSController : MonoBehaviour
    {
        [SerializeField] private AndroidTTSService textToSpeech;
        [SerializeField] private Button speakButton;
        [SerializeField] private TMP_InputField textInput;
        [SerializeField] private TMP_InputField speechRateInput;
        [SerializeField] private TMP_InputField pitchInput;
        [SerializeField] private TMP_Text debugText;
        
        private void Start()
        {
            speakButton.onClick.AddListener(Speak);
            
            speechRateInput.onValueChanged.AddListener(str =>
                textToSpeech.SetSpeechRate(float.Parse(speechRateInput.text))
            );
            
            pitchInput.onValueChanged.AddListener(str =>
                textToSpeech.SetPitch(float.Parse(pitchInput.text))
            );
            
            speechRateInput.text = textToSpeech.SpeechRate.ToString();
            pitchInput.text = textToSpeech.Pitch.ToString();
        }
        
        private void OnEnable()
        {
            textToSpeech.OnInitialized += LogMessage;
            textToSpeech.OnInitFailed += LogMessage;
            textToSpeech.OnLocaleUnsupported += LogMessage;
            textToSpeech.OnSpeechStart += LogMessage;
            textToSpeech.OnSpeechDone += LogMessage;
            textToSpeech.OnSpeechError += LogMessage;
            
            textToSpeech.Initialize();
        }
        
        private void OnDisable()
        {
            textToSpeech.Shutdown();
            
            textToSpeech.OnInitialized -= LogMessage;
            textToSpeech.OnInitFailed -= LogMessage;
            textToSpeech.OnLocaleUnsupported -= LogMessage;
            textToSpeech.OnSpeechStart -= LogMessage;
            textToSpeech.OnSpeechDone -= LogMessage;
            textToSpeech.OnSpeechError -= LogMessage;
        }

        private void Speak()
        {
            string text = textInput.text;
            float speechRate = float.Parse(speechRateInput.text);
            float pitch = float.Parse(pitchInput.text);
            
            textToSpeech.SetSpeechRate(speechRate);
            textToSpeech.SetPitch(pitch);
            textToSpeech.Speak(text);
        }
        
        private void LogMessage(string message)
        {
            debugText.text = message;
        }
        
    }
}