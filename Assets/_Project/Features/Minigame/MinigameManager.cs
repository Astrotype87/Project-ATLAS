using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.System;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Minigame
{
    /// <summary>
    /// Loads and initializes minigames. <br/>
    /// Provides medal, score and playtime information when simulation is completed.
    /// </summary>
    public class MinigameManager : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private bool loadPrefab;
        [SerializeField, AssetOnly] private GameObject minigamePrefab;
        [SerializeField] private Difficulty difficulty;
        
        [Header("Container")]
        [SerializeField] private bool autoStart;
        [SerializeField, SceneOnly] private GameObject minigameContainer;
        
        [Header("Components")]
        [SerializeField] private MinigameSystem minigameSystem;
        
        [Header("Debug")]
        [SerializeField] private bool enableLog;
        
        // PROPERTIES
        public float Score => minigameSystem ? minigameSystem.Score : 10;
        public float MaxScore => minigameSystem ? minigameSystem.MaxScore : 10;
        public float PlayTime => minigameSystem ? minigameSystem.PlayTime : 5;
        public PointsEntry[] PointsEntries => minigameSystem ? minigameSystem.PointsEntries : new PointsEntry[]
        {
            new("Score", 1000),
            new("Time Bonus", 100),
        };
        
        public bool IsCompleted => minigameSystem ? minigameSystem.IsCompleted : true;
        public bool IsBronzeAwarded => minigameSystem ? minigameSystem.IsBronzeAwarded : true;
        public bool IsSilverAwarded => minigameSystem ? minigameSystem.IsSilverAwarded : true;
        public bool IsGoldAwarded => minigameSystem ? minigameSystem.IsGoldAwarded : true;
        
        public bool IsMinigameClosable => minigameSystem ? minigameSystem.IsMinigameClosable : true || skipMinigame;
        
        private bool skipMinigame;
        
        // MONOBEHAVIOUR METHODS
        private IEnumerator Start()
        {
            if (loadPrefab)
            {
                yield return LoadMinigamePrefab(minigamePrefab);
            }
            if (autoStart)
            {
                // Get reference to minigame
                minigameSystem = minigameContainer.GetComponent<MinigameSystem>();
                
                InitializeMinigame(null, difficulty);
            }
        }
        
        
        // PUBLIC METHODS
        public IEnumerator LoadMinigamePrefab(GameObject minigamePrefab)
        {
            if (minigamePrefab == null)
            {
                skipMinigame = true;
                yield break;
            }
            
            // Spawn minigame prefab
            this.minigamePrefab = minigamePrefab;
            
            if (minigameContainer)
            {
                // // Save parent and sibling index information
                // Transform parent = minigameContainer.transform.parent;
                // int siblingIndex = minigameContainer.transform.GetSiblingIndex();
                
                // Destroy, instantiate, and wait for instantiation to finish
                Destroy(minigameContainer);
            }
            
            var instantiateMinigamePrefab = InstantiateAsync(minigamePrefab); //, parent);
            if (LoadingScreen.Instance)
                yield return instantiateMinigamePrefab.ReportLoadingProgress("Loading minigame prefab");
            else
                yield return instantiateMinigamePrefab;
            
            // Assign instantiated minigamePrefab, and move to minigame scene
            minigameContainer = instantiateMinigamePrefab.Result[0];
            // minigameContainer.transform.SetSiblingIndex(siblingIndex);
            SceneManager.MoveGameObjectToScene(minigameContainer, gameObject.scene);
            
            // Get reference to minigame
            minigameSystem = minigameContainer.GetComponent<MinigameSystem>();
        }
        
        public void InitializeMinigame(ChallengeLevelData challengeLevelData, Difficulty difficulty)
        {
            this.difficulty = difficulty;
            if (minigameSystem) minigameSystem.InitializeMinigame(challengeLevelData, difficulty);
        }
        
        public void PauseMinigame()
        {
            minigameSystem.PauseMinigame();
        }
        
        public void ResumeMinigame()
        {
            minigameSystem.ResumeMinigame();
        }
        
        public IEnumerator WaitForMinigameEnd()
        {
            yield return new WaitUntil(() => IsMinigameClosable);
        }
    }
}
