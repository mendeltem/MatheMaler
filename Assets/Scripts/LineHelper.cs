using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
namespace Picasso
{
public class LineHelper : MonoBehaviour
{
    public Material material;
    
    public float width;
    
    //public SteamVR_Behaviour_Pose rightController;
    public GameObject rightController;
    
    public Transform painterPosition => rightController.transform.Find("RightControllerHitBox").transform;
    
    public GameObject StaticHelper;
    
    public GameObject Helper;
    
    public GameObject Coordinate;
    
    public GameObject player;
    
    
    [Tooltip("Event when the button starts being pressed")]
    public UnityEvent OnPress;

    [Tooltip("Event when the button is released")]
    public UnityEvent OnRelease;
        
    // to check whether it's being pressed
    public bool IsPressed { get; private set; }
        
    bool TriggerButtonValue;
        
    private InputDevice targetDevice;

    public PaintMode paintMode;
    
    private void Start()
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
    
    
    private void Awake()
    {
        //trigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
        Helper.SetActive(false);
        StaticHelper.SetActive(false);
    }
    
    void Update()
    {
        //Hilfslinie verfolgt den Pinselpunkt
        Helper.transform.position = painterPosition.position;
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
        {
                //wenn die Zeichentaste gedrück wurde aktivieren sich die Hilfslinien
                Helper.SetActive(true);
                StaticHelper.SetActive(true);
               

        }
        else
        {
                //beim loslassen deaktivieren sich die Hilfslinien
                Helper.SetActive(false);
                StaticHelper.SetActive(false);
                StaticHelper.transform.position = painterPosition.position; 
        }
        
    }
}}