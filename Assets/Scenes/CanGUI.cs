using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR;

public class CanGUI : MonoBehaviour
{
    [SerializeField]
    private Button A;
    
    public TextMeshProUGUI lognews;
    
    // Start is called before the first frame update
    void Start()
    {
        A.onClick.AddListener(() =>
        {
                lognews.text = "Button Clicked ";
        });
    }

    // Update is called once per frame
    void Update()
    {
        A.onClick.AddListener(() =>
        {
            lognews.text = "Button Clicked ";
        });
    }
}
