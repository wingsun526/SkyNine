using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTile : MonoBehaviour
{
    [SerializeField] private float toggleDelta;
    private Toggle m_Toggle;
    private float originalYpos;
    private RectTransform imageRectTransform;

    void Start()
    {
        m_Toggle = GetComponent<Toggle>();
        m_Toggle.onValueChanged.AddListener(delegate
        {
            ToggleTheTile();
        });
        m_Toggle.isOn = false;
        imageRectTransform = GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>();
        originalYpos = imageRectTransform.anchoredPosition.y;
        
    }

    private void ToggleTheTile()
    {
        float yPos = m_Toggle.isOn ? originalYpos + toggleDelta : originalYpos;
        var pos = imageRectTransform.anchoredPosition;
        imageRectTransform.anchoredPosition = new Vector2(pos.x, yPos);
    }

}
