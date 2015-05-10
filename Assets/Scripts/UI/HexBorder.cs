using UnityEngine;
using System.Collections;

public class HexBorder : MonoBehaviour
{
    public enum HexBorderMaterial
    {
        From,
        To,
        Promote, 
        None
    }

    [SerializeField]
    private Renderer m_Renderer;

    [SerializeField]
    private float m_RotationSpeed;

    [SerializeField]
    private Material[] m_Materials;

	void Update() 
    {
        Vector2 currentOffset = m_Renderer.material.mainTextureOffset;
        currentOffset.x += Time.deltaTime;

        m_Renderer.material.mainTextureOffset = currentOffset;
	}

    public void SetMaterial(HexBorderMaterial material)
    {
        int matID = (int)material;
        if (matID >= m_Materials.Length)
        {
            m_Renderer.enabled = false;
            return;
        }

        Material mat = m_Materials[matID];
        m_Renderer.material = mat;
        m_Renderer.enabled = true;
    }
}
