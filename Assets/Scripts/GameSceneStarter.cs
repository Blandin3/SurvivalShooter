using UnityEngine;

// Temporary stand-in for AR plane placement: automatically starts the match as soon as
// the Game scene loads, instead of waiting for a tap-to-place trigger. Once ARPlacementManager
// is wired up to call GameManager.StartGame() itself after a real plane tap, remove this
// component (or leave it as a Editor/no-AR-device fallback - it's harmless either way,
// since StartGame() is only called if the state isn't already Playing).
public class GameSceneStarter : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameSceneStarter.Start(): GameManager.Instance is null, cannot start the game.", this);
            return;
        }

        if (GameManager.Instance.State != GameState.Playing)
        {
            Debug.Log("GameSceneStarter.Start(): calling StartGame() (current state was " + GameManager.Instance.State + ").", this);
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.Log("GameSceneStarter.Start(): state is already Playing, not calling StartGame() again.", this);
        }
    }
}
