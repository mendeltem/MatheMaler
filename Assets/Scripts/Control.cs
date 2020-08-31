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

    // Start is called before the first frame update
    void Start()
    {
        // Die Buttons werden aufgelistet um darauf zugreifen zu können
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
        //Wenn auf linke Steuerung die Taste X gedrückt wird gibt es ein Reload

        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
            out PrimaryButtonValue) && PrimaryButtonValue)
        {
            if (!IsAPressed)
            {
                IsAPressed = true;
                OnPress.Invoke();

				
				//neu laden der Szene
				SceneManager.LoadScene("MatheMaler"); 
				
            }
           
        }
        else if (IsAPressed)
        {
            IsAPressed = false;
            OnRelease.Invoke();
        }
    }
}
