using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Obj_MainPlayer : Obj
{
    public NavMeshAgent nav;
    public Vector3 targetPos;

    public Obj_MainPlayer(GameObject go)
    {
        Go = go;
        nav = go.GetComponent<NavMeshAgent>();
        if (nav==null)
        {
            nav = go.AddComponent<NavMeshAgent>();
        }
    }

 


    public bool AutoMove(Vector3 pos)
    {
        Ray ray = new Ray(pos,Vector3.down);
        if (Physics.Raycast(ray,out RaycastHit info))
        {
            if (nav.CalculatePath(info.point, nav.path))
            {
                nav.SetDestination(info.point);
                return true;
            }
            else
            {
                Debug.Log("无法到达" + info.point);
            }
        }
        return false;
    }

    public void StopMove()
    {
        nav.SetDestination(Go.transform.position);
        nav.isStopped = true;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    public void Update()
    {
        if (Go==null)
        {
            return;
        }
        if (targetPos!=Vector3.zero)
        {
            if (Vector3.Distance(targetPos,Go.transform.position)<0.2f)
            {
                nav.isStopped = true;
                targetPos = Vector3.zero;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            Go.transform.position += Go.transform.forward * 5 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Go.transform.Rotate(Vector3.up * 40 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Go.transform.Rotate(Vector3.up * -40 * Time.deltaTime);
        }
    }
}
