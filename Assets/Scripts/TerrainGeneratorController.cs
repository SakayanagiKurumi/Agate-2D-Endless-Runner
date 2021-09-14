using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
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

    [Header("Item")]
    public GameObject itemPrefab;

    private List<GameObject> spawnedTerrain;
    public List<GameObject> spawnedItem;
    private float lastGeneratedPositionX;
    private float lastRemovedPositionX;
    private const float debugLineHeight = 10.0f;

    private void Start()
    {
        spawnedTerrain = new List<GameObject>();
        spawnedItem = new List<GameObject>();
        lastGeneratedPositionX = GetHorizontalPositionStart();
        lastRemovedPositionX = lastGeneratedPositionX - terrainTemplateWidth;

        foreach(TerrainTemplateController terrain in earlyTerrainTemplates)
        {
            GenerateTerrain(lastGeneratedPositionX, terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while(lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }

    private void GenerateTerrain(float posX, TerrainTemplateController forceterrain = null)
    {
        GameObject newTerrain = Instantiate(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);
        newTerrain.transform.position = new Vector2(posX, 0f);
        spawnedTerrain.Add(newTerrain);
    }

    private void GenerateItem(float posX)
    {
        GameObject newItem = Instantiate(itemPrefab, transform);
        newItem.transform.position = new Vector2(posX, Random.Range(1, 3));
        spawnedItem.Add(newItem);

    }

    private void Update()
    {
        while(lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            GenerateItem(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while(lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart())
        {
            lastRemovedPositionX += terrainTemplateWidth;
            RemoveTerrain(lastRemovedPositionX);
            RemoveItem(lastRemovedPositionX);
        }
    }

    private void RemoveTerrain(float posX)
    {
        GameObject terrainToRemove = null;

        // Find terrain at posX
        foreach(GameObject item in spawnedTerrain)
        {
            if(item.transform.position.x == posX)
            {
                terrainToRemove = item;
                break;
            }
        }

        // After found
        if(terrainToRemove != null)
        {
            spawnedTerrain.Remove(terrainToRemove);
            Destroy(terrainToRemove);
        }
    }

    private void RemoveItem(float posX)
    {
        GameObject itemToRemove = null;

        // Find item at posX
        foreach(GameObject item in spawnedItem)
        {
            if(item.transform.position.x == posX)
            {
                itemToRemove = item;
                break;
            }
        }

        // After found
        if(itemToRemove != null)
        {
            spawnedItem.Remove(itemToRemove);
            Destroy(itemToRemove);
        }
    }

    private float GetHorizontalPositionStart()
    {
        return (gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset);
    }

    private float GetHorizontalPositionEnd()
    {
        return (gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f))).x + areaEndOffset;
    }

    // Debug
    private void OnDrawGizmos()
    {
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineHeight / 2, areaStartPosition + Vector3.down * debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineHeight / 2, areaEndPosition + Vector3.down * debugLineHeight / 2, Color.red);
    }
}
