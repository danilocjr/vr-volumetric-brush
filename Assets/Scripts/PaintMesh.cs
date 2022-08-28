using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class PaintMesh : PaintTool
{

    [SerializeField] private Material m_material;
    public Material PaintMaterial {get { return m_material; }}

    [SerializeField] private Color m_drawColor = Color.white;
    public Color DrawColor { get { return m_drawColor; } }

    [SerializeField] private float m_smoothingDelay = 0.01f;

    [SerializeField] private float m_drawRadius = 0.002f;
    public float DrawRadius { get { return m_drawRadius; } }

    [SerializeField] private int m_drawResolution = 8;
    public int DrawResolution {  get { return m_drawResolution; } }

    [SerializeField] private float m_minSegmentLength = 0.005f;
    public float MinSegmentLength {  get { return m_minSegmentLength; } }

    private PaintMeshState m_drawState;
    private bool isDrawing = false;

    public static Stopwatch stopWatch = new Stopwatch();

    public GameObject MeshGameobject;

    private RaycastHit hit;
    private Ray ray;



    void Start()
    {
        m_drawState = new PaintMeshState(this);
    }

    public override void SetRadius(float radius)
    {
        base.SetRadius(radius);
        m_drawRadius = ToolRadius;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector4 mwp = Vector3.positiveInfinity;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == MeshGameobject)
                {

                    if (isDrawing == false)
                    {
                        isDrawing = true;
                        m_drawState.BeginNewLine();
                    }
                    mwp = hit.point;
                    m_drawState.UpdateLine(mwp);
                }
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            m_drawState.FinishLine();
            stopWatch.Stop();
        }
    }
}
