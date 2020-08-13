using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using System;

namespace Picasso
{
    [System.Serializable]
    public class PrimaryButtonEvent : UnityEvent<bool>
    {
    }

    public class Whiteboard : MonoBehaviour
    {
        public PaintMode paintMode;
        //public PaintMode paintMode;

        public Transform linesParent;
        public Transform DummyParent;
        
        public GameObject linesParentObject;

        public string materialColor;
        
        private InputDevice targetDevice;
        public GameObject linePrefab;
        public GameObject rightController;

        public Transform painterPosition => rightController.transform.Find("RightControllerHitBox").transform;

        public bool isDrawing;
        public bool isGrabbing;
        public GameObject HitBox;
        public GameObject RayInteractor;
        
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
        public bool IsAPressed { get; private set; }
        
        bool TriggerButtonValue;
        bool SecondaryButtonValue;
        bool PrimaryButtonValue;
        
        
        public Ray forwardRay;
        public LayerMask toolboxLayer;
        public LayerMask UI;
        public GameObject toolboxHitObject;
        public GameObject th_box;
        public GameObject Laser;
        
        
        public Toggle toggle;
        public Toggle toggle2;
        public Toggle toggle3;
        public Toggle toggle4;
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

        public void enableDrawMode(bool isEnabled)
        {
            paintMode = isEnabled ? PaintMode.Draw : PaintMode.DrawingLines;
        }

        public void enableDrawingLinesMode(bool isEnabled)
        {
            paintMode = isEnabled ? PaintMode.DrawingLines : PaintMode.Draw;
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
            RaycastToolbox();
            
            if (toolboxHitObject == null)
            {
                switch (paintMode)
                {
                    case PaintMode.Draw:
                        Draw();
                        //lognews.text = "Draw";
                            
                        break;
                    
                    case PaintMode.DrawingLines:
                        DrawLine();
                        //lognews.text = "DrawLine";
                        break;
                    
                    case PaintMode.DrawLongLine:
                        DrawLongLine();
                        //lognews.text = "DrawLine";
                        break;
                    
                    case PaintMode.DrawPoints:
                        DrawPoints();
                        //lognews.text = "DrawLine";
                        break;             
                    
                        

                    default:
                        break;
                }

            }
            
            /*
            if (Physics.Raycast(forwardRay, out hit, 100f, toolboxLayer.value))
            {
                lognews.text = "ForwardRay HIT";
            }
            */
            

            //string x = painterPosition.position.x.ToString("F");
            //string z = painterPosition.position.z.ToString("F"); 
            //string y = painterPosition.position.y.ToString("F");
            
            //float fc = (float)Math.Round(f * 100f) / 100f;



            float position_x = (float) Math.Round(((painterPosition.position.x - coord.transform.position.x) * 100),0);
                
            string position_x_s = position_x.ToString("F"); 
            
            
            string position_y = ((painterPosition.position.y - coord.transform.position.y -0.07f) * 100f).ToString("F");
            string position_z = ((painterPosition.position.z - coord.transform.position.z - 0.03) * 100f).ToString("F");
            
            //lognews.text = lineHitObject.name;
            
            //lognews.text = "ForwardRay " + forwardRay;
                
            lineLengthLabel.text = "( "+position_x_s + " , "+position_z + " , " +position_y+" )";
            /*

            targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool SecondaryButtonValue);
            targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool PrimaryButtonValue);
            targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool TriggerButtonValue);
*/
            //create dummy Line for color change
            if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                out PrimaryButtonValue) && PrimaryButtonValue)
            {
                if (!IsAPressed)
                {
                    IsAPressed = true;
                    OnPress.Invoke();
                    currentLine = Instantiate(linePrefab).GetComponent<Line>();
                    currentLine.tag = "DummyLine";
                    currentLine.boxCollider.enabled = false;
                    currentLine.end.GetComponent<SphereCollider>().enabled = false;
                    currentLine.start.GetComponent<SphereCollider>().enabled = false;
                    currentLine.start.position = new Vector3(0, 0, 0);
                    currentLine.end.position = new Vector3(0, 0, 0);
                    currentLine.transform.parent = DummyParent;

                }
                else
                {
                    var color = material.color;
                    material = new Material(material.shader);
                    material.color = color;
                    currentLine.material = material;
                }

            }  // check for button release
            else if (IsAPressed)
            {
                IsAPressed = false;
                OnRelease.Invoke();
                
                GameObject go = currentLine.transform.gameObject;
                var cs = go.GetComponent<Line>();
                
            }
        }

        void FixedUpdate()
        {
            
        }

        private void RaycastToolbox()
        {
            forwardRay = new Ray(painterPosition.position, painterPosition.transform.TransformDirection(Vector3.forward));
            RaycastHit hit;
            
            if (Physics.Raycast(forwardRay, out hit, Mathf.Infinity)) {
                
                //lognews.text = hit.transform.gameObject.name;


                if (hit.transform.gameObject.name == "Checkmark")
                {
                    if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                            out PrimaryButtonValue) && PrimaryButtonValue)
                    {
                        // if start pressing, trigger event
                        if (!IsPressed)
                        {
                            IsPressed = true;
                            OnPress.Invoke();
                            
                            //lognews.text = "Toggle!";
                            
                            toggle.isOn = true;
                            toggle2.isOn = false;
                            toggle3.isOn = false;
                            toggle4.isOn = false;

                            paintMode = PaintMode.Draw;
                        }
                    }
                }
                if (hit.transform.gameObject.name == "Checkmark2")
                {
                    if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                        out PrimaryButtonValue) && PrimaryButtonValue)
                    {
                        // if start pressing, trigger event
                        if (!IsPressed)
                        {
                            IsPressed = true;
                            OnPress.Invoke();
                            
                            //lognews.text = "Toggle2!";
                            
                            toggle.isOn = false;
                            toggle2.isOn = true;
                            toggle3.isOn = false;
                            toggle4.isOn = false;
                            paintMode = PaintMode.DrawingLines;
                        }
                    }
                }
                if (hit.transform.gameObject.name == "Checkmark3")
                {
                    if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                        out PrimaryButtonValue) && PrimaryButtonValue)
                    {
                        // if start pressing, trigger event
                        if (!IsPressed)
                        {
                            IsPressed = true;
                            OnPress.Invoke();
                            
                            //lognews.text = "Toggle2!";
                            
                            toggle.isOn = false;
                            toggle2.isOn = false;
                            toggle3.isOn = true;
                            toggle4.isOn = false;
                            paintMode = PaintMode.DrawLongLine;
                        }
                    }
                }
                if (hit.transform.gameObject.name == "Checkmark4")
                {
                    if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                        out PrimaryButtonValue) && PrimaryButtonValue)
                    {
                        // if start pressing, trigger event
                        if (!IsPressed)
                        {
                            IsPressed = true;
                            OnPress.Invoke();
                            
                            //lognews.text = "Toggle2!";
                            
                            toggle.isOn = false;
                            toggle2.isOn = false;
                            toggle3.isOn = false;
                            toggle4.isOn = true;
                            paintMode = PaintMode.DrawPoints;
                        }
                    }
                }               
                if (hit.transform.gameObject.layer == 8  ) {
                    toolboxHitObject = hit.collider.gameObject;
                    RayInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
                } else
                {
                    toolboxHitObject = null;
                    RayInteractor.GetComponent<XRInteractorLineVisual>().enabled = false;
                }
            }
            
            
         /*   
            if (Physics.Raycast(forwardRay, out hit, 100f, toolboxLayer.value))
            {
                lognews.text = "tool: " +hit.transform.gameObject.layer.ToString("F");
                RayInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
            }
            else
            {
                lognews.text = "else: " + hit.transform.gameObject.layer.ToString("F");
                RayInteractor.GetComponent<XRInteractorLineVisual>().enabled = false;
            }
            */
            
            /*
            if (Physics.Raycast(forwardRay, out hit, Mathf.Infinity)) {
                
                lognews.text = hit.transform.gameObject.layer.ToString("F");
                     
                if (hit.transform.gameObject.layer == toolboxLayer  ) {
                    toolboxHitObject = hit.collider.gameObject;
                    RayInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
                } else
                {
                    toolboxHitObject = null;
                    RayInteractor.GetComponent<XRInteractorLineVisual>().enabled = false;
                }
            }
            */


        }
        private void Delete()
        {
            //Delete Lines
            if (lines.Count > 0)
            {
                if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool SecondaryButtonValue) &&
                    SecondaryButtonValue)
                {

                    if (lineHitObject.name == "LineRenderer" || lineHitObject.name == "Start" || lineHitObject.name == "End")
                    {
                        //line=lineHitObject.transform.parent.gameObject;
                        
                        GameObject line = lineHitObject.transform.parent.gameObject;
                        
                        var cs = line.GetComponent<Line>();

                        //lognews.text = "Delete: "+ line.name;
                        
                        cs.end.GetComponent<SphereCollider>().enabled = false;
                        cs.start.GetComponent<SphereCollider>().enabled = false;
                        //cs.lineRenderer.GetComponent<BoxCollider>().enabled = false;
                        cs.boxCollider.enabled = false;
                       
                        
                        cs.boxCollider.isTrigger = false;
                        
                        cs.enabled = false;
                        line.SetActive(false);

                        Destroy(line);


                        //currentLine.boxCollider.enabled = false;

                        //currentLine.start.GetComponent<SphereCollider>().enabled = false;

                        //line.boxCollider.enabled = false;
                        //line.end.GetComponent<SphereCollider>().enabled = false;
                        //line.start.GetComponent<SphereCollider>().enabled = false;
                        //transform.parent=null;
                        //line.SetActive(false);
                        //GameObject go = lineHitObject.transform.parent.gameObject;
                        //var cs = go.GetComponent<Line>();
                        //cs.start.position = new Vector3(0, 0, 0);
                        //cs.end.position = new Vector3(0, 0, 0);


                        //cs.boxCollider.enabled = false;
                        //cs.end.GetComponent<SphereCollider>().enabled = false;
                        //cs.start.GetComponent<SphereCollider>().enabled = false;

                        //go.SetActive(false);
                        //cs.end.parent.gameObject.SetActive(false);
                        //cs.start.parent.gameObject.SetActive(false);

                        //lognews.text = " press B Button\n";

                    }

                }
            }
        }
        private void Draw()
        {
            
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
                    isDrawing = true;

                    currentLine = Instantiate(linePrefab).GetComponent<Line>();
                    currentLine.tag = "free";
                    currentLine.draw_type = 2;
                    currentLine.width = 0.01f;
                    currentLine.transform.parent = linesParent; 
                    
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
                    currentLine.numClicks = 0;

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
                currentLine.tag = "free";

                
                isDrawing = false;
                var color = material.color;
                material = new Material(material.shader);
                material.color = color;

                lines.Add(currentLine);
                
                //lognews.text = "Trigger is released\n";
            }
        }
        private void DrawLine()
        {
 
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
                    
                    currentLine.boxCollider.enabled = false;
                    //currentLine.meshCollider.enabled = false;
                    currentLine.end.GetComponent<SphereCollider>().enabled = false;
                    currentLine.start.GetComponent<SphereCollider>().enabled = false;

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
        private void DrawLongLine()
        {
 
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
                    currentLine.draw_type = 3;
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
                    
                    currentLine.boxCollider.enabled = false;
                    //currentLine.meshCollider.enabled = false;
                    currentLine.end.GetComponent<SphereCollider>().enabled = false;
                    currentLine.start.GetComponent<SphereCollider>().enabled = false;

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

        
        private void DrawPoints()
        {
 
            //Draw  Lines that can snap to each Lines and Vertices from the lines
            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
            {
                // if start pressing, trigger event
                if (!IsPressed)
                {
                    
                    currentLine = Instantiate(linePrefab).GetComponent<Line>();
                    IsPressed = true;
                    OnPress.Invoke();
                    
                }
                //The Button is pressed 
                else
                {
                    
                    currentLine.start.position = painterPosition.position;
                    currentLine.transform.parent = linesParent;
                    currentLine.draw_type = 4;
              
                }
            }
 
            // check for button release
            else if (IsPressed)
            {
                IsPressed = false;
                OnRelease.Invoke();
                currentLine.boxCollider.enabled = true;
                currentLine.start.GetComponent<SphereCollider>().enabled = true;
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