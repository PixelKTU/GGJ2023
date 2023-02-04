using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Splines;

public class Scorpion : Enemy
{

    Camera cam;

    protected override void Start()
    {
        base.Start();
        cam = Camera.main;
    }

    protected override void Update()
    {
        base.Update();

        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (Physics.Raycast(ray, out hit, 100))
        //    {
        //        agent.SetDestination(hit.point);
        //    }
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    agent.SetDestination(MainTree.Instance.GetPosition());
        //}
    }
}
