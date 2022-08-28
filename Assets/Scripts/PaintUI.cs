using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintUI : MonoBehaviour
{

    [SerializeField] private ToolProperties m_toolProperties;

    [SerializeField] private Toggle m_paintMeshToggle;
    [SerializeField] private Toggle m_paintTextureToggle;

    [SerializeField] private PaintTool m_paintMeshTool;
    [SerializeField] private PaintTool m_paintTextureTool;

    private void Awake()
    {
        m_paintMeshToggle.onValueChanged.AddListener(SetCreateMeshTool);
        m_paintTextureToggle.onValueChanged.AddListener(SetPaintTextureTool);
    }

    private void Start()
    {
        SetCreateMeshTool(true);
    }

    private void SetCreateMeshTool(bool isSelected)
    {
        if (isSelected == true)
        {
            m_toolProperties.SetCurrentTool(m_paintMeshTool);
        }
    }

    private void SetPaintTextureTool(bool isSelected)
    {
        if (isSelected == true)
        {
            m_toolProperties.SetCurrentTool(m_paintTextureTool);
        }
    }
}
