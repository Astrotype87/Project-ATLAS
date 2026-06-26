using System;
using UnityEngine;

using ProjectATLAS.Architecture;

namespace ProjectATLAS.TTS
{
    /// <summary>
    /// Bridge to native Android TTS through Java code.<br/>
    /// You must manually call <c>Initialize()</c> before use
    /// and call <c>Shutdown()</c> after use.
    /// </summary>
    public class AndroidTTSService : PersistentSingletonGameObject<AndroidTTSService>
    {
        [SerializeField] private bool initializeOnEnable = true;
        [SerializeField] private float speechRate = 1;
        [SerializeField] private float pitch = 1;
        [SerializeField] private string localeCode = "en-US";
        [SerializeField] private bool isMuted = false;
        [Tooltip("Call silence() after initialization to prevent the first speak() lag.")]
        [SerializeField] private bool speakSilenceOnInitialize = true;
        
        public float SpeechRate => speechRate;
        public float Pitch => pitch;
        public string LocaleCode => localeCode;
        public bool IsMuted => isMuted;
        public bool IsSpeaking => _isSpeaking;
        
        private AndroidJavaClass _unityPlayer;
        private AndroidJavaObject _currentActivity;
        private AndroidJavaClass _ttsClass;
        private bool _isInitialized;
        private bool _isSpeaking;
        
        public event Action<string> OnInitialized;
        public event Action<string> OnInitFailed;
        public event Action<string> OnLocaleUnsupported;
        public event Action<string> OnSpeechStart;
        public event Action<string> OnSpeechDone;
        public event Action<string> OnSpeechStopped;
        public event Action<string> OnSpeechError;
        
        
        private void OnEnable()
        {
            if (initializeOnEnable) Initialize();
        }
        
        private void OnDisable()
        {
            Shutdown();
        }
        
        private void OnApplicationFocus(bool focus)
        {
            // Prevent TTS from playing when the game is not focused
            if (!focus) Stop();
        }
        
        
        public void Initialize()
        {
            if (_isInitialized) return;
            
            try
            {
                _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _ttsClass = new AndroidJavaClass("com.teamatlas.androidtts.AndroidTTS");
                
                _ttsClass?.CallStatic("initialize", _currentActivity, gameObject.name);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{GetType().Name}] C# side initialization failed: {e.GetType().Name}: {e.Message}");
                OnInitFailed?.Invoke($"c_sharp_init_failed: {e.GetType().Name}: {e.Message}");
            }
        }
        
        public void Shutdown()
        {
            if (_isInitialized)
            {
                _isInitialized = false;
                _ttsClass?.CallStatic("shutdown");
            }
        }
        
        
        public void SetSpeechRate(float speechRate)
        {
            this.speechRate = speechRate;
            _ttsClass?.CallStatic("setSpeechRate", speechRate);
        }
        
        public void SetPitch(float pitch)
        {
            this.pitch = pitch;
            _ttsClass?.CallStatic("setPitch", pitch);
        }
        
        /// <summary> Use IETF BCP 47 language tag string. (ex: en-US) </summary>
        public void SetLanguage(string localeCode)
        {
            this.localeCode = localeCode;
            _ttsClass?.CallStatic("setLanguage", localeCode);
        }
        
        public void SetMute(bool value)
        {
            // If first time muting, and is currently speaking, stop TTS
            if (value && value == isMuted && _isSpeaking)
            {
                Stop();
            }
            isMuted = value;
        }
        
        
        /// <summary>
        /// Start text-to-speech to speak the message.
        /// </summary>
        /// <param name="text">The message to speak.</param>
        /// <param name="interrupt">Interrupts currently speaking text-to-speech.</param>
        public void Speak(string text, bool interrupt = true)
        {
            if (isMuted) return;
            
            _ttsClass?.CallStatic("setSpeechRate", speechRate);
            _ttsClass?.CallStatic("setPitch", pitch);
            _ttsClass?.CallStatic("setLanguage", localeCode);
            _ttsClass?.CallStatic("speak", text, interrupt);
        }
        
        /// <summary> Add silence to text-to-speech queue. </summary>
        public void Silence(long durationInMs, bool interrupt)
        {
            if (isMuted) return;
            
            _ttsClass?.CallStatic("silence", durationInMs, interrupt);
        }
        
        /// <summary> Stop text-to-speech from speaking. </summary>
        public void Stop()
        {
            _ttsClass?.CallStatic("stop");
            _isSpeaking = false;
            OnSpeechStopped?.Invoke("stopped");
        }
        
        
        // AndroidTTS.jar plugin callbacks
        public void TTS_OnInitialized(string status)
        {
            _isInitialized = true;
            OnInitialized?.Invoke($"initialized: status: {status}");
            
            if (speakSilenceOnInitialize)
            {
                _ttsClass?.CallStatic("speak", "", true);
            }
        }
        
        public void TTS_OnInitFailed(string error)
        {
            Debug.LogError($"TTS: Init Failed: {error}");
            OnInitFailed?.Invoke($"init_failed: message: {error}");
        }
        
        public void TTS_OnLocaleUnsupported(string locale)
        {
            Debug.LogError($"TTS: Init Failed: {locale}. Falling back to English.");
            OnLocaleUnsupported?.Invoke($"locale_unsupported: locale: {locale}. Falling back to en-US");
        }
        
        public void TTS_OnSpeechStart(string utteranceID)
        {
            _isSpeaking = true;
            OnSpeechStart?.Invoke($"speech_started: id: {utteranceID}");
        }
        
        public void TTS_OnSpeechDone(string utteranceID)
        {
            _isSpeaking = false;
            OnSpeechDone?.Invoke($"speech_done: id: {utteranceID}");
        }
        
        public void TTS_OnSpeechError(string utteranceID)
        {
            OnSpeechError?.Invoke($"speech_error: {utteranceID}");
        }
    }
}
