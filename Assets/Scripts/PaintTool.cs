using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTool : MonoBehaviour
{

    public bool HasRadius = true;
    public bool HasColor = true;

    private Color m_toolColor;
    public Color ToolColor { get { return m_toolColor; } }

    private float m_toolRadius;
    public float ToolRadius { get { return m_toolRadius; } }



    void Start()
    {
        
    }

    public virtual void SetActive()
    {

    }

    public virtual void SetRadius(float radius)
    {
        m_toolRadius = radius;
    }

    public virtual void SetColor(Color color)
    {
        m_toolColor = color;
    }

}
