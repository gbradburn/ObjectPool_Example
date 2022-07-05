using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    #region Private members

    private static ObjectPoolManager _instance;
    private readonly Dictionary<string, Queue<GameObject>> _pool = new Dictionary<string, Queue<GameObject>>();

    #endregion Private Members

    #region Unity Callbacks

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion Unity Callbacks

    #region Spawning

    public static GameObject SpawnGameObject(GameObject prefab, bool setActive = true)
    {
        if (_instance == null) return null;

        GameObject go = _instance.DequeGameObject(prefab);
        if (go != null)
        {
            go.SetActive(setActive);
        }
        else
        {
            go = _instance.InstantiateGameObject(prefab, setActive);
        }

        return go;
    }

    public static GameObject SpawnGameObject(GameObject prefab, Vector3 position, Quaternion rotation, bool setActive = true)
    {
        if (_instance == null) return null;
        GameObject go = _instance.DequeGameObject(prefab);
        if (go != null)
        {
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(setActive);
        }
        else
        {
            go = _instance.InstantiateGameObject(prefab, position, rotation, setActive);
        }

        return go;
    }

    #endregion Spawning

    #region Despawning / Destroying

    public static void DespawnGameObject(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        var pool = _instance.GetPool(go);
        pool.Enqueue(go);
    }

    public static void PermanentlyDestroyGameObjectsOfType(GameObject prefab)
    {
        if (_instance == null) return;
        var queue = _instance.GetPool(prefab);
        GameObject go;
        while (queue?.Count > 0)
        {
            go = queue.Dequeue();
            if (go != null)
            {
                if (go.activeSelf)
                {
                    go.SetActive(false);
                }

                Destroy(go);
            }
        }
    }

    public static void EmptyPool()
    {
        if (_instance == null) return;
        foreach (Queue<GameObject> pool in _instance._pool.Values)
        {
            while (pool.Count > 0)
            {
                GameObject go = pool.Dequeue();
                if (go != null)
                {
                    Destroy(go);
                }
            }
        }
        _instance._pool.Clear();
    }

    #endregion

    #region Private methods

    private GameObject DequeGameObject(GameObject prefab)
    {
        var queue = GetPool(prefab);
        if (queue.Count < 1) return null;
        GameObject go = queue.Dequeue();
        if (go == null)
        {
            Debug.LogWarning("Dequeued null gameObject (" + prefab.name + ") from pool.");
        }

        return go;
    }

    private GameObject InstantiateGameObject(GameObject prefab, bool setActive)
    {
        var queue = GetPool(prefab);
        var go = Instantiate(prefab);
        DontDestroyOnLoad(go);
        go.SetActive(setActive);
        return go;
    }

    private GameObject InstantiateGameObject(GameObject prefab, Vector3 position, Quaternion rotation, bool setActive)
    {
        GameObject go = InstantiateGameObject(prefab, setActive);
        go.transform.position = position;
        go.transform.rotation = rotation;
        return go;
    }

    private GameObject InstantiateGameObject(GameObject prefab, Transform parentTransform, bool setActive)
    {
        GameObject go = InstantiateGameObject(prefab, parentTransform.position, parentTransform.rotation, setActive);
        go.transform.SetParent(parentTransform);
        return go;
    }

    private Queue<GameObject> GetPool(GameObject prefab)
    {
        Queue<GameObject> pool;

        if (_pool.ContainsKey(prefab.BaseName()))
        {
            pool = _pool[prefab.BaseName()];
        }
        else
        {
            pool = new Queue<GameObject>();
            _pool.Add(prefab.BaseName(), pool);
        }
        return pool;
    }

    #endregion
}
