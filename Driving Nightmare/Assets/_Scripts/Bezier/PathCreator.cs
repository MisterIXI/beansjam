using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathCreator : MonoBehaviour
{
    public Material StreetMaterial;
    public Material GroundMaterial;
    public int VertCount = 30;
    public float GroundStretchFactor = 5f;
    public float EdgeLength = 1f;
    private int _subMeshCount;
    private float[] _subMeshFactors;
    private Mesh _mesh;
    private List<Vector3> _vertices;
    private List<List<int>> _subMeshes;
    // private List<int> _triangles;
    private List<Vector2> _uvs;
    private Vector3 _startOffset;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _vertices = new List<Vector3>();
        _subMeshes = new List<List<int>>();
        // _triangles = new List<int>();
        _uvs = new List<Vector2>();
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _subMeshCount = 3;
        
        // ReferenceHolder.EventHandler.SubscribeToEvent("DebugButton", MeshCallback);
    }

    private void MeshCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CreateMesh();
        }
    }

    private void BasePoints()
    {
        int totalVerts = VertCount  * _subMeshCount;
        float currX_offset = _startOffset.x;
        for (int meshNum = 0; meshNum < _subMeshCount; meshNum++)
        {
            for (int x = 0; x < VertCount; x++)
            {
                for (int y = 0; y < VertCount; y++)
                {
                    _vertices.Add(new Vector3(x * _subMeshFactors[meshNum] * EdgeLength+ currX_offset, x%2* _subMeshFactors[meshNum] + y%2, y * EdgeLength * 5f));
                    _uvs.Add(new Vector2((float)x / VertCount * _subMeshFactors[meshNum] + VertCount * meshNum, (float)y / VertCount + VertCount * meshNum));
                }
            }
            currX_offset += VertCount * _subMeshFactors[meshNum] * EdgeLength;
        }
        int stepSize = (VertCount - 1) / _subMeshCount;
        for (int meshNum = 0; meshNum < _subMeshCount; meshNum++)
        {
            List<int> subMesh = new List<int>();
            _subMeshes.Add(subMesh);
            for (int x = stepSize * meshNum; x < stepSize * (meshNum +1); x++)
            {
                for (int y = 0; y < stepSize; y++)
                {
                    subMesh.Add(x * VertCount + y);
                    subMesh.Add(x * VertCount + y + 1);
                    subMesh.Add((x + 1) * VertCount + y);

                    subMesh.Add((x + 1) * VertCount + y);
                    subMesh.Add(x * VertCount + y + 1);
                    subMesh.Add((x + 1) * VertCount + y + 1);
                }
            }
        }
    }
    private void CreateMesh()
    {
        _vertices.Clear();
        _subMeshes.Clear();
        _uvs.Clear();
        _subMeshFactors = new float[]{GroundStretchFactor, 1f, GroundStretchFactor};
        _startOffset = transform.position - transform.right * (GroundStretchFactor * EdgeLength * VertCount + EdgeLength * VertCount) / 2f;
        BasePoints();
        Debug.Log("Verts vs uv counts: " + _vertices.Count + " " + _uvs.Count);
        _mesh = new Mesh();
        _mesh.SetVertices(_vertices);
        _mesh.subMeshCount = _subMeshes.Count;
        for (int i = 0; i < _subMeshes.Count; i++)
        {   
            _mesh.SetTriangles(_subMeshes[i], i);
        }
        _mesh.RecalculateNormals();
        _mesh.SetUVs(0, _uvs);
        
        _meshFilter.mesh = _mesh;
        _meshRenderer.materials = new Material[] { GroundMaterial, StreetMaterial, GroundMaterial };
    }
}
