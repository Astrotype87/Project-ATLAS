using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.System;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Simulation
{
    /// <summary>
    /// Loads and initializes simulations. <br/>
    /// Provides medal, score and playtime information when simulation is completed.
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private bool loadPrefab;
        [SerializeField, AssetOnly] private GameObject simulationPrefab;
        [SerializeField] private Difficulty difficulty;
        
        [Header("Container")]
        [SerializeField] private bool autoStart;
        [SerializeField, SceneOnly] private GameObject simulationContainer;
        
        [Header("Components")]
        [SerializeField] private SimulationSystem simulationSystem;
        
        [Header("Debug")]
        [SerializeField] private bool enableLog;
        
        // PROPERTIES
        public float Score => simulationSystem.Score;
        public float MaxScore => simulationSystem.MaxScore;
        public float PlayTime => simulationSystem.PlayTime;
        public PointsEntry[] PointsEntries => simulationSystem.PointsEntries;
        
        public bool IsCompleted => simulationSystem.IsCompleted;
        public bool IsBronzeAwarded => simulationSystem.IsBronzeAwarded;
        public bool IsSilverAwarded => simulationSystem.IsSilverAwarded;
        public bool IsGoldAwarded => simulationSystem.IsGoldAwarded;
        
        public bool IsSimulationClosable => simulationSystem.IsSimulationClosable;
        
        
        // MONOBEHAVIOUR METHODS
        private IEnumerator Start()
        {
            if (loadPrefab)
            {
                yield return LoadSimulationPrefab(simulationPrefab);
            }
            if (autoStart)
            {
                // Get reference to simulation
                simulationSystem = simulationContainer.GetComponent<SimulationSystem>();
                
                InitializeSimulation(null, difficulty);
            }
        }
        
        
        // PUBLIC METHODS
        public IEnumerator LoadSimulationPrefab(GameObject simulationPrefab)
        {
            // Spawn simulation prefab
            this.simulationPrefab = simulationPrefab;
            
            if (simulationContainer)
            {
                // // Save parent and sibling index information
                // Transform parent = simulationContainer.transform.parent;
                // int siblingIndex = simulationContainer.transform.GetSiblingIndex();
                
                // Destroy, instantiate, and wait for instantiation to finish
                Destroy(simulationContainer);
            }
            
            var instantiateSimulationPrefab = InstantiateAsync(simulationPrefab); //, parent);
            if (LoadingScreen.Instance)
                yield return instantiateSimulationPrefab.ReportLoadingProgress("Loading simulation prefab");
            else
                yield return instantiateSimulationPrefab;
            
            // Assign instantiated simulationPrefab, and move to simulation scene
            simulationContainer = instantiateSimulationPrefab.Result[0];
            // simulationContainer.transform.SetSiblingIndex(siblingIndex);
            SceneManager.MoveGameObjectToScene(simulationContainer, gameObject.scene);
            
            // Get reference to simulation
            simulationSystem = simulationContainer.GetComponent<SimulationSystem>();
        }
        
        public void InitializeSimulation(SimulationLevelData simulationLevelData, Difficulty difficulty)
        {
            this.difficulty = difficulty;
            simulationSystem.InitializeSimulation(simulationLevelData, difficulty);
        }
        
        public void PauseSimulation()
        {
            simulationSystem.PauseSimulation();
        }
        
        public void ResumeSimulation()
        {
            simulationSystem.ResumeSimulation();
        }
        
        public IEnumerator WaitForSimulationEnd()
        {
            yield return new WaitUntil(() => IsSimulationClosable);
        }
    }
}
