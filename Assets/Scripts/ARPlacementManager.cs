using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacementManager : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public EnemySpawner spawner;
    public GameObject gameRoot;

    ARRaycastManager raycastManager;
    static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    bool placed;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void OnEnable()
    {
        placed = false;
        if (gameRoot != null) gameRoot.SetActive(false);
        if (planeManager != null) planeManager.enabled = true;
    }

    void Update()
    {
        if (placed) return;
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Placement) return;
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            PlaceGame(hits[0].pose);
        }
    }

    void PlaceGame(Pose pose)
    {
        placed = true;

        gameRoot.transform.SetPositionAndRotation(pose.position, pose.rotation);
        gameRoot.SetActive(true);

        // Stop detecting/rendering planes once the game world is anchored.
        if (planeManager != null)
        {
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            planeManager.enabled = false;
        }

        spawner.BeginSpawning(gameRoot.transform);
        GameManager.Instance.StartGame();
    }
}
