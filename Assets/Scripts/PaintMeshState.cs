﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintMeshState : MonoBehaviour
{

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _tris = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();
    private List<Color> _colors = new List<Color>();

    private PaintMesh _parent;

    private int _rings = 0;

    private Vector3 _prevRing0 = Vector3.zero;
    private Vector3 _prevRing1 = Vector3.zero;

    private Vector3 _prevNormal0 = Vector3.zero;

    private Mesh _mesh;
    private SmoothedVector3 _smoothedPosition;

    public PaintMeshState(PaintMesh parent)
    {
        _parent = parent;

        _smoothedPosition = new SmoothedVector3();
        _smoothedPosition.delay = 0.001f; // parent._smoothingDelay;
        _smoothedPosition.reset = true;
    }

    public GameObject BeginNewLine()
    {
        PaintMesh.stopWatch.Reset();
        PaintMesh.stopWatch.Start();
        _rings = 0;
        _vertices.Clear();
        _tris.Clear();
        _uvs.Clear();
        _colors.Clear();

        _smoothedPosition.reset = true;

        _mesh = new Mesh();
        _mesh.name = "Line Mesh";
        _mesh.MarkDynamic();
        

        GameObject lineObj = new GameObject("Line Object");
        lineObj.transform.position = Vector3.zero;
        lineObj.transform.rotation = Quaternion.identity;
        lineObj.transform.localScale = Vector3.one;
        lineObj.AddComponent<MeshFilter>().mesh = _mesh;
        lineObj.AddComponent<MeshRenderer>().sharedMaterial = _parent.PaintMaterial;

        return lineObj;
    }

    public void UpdateLine(Vector3 position)
    {
        _smoothedPosition.Update(position, Time.deltaTime);

        bool shouldAdd = false;

        shouldAdd |= _vertices.Count == 0;
        shouldAdd |= Vector3.Distance(_prevRing0, _smoothedPosition.value) >= _parent.MinSegmentLength;

        if (shouldAdd)
        {
            addRing(_smoothedPosition.value);
            updateMesh();
        }
    }

    public void FinishLine()
    {
        //_mesh.UploadMeshData(true);
        if (_mesh != null)
        {
            _mesh.UploadMeshData(false);
        }
    }

    private void updateMesh()
    {
        if (_mesh == null)
        {
            return;
        }

        _mesh.SetVertices(_vertices);
        _mesh.SetColors(_colors);
        _mesh.SetUVs(0, _uvs);
        _mesh.SetIndices(_tris.ToArray(), MeshTopology.Triangles, 0);
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
    }

    private void addRing(Vector3 ringPosition)
    {
        _rings++;

        if (_rings == 1)
        {
            addVertexRing();
            addVertexRing();
            addTriSegment();
        }

        addVertexRing();
        addTriSegment();

        Vector3 ringNormal = Vector3.zero;
        if (_rings == 2)
        {
            Vector3 direction = ringPosition - _prevRing0;
            float angleToUp = Vector3.Angle(direction, Vector3.up);

            if (angleToUp < 10 || angleToUp > 170)
            {
                ringNormal = Vector3.Cross(direction, Vector3.right);
            }
            else
            {
                ringNormal = Vector3.Cross(direction, Vector3.up);
            }

            ringNormal = ringNormal.normalized;

            _prevNormal0 = ringNormal;
        }
        else if (_rings > 2)
        {
            Vector3 prevPerp = Vector3.Cross(_prevRing0 - _prevRing1, _prevNormal0);
            ringNormal = Vector3.Cross(prevPerp, ringPosition - _prevRing0).normalized;
        }

        if (_rings == 2)
        {
            updateRingVerts(0,
                            _prevRing0,
                            ringPosition - _prevRing1,
                            _prevNormal0,
                            0);
        }

        if (_rings >= 2)
        {
            updateRingVerts(_vertices.Count - _parent.DrawResolution,
                            ringPosition,
                            ringPosition - _prevRing0,
                            ringNormal,
                            0);
            updateRingVerts(_vertices.Count - _parent.DrawResolution * 2,
                            ringPosition,
                            ringPosition - _prevRing0,
                            ringNormal,
                            1);
            updateRingVerts(_vertices.Count - _parent.DrawResolution * 3,
                            _prevRing0,
                            ringPosition - _prevRing1,
                            _prevNormal0,
                            1);
        }

        _prevRing1 = _prevRing0;
        _prevRing0 = ringPosition;

        _prevNormal0 = ringNormal;
    }

    private void addVertexRing()
    {
        for (int i = 0; i < _parent.DrawResolution; i++)
        {
            _vertices.Add(Vector3.zero);  //Dummy vertex, is updated later
            _uvs.Add(new Vector2(i / (_parent.DrawResolution - 1.0f), 0));
            _colors.Add(_parent.DrawColor);
        }
    }

    //Connects the most recently added vertex ring to the one before it
    private void addTriSegment()
    {
        for (int i = 0; i < _parent.DrawResolution; i++)
        {
            int i0 = _vertices.Count - 1 - i;
            int i1 = _vertices.Count - 1 - ((i + 1) % _parent.DrawResolution);

            _tris.Add(i0);
            _tris.Add(i1 - _parent.DrawResolution);
            _tris.Add(i0 - _parent.DrawResolution);

            _tris.Add(i0);
            _tris.Add(i1);
            _tris.Add(i1 - _parent.DrawResolution);
        }
    }

    private void updateRingVerts(int offset, Vector3 ringPosition, Vector3 direction, Vector3 normal, float radiusScale)
    {
        direction = direction.normalized;
        normal = normal.normalized;

        for (int i = 0; i < _parent.DrawResolution; i++)
        {
            float angle = 360.0f * (i / (float)(_parent.DrawResolution));
            Quaternion rotator = Quaternion.AngleAxis(angle, direction);
            Vector3 ringSpoke = rotator * normal * _parent.DrawRadius * radiusScale;
            _vertices[offset + i] = ringPosition + ringSpoke;
        }
    }


}
