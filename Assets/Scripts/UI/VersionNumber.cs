using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;

public class VersionNumber : MonoBehaviour
{
    [SerializeField]
    private Text m_TextField;

	void Start ()
    {
	    //Get the version number & set it to the textfield
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        m_TextField.text = "Version: " + version;
	}
	
}
