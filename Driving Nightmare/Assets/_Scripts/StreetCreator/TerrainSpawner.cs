using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public int averageTreeCount = 180;
    public int averageDecorationCount = 180;
    public float FenceChance = 0.005f; // 0.5%
    public float BeeHiveChance = 0.005f; // 0.5%
    public GameObject Oak;
    public GameObject Spruce;
    public GameObject DeadTree;
    public GameObject Bush;
    public GameObject Rock;
    public GameObject Grass;
    public GameObject Fence;
    public GameObject Bee;
    public GameObject Beehive;
    private List<GameObject> _objects;

    public List<GameObject> SpawnObjects(StreetCreator sc, Vector3 startPos, Vector2 startDir, Vector2 targetDir)
    {
        _objects = new List<GameObject>();
        Vector3 startVector = new Vector3(startDir.x, startDir.y, 1);
        Vector3 targetVector = new Vector3(targetDir.x, targetDir.y, 1);
        Vector3 currPos = startPos;
        Vector3 currDir = startVector;

        for (int y = 0; y < StreetCreator.VERTCOUNT * 3 *3; y++)
        {
            // call method for all objects each
            SpawnFoliage(Oak, currPos);
            SpawnFoliage(Spruce, currPos);
            SpawnFoliage(DeadTree, currPos);
            SpawnFoliage(Bush, currPos);
            SpawnFoliage(Rock, currPos);
            SpawnFoliage(Grass, currPos);
            SpawnFence(currPos);
            SpawnBeeHive(currPos);
            currDir = StreetChunk.Interpolate(startVector, targetVector, y / (float)(StreetCreator.VERTCOUNT * 3));
            currPos += currDir * StreetCreator.EDGELENGTH *3f;
        }

            return _objects;
    }

    private void SpawnFoliage(GameObject tree, Vector3 position)
    {
        // roll if tree should be spawned
        float treeSpawnPercentage = StreetCreator.VERTCOUNT * 3 / (float)averageTreeCount / 100f;
        if (Random.Range(0f, 1f) < treeSpawnPercentage)
        {
            Vector2 grassLeft = CalcGrasLeft(position);
            Vector2 grassRight = CalcGrasRight(position);
            // roll if grass left or right
            if (Random.Range(0f, 1f) < 0.5f)
            {
                // roll random position on grass left
                float rolledPos = Random.Range(grassLeft[0], grassLeft[1]);
                Vector3 spawnPos = new Vector3(rolledPos, position.y, position.z);
                SpawnObject(tree, spawnPos);
            }
            else
            {
                // roll random position on grass left
                float rolledPos = Random.Range(grassRight[0], grassRight[1]);
                Vector3 spawnPos = new Vector3(rolledPos, position.y, position.z);
                SpawnObject(tree, spawnPos);
            }
        }
    }



    private void SpawnFence(Vector3 position)
    {

    }

    private void SpawnBeeHive(Vector3 position)
    {

    }

    private Vector2 CalcGrasLeft(Vector3 position)
    {
        float left = position.x;
        // substract half of the street width
        left -= StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        // substract grass width
        left -= StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH * StreetCreator.GROUNDSTRETCHFACTOR;
        float right = position.x;
        // substract half of the street width
        right -= StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        return new Vector2(left, right);
    }

    private Vector2 CalcGrasRight(Vector3 position)
    {
        float left = position.x;
        // substract half of the street width
        left += StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        float right = position.x;
        // substract half of the street width
        right += StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        // substract grass width
        right += StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH * StreetCreator.GROUNDSTRETCHFACTOR;
        return new Vector2(left, right);
    }

    private Vector2 CalcStreet(Vector3 position)
    {
        float left = position.x;
        // substract half of the street width
        left -= StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        float right = position.x;
        // substract half of the street width
        right += StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        return new Vector2(left, right);
    }

    private void SpawnObject(GameObject obj, Vector3 position)
    {
        float heightOffset = 100f;
        Vector3 offsetPosition = position + Vector3.up * heightOffset;
        // raycast to ground
        RaycastHit hit;
        if (Physics.Raycast(offsetPosition, Vector3.down, out hit, heightOffset*2))
        {

        Quaternion rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles.x, Random.Range(0, 360), obj.transform.rotation.eulerAngles.z);
        _objects.Add(Instantiate(obj, hit.point, rotation));
        }
        // random rotation around y axis

    }

}
