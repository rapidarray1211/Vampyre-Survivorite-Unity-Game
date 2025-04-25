using UnityEngine;
public class DestroyAfterSeconds : MonoBehaviour
{
    public float lifetime = 0.2f;
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
