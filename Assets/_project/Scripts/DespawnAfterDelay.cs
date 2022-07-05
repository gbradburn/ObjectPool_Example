using UnityEngine;

public class DespawnAfterDelay : MonoBehaviour
{
    [SerializeField] [Range(5f, 30f)] float _despawnDelay = 10f;

    private void OnEnable()
    {
        Invoke(nameof(Despawn), _despawnDelay);
    }

    private void Despawn()
    {
        ObjectPoolManager.DespawnGameObject(gameObject);
    }
    
}
