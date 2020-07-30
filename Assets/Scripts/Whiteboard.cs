using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
    
namespace Picasso
{
    [System.Serializable]
    public class PrimaryButtonEvent : UnityEvent<bool>
    {
    }

    public class Whiteboard : MonoBehaviour
    {
        //public PaintMode paintMode;

        public Transform linesParent;
        public GameObject linesParentObject;

        public string materialColor;
        
        private InputDevice targetDevice;
        public GameObject linePrefab;
        public GameObject rightController;

        public Transform painterPosition => rightController.transform.Find("RightControllerHitBox").transform;

        public bool isDrawing;
        public bool isGrabbing;
        public GameObject HitBox;
        public GameObject lineHitObject;
        public float hitRadiusVertexSnapping = 1f;
        public List<Line> lines;
        public Line currentLine;
        public Material material;



        public float width;


        

        public PrimaryButtonEvent primaryButtonPress;

        public TextMeshProUGUI lognews;
        public TextMeshProUGUI lineLengthLabel;

        public GameObject red_sphere;
        [Tooltip("Event when the button starts being pressed")]
        public UnityEvent OnPress;

        [Tooltip("Event when the button is released")]
        public UnityEvent OnRelease;
        
        // to check whether it's being pressed
        public bool IsPressed { get; private set; }
        
        bool TriggerButtonValue;
        bool SecondaryButtonValue;
        bool PrimaryButtonValue;
        
        
        public Ray forwardRay;
        public LayerMask toolboxLayer;
        public GameObject toolboxHitObject;
        public GameObject th_box;
        public GameObject Laser;
        
        /*
        public Draggable hueDraggable;
        public Draggable saturationDraggable;
*/


 

        public Vector3 dir;
        public UnityEngine.Color temp_color;
        public GameObject line;
        public GameObject coord;

        

        void Avake()
        {
            lognews.text = "";
            coord = GameObject.FindWithTag("coord");

            //Laser = GameObject.FindWithTag("Laser");
            
            GameObject varGameObject = GameObject.FindWithTag("HitBox");
        }


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

        private void Update()
        {

            Delete();
            DrawLine();
            RaycastToolbox();
            
            /*
            if (Physics.Raycast(forwardRay, out hit, 100f, toolboxLayer.value))
            {
                lognews.text = "ForwardRay HIT";
            }
            */
            

            string x = painterPosition.position.x.ToString("F");
            string z = painterPosition.position.z.ToString("F"); 
            string y = painterPosition.position.y.ToString("F");
            
            string position_x = ((painterPosition.position.x - coord.transform.position.x) * 100f).ToString("F");
            string position_y = ((painterPosition.position.y - coord.transform.position.y -0.07f) * 100f).ToString("F");
            string position_z = ((painterPosition.position.z - coord.transform.position.z - 0.03) * 100f).ToString("F");
            
            //lognews.text = lineHitObject.name;
            
            //lognews.text = "ForwardRay " + forwardRay;
                
            lineLengthLabel.text = "( "+position_x + " , "+position_z + " , " +position_y+" )";
            /*

            targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool SecondaryButtonValue);
            targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool PrimaryButtonValue);
            targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool TriggerButtonValue);
*/

        }
        

private void RaycastToolbox()
{
    forwardRay = new Ray(painterPosition.position, painterPosition.transform.TransformDirection(Vector3.forward));
    RaycastHit hit;
    
            
    if (Physics.Raycast(forwardRay, out hit, Mathf.Infinity)) {
             
        if (hit.transform.gameObject.layer != toolboxLayer) {
            toolboxHitObject = hit.collider.gameObject;
            HitBox.GetComponent<XRInteractorLineVisual>().enabled = true;
            // Make a path
        } else {
            toolboxHitObject = null;
            HitBox.GetComponent<XRInteractorLineVisual>().enabled = false;
            // Do whatever you want
        }
    }


}


        private void Delete()
        {
            //Delete Lines
            if (lines.Count > 0)
            {
                if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool SecondaryButtonValue) &&
                    SecondaryButtonValue)
                {

                    if (lineHitObject.name == "LineRenderer")
                    {
                        GameObject go = lineHitObject.transform.parent.gameObject;

                        var cs = go.GetComponent<Line>();

                        cs.start.position = new Vector3(0, 0, 0);
                        cs.end.position = new Vector3(0, 0, 0);
                        
                        //lognews.text = " press B Button\n";
                        
                    }

                }
            }
        }

        private void DrawLine()
        {
            //create dummy Line for color change
            if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                out PrimaryButtonValue) && PrimaryButtonValue)
            {
                currentLine = Instantiate(linePrefab).GetComponent<Line>();
                currentLine.start.position = new Vector3(0, 0, 0);
                currentLine.end.position = new Vector3(0, 0, 0);
                var color = material.color;
                material = new Material(material.shader);
                material.color = color;
                currentLine.material = material;
                currentLine.tag = "CurrentLine";
                currentLine.boxCollider.enabled = false;
                currentLine.end.GetComponent<SphereCollider>().enabled = false;
                currentLine.start.GetComponent<SphereCollider>().enabled = false;
            }

            //Draw  Lines that can snap to each Lines and Vertices from the lines
            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
            {
                // if start pressing, trigger event
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPress.Invoke();
                    
                    //creating lineprefab
                    currentLine = Instantiate(linePrefab).GetComponent<Line>();
                    currentLine.start.position = painterPosition.position;
                    currentLine.transform.parent = linesParent;
                    currentLine.draw_type = 1;
                    currentLine.tag = "CurrentLine";
                    currentLine.width = width;
                    
                    var color = material.color;
                    material = new Material(material.shader);
                    material.color = color;    
                    currentLine.material = material;
                    
                    
                    
                    currentLine.boxCollider.enabled = false;
                    currentLine.end.GetComponent<SphereCollider>().enabled = false;
                    currentLine.start.GetComponent<SphereCollider>().enabled = false;
                    
                    if (lineHitObject.name == "Start")
                    {
                        //Start Snap
                        GameObject go = lineHitObject.transform.parent.gameObject;
                            
                        var cs = go.GetComponent<Line>();
                        currentLine.start.position = cs.start.position;
                    }

                    if (lineHitObject.name == "End")
                    {
                        //End Snap
                        GameObject go = lineHitObject.transform.parent.gameObject;
                            
                        var cs = go.GetComponent<Line>();
                        currentLine.start.position = cs.end.position;
                    }
                    
                    //lognews.text = "start press Trigger\n";
                    
                    

                }
                //The Button is pressed 
                else
                {
                    //lognews.text = "Trigger is pressed\n"+ painterPosition.position;
                    currentLine.end.position = painterPosition.position;

                    if (lineHitObject.name == "LineRenderer")
                    {
                        //Snap on the line
                        GameObject go = lineHitObject.transform.parent.gameObject;
                            
                        var cs = go.GetComponent<Line>();
                        var start = cs.start;
                        var end = cs.end;
                        var p  = HitBox.transform.position;
                        var postion = NearestPointOnLine(start.position, end.position, p);
                        currentLine.end.position = NearestPointOnLine(start.position, end.position, p);
                    }

                    if (lineHitObject.name == "Start")
                    {
                        //Start Snap
                        GameObject go = lineHitObject.transform.parent.gameObject;
                            
                        var cs = go.GetComponent<Line>();
                        currentLine.end.position = cs.start.position;
                    }

                    if (lineHitObject.name == "End")
                    {
                        //End Snap
                        GameObject go = lineHitObject.transform.parent.gameObject;
                            
                        var cs = go.GetComponent<Line>();
                        currentLine.end.position = cs.end.position;
                    }
                }
            }
 
            // check for button release
            else if (IsPressed)
            {
                IsPressed = false;
                OnRelease.Invoke();
                currentLine.boxCollider.enabled = true;
                currentLine.end.GetComponent<SphereCollider>().enabled = true;
                currentLine.start.GetComponent<SphereCollider>().enabled = true;
                currentLine.tag = "Line";

                
                lines.Add(currentLine);
                
                //lognews.text = "Trigger is released\n";
            }
        }

        public static Vector3 NearestPointOnLine(Vector3 start, Vector3 end, Vector3 pnt)
        {
            var line = (end - start);
            var len = line.magnitude;
            line.Normalize();

            var v = pnt - start;
            var d = Vector3.Dot(v, line);
            d = Mathf.Clamp(d, 0f, len);
            return start + line * d;
        }
        void OnColorChange(HSBColor color)
        {
            var c = color.ToColor();
            c.a = 1f;
            material.color = c;
            materialColor = "#" + ToHex(c.a) + ToHex(c.r) + ToHex(c.g) + ToHex(c.b);
        }
        string ToHex(float n)
        {
            return ((int) (n * 255)).ToString("X").PadLeft(2, '0');
        }

    }
}