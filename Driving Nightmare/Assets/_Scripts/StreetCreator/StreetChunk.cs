using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StreetChunk
{
    static int id_helper = 0;
    public int chunkID;
    public Mesh Mesh;
    public Vector2 TargetChange;
    public Vector3 EndPos;
    public Vector3 EndDir;
    private Vector3 _startPos;

    private int _vertCount;
    private float BaseLength;
    private float GroundStretchFactor;
    private Vector2 _oldTarget;
    private Vector2 _targetSize;
    private List<Vector3> _vertices;
    private List<List<int>> _subMeshes;
    private List<Vector2> _uvs;
    private GameObject go;
    private Material[] _materials;

    public StreetChunk(Vector3 startPos, Vector2 oldTarget, int VertCount, float EdgeLength, float GroundStretch, Vector2 targetBoxSize, GameObject go, Material[] materials)
    {
        if (VertCount % 3 != 0)
        {
            // Debug.LogError("VertCount must be a multiple of 3");
            throw new System.Exception("VertCount must be a multiple of 3");
            // return;
        }
        this.go = go;
        _materials = materials;
        _targetSize = targetBoxSize;
        chunkID = id_helper++;
        this._startPos = startPos;
        _oldTarget = oldTarget;
        this._vertCount = VertCount;
        BaseLength = EdgeLength;
        GroundStretchFactor = GroundStretch;
        _vertices = new List<Vector3>();
        _subMeshes = new List<List<int>>();
        _uvs = new List<Vector2>();
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        _vertices.Clear();
        _subMeshes.Clear();
        _uvs.Clear();
        float[] _subMeshFactors = new float[] { GroundStretchFactor, 1f, GroundStretchFactor };
        TargetChange = new Vector2(Random.Range(-_targetSize.x / 2, _targetSize.x / 2), Random.Range(-_targetSize.y / 2, _targetSize.y / 2));
        Mesh = new Mesh();
        Vector3 target = new Vector3(TargetChange.x, TargetChange.y, 1);
        Vector3 startDir = new Vector3(_oldTarget.x, _oldTarget.y, 1);
        FillVertices(_subMeshFactors,startDir, target);
        Mesh.SetVertices(_vertices);
        Mesh.subMeshCount = _subMeshes.Count;
        for (int i = 0; i < _subMeshes.Count; i++)
        {
            Mesh.SetTriangles(_subMeshes[i], i);
        }
        Mesh.RecalculateNormals();
        Mesh.SetUVs(0, _uvs);
        go.AddComponent<MeshFilter>().mesh = Mesh;
        go.AddComponent<MeshRenderer>().materials = _materials;
    }
    private Vector2 Interpolate(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
    }

    private Vector3 Interpolate(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
    }
    private void FillVertices(float[] _subMeshFactors, Vector3 startDir, Vector3 target)
    {
        Vector3 currPos = _startPos;
        Vector3 currDir = startDir;
        for (int y = 0; y < _vertCount * 3; y++)
        {
            Vector3 vertPos = currPos + Vector3.left * ((_vertCount+1) * BaseLength * _subMeshFactors[0] + _vertCount * BaseLength * _subMeshFactors[1] / 2f);
            for (int x = 0; x < _vertCount * 3; x++)
            {
                // Debug.Log(x / _vertCount);
                vertPos += Vector3.right * BaseLength * _subMeshFactors[x/_vertCount];
                _vertices.Add(vertPos);
                _uvs.Add(new Vector2((float)x, (float)y));
            }
            currDir = Interpolate(currDir, target, y / (float)(_vertCount * 3));
            currPos += currDir * BaseLength *3f;
            Debug.Log("Curr dir: " + currPos);
        }
        currPos -= currDir * BaseLength *3f;

        EndPos = currPos;
        EndDir = currDir;

        for (int subMesh = 0; subMesh < _subMeshFactors.Length; subMesh++)
        {
            List<int> subMeshIndices = new List<int>();
            _subMeshes.Add(subMeshIndices);
            
            for (int y = 0; y < _vertCount * 3 ; y++)
            {
                for (int x = 0; x < _vertCount; x++)
                {
                    // y check everytime
                    if( y == _vertCount * 3 - 1)
                    {
                        continue;
                    }
                    // x check
                    if ( subMesh ==2 && x == _vertCount -1)
                    {
                        continue;
                    }

                    int baseX = x + subMesh * _vertCount;
                    int yMult = _vertCount * 3;
                    subMeshIndices.Add(baseX + y * yMult);
                    subMeshIndices.Add(baseX + (y + 1) * yMult);
                    subMeshIndices.Add(baseX + 1 + y * yMult);

                    subMeshIndices.Add(baseX + 1 + y * yMult);
                    subMeshIndices.Add(baseX + (y + 1) * yMult);
                    subMeshIndices.Add(baseX + 1 + (y + 1) * yMult);
                }
            }
        }


    }


}