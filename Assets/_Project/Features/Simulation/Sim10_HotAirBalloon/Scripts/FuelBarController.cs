using UnityEngine;
using UnityEngine.UI;

public class FuelBarController : MonoBehaviour
{
    [Header("Fuel Settings")]
    [SerializeField] private Image fuelFill;
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float smoothSpeed = 4f;

    private float currentFuel;
    private float targetFuel;
    private bool initialized = false;

    void Start()
    {
        currentFuel = maxFuel;
        targetFuel = maxFuel;
        UpdateFuelUI(true);
        initialized = true;
    }

    void Update()
    {
        if (!initialized || fuelFill == null) return;

        // 🔹 Smoothly animate the fuel bar
        if (Mathf.Abs(fuelFill.fillAmount - targetFuel / maxFuel) > 0.001f)
        {
            float newFill = Mathf.Lerp(fuelFill.fillAmount, targetFuel / maxFuel, Time.deltaTime * smoothSpeed);
            fuelFill.fillAmount = newFill;
        }
    }

    /// <summary>
    /// Decreases fuel by the given amount.
    /// </summary>
    public void UseFuel(float amount)
    {
        targetFuel = Mathf.Clamp(targetFuel - amount, 0, maxFuel);
        currentFuel = targetFuel;
    }

    /// <summary>
    /// Increases fuel by the given amount.
    /// </summary>
    public void AddFuel(float amount)
    {
        targetFuel = Mathf.Clamp(targetFuel + amount, 0, maxFuel);
        currentFuel = targetFuel;
    }

    /// <summary>
    /// Instantly updates the UI bar.
    /// </summary>
    private void UpdateFuelUI(bool instant = false)
    {
        if (fuelFill == null) return;

        if (instant)
            fuelFill.fillAmount = currentFuel / maxFuel;
        else
            fuelFill.fillAmount = Mathf.Lerp(fuelFill.fillAmount, currentFuel / maxFuel, Time.deltaTime * smoothSpeed);
    }

    /// <summary>
    /// Sets fuel directly (useful for syncing from another script).
    /// </summary>
    public void SetFuel(float value)
    {
        targetFuel = Mathf.Clamp(value, 0, maxFuel);
        currentFuel = targetFuel;
        UpdateFuelUI(true);
    }

    /// <summary>
    /// Returns true if no fuel remains.
    /// </summary>
    public bool IsOutOfFuel()
    {
        return targetFuel <= 0.01f;
    }

    /// <summary>
    /// Returns fuel percentage (0–100).
    /// </summary>
    public float GetFuelPercent()
    {
        return (targetFuel / maxFuel) * 100f;
    }
}
