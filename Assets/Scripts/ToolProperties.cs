using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolProperties : MonoBehaviour
{

    [SerializeField] Slider m_radiusSlider;
    [SerializeField] TextMeshProUGUI m_radiusLabel;

    private PaintTool m_currentTool;

    

    // Start is called before the first frame update
    void Start()
    {
        m_radiusSlider.onValueChanged.AddListener( ChangeRadius );
    }

    private void ChangeRadius(float radius)
    {
       m_currentTool.SetRadius(radius);
        m_radiusLabel.text = radius.ToString("F2");
    }


    public void SetCurrentTool(PaintTool paintTool)
    {
        m_currentTool = paintTool;
        m_radiusSlider.gameObject.SetActive(m_currentTool.HasRadius);
    }
}
