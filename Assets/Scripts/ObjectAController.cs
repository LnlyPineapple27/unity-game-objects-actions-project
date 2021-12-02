using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Student name: Phan Tan Dat
//Student id: 18127078

public class ObjectAController : MonoBehaviour
{
    Transform mover;

    // Rotation control
    bool keepRotating = false;
    float rotateSpeed = 320.0f;

    // Moving in Y-axis between range -10 -> +10
    static int MSSV = 18127078;
    float move_speed = (2 + (MSSV % 10)),
        y_bound0 = 1f,
        y_bound1 = 12f;
    bool moveDirectionUp = true,
        keepMoving = false;


    // Change color on mouse hover controller
    Color onMouseHover = Color.cyan, 
        offMouseHover;
    MeshRenderer Mrenderer;

    // Spawn Objects B
    public GameObject spawnee;
    List<GameObject> objB_list = new List<GameObject>();
    int newObjBNum = 5;

    // Floating Obj B then Rotate
    float float_bound = 15f;
    bool isFloating = false;

    // Calculate Point for Obj A
    static int points = 0;
    private void OnMouseOver()
    {
        Mrenderer.material.color = onMouseHover;
    }
    private void OnMouseExit()
    {
        Mrenderer.material.color = offMouseHover;
    }
    IEnumerator ClearListAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        while (objB_list.Count != 0)
        {
            int index = Random.Range(0, objB_list.Count);
            yield return new WaitForSeconds(0.3f);
            Destroy(objB_list[index]);
            objB_list.RemoveAt(index);
        }
    }
    void Start()
    {
        mover = gameObject.GetComponent<Transform>();
        int k = Random.Range(10, 20);
        Debug.Log("Random pos of obj A: " + k.ToString());
        mover.position = new Vector3(0, k, 0);

        Mrenderer = GetComponent<MeshRenderer>();
        offMouseHover = Mrenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (keepRotating) { 
            mover.Rotate(0, rotateSpeed * Time.deltaTime, 0); 
        }//rotates <rotateSpeed> degrees per second around y axis

        if (keepMoving)
        {
            if (moveDirectionUp)
            {
                mover.Translate(Vector3.up * move_speed * Time.deltaTime);
            }
            else
            {
                mover.Translate(-Vector3.up * move_speed * Time.deltaTime);
            }
            if (mover.position.y >= y_bound1)
                moveDirectionUp = false;
            if (mover.position.y <= y_bound0)
                moveDirectionUp = true;
        }

        if (isFloating)
        {
            for (int i = 0; i < objB_list.Count; i++)
            {
                Transform t = objB_list[i].GetComponent<Transform>();
                if (t.position.y < float_bound)
                    t.Translate(Vector3.up * Time.deltaTime * 10);
                else
                    t.Rotate(0, rotateSpeed * Time.deltaTime * 2, rotateSpeed * Time.deltaTime * 2);
            }
        }

        // Keyboard handler
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            mover.Translate(Vector3.up * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            mover.Translate(Vector3.down * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            mover.Translate(Vector3.left * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            mover.Translate(Vector3.right * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            keepRotating = !keepRotating;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            keepMoving = !keepMoving;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < newObjBNum; i++)
            {
                GameObject p = GameObject.Instantiate(spawnee,
                                new Vector3(Random.Range(-4, 4), Random.Range(10, 10 + MSSV % 10), Random.Range(-5, 5)),
                                Quaternion.identity);
                p.GetComponent<Rigidbody>().AddForce(Random.Range(-10, 10), Random.Range(-10, 0), Random.Range(-10, 10));
                objB_list.Add(p);
            }  
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ClearListAfterTime(2f));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            for(int i = 0; i < objB_list.Count; i++)
            {
                Renderer r = objB_list[i].GetComponent<Renderer>();
                Color c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                r.material.SetColor("_Color", c);
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            isFloating = !isFloating;
            if (!isFloating)
            {// if not floating then gravity should be returned
                for (int i = 0; i < objB_list.Count; i++)
                {
                    Rigidbody p = objB_list[i].GetComponent<Rigidbody>();
                    p.useGravity = true;
                    p.AddForce(Random.Range(-10, 10), Random.Range(-10, 0), Random.Range(-10, 10));
                }
            }
            else
            {
                for (int i = 0; i < objB_list.Count; i++)
                {
                    Rigidbody p = objB_list[i].GetComponent<Rigidbody>();
                    p.useGravity = false;
                    Transform t = objB_list[i].GetComponent<Transform>();
                    //float x = t.position.x,
                    //    z = t.position.z;
                    //t.position = new Vector3(x, 0, z);
                    t.rotation = Quaternion.identity;
                    p.velocity = Vector3.zero;
                    p.angularVelocity = Vector3.zero;
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                for (int i = 0; i < objB_list.Count; i++) {
                    if (GameObject.ReferenceEquals(hit.collider.gameObject, objB_list[i]))
                    {
                        Destroy(objB_list[i]);
                        objB_list.RemoveAt(i);
                        points += 1;
                    }
                }
                
            }
        }
    }
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Points: " + points, style);
    }
}
