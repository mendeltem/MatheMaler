using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Video;

 using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
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



    public VideoPlayer videoPlayer;

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


        videoPlayer.loopPointReached += CloseVideo;
    }

    
     void CloseVideo(VideoPlayer vp)
     {

          Image1.enabled = false;
     }



/*
    // Update is called once per frame
    void Update()
    {
        //create dummy Line for color change
        if (targetDevice.TryGetFeatureValue(CommonUsages.gripButton,
            out gripButtonValue) && gripButtonValue)
        {
            if (!IsAPressed)
            {
                IsAPressed = true;
                OnPress.Invoke();
                pageNumber++;

                Image1.enabled = true;
                videoPlayer.enabled = true;
				
				
				if (pageNumber > 4)
				{
					pageNumber=1;
				}
				
            }
           
        }

        
    }
    */
}
