using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICtrl : MonoBehaviour
{
    public GPCtrl GP;

    void Start()
    {
        GP = FindObjectOfType<GPCtrl>();
    }
}
