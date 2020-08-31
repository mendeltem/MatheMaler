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
        public Transform linesParent;
        public Transform DummyParent;
        public GameObject linesParentObject;
        public string materialColor;
        private InputDevice targetDevice;
        public GameObject linePrefab;
        public GameObject circlePrefab;
        public GameObject rightController;
        //Pinsel Punkt zum Zeichnen
        public Transform painterPosition => rightController.transform.Find("RightControllerHitBox").transform;
        public bool isDrawing;
        public bool isGrabbing;
        public GameObject HitBox;
        public GameObject RayInteractor;
        public GameObject lineHitObject;
        public LineRenderer currentCircle;
        public float hitRadiusVertexSnapping = 1f;
        public float x;
        public List<Line> lines;
        public Line currentLine;
        public Material material;
        public float width;
        public float radiuslength;
        public PrimaryButtonEvent primaryButtonPress;
        public TextMeshProUGUI lognews;
        public TextMeshProUGUI lineLengthLabel;
        public TextMeshProUGUI x_vectorLabel;
        public TextMeshProUGUI y_vectorLabel;
        public TextMeshProUGUI z_vectorLabel;
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
        public GameObject Tutorial_trigger;
        public GameObject Tutorial_abutton;
        public GameObject Tutorial_bbutton;
        public Vector3 dir;
        public Vector3 pp;
        public Vector3 start;
        public Vector3 end;
        public Vector3 radius;
        public UnityEngine.Color temp_color;
        public GameObject line;
        public GameObject coord;
		private bool coloring;
        double painter_position_x; 
        double painter_position_y;
        double painter_position_z;

        void Avake()
        {
            //Text zum Debugen 
            lognews.text = "";
            //Koordinatensystem 
            coord = GameObject.FindWithTag("coord");
        }

   
        private void Start()
        {
            // Die Buttons werden aufgelistet um darauf zugreifen zu können
            List<InputDevice> devices = new List<InputDevice>();
            InputDeviceCharacteristics rightControllerCharacteristics =
                InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
            
            if (devices.Count > 0)
            {
                targetDevice = devices[0];
            }
			
			coloring = false;

        }

        private void Update()
        {
            //löschen von Linien, Punkte und Kreis immer erreichbar
            Delete();
            //Laser wird nur sichtbar wenn auf Toolbox gezeigt wird
            RaycastToolbox();
            
            
            if (toolboxHitObject == null)
            {
                //Zeichenmode auswahl
                switch (paintMode)
                {
                    case PaintMode.Draw:
                        //freies zeichen
                        Draw();
                        break;
                    
                    case PaintMode.DrawingLines:
                        //Linien Zeichnen
                        DrawLine();
                        break;
                    
                    case PaintMode.DrawLongLine:
                        //Kreis Zeichnen
                        DrawCircle();
                        break;
                    
                    case PaintMode.DrawPoints:
                        //Punkte eintragen
                        DrawPoints();
                        break;             
                    
                    default:
                        break;
                }

            }
            //zum Debuggen 
            //lognews.text = lineHitObject.name;

            //Die Pinselpunkt Position im Verhältniss zu Koordinatensystem und die Umrechnung
            painter_position_x = ((painterPosition.position.x - coord.transform.position.x) * 100);
            painter_position_y = ((painterPosition.position.y - coord.transform.position.y - 0.07f) * 100f);
            painter_position_z = ((painterPosition.position.z - coord.transform.position.z - 0.03) * 100f);
            
            //Vektorangaben zum Pinselpunkt Position
            x_vectorLabel.text = "X: "+painter_position_x.ToString("F0");
            y_vectorLabel.text = "Y: "+painter_position_z.ToString("F0");
            z_vectorLabel.text = "Z: "+painter_position_y.ToString("F0");

            //Erzeugt ein Dummy Linie, dessen Farbe sich ändern sollte statt des vor kurzem erzeugten Linie
            //Es da ist um ein Bug zu bekämpfen
            if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                out PrimaryButtonValue) && PrimaryButtonValue)
            {
                //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
                Tutorial_abutton.SetActive(false);

				coloring = true;
                if (!IsAPressed)
                {
                    IsAPressed = true;
                    OnPress.Invoke();
                    currentLine = Instantiate(linePrefab).GetComponent<Line>();
                    currentLine.tag = "CurrentLine";
                    currentLine.boxCollider.enabled = false;
                    currentLine.end.GetComponent<SphereCollider>().enabled = false;
                    currentLine.start.GetComponent<SphereCollider>().enabled = false;
                    //die dummy linie sollte nicht sichtbar sein für den User
                    currentLine.start.position = new Vector3(0, 0, 0);
                    currentLine.end.position = new Vector3(0, 0, 0);
                    //die Sammlung befindet sich in Dummy Parent
                    currentLine.transform.parent = DummyParent;

                    currentLine.material = material;
					currentLine.draw_type = "free";
                }
            

            }  // Beim loslassen
            else if (IsAPressed)
            {
				coloring = false;
                IsAPressed = false;
                OnRelease.Invoke();
                
                GameObject go = currentLine.transform.gameObject;
                var cs = go.GetComponent<Line>();

                var color = material.color;
                material = new Material(material.shader);
                material.color = color;
                
            }
        }


        private void RaycastToolbox()
        {
            // hier findet der Modi Wechsel statt
            forwardRay = new Ray(painterPosition.position, painterPosition.transform.TransformDirection(Vector3.forward));
            RaycastHit hit;


            //wenn Raycast trifft
            if (Physics.Raycast(forwardRay, out hit, Mathf.Infinity)) {
                //auf die jeweiligen Toggle buttons
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
                            //Toggles werden markiert und demarkiert
                            toggle.isOn = true;
                            toggle2.isOn = false;
                            toggle3.isOn = false;
                            toggle4.isOn = false;

                            //Modi wird auf freies Zeichnnen gewechselt
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
                            
                            //Toggles werden markiert und demarkiert
                            toggle.isOn = false;
                            toggle2.isOn = true;
                            toggle3.isOn = false;
                            toggle4.isOn = false;

                            //Modi wird auf Linien Zeichnnen gewechselt
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
                            //Toggles werden markiert und demarkiert
                            toggle.isOn = false;
                            toggle2.isOn = false;
                            toggle3.isOn = true;
                            toggle4.isOn = false;
                            //Modi wird auf Kreis Zeichnnen gewechselt
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
                            //Toggles werden markiert und demarkiert
                            toggle.isOn = false;
                            toggle2.isOn = false;
                            toggle3.isOn = false;
                            toggle4.isOn = true;
                            //Modi wird auf Pinkto Zeichnnen gewechselt
                            paintMode = PaintMode.DrawPoints;
                        }
                    }
                }               
                if (hit.transform.gameObject.layer == 8  ) {
                    toolboxHitObject = hit.collider.gameObject;
                    //Laser wird sichtbar wenn wenn der Toolbox getroffen wird
                    RayInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
                } else
                {
                    //Laser wird unsichtbar wenn wenn der Toolbox nicht getroffen wird
                    toolboxHitObject = null;
                    RayInteractor.GetComponent<XRInteractorLineVisual>().enabled = false;
                }
            }
            

        }
        private void Delete()
        {
            // Es löscht die vom Pinselpunkt berühten Punkt
            //Delete Lines
            if (lines.Count > 0)
            {
                if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool SecondaryButtonValue) &&
                    SecondaryButtonValue)
                {
                    //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
                    Tutorial_bbutton.SetActive(false);

                    if (lineHitObject.name == "LineRenderer" || lineHitObject.name == "Start" || lineHitObject.name == "End")
                    {
                        //Der Eltern von Linerenderer der berührt wurde wird zerstört.
                        GameObject line = lineHitObject.transform.parent.gameObject;
                        Destroy(line);

                    }
                    else{
                        //Es macht den Kreis unsichtbar
                        GameObject line = lineHitObject.transform.gameObject;
                        line.SetActive(false);
                    }


                }
            }
        }
        private void Draw()
        {
            //Mit draw kann frei gezeichnet werden.
            
            //Draw  Lines that can snap to each Lines and Vertices from the lines
            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
            {
                //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
                Tutorial_trigger.SetActive(false);
                // if start pressing, trigger event
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPress.Invoke();
                    
                    //creating lineprefab
                    isDrawing = true;
                    //erzeuge ein LinePrefab GameObjekt und hole den Komponent Linie
                    currentLine = Instantiate(linePrefab).GetComponent<Line>();
                    
                    currentLine.tag = "free";

                    currentLine.draw_type = "free";
                    currentLine.width = 0.01f;
                    currentLine.transform.parent = linesParent; 
                    
                    //die Farbe der Zeichnung wird gewählt
                    var color = material.color;
                    material = new Material(material.shader);
                    material.color = color;    
                    currentLine.material = material;
                    
                    
                    currentLine.boxCollider.enabled = false;
                    currentLine.end.GetComponent<SphereCollider>().enabled = true;
                    currentLine.start.GetComponent<SphereCollider>().enabled = true;
                    
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
                //die Farbe der Zeichnung wird gewählt
                var color = material.color;
                material = new Material(material.shader);
                material.color = color;

                lines.Add(currentLine);
                
                //lognews.text = "Trigger is released\n";
            }
        }
        private void DrawLine()
        {
            //Zeichnung von geraden Linien

            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
            {
                //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
				Tutorial_trigger.SetActive(false);
				
                // if start pressing, trigger event
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPress.Invoke();
					
                    //die Farbe der Zeichnung wird gewählt
					var color = material.color;
                    material = new Material(material.shader);
                    material.color = color;    
                    
                    //creating lineprefab
                    currentLine = Instantiate(linePrefab).GetComponent<Line>();
                    currentLine.start.position = painterPosition.position;
                    currentLine.transform.parent = linesParent;
                    currentLine.draw_type = "straighline";
                    currentLine.tag = "CurrentLine";
                    currentLine.width = width;
					
					if (coloring==false)
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
                    
                }
                //The Button is pressed 
                else
                {
                    currentLine.end.position = painterPosition.position;
                    currentLine.boxCollider.enabled = false;
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

            }
        }

        

        private void DrawCircle()
        {

            //Zeichnung von Kreisen

            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
            {
                //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
                Tutorial_trigger.SetActive(false);
                // if start pressing, trigger event
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPress.Invoke();
                    
                    //Erzeugung von Gameobjekt mit Linerenderer
                    currentCircle = Instantiate(circlePrefab).GetComponent<LineRenderer>();

                    currentCircle.transform.position = painterPosition.position;
                    start = painterPosition.position;

                }
                //The Button is pressed 
                else
                {     
                    //wenn die Taste gedrückt wurde erzeugt es den Kreis
                    end = painterPosition.position;
                    radiuslength = (end - start).magnitude;

                    var segments = 360;
                    currentCircle.useWorldSpace = false;
                    currentCircle.startWidth = width;
                    currentCircle.endWidth = width;
                    currentCircle.positionCount = segments + 1;
                    currentCircle.material = material;

                    currentCircle.transform.parent = linesParent;
                    currentCircle.tag = "Line";

                    var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
                    var points = new Vector3[pointCount];


                    for (int i = 0; i < pointCount; i++)
                    {
                        var rad = Mathf.Deg2Rad * (i * 360f / segments);
                        points[i] = new Vector3(Mathf.Sin(rad) * radiuslength, 0, Mathf.Cos(rad) * radiuslength);
                    }

                    currentCircle.SetPositions(points);
                }
            }
 
            // check for button release
            else if (IsPressed)
            {
                IsPressed = false;
                OnRelease.Invoke();
  
                MeshCollider meshCollider = currentCircle.gameObject.GetComponent<MeshCollider>();
                Mesh mesh = new Mesh();
                currentCircle.BakeMesh(mesh, true);
                meshCollider.sharedMesh = mesh;
            }
        }
        
        int snap = 0;

        private void DrawPoints()
        {
            //Draw Points that can snap at every (10th | 10th | 10th) points. 

            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
            {
                //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
                Tutorial_trigger.SetActive(false);
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

                    //Die Berechnung zu schnappen der Punkte auf 10er Stellen
                    //auf der Koordinatensystem


                    if ((painter_position_x % 10 < 2 || 
                         painter_position_x % 10 > 8  ) && 
                        ( painter_position_y % 10 < 1 ||
                          painter_position_y % 10 > 8  )&& 
                         (painter_position_z % 10 < 1 ||
                         painter_position_z % 10 > 8)  
                         ){


                        if (snap == 0){

                            //die Punkte snappen sich sich wenn es der 10er Linie um 2 oder weniger nähert
                            var temp_x = (int)Math.Round(currentLine.start_position_x*100) ;

                            var x_p = (int)Math.Round(painter_position_x/10)*10;
                            var y_p = (int)Math.Round(painter_position_y/10)*10;
                            var z_p = (int)Math.Round(painter_position_z/10)*10;

                            var x =  x_p * 0.01f  + coord.transform.position.x;
                            var y =  y_p * 0.01f  + coord.transform.position.y + 0.07f;
                            var z =  z_p * 0.01f  + coord.transform.position.z + 0.03f;

                            pp = new Vector3(x , y, z);

                            currentLine.start.position =  pp;
                        }

                         snap = 1;
                    }else{
                        //ansonsten snappt es sich nicht
                        currentLine.start.position = painterPosition.position;

                        snap = 0;
                    }

                    currentLine.transform.parent = linesParent;
                    currentLine.draw_type = "points";

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
                
            }
        }

        public static Vector3 NearestPointOnLine(Vector3 start, Vector3 end, Vector3 pnt)
        {
            //Algorythmus zu snapen an der Linie
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
            //Farbwechsel mit Colorpicker
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