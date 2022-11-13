using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public float treeChance = 0.01f;
    public float decorationChance = 0.05f;
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
    public float offset;
    private List<GameObject> _objects;

    public List<GameObject> SpawnObjects(StreetCreator sc, Vector3 startPos, Vector2 startDir, Vector2 targetDir)
    {
        _objects = new List<GameObject>();

        // start Spawnloop coroutine
        StartCoroutine(SpawnLoop(sc, startPos, startDir, targetDir));

        return _objects;
    }

    private IEnumerator SpawnLoop(StreetCreator sc, Vector3 startPos, Vector2 startDir, Vector2 targetDir)
    {
        Vector3 startVector = new Vector3(startDir.x, startDir.y, 1);
        Vector3 targetVector = new Vector3(targetDir.x, targetDir.y, 1);
        Vector3 currPos = startPos;
        Vector3 currDir = startVector;
        for (int y = 0; y < StreetCreator.VERTCOUNT * 3; y++)
        {
            // call method for all objects each
            SpawnFoliage(Oak, currPos, treeChance, 0.8f);
            SpawnFoliage(Spruce, currPos, treeChance, 0.8f);
            SpawnFoliage(DeadTree, currPos, treeChance, 0.8f);
            SpawnFoliage(Bush, currPos, decorationChance, 0.25f);
            SpawnFoliage(Rock, currPos, decorationChance, 0.25f);
            SpawnFoliage(Grass, currPos, decorationChance, 0.25f);
            SpawnFence(currPos);
            SpawnBeeHive(currPos);
            currDir = StreetChunk.Interpolate(startVector, targetVector, y / (float)(StreetCreator.VERTCOUNT * 3));
            currPos += currDir * StreetCreator.EDGELENGTH * 3f;
            if(y % 40 == 0)
                yield return null;
        }
    }

    private void SpawnFoliage(GameObject tree, Vector3 position, float chance, float edgeBias)
    {
        // roll if tree should be spawned
        if (Random.Range(0f, 1f) < chance)
        {
            Vector2 range;
            if (Random.Range(0f, 1f) < 0.5f)
                range = CalcGrasLeft(position);
            else
                range = CalcGrasRight(position);

            if (range[0] < range[1])
            {
                // range = new Vector2(range[1], range[0]);
                // edgeBias = 1 - edgeBias;
            }
            float t = Mathf.Abs(Random.value + Random.value * edgeBias);
            float rolledPos = Mathf.Lerp(range[0], range[1] * 0.95f, t);
            Vector3 spawnPos = new Vector3(rolledPos, position.y, position.z);
            SpawnObject(tree, spawnPos);
            // Debug.Log("Spawned " + tree.name + " at " + spawnPos + " with range " + range + " and rolled " + rolledPos);
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
        float outer = position.x;
        // substract half of the street width
        outer -= StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        // substract grass width
        outer -= StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH * StreetCreator.GROUNDSTRETCHFACTOR;
        float inner = position.x;
        // substract half of the street width
        inner -= StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        return new Vector2(inner, outer);
    }

    private Vector2 CalcGrasRight(Vector3 position)
    {
        float inner = position.x;
        // substract half of the street width
        inner += StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        float outer = position.x;
        // substract half of the street width
        outer += StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        // substract grass width
        outer += StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH * StreetCreator.GROUNDSTRETCHFACTOR;
        return new Vector2(inner, outer);
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
        if (Physics.Raycast(offsetPosition, Vector3.down, out hit, heightOffset * 2))
        {

            Quaternion rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles.x, Random.Range(0, 360), obj.transform.rotation.eulerAngles.z);
            _objects.Add(Instantiate(obj, hit.point, rotation));
        }
        // random rotation around y axis

    }

}
