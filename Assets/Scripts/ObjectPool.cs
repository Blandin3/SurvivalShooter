using System.Collections.Generic;
using UnityEngine;

// Generic reusable Object Pool. Attach to an empty GameObject per prefab type
// (e.g. one for player bullets, one for enemy bullets). No Instantiate/Destroy
// happens during gameplay once the pool is warmed up in Awake.
public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize = 20;

    readonly Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            pool.Enqueue(CreateNew());
        }
    }

    GameObject CreateNew()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateNew();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}
