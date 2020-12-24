using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Event management class. It must be active in scene.
public class GameEvents : MonoBehaviour
{
    #region Singleton

    private static GameEvents _instance;

    public static GameEvents Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }
    #endregion

    public static event Action OnPrizeCollection;
    public static event Action OnPlayerDeath;
    public static event Action OnPlayerDeathCompleted;
    public static event Action OnReplicaHit;
    public static event Action<int, int> OnChangeGridValue;
    public static event Action<bool> OnReplicaArea;
    public static event Action OnWallHit;
    public static event Action OnFloodFill;
    public static event Action<int> OnLevelComplete;
    public static event Action OnContinue;
    public static event Action OnNoThanks;
    public static event Action<Vector3> OnCubeLastSafeLocation;
    public static event Action<int> OnBonusRoundTimerChange;
    public static event Action OnBonusRoundComplete;
    public static event Action<int> OnFillCountChange;
    public static event Action<float,string> OnSpeedPowerUp;
    public static event Action<int,string> OnDiamondPowerUp;
    public static event Action OnProgressChange;

    /// <summary>
    /// Event invoker for progress UI change. It is utilized to inform UI to update progress bar.
    /// </summary>
    public void ProgressChangeInvoker()
    {
        OnProgressChange?.Invoke();
    }

    /// <summary>
    /// Event invoker for diamond power up. It is fired when diamond power up is collected.
    /// </summary>
    public void DiamondPowerUpInvoker(int diamondMultiplier, string powerUpType)
    {
        OnDiamondPowerUp?.Invoke(diamondMultiplier, powerUpType);
    }

    /// <summary>
    /// Event invoker for speed power up. It is fired when speed power up is collected.
    /// </summary>
    public void SpeedPowerUpInvoker(float speedMultiplier,string powerUpType)
    {
        OnSpeedPowerUp?.Invoke(speedMultiplier,powerUpType);
    }

    /// <summary>
    /// Event invoker for fill count. It is fired when flood fill is performed to update fill count in UI.
    /// </summary>
    public void FillCountChangeInvoker(int counter)
    {
        OnFillCountChange?.Invoke(counter);
    }

    /// <summary>
    /// Event invoker for bonus round completion. It is fired to load new level when bonus round completed.
    /// </summary>
    public void BonusRoundCompleteInvoker()
    {
        OnBonusRoundComplete?.Invoke();
    }

    /// <summary>
    /// Event invoker for bonus round timer. It is fired to update timer UI.
    /// </summary>
    public void BonusRoundTimerChangeInvoker(int counter)
    {
        OnBonusRoundTimerChange?.Invoke(counter);
    }

    /// <summary>
    /// Event invoker for player's cube last safe location. It is fired when player leaves safe area.
    /// </summary>
    public void CubeLastSafeLocationInvoker(Vector3 pos)
    {
        OnCubeLastSafeLocation?.Invoke(pos);
    }

    /// <summary>
    /// Event invoker for continue selection. It is fired when player decide to continue level.
    /// </summary>
    public void ContinueInvoker()
    {
        OnContinue?.Invoke();
    }

    /// <summary>
    /// Event invoker for no thanks selection. It is fired when player decide not to continue level.
    /// </summary>
    public void NoThanksInvoker()
    {
        OnNoThanks?.Invoke();
    }

    /// <summary>
    /// Event invoker for diamond prize collection. It is fired when player collect a diamond to update diamond count in UI.
    /// </summary>
    public void PrizeCollectionInvoker()
    {
        OnPrizeCollection?.Invoke();
    }

    /// <summary>
    /// Event invoker for player death. It is fired when player die.
    /// </summary>
    public void PlayerDeathInvoker()
    {
        OnPlayerDeath?.Invoke();
    }

    /// <summary>
    /// Event invoker for player death process completion. It is fired when player's death animations ended to activate continue screen.
    /// </summary>
    public void PlayerDeathCompletedInvoker()
    {
        OnPlayerDeathCompleted?.Invoke();
    }

    /// <summary>
    /// Event invoker for cube replica hit. It is fired when player hit a filled cube to inform other unfilled cubes to fill them.
    /// </summary>
    public void ReplicaHitInvoker()
    {
        OnReplicaHit?.Invoke();
    }

    /// <summary>
    /// Event invoker for grid value change. It is fired when a cube try to change the grid value for its position.
    /// </summary>
    public void ChangeGridValueInvoker(int col, int row)
    {
        OnChangeGridValue?.Invoke(col,row);
    }

    /// <summary>
    /// Event invoker for safe area notification. It is fired when player enters/leaves safe area.
    /// </summary>
    public void ReplicaAreaInvoker(bool shouldHarm)
    {
        OnReplicaArea?.Invoke(shouldHarm);
    }

    /// <summary>
    /// Event invoker for wall hit. It is fired when player hits a wall.
    /// </summary>
    public void WallHitInvoker()
    {
        OnWallHit?.Invoke();
    }

    /// <summary>
    /// Event invoker for flood fill. It is fired to start flood fill process.
    /// </summary>
    public void FloodFillInvoker()
    {
        OnFloodFill?.Invoke();
    }

    /// <summary>
    /// Event invoker for level completion. It is fired when a normal level is completed.
    /// </summary>
    public void LevelCompleteInvoker(int fillCount)
    {
        OnLevelComplete?.Invoke(fillCount);
    }
}
