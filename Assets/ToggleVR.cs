using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.Events;

public class ToggleVR : MonoBehaviour
{
    public Toggle toggle;
    
    [Tooltip("Event when the button starts being pressed")]
    public UnityEvent OnPress;

    [Tooltip("Event when the button is released")]
    public UnityEvent OnRelease;
        
    // to check whether it's being pressed
    public bool IsPressed { get; private set; }
        
    bool PrimaryButtonValue;
        
    private InputDevice targetDevice;
    
    public GameObject rightController;

    public Transform painterPosition => rightController.transform.Find("RightControllerHitBox").transform;
    
    public TextMeshProUGUI lognews;
    // Start is called before the first frame update
    void Start()
    {
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
        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
            out PrimaryButtonValue) && PrimaryButtonValue)
        {
            var forwardRay = new Ray(painterPosition.transform.position,  painterPosition.transform.TransformDirection(Vector3.forward));
            RaycastHit hit;
         /*
            if (GetComponent<Collider>().Raycast(forwardRay, out hit, 100f))
            {
                toggle.isOn = !toggle.isOn;
            }
            */
            if (Physics.Raycast(forwardRay, out hit, Mathf.Infinity)) {
                
                if (hit.transform.gameObject.name == "Checkmark"  ) {
                    
                    lognews.text = "Toggle!";
                    
                    toggle.isOn = !toggle.isOn;
                }
                
                
                
            }
            lognews.text = "not Toggle!";
            
        }
        
        
        

    }
}
