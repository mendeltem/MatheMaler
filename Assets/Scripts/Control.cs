using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
 
 
public class Control : MonoBehaviour
{
    [Tooltip("Event when the button starts being pressed")]
    public UnityEvent OnPress;

    [Tooltip("Event when the button is released")]
    public UnityEvent OnRelease;
    
    // to check whether it's being pressed
    public bool IsPressed { get; private set; }
    public bool IsAPressed { get; private set; }

    bool gripButtonValue;
	
	bool PrimaryButtonValue;

    private InputDevice targetDevice;

    public int pageNumber = 1;
    

    public TextMeshProUGUI lognews;
    // Start is called before the first frame update
    void Start()
    {

        lognews.text = "";
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics leftControllerCharacteristics =
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }

    }

    // Update is called once per frame
    void Update()
    {
        lognews.text = pageNumber.ToString();
        
		
        //create dummy Line for color change
        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
            out PrimaryButtonValue) && PrimaryButtonValue)
        {
            if (!IsAPressed)
            {
                IsAPressed = true;
                OnPress.Invoke();
                pageNumber++;
				
				
				SceneManager.LoadScene("MatheMaler"); 
				
				if (pageNumber > 4)
				{
					pageNumber=1;
				}
				
            }
           
        }
        else if (IsAPressed)
        {
            IsAPressed = false;
            OnRelease.Invoke();
        }
    }
}
