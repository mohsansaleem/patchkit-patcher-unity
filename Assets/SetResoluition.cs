using UnityEngine;
using System.Collections;

public class SetResoluition : MonoBehaviour {

    void Awake()
    {
        Screen.SetResolution(1280, 720, false);
    }
}
