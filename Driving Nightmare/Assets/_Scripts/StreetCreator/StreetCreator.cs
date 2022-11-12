using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StreetCreator : MonoBehaviour
{
    const int VERTCOUNT = 60;
    const float EDGELENGTH = 1f;
    const float GROUNDSTRETCHFACTOR = 8f;

    public float ScrollSpeed = 5f;

    public Material StreetMaterial;
    public Material GroundMaterial;
    public float Deviation = 1f;
    private List<StreetChunk> _streetChunks;
    private List<GameObject> _streetChunkObjects;
    private List<Vector3> _streetChunkPositions;
    private Vector3 _currPos;
    private Vector3 _currDir;
    private Vector2 _lastDir;
    private float _lastSpawnTime;
    private void Start()
    {
        _streetChunks = new List<StreetChunk>();
        _streetChunkObjects = new List<GameObject>();
        _streetChunkPositions = new List<Vector3>();
        _currDir = Vector3.forward;
        _currPos = Vector3.zero;
        _lastDir = Vector2.zero;
        ReferenceHolder.EventHandler.SubscribeToEvent("DebugButton", DebugCallback);
    }

    private void DebugCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            NextChunk();
        }
    }

    private void FixedUpdate()
    {
        // if(_streetChunkPositions.Count > 0)
        //         Debug.Log("Furthest Z: " + _streetChunkPositions[_streetChunkPositions.Count - 1].z);
        if (_streetChunkObjects.Count == 0 || _streetChunkPositions[_streetChunkPositions.Count - 1].z < 100)
        {
            NextChunk();
        }

        Vector3 movement = Vector3.back * ScrollSpeed * Time.deltaTime;
        // move all Gameobjects back
        for (int i = 0; i < _streetChunkObjects.Count; i++)
        {
            _streetChunkObjects[i].transform.position += movement;
            _streetChunkPositions[i] += movement;
        }
        // _currDir += movement;
        _currPos += movement;
    }
    private void NextChunk()
    {
        GameObject newChunk = new GameObject();
        // newChunk.transform.position = _currPos;
        // newChunk.transform.rotation = Quaternion.LookRotation(_currDir);
        newChunk.transform.parent = transform;
        _streetChunkObjects.Add(newChunk);
        Material[] materials = new Material[] { GroundMaterial, StreetMaterial, GroundMaterial };
        StreetChunk newSC = new StreetChunk(_currPos, _lastDir, VERTCOUNT, EDGELENGTH, GROUNDSTRETCHFACTOR, new Vector2(Deviation, Deviation), newChunk, materials);
        _streetChunks.Add(newSC);
        _streetChunkPositions.Add(_currPos);
        _currPos = newSC.EndPos;
        _currDir = newSC.EndDir;
        _lastDir = newSC.TargetChange;
        newChunk.AddComponent<MeshCollider>();

        if (_streetChunks.Count > 10)
        {
            Destroy(_streetChunkObjects[0]);
            _streetChunkObjects.RemoveAt(0);
            _streetChunks.RemoveAt(0);
            _streetChunkPositions.RemoveAt(0);
        }
    }




    private void OnDrawGizmos()
    {
        if (_streetChunks != null && _streetChunks.Count > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_currPos, 5f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_currPos + _currDir, 5f);
        }
    }


}
