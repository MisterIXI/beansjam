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
    public GameObject Car;
    public GameObject Pylons;
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
        int lastPylonSpawn = 0;
        for (int y = 0; y < StreetCreator.VERTCOUNT * 3; y++)
        {
            // call method for all objects each
            SpawnFoliage(Oak, currPos, treeChance, 0.8f);
            SpawnFoliage(Spruce, currPos, treeChance, 0.8f);
            SpawnFoliage(DeadTree, currPos, treeChance, 0.8f);
            SpawnFoliage(Bush, currPos, decorationChance, 0.25f);
            SpawnFoliage(Rock, currPos, decorationChance, 0.25f);
            SpawnFoliage(Grass, currPos, decorationChance, 0.25f);
            SpawnCar(currPos, currDir);
            SpawnFence(currPos);
            SpawnBeeHive(currPos);
            lastPylonSpawn++;
            if (lastPylonSpawn > StreetCreator.VERTCOUNT / 3)
            {
                lastPylonSpawn = 0;
                SpawnPylon(currPos);
            }
            currDir = StreetChunk.Interpolate(startVector, targetVector, y / (float)(StreetCreator.VERTCOUNT * 3));
            currPos += currDir * StreetCreator.EDGELENGTH * 3f;
            if (y % 40 == 0)
                yield return null;
        }
    }

    private void SpawnFoliage(GameObject obj, Vector3 position, float chance, float edgeBias)
    {
        // roll if tree should be spawned
        if (Random.Range(0f, 1f) < chance)
        {
            Vector2 range;
            if (Random.Range(0f, 1f) < 0.5f)
                range = CalcGrasLeft(position);
            else
                range = CalcGrasRight(position);

            BiasedSpawn(obj, position, chance, edgeBias, range);
            // Debug.Log("Spawned " + tree.name + " at " + spawnPos + " with range " + range + " and rolled " + rolledPos);
        }
    }

    private void BiasedSpawn(GameObject obj, Vector3 position, float chance, float edgeBias, Vector2 range)
    {
        float t = Mathf.Abs(Random.value + Random.value * edgeBias);
        float rolledPos = Mathf.Lerp(range[0], range[1] * 0.95f, t);
        Vector3 spawnPos = new Vector3(rolledPos, position.y, position.z);
        // roll random color
        SpawnObject(obj, spawnPos);
    }

    private void SpawnCar(Vector3 position, Vector3 direction)
    {
        if (Random.Range(0f, 1f) < 0.005f)
        {
            Vector2 range = CalcStreetLeft(position);
            // get middle of street
            float rolledPos = Mathf.Lerp(range[0], range[1], 0.5f);
            Vector3 spawnPos = new Vector3(rolledPos, position.y, position.z);
            // two ray casts front and back for rotation
            GameObject car = SpawnObject(Car, spawnPos);
            if (car != null)
            {
                car.transform.rotation = Quaternion.LookRotation(-direction);
                car.transform.Rotate(new Vector3(-90, 0, -180), Space.Self);
                car.transform.position = car.transform.position + Vector3.up * 5f;
                // side rotation with two raycasts
                // roll random color
                Color randColor = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
                // copy material of car
                Material mat = new Material(car.GetComponent<Renderer>().material);
                mat.color = randColor;
                car.GetComponent<MeshRenderer>().material = mat;
            }
        }
        if (Random.Range(0f, 1f) < 0.005f)
        {
            Vector2 range = CalcStreet(position);
            // get middle of street
            float rolledPos = Mathf.Lerp(range[0], range[1], 0.75f);
            Vector3 spawnPos = new Vector3(rolledPos, position.y, position.z);
            // two ray casts front and back for rotation
            GameObject car = SpawnObject(Car, spawnPos);
            if (car != null)
            {
                car.transform.rotation = Quaternion.LookRotation(-direction);
                car.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
                car.transform.position = car.transform.position + Vector3.up * 5f;
                // side rotation with two raycasts
                // roll random color
                Color randColor = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
                // copy material of car
                Material mat = new Material(car.GetComponent<Renderer>().material);
                mat.color = randColor;
                car.GetComponent<MeshRenderer>().material = mat;
            }
        }
    }

    private void SpawnPylon(Vector3 position)
    {
        Vector2 range = CalcStreet(position);
        Vector3 raycastPosLeft = new Vector3(range[0] - 10f, position.y, position.z);
        Vector3 raycastPosRight = new Vector3(range[1] + 10f, position.y, position.z);
        RaycastHit hitLeft;
        RaycastHit hitRight;
        if (Physics.Raycast(raycastPosLeft, Vector3.down, out hitLeft, 100f) && Physics.Raycast(raycastPosRight, Vector3.down, out hitRight, 100f))
        {

            SpawnObject(Pylons, hitLeft.point);
            var go = SpawnObject(Pylons, hitRight.point);
            go.transform.RotateAround(go.transform.position, Vector3.up, 180f);
        }
    }
    private void SpawnFence(Vector3 position)
    {

    }

    private void SpawnBeeHive(Vector3 position)
    {
        if (Random.Range(0f, 1f) < 0.01f)
        {
            Vector2 range = CalcStreet(position);
            // get random position on street
            float rolledPos = Mathf.Lerp(range[0], range[1], Random.Range(-0.3f, 1.3f));
            // raycast to playce bee hive on ground
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(rolledPos, position.y, position.z), Vector3.down, out hit, 100f))
            {
                Vector3 spawnPos = new Vector3(rolledPos, hit.point.y, position.z);
                SpawnObject(Beehive, spawnPos);
            }
        }
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

    private Vector2 CalcStreetLeft(Vector3 position)
    {
        float left = position.x;
        // substract half of the street width
        left -= StreetCreator.VERTCOUNT * StreetCreator.EDGELENGTH / 2;
        float right = position.x;
        return new Vector2(left, right);
    }

    private GameObject SpawnObject(GameObject obj, Vector3 position)
    {
        float heightOffset = 100f;
        Vector3 offsetPosition = position + Vector3.up * heightOffset;
        // raycast to ground
        RaycastHit hit;
        GameObject result = null;
        if (Physics.Raycast(offsetPosition, Vector3.down, out hit, heightOffset * 2))
        {

            Quaternion rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles.x, Random.Range(0, 360), obj.transform.rotation.eulerAngles.z);
            result = Instantiate(obj, hit.point, rotation);
            _objects.Add(result);
        }
        return result;
        // random rotation around y axis

    }

}
