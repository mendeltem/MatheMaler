using System.Collections.Generic;
using System.Collections;
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
    [RequireComponent(typeof(MeshFilter))]
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
        public GameObject surfacePrefab;
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

        public MeshGenerator currentSurface;
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
        public Toggle toggle5;
        public Toggle toggle6;
        public GameObject Tutorial_trigger;
        public GameObject Tutorial_abutton;
        public GameObject Tutorial_bbutton;
        public Vector3 dir;
        public Vector3 pp;
        public Vector3 start;
        public Vector3 end;
        public Vector3 radius;

        public Vector3 factor1;
        public Vector3 factor2;
        
        public UnityEngine.Color temp_color;
        public GameObject line;
        public GameObject coord;
		private bool coloring;
        double painter_position_x; 
        double painter_position_y;
        double painter_position_z;

        
        float surface_with;
        float surface_height;

        //index for surface
        int index = 1;

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

            toggle2.isOn = true;


        }

        private void Update()
        {
            //löschen von Linien, Punkte und Kreis immer erreichbar
            Delete();
            //Laser wird nur sichtbar wenn auf Toolbox gezeigt wird
            RaycastToolbox();
            
            //Die Pinselpunkt Position im Verhältniss zu Koordinatensystem und die Umrechnung
            painter_position_x = ((painterPosition.position.x - coord.transform.position.x) * 100);
            painter_position_y = ((painterPosition.position.y - coord.transform.position.y - 0.07f) * 100f);
            painter_position_z = ((painterPosition.position.z - coord.transform.position.z - 0.03) * 100f);
            
            //Vektorangaben zum Pinselpunkt Position

            //if (isDrawing == false){
            x_vectorLabel.text = "X: "+painter_position_x.ToString("F0");
            y_vectorLabel.text = "Y: "+painter_position_z.ToString("F0");
            z_vectorLabel.text = "Z: "+painter_position_y.ToString("F0");

            //}

            //lognews.text =  ""+ (index).ToString();

            lognews.text = lineHitObject.name;

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

                    case PaintMode.Multiply:
                        //Punkte eintragen
                        Multiply();
                        break;  

                    case PaintMode.Surface:
                        //Punkte eintragen
                        Surface();
                        break;  

 
                    default:
                        break;
                }

            }
            //zum Debuggen 
            //lognews.text = lineHitObject.name;
            //Erzeugt ein Dummy Linie, dessen Farbe sich ändern sollte statt des vor kurzem erzeugten Linie
            //Es da ist um ein Bug zu bekämpfen
            if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton,
                out PrimaryButtonValue) && PrimaryButtonValue)
            {
                //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
                Tutorial_abutton.SetActive(false);
                //die Farbe der Zeichnung wird gewählt
				var color = material.color;
                material = new Material(material.shader);
                material.color = color;   
                index = 1;
        

                if (currentSurface != null){
                    currentSurface.point1.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point2.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point3.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point4.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point5.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point6.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point7.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point8.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point9.GetComponent<MeshRenderer>().enabled = false;
                    currentSurface.point10.GetComponent<MeshRenderer>().enabled = false;
                    //currentSurface = Instantiate(surfacePrefab).GetComponent<MeshGenerator>();
                    //currentSurface.material = material;

                    currentSurface.index = 1;
                    currentSurface = null;

                }


				coloring = true; 

            }  // Beim loslassen
            else if (IsAPressed)
            {
				coloring = false;
                IsAPressed = false;
                OnRelease.Invoke();
                
 
            }
        }


        private void Surface()
        {

            //Zeichnen von Ebenen
            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
            {

                //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
				Tutorial_trigger.SetActive(false);
				isDrawing = true;

                var color = material.color;
                material = new Material(material.shader);
                material.color = color;

                // if start pressing, trigger event
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPress.Invoke();

                    if(index == 1){
                        currentSurface = Instantiate(surfacePrefab).GetComponent<MeshGenerator>();
                        currentSurface.material = material;
                        currentSurface.index = 1;

                        

                    }
                
                    if(index == 2){
                        currentSurface.index = 2;
                    }
                    
                    if(index == 3){
                        var meshRenderer = currentSurface.GetComponent<MeshRenderer>();
                        meshRenderer.enabled = true;
                        currentSurface.index = 3;
                    }
                    if(index == 4){
                        currentSurface.index = 4;
                    }
                    if(index == 5){
                        currentSurface.index = 5;
                    }
                    if(index == 6){
                        currentSurface.index = 6;
                    }
                    if(index == 7){
                        currentSurface.index = 7;
                    }
                    if(index == 8){
                        currentSurface.index = 8;
                    }
                    if(index == 9){
                        currentSurface.index = 9;
                    }
                    if(index == 10){
                        currentSurface.index = 10;

                        currentSurface.point1.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point2.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point3.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point4.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point5.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point6.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point7.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point8.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point9.GetComponent<MeshRenderer>().enabled = false;
                        currentSurface.point10.GetComponent<MeshRenderer>().enabled = false;
                    }

                }else
                {
 
                    if(currentSurface.index == 1){
                        currentSurface.point1.position = painterPosition.position;

                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point1.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point1.position = cs.end.position;
                        }

                    }
                
                    if(currentSurface.index == 2){
                        currentSurface.point2.position = painterPosition.position;

                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point2.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point2.position = cs.end.position;
                        }


                    }
                    
                    if(currentSurface.index == 3){
                        currentSurface.point3.position = painterPosition.position;

                        
                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point3.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point3.position = cs.end.position;
                        }
                    }

                    if(currentSurface.index == 4){
                        currentSurface.point4.position = painterPosition.position;


                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point4.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point4.position = cs.end.position;
                        }
                    }
                    
                    if(currentSurface.index == 5){
                        currentSurface.point5.position = painterPosition.position;


                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point5.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point5.position = cs.end.position;
                        }
                    }
                    if(currentSurface.index == 6){
                        currentSurface.point6.position = painterPosition.position;


                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point6.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point6.position = cs.end.position;
                        }
                    }
                    if(currentSurface.index == 7){
                        currentSurface.point7.position = painterPosition.position;


                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point7.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point7.position = cs.end.position;
                        }
                    }
                    if(currentSurface.index == 8){
                        currentSurface.point8.position = painterPosition.position;


                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point8.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point8.position = cs.end.position;
                        }
                    }

                    if(currentSurface.index == 9){
                        currentSurface.point9.position = painterPosition.position;


                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point9.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point9.position = cs.end.position;
                        }
                    }



                    if(currentSurface.index == 10){
                        currentSurface.point10.position = painterPosition.position;


                        if (lineHitObject.name == "Start")
                        {
                            //Start Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point10.position = cs.start.position;
                        }

                        if (lineHitObject.name == "End")
                        {
                            //End Snap
                            GameObject go = lineHitObject.transform.parent.gameObject;
                                
                            var cs = go.GetComponent<Line>();
                            currentSurface.point10.position = cs.end.position;
                        }
                    }
                }
            }            // check for button release
            else if (IsPressed)
            {
                isDrawing = false;
                IsPressed = false;
                OnRelease.Invoke();

                if(currentSurface.index == 1){
                    index = 2;
                }
                if(currentSurface.index == 2){
                    index = 3;
                }

                if(currentSurface.index == 3) {
                    index = 4;
                }

                if(currentSurface.index == 4) {
                    index = 5;
                }                 
                if(currentSurface.index == 5){
                    index = 6;
                }
                if(currentSurface.index == 6){
                    index = 7;
                }

                if(currentSurface.index == 7) {
                    index = 8;
                }

                if(currentSurface.index == 8) {
                    index = 9;
                }  
                if(currentSurface.index == 9) {
                    index = 10;
                }

                if(currentSurface.index == 10) {
                    index = 1;
                }  
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
                            toggle5.isOn = false;
                            toggle6.isOn = false;

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
                            toggle5.isOn = false;
                            toggle6.isOn = false;

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
                            toggle5.isOn = false;
                            toggle6.isOn = false;
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
                            toggle5.isOn = false;
                            toggle6.isOn = false;
                            //Modi wird auf Pinkto Zeichnnen gewechselt
                            paintMode = PaintMode.DrawPoints;
                        }
                    }
                } 
                if (hit.transform.gameObject.name == "Checkmark5")
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
                            toggle4.isOn = false;
                            toggle5.isOn = true;
                            toggle6.isOn = false;
                            //Modi wird auf Pinkto Zeichnnen gewechselt
                            paintMode = PaintMode.Multiply;
                        }
                    }
                }     
                if (hit.transform.gameObject.name == "Checkmark6")
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
                            toggle4.isOn = false;
                            toggle5.isOn = false;
                            toggle6.isOn = true;
                            //Modi wird auf Pinkto Zeichnnen gewechselt
                            paintMode = PaintMode.Surface;
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
            //if (lines.Count > 0)
            
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


        private void Draw()
        {
            //Mit draw kann frei gezeichnet werden.
            
            //Draw  Lines that can snap to each Lines and Vertices from the lines
            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                    out TriggerButtonValue) && TriggerButtonValue)
            {
                //Hilfestellung auf der Rechte Steuerung wird unsichtbar wenn mann die Tasten benutzt
                Tutorial_trigger.SetActive(false);
                isDrawing = true;
                // if start pressing, trigger event
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPress.Invoke();
                    
                    //creating lineprefab
                    //erzeuge ein LinePrefab GameObjekt und hole den Komponent Linie
                    currentLine = Instantiate(linePrefab).GetComponent<Line>();
                    
                    currentLine.tag = "free";

                    currentLine.draw_type = "free";
                    currentLine.width = 0.01f;
                    currentLine.transform.parent = linesParent; 
                    
                    //die Farbe der Zeichnung wird gewählt
                    //var color = material.color;
                    //material = new Material(material.shader);
                    //material.color = color;    
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
                //var color = material.color;
                //material = new Material(material.shader);
                //material.color = color;

                //lines.Add(currentLine);
                
                //lognews.text = "Trigger is released\n";



                                //die Farbe der Zeichnung wird gewählt
                var color = material.color;
                material = new Material(material.shader);
                material.color = color;
                /*
                
                //creating lineprefab
                currentLine = Instantiate(linePrefab).GetComponent<Line>();
                currentLine.draw_type = "straighline";

                currentLine.width = width;
                
                if (coloring==false)
                    currentLine.material = material;
                
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

                */
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
				isDrawing = true;
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
					
					//if (coloring==false)
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
                    currentLine.LineVector_X.transform.position = (currentLine.start.position + currentLine.end.position) / 2 + new Vector3(-0.015f, 0.01f, 0.0f);
                    currentLine.LineVector_Y.transform.position = (currentLine.start.position + currentLine.end.position) / 2 + new Vector3(0.00f, 0.01f, 0.0f);
                    currentLine.LineVector_Z.transform.position = (currentLine.start.position + currentLine.end.position) / 2 + new Vector3(0.015f, 0.01f, 0.0f);

                    var vector_x =  currentLine.start_position_x - currentLine.end_position_x;
                    var vector_y =  currentLine.start_position_y - currentLine.end_position_y;
                    var vector_z =  currentLine.start_position_z - currentLine.end_position_z;


                    currentLine.LineVector_X.text = ""+ (-vector_x*100f).ToString("F0");
                    currentLine.LineVector_Z.text = ""+ (-vector_y*100f).ToString("F0");
                    currentLine.LineVector_Y.text = ""+ (-vector_z*100f).ToString("F0");
                }
            }
 
            // check for button release
            else if (IsPressed)
            {
                isDrawing = false;
                IsPressed = false;
                OnRelease.Invoke();
                currentLine.boxCollider.enabled = true;
                currentLine.end.GetComponent<SphereCollider>().enabled = true;
                currentLine.start.GetComponent<SphereCollider>().enabled = true;
                currentLine.tag = "Line";
                //lines.Add(currentLine);

                //die Farbe der Zeichnung wird gewählt
                var color = material.color;
                material = new Material(material.shader);
                material.color = color;

                /*
                
                //creating lineprefab
                currentLine = Instantiate(linePrefab).GetComponent<Line>();
                currentLine.draw_type = "straighline";

                currentLine.width = width;
                
                if (coloring==false)
                    currentLine.material = material;
                
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

                //lines.Add(currentLine);

                */
                //

            }
        }

        bool multi = false;

        private void Multiply()
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
                    
                    IsPressed = true;
                    OnPress.Invoke();

                    if (lineHitObject.name == "LineRenderer")
                    {
                    Line line = lineHitObject.transform.parent.GetComponent<Line>();


                    var vector_x =  line.start_position_x - line.end_position_x;
                    var vector_y =  line.start_position_y - line.end_position_y;
                    var vector_z =  line.start_position_z - line.end_position_z;


                    factor1 = new Vector3(-vector_x, -vector_y, -vector_z);

                    lognews.text =  ""+ (factor1*100f).ToString("F0");

                    multi = true;
                    }
                    
                }
                //The Button is pressed 
                else
                {
                    currentLine.transform.parent = linesParent;
                    //currentLine.draw_type = "points";
                }
            }
 
            // check for button release
            else if (IsPressed)
            {
                IsPressed = false;
                OnRelease.Invoke();

                if (lineHitObject.name == "LineRenderer" && multi == true )
                {
                Line line = lineHitObject.transform.parent.GetComponent<Line>();
                var vector_x =  line.start_position_x - line.end_position_x;
                var vector_y =  line.start_position_y - line.end_position_y;
                var vector_z =  line.start_position_z - line.end_position_z;

                factor2 = new Vector3(-vector_x, -vector_y, -vector_z);


                //Vector3 crossProduct  = Vector3.Cross(factor1, factor2);

                Vector3 crossProduct  = factor1 + factor2;

                Vector3 crossProduct_new = new Vector3(crossProduct.x*100, crossProduct.y*100, crossProduct.z*100);

                currentLine = Instantiate(linePrefab).GetComponent<Line>();
                currentLine.transform.parent = linesParent;
                currentLine.draw_type = "straighline";
                currentLine.width = width;
                currentLine.material = material;

                currentLine.start.position = line.start.position;
                currentLine.end.position = currentLine.start.position + crossProduct;
                currentLine.boxCollider.enabled = true;
                currentLine.tag = "Line";

                currentLine.LineVector_X.transform.position = (currentLine.start.position + currentLine.end.position) / 2 + new Vector3(-0.015f, 0.01f, 0.0f);
                currentLine.LineVector_Y.transform.position = (currentLine.start.position + currentLine.end.position) / 2 + new Vector3(0.00f, 0.01f, 0.0f);
                currentLine.LineVector_Z.transform.position = (currentLine.start.position + currentLine.end.position) / 2 + new Vector3(0.015f, 0.01f, 0.0f);


                var n_vector_x =  currentLine.start.position.x - currentLine.end.position.x;
                var n_vector_y =  currentLine.start.position.y - currentLine.end.position.y;
                var n_vector_z =  currentLine.start.position.z - currentLine.end.position.z;

                lognews.text =  ""+ ((currentLine.end.position-currentLine.start.position)*100).ToString("F0");


                currentLine.LineVector_X.text = ""+ (-n_vector_x*100f).ToString("F0");
                currentLine.LineVector_Z.text = ""+ (-n_vector_y*100f).ToString("F0");
                currentLine.LineVector_Y.text = ""+ (-n_vector_z*100f).ToString("F0");
                }
                
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
                isDrawing = true;
                // if start pressing, trigger event
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPress.Invoke();
                    
                    //Erzeugung von Gameobjekt mit Linerenderer
                    currentCircle = Instantiate(circlePrefab).GetComponent<LineRenderer>();

                    currentLine = currentCircle.GetComponent<Line>();

                    currentCircle.transform.position = painterPosition.position;


                    currentLine.start.position = painterPosition.position;
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

                    float deg = 90f;
                    
                    dir =  start - end;

                    float ll  =  radiuslength * 10000;

                    lognews.text = "b: "  + radiuslength + "\n  a: " + ll;
                    for (int i = 0; i < pointCount; i++)
                    {
                        var rad = Mathf.Deg2Rad * (i * 360f / segments) ;

                        points[i] = new Vector3(Mathf.Sin(rad) * radiuslength, 0, Mathf.Cos(rad) * radiuslength);
                    }
                    //MeshCollider meshCollider = currentCircle.gameObject.GetComponent<MeshCollider>();
                    //Mesh mesh = new Mesh();

                    //meshCollider.transform.rotation  = currentCircle.transform.rotation;
                    //currentCircle.BakeMesh(mesh, true);
                    //meshCollider.sharedMesh = mesh;
                    //meshCollider.transform.rotation  = painterPosition.transform.rotation;
                    
 
                    currentCircle.SetPositions(points);
                    currentCircle.transform.rotation  = painterPosition.transform.rotation;
                    
                    x_vectorLabel.text =  "";
                    y_vectorLabel.text = "Radius Length: "+(radiuslength*100).ToString("F0")+" cm";
                    z_vectorLabel.text = "";

                    
                }
            }
 
            // check for button release
            else if (IsPressed)
            {
                isDrawing = false;
                IsPressed = false;
                OnRelease.Invoke();

                //MeshCollider meshCollider = currentCircle.gameObject.AddComponent<MeshCollider>();

                //Mesh mesh = new Mesh();
                //currentCircle.BakeMesh(mesh, true);
                //meshCollider.sharedMesh = mesh;


                var color = material.color;
                material = new Material(material.shader);
                material.color = color;
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
                //lines.Add(currentLine);
                
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