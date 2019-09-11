using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System;

[System.Serializable]
public class Line
{
    [SerializeField]
    public List<Point> points ;
    [SerializeField]
    public Color color;
    [SerializeField]
    public int penWidth;
    [SerializeField]
    public bool isAnim;

    public Line(Color color, int penWidth, bool isAnim)
    {
        points = new List<Point>();
        this.color = color;
        this.penWidth = penWidth;
        this.isAnim = isAnim;
    }
}

[System.Serializable]
public class Point
{
    [SerializeField]
    public Vector2 coordinate;
    [SerializeField]
    public float timeStamp;

    public Point(Vector2 coordin, float timeStamp)
    {
        this.coordinate = coordin;
        this.timeStamp = timeStamp;
    }
}
namespace FreeDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]

    public class Drawable : MonoBehaviour
    {
        //public variables
        public Drawable(object messageBox) 
        {
            this.MessageBox = messageBox;
               
        }
                public object MessageBox { get; private set; }
        public GameObject layerButton;
        public static Color Pen_Colour = Color.red;  // Change these to change the default drawing settings
        public static int Pen_Width = 1;
        public Brush_Function current_brush;
        public delegate void Brush_Function(Vector2 screenPoint, Vector2 world_position, List<Point> list);
        public LayerMask Drawing_Layers;
        public bool Reset_Canvas_On_Play = true;
        public Color Reset_Colour = new Color(0, 0, 0, 0);  //By default, reset the canvas to be transparent

        //lists tracking lines
        private List<Line> trackingChanges;
        private Dictionary<GameObject, Line> trackingAnim;
        private Sprite spritePrefab;

        // MUST HAVE READ/WRITE enabled set in the file editor of Unity
        Sprite drawable_sprite;
        Texture2D drawable_texture;
        Line currLine;
        Vector2 previous_drag_position;
        Color[] clean_colours_array;
        Color transparent;
        Color32[] cur_colors;
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;
        GameObject currLayer;
        bool isAnimation;
        private float referenceTime;
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////Awake
        void Awake()
        {
            trackingChanges = new List<Line>();
            trackingAnim = new Dictionary<GameObject, Line>();
            dbSceneUpload = new DBScene();
            dbSceneSave = new DBScene();
            // DEFAULT BRUSH SET HERE
            current_brush = PenBrush;
            //setsprite
            if (DrawingSettings.editingImage == null)
            {
                this.GetComponent<SpriteRenderer>().sprite = GameObject.Find("DrawingSettings").GetComponent<DrawingSettings>().spritePrefabs[this.transform.GetSiblingIndex()];
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = DrawingSettings.editingImage;
            }
            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
            drawable_texture = drawable_sprite.texture;

            if (DrawingSettings.editingImage == null)
            {
                // Initialize clean pixels to use
                clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];

                for (int x = 0; x < clean_colours_array.Length; x++)
                {
                    clean_colours_array[x] = Reset_Colour;
                }

                // Should we reset our canvas image when we hit play in the editor?
                if (Reset_Canvas_On_Play)
                {
                    ResetCanvas();
                }
                //initially try to link button with this layer
                this.GetComponent<SpriteRenderer>().sortingOrder = this.transform.GetSiblingIndex();
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////update
        void Update()
        {
            // Is the user holding down the left mouse button?
            bool mouse_held_down = Input.GetMouseButton(0);
            if (mouse_held_down && !no_drawing_on_current_drag&&transform.parent.GetComponentInParent<Canvas>().enabled)
            {

                // Convert mouse coordinates to world coordinates
                Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Check if the current mouse position overlaps our image
                Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                if (hit != null && hit.transform != null)
                {
                    // We're over the texture we're drawing on!
                    // Use whatever function the current brush is
                    if (!mouse_was_previously_held_down)
                    {
                        //yifan
                        if (!isAnimation)
                        {
                            Line list = new Line(Pen_Colour, Pen_Width, isAnimation);
                            trackingChanges.Add(list);
                            currLine = list;
                        }
                        else
                        {
                            Line list = new Line(Pen_Colour, Pen_Width, isAnimation);
                            if (raycast() != null)
                            {
                                trackingAnim.Add(raycast(), list);
                            }
                            else
                            {
                                Debug.Log("nothing selected");
                            }
                            currLine = list;
                        }

                    }
                    current_brush(Input.mousePosition, mouse_world_position, currLine.points);
                }

                else
                {
                    // We're not over our destination texture
                    previous_drag_position = Vector2.zero;
                    if (!mouse_was_previously_held_down)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        no_drawing_on_current_drag = true;
                    }
                }
            }
            // Mouse is released
            else if (!mouse_held_down)
            {
                previous_drag_position = Vector2.zero;
                no_drawing_on_current_drag = false;
            }
            mouse_was_previously_held_down = mouse_held_down;
            //display?
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////penBrush
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void PenBrush(Vector2 screenPoint, Vector2 world_point, List<Point> list)
        {
            Vector2 pixel_pos = WorldToPixelCoordinates(world_point);

            cur_colors = drawable_texture.GetPixels32();

            if (previous_drag_position == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
                referenceTime = Time.time;
            }
            else
            {
                // Colour in a line from where we were on the last update call
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ApplyMarkedPixelChanges();

            //add this current point to list.
            previous_drag_position = pixel_pos;
            Point newPoint;
            if (!isAnimation)
            {
                newPoint = new Point(pixel_pos, Time.time - referenceTime);
            }
            else
            {
                newPoint = new Point(screenPoint, Time.time - referenceTime);
            }
            list.Add(newPoint);
        }
        public void SetPenBrush(bool isAnimation)
        {
            this.isAnimation = isAnimation;
            current_brush = PenBrush;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////repaint
        IEnumerator rePaintHelper(float time)
        {
            for (int i = 0; i < trackingChanges.Count; i++)
            {
                for (int j = 0; j < trackingChanges[i].points.Count - 1; j++)
                {
                    ColourBetween(trackingChanges[i].points[j].coordinate, trackingChanges[i].points[j + 1].coordinate, trackingChanges[i].penWidth, Color.green);
                    yield return new WaitForSeconds(time);
                    ApplyMarkedPixelChanges();
                }
            }

        }
        public void rePaint()
        {
            StartCoroutine(rePaintHelper(0.02f));
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////withDraw
        public void withDraw()
        {
            if (trackingChanges.Count != 0)
            {
                for (int j = 0; j < trackingChanges[trackingChanges.Count - 1].points.Count - 1; j++)
                {
                    ColourBetween(trackingChanges[trackingChanges.Count - 1].points[j].coordinate, trackingChanges[trackingChanges.Count - 1].points[j + 1].coordinate, trackingChanges[trackingChanges.Count - 1].penWidth, Reset_Colour);
                }
                ApplyMarkedPixelChanges();
                trackingChanges.Remove(trackingChanges[trackingChanges.Count - 1]);
                for (int i = 0; i < trackingChanges.Count; i++)
                {
                    for (int j = 0; j < trackingChanges[i].points.Count - 1; j++)
                    {
                        ColourBetween(trackingChanges[i].points[j].coordinate, trackingChanges[i].points[j + 1].coordinate, trackingChanges[i].penWidth, trackingChanges[i].color);
                    }
                }
                ApplyMarkedPixelChanges();
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        GameObject raycast()
        {
            GraphicRaycaster gr = GameObject.Find("PaintUI").GetComponent<GraphicRaycaster>();
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);
            foreach (RaycastResult obj in results)
            {
                if (obj.gameObject.tag.Equals("component")) { return obj.gameObject; }
            }
            return null;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////colourBetween
        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(start_point, end_point);
            Vector2 direction = (start_point - end_point).normalized;

            Vector2 cur_position = start_point;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, width, color);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////markPixeltoColour
        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int)drawable_sprite.rect.width || x < 0)
                    continue;

                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    MarkPixelToChange(x, y, color_of_pen);
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////markPixelToChange
        public void MarkPixelToChange(int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int)drawable_sprite.rect.width + x;

            // Check if this is a valid position
            if (array_pos > cur_colors.Length || array_pos < 0)
                return;

            cur_colors[array_pos] = color;
        }
        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }
        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////worldToPixelCoordinates
        public Vector2 WorldToPixelCoordinates(Vector2 world_position)
        {
            // Change coordinates to local coordinates of this image
            Vector3 local_pos = transform.InverseTransformPoint(world_position);

            // Change these to coordinates of pixels
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

            // Need to center our coordinates
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            return pixel_pos;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////resetCanvas
        // Changes every pixel to be the reset colour
        public void ResetCanvas()
        {
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////save
        public void save()
        {
            if (PaintManager.isNamed == true)
            {
                // Encode texture into PNG
                byte[] bytes = drawable_texture.EncodeToPNG();

                // For testing purposes, also write to a file in the project folder
                if (bytes != null)
                {
                    string mainPngPath = Application.dataPath + "/SavedPics/" + PaintManager.paintName + ".png";
                    File.WriteAllBytes(mainPngPath, bytes);
                    dbSceneSave.mainPngAddr = mainPngPath;
                }
                else
                {
                    Debug.Log("Wrong");
                }
                //declare an array to store addr of png components;
                string[] subPngAddrs = new string[this.GetComponentsInChildren<Image>().Length];
                for(int i=0; i< this.GetComponentsInChildren<Image>().Length;i++)
                {
                    Image image = this.GetComponentsInChildren<Image>()[i];
                    bytes = image.sprite.texture.EncodeToPNG();
                    if (bytes != null)
                    {
                        string subPngPath = Application.dataPath + "/SavedPics/" + PaintManager.paintName + i + ".png";
                        File.WriteAllBytes(subPngPath, bytes);
                        subPngAddrs[i] = subPngPath;
                    }
                    else
                    {
                        Debug.Log("Wrong");
                    }
                }
                dbSceneSave.pngAddress = subPngAddrs;
            }
            else
            {
                //Do something
            }
            ConvertToJson(dbSceneSave);
                        //write to json file
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/SceneJson/" + PaintManager.paintName + ".json", JsonUtility.ToJson(dbSceneSave));
            //SlideShowJson
        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////upload
        DBScene dbSceneUpload;
        //upload img
        public void uploadImg(Texture2D img, string imgName)
        {
            //upload img
            if (PaintManager.isNamed == true)
            {
                //Encode texture into PNG
                byte[] bytesImg = img.EncodeToPNG();
                Debug.Log( BitConverter.ToString(bytesImg));
                WWWForm formImg = new WWWForm();
                formImg.AddField("Name", imgName);
                formImg.AddBinaryData("post", bytesImg);
                WWW wwwImg = new WWW("http://18.191.23.16/imageServer/UnityUpload.php", formImg);
                while (!wwwImg.isDone) { }
                Debug.Log(wwwImg.error);
            }
        }

        //upload json
        public void uploadDBJson(DBScene db, string name){
            //upload json
            ConvertToJson(db);
            string contents = "";
            string urlJson = "http://18.191.23.16/SceneJsonServer/UnityUpload.php";
            contents = JsonUtility.ToJson(db);
            byte[] bytes = Encoding.ASCII.GetBytes(contents);
            WWWForm form = new WWWForm();
            form.AddField("Name", name);
            form.AddBinaryData("post", bytes);
            WWW www = new WWW(urlJson, form);
        }

        public void uploadHelper(){
            //mainImg
            if(insertNewImg(PaintManager.paintName)){
                uploadImg(drawable_texture, PaintManager.paintName);
            }
            dbSceneUpload.mainPngAddr = "http://18.191.23.16/imageServer/uploadImages/" + PaintManager.paintName + ".png";

            //subPngs
            string[] subPngAddrs = new string[this.GetComponentsInChildren<Image>().Length];
            for(int i=0; i<this.GetComponentsInChildren<Image>().Length;i++){
                Image img = this.GetComponentsInChildren<Image>()[i];
                if(insertNewImg(PaintManager.paintName + i)){
                    uploadImg(img.GetComponent<Image>().sprite.texture, PaintManager.paintName + i);
                }
                subPngAddrs[i] = "http://18.191.23.16/imageServer/uploadImages/" + PaintManager.paintName + i + ".png";
            }
            dbSceneUpload.pngAddress = subPngAddrs;
            //upload json
            if(insertNewJson(PaintManager.paintName)){
                uploadDBJson(dbSceneUpload, PaintManager.paintName);
            }
        }

        //insert new img name to database and see if there's naming conventions
        public bool insertNewImg(string filename)
        {
            WWWForm form = new WWWForm();
            form.AddField("fileNamePost", filename);
            WWW www = new WWW("http://18.191.23.16/imageServer/InsertData.php", form);
            if (www.error != null)
            {
                Debug.Log(www.error);
                return false;
            }

            else
            {
                while (!www.isDone)
                {
                }
                if (www.text != "exist")
                {
                    Debug.Log("can upload it now");
                    return true;
                }
                else
                {
                    Debug.Log("rename it" + www.text);
                    return false;
                }
            }
        }

        public bool insertNewJson(string filename)
        {
            WWWForm form = new WWWForm();
            form.AddField("fileNamePost", filename);
            WWW www = new WWW("http://18.191.23.16/SceneJsonServer/InsertData.php", form);
            if (www.error != null)
            {
                Debug.Log(www.error);
                return false;
            }

            else
            {
                while (!www.isDone)
                {
                }
                if (www.text != "exist")
                {
                    Debug.Log("can upload it now");
                    return true;
                }
                else
                {
                    Debug.Log("rename it" + www.text);
                    return false;
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////display
        private bool start;
        float refTime;
        public void display()
        {
            StartCoroutine(displayHelper());
        }
        IEnumerator displayHelper()
        {
            //clean each anim line    
            foreach (GameObject obj in trackingAnim.Keys)
            {
                Line currLine = trackingAnim[obj];
                for (int j = 0; j < currLine.points.Count - 1; j++)
                {
                    ColourBetween(WorldToPixelCoordinates(Camera.main.ScreenToWorldPoint(currLine.points[j].coordinate)), WorldToPixelCoordinates(Camera.main.ScreenToWorldPoint(currLine.points[j + 1].coordinate)), currLine.penWidth, Color.clear);
                    ApplyMarkedPixelChanges();
                }
            }
            for (int i = 0; i < trackingChanges.Count; i++)
            {
                for (int j = 0; j < trackingChanges[i].points.Count - 1; j++)
                {
                    ColourBetween(trackingChanges[i].points[j].coordinate, trackingChanges[i].points[j + 1].coordinate, trackingChanges[i].penWidth, trackingChanges[i].color);
                }
            }
            ApplyMarkedPixelChanges();
            foreach (GameObject obj in trackingAnim.Keys)
            {

                //take reference time
                refTime = Time.realtimeSinceStartup;
                for (int i = 0; i < trackingAnim[obj].points.Count; i++)
                {
                    float timeElapsed = Time.realtimeSinceStartup - refTime;
                    if (trackingAnim[obj].points[i].timeStamp < timeElapsed)
                    {
                        move(obj, trackingAnim[obj].points[i].coordinate);
                        yield return null;
                    }
                    else
                    {
                        i--;
                    }
                }
            }
        }
        //move point to next place
        void move(GameObject obj, Vector2 position)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(obj.transform.position, position);
            Vector2 direction = ((Vector2)obj.transform.position - position).normalized;
            Vector2 cur_position = obj.transform.position;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(obj.transform.position, position, lerp);
                obj.transform.position = cur_position;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public DBScene dbSceneSave;
        static int counter = 0;
        void ConvertToJson(DBScene dbScene)
        {
            //assign json            
            dbScene.lines = new line[trackingAnim.Count];
            int lineNum = 0;
            foreach(Line rline in trackingAnim.Values)
            {
                line jsonLine = new line();
                jsonLine.points = new point[rline.points.Count];
                for(int j=0; j<rline.points.Count; j++)
                {
                    point jsonP = new point();
                    jsonP.x = rline.points[j].coordinate.x;
                    jsonP.y = rline.points[j].coordinate.y;
                    jsonP.timeStamp = rline.points[j].timeStamp;
                    jsonLine.points[j] = jsonP;
                }
                jsonLine.penWidth = rline.penWidth;
                jsonLine.isAnim = rline.isAnim;
                dbScene.lines[lineNum] = jsonLine;
                lineNum++;
            }
        }
    }
}