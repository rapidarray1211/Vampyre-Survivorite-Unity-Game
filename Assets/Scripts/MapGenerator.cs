using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth = 5000;
    public int mapHeight = 5000;

    public GameObject wallPrefab;

    [Range(0f, 1f)]
    public float wallChance = 0.08f;

    public Transform mapRoot;

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 position = new Vector3(x, y, 0);

                if (Random.value < wallChance)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, mapRoot);
                }
            }
        }
    }
}
