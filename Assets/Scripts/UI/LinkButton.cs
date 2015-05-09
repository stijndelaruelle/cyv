using UnityEngine;
using System.Collections;

public class LinkButton : MonoBehaviour
{
    [SerializeField]
    private string m_Link;

    public void GoToLink()
    {
        if (string.IsNullOrEmpty(m_Link))
            return;

        Application.OpenURL(m_Link);
    }
}
