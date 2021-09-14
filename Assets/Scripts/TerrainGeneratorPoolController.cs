using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorPoolController : MonoBehaviour
{
    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    public float terrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;

    [Header("Force Early Template")]
    public List<TerrainTemplateController> earlyTerrainTemplates;

    private List<GameObject> spawnedTerrain;
    private float lastGeneratedPositionX;
    private float lastRemovedPositionX;
    private const float debugLineHeight = 10.0f;

    // pool list
    private Dictionary<string, List<GameObject>> pool;

    private void GenerateTerrain(float posX, TerrainTemplateController forceterrain = null)
    {
        GameObject newTerrain = Instantiate(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);
        newTerrain.transform.position = new Vector2(posX, 0f);
        spawnedTerrain.Add(newTerrain);
    }

    private float GetHorizontalPositionStart()
    {
        return (gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset);
    }

    private float GetHorizontalPositionEnd()
    {
        return (gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f))).x + areaEndOffset;
    }

    private void Start()
    {
        // init pool
        pool = new Dictionary<string, List<GameObject>>();

        spawnedTerrain = new List<GameObject>();

        lastGeneratedPositionX = GetHorizontalPositionStart();
        lastRemovedPositionX = lastGeneratedPositionX - terrainTemplateWidth;


        foreach (TerrainTemplateController terrain in earlyTerrainTemplates)
        {
            GenerateTerrain(lastGeneratedPositionX, terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }


        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }

    private void Update()
    {
        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while (lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart())
        {
            lastRemovedPositionX += terrainTemplateWidth;
            //RemoveTerrain(lastRemovedPositionX);
        }
    }

    // pool function
    private GameObject GenerateFromPool(GameObject item, Transform parent)
    {
        if (pool.ContainsKey(item.name))
        {
            // if item available in pool
            if (pool[item.name].Count > 0)
            {
                GameObject newItemFromPool = pool[item.name][0];
                pool[item.name].Remove(newItemFromPool);
                newItemFromPool.SetActive(true);
                return newItemFromPool;
            }
        }
        else
        {
            // if item list not defined, create new one
            pool.Add(item.name, new List<GameObject>());
        }


        // create new one if no item available in pool
        GameObject newItem = Instantiate(item, parent);
        newItem.name = item.name;
        return newItem;
    }

    private void ReturnToPool(GameObject item)
    {
        if (!pool.ContainsKey(item.name))
        {
            Debug.LogError("INVALID POOL ITEM!!");
        }


        pool[item.name].Add(item);
        item.SetActive(false);
    }
}
