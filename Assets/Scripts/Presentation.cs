using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
using TMPro;

public class Presentation : MonoBehaviour
{
    [Tooltip("Event when the button starts being pressed")]
    public UnityEvent OnPress;

    [Tooltip("Event when the button is released")]
    public UnityEvent OnRelease;
    
    // to check whether it's being pressed
    public bool IsPressed { get; private set; }
    public bool IsAPressed { get; private set; }

    bool gripButtonValue;

    private InputDevice targetDevice;

    public int pageNumber = 1;
    
    public RawImage Image1;
	public RawImage Image2;
	public RawImage Image3;
	public RawImage Image4;
	public RawImage Image5;
	

    public TextMeshProUGUI lognews;
    // Start is called before the first frame update
    void Start()
    {

        lognews.text = "";
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics =
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }

    }

    // Update is called once per frame
    void Update()
    {
        //lognews.text = pageNumber.ToString();
        
		
        switch (pageNumber)
        {
			case 1:
				Image1.enabled = true;
				Image2.enabled = false;
				Image3.enabled = false;
				Image4.enabled = false;
				Image5.enabled = false;
				break;

			case 2:
				Image1.enabled = false;
				Image2.enabled = true;
				Image3.enabled = false;
				Image4.enabled = false;
				Image5.enabled = false;
				break;
				
			case 3:
				Image1.enabled = false;
				Image2.enabled = false;
				Image3.enabled = true;
				Image4.enabled = false;
				Image5.enabled = false;
				break;
				
			case 4:
				Image1.enabled = false;
				Image2.enabled = false;
				Image3.enabled = false;
				Image4.enabled = true;
				Image5.enabled = false;
				break;
				
			case 5:
				Image1.enabled = false;
				Image2.enabled = false;
				Image3.enabled = false;
				Image4.enabled = false;
				Image5.enabled = true;
				break;
				
			default:
				break;
		}
		
		 
        //create dummy Line for color change
        if (targetDevice.TryGetFeatureValue(CommonUsages.gripButton,
            out gripButtonValue) && gripButtonValue)
        {
            if (!IsAPressed)
            {
                IsAPressed = true;
                OnPress.Invoke();
                pageNumber++;
				
				
				if (pageNumber > 2)
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
