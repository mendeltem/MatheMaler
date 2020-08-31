using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;

public class Draggable : MonoBehaviour
{
	public bool fixX;
	public bool fixY;
	public Transform thumb;	
	bool dragging;

    public GameObject rightController;
    
    public Transform painterPosition => rightController.transform.Find("RightControllerHitBox").transform;
    
	[Tooltip("Event when the button starts being pressed")]
    public UnityEvent OnPress;

    [Tooltip("Event when the button is released")]
    public UnityEvent OnRelease;
        
    // to check whether it's being pressed
    public bool IsPressed { get; private set; }
        
    bool PrimaryButtonValue;
        
    private InputDevice targetDevice;

	public Ray forwardRay;

    public int toolboxLayer;

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
 		toolboxLayer = LayerMask.NameToLayer("Toolbox");

    }

	void FixedUpdate()
	{
        //hier wurde die Funktion nach Button und raycast angepasst
        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                    out PrimaryButtonValue) && PrimaryButtonValue)
            {
            var ray = new Ray(painterPosition.position, rightController.transform.TransformDirection(Vector3.forward));
            RaycastHit hit;
            
            if (GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                var point = hit.point;
                point = GetComponent<Collider>().ClosestPointOnBounds(point);
                SetThumbPosition(point);
                
                
                SendMessage("OnDrag", Vector3.one - (thumb.position - GetComponent<Collider>().bounds.min) /
                    GetComponent<Collider>().bounds.size.x);
                
            }
        }

	}

	void SetDragPoint(Vector3 point)
	{
		point = (Vector3.one - point) * GetComponent<Collider>().bounds.size.x + GetComponent<Collider>().bounds.min;
		SetThumbPosition(point);
	}

	void SetThumbPosition(Vector3 point)
	{
		thumb.position = new Vector3(fixX ? thumb.position.x : point.x, fixY ? thumb.position.y : point.y, thumb.position.z);
	}
}
