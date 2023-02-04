using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ProceduralVine : MonoBehaviour
{
    public Camera cam;
    public Transform vineOrigin;
    public Object[] vineEndings;
    Vector3 closestPoint;
    [Space]
    public float recycleInterval = 30;
    [Space]
    public int branches = 3;
    public int maxPointsForBranch = 100;
    public float segmentLength = .002f;
    public float maxSegmentRadius = 2f;
    public float branchRadiusPerSegment = 0.02f;
    public float minbranchRadius = 0.02f;
    [Space]
    public Material branchMaterial;
    LineRenderer lRender;
    float yOffsetWeight;

    float originDistance = 0;
    float minDistance = float.MaxValue;
    public Spline spline;
    GameObject building = null;

    int vineCount = 0;
    int buildingLayer = 7;
    int lm; 

    private void Start()
    {
        BezierKnot knot = new BezierKnot(vineOrigin.position);
        spline.Add(knot);
        lRender = GetComponent<LineRenderer>();
        yOffsetWeight = minbranchRadius * 4;
        lm = (1 << buildingLayer);
    }

    private void FixedUpdate()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            minDistance = float.MaxValue;
            foreach (BezierKnot knots in spline)
            {
                if (minDistance > Vector3.Distance(knots.Position, hit.point))
                {
                    minDistance = Vector3.Distance(new Vector3(knots.Position.x, knots.Position.y, knots.Position.z), hit.point);// + transform.position, hit.point);
                    closestPoint = new Vector3(knots.Position.x, knots.Position.y, knots.Position.z);// + transform.position;
                }
            }
            segmentLength = minDistance / maxPointsForBranch;
            lRender.SetPosition(0, closestPoint);
            lRender.SetPosition(1, closestPoint+(hit.point - closestPoint).normalized  * minDistance);
        }
        
    }

    void Update()
    {

        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    // call this method when you are ready to group your meshes
        //    //combineAndClear();
        //}

        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                building = null;
                RaycastHit hitsphere;
                if(Physics.SphereCast(ray,2f,out hitsphere,100f,lm))
                {
                    building = hitsphere.collider.gameObject;
                }

                createVine(hit);
            }
        }
    }

    Vector3 findTangentFromArbitraryNormal(Vector3 normal)
    {
        Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
        Vector3 t2 = Vector3.Cross(normal, Vector3.up);
        if (t1.magnitude > t2.magnitude)
        {
            return t1;
        }
        return t2;
    }

    public void createVine(RaycastHit hit)
    {
        //Search for closest point

        minDistance = float.MaxValue;
        foreach (BezierKnot knots in spline)
        {
            if(minDistance > Vector3.Distance(knots.Position, hit.point))
            {
                minDistance = Vector3.Distance(new Vector3(knots.Position.x, knots.Position.y, knots.Position.z), hit.point);
                closestPoint = new Vector3(knots.Position.x, knots.Position.y, knots.Position.z);
            }
        }

        Vector3 tangent = findTangentFromArbitraryNormal(hit.normal);
        GameObject Vine = new GameObject("Vine " + vineCount);
        Vine.transform.SetParent(transform);
        for (int i = 0; i < branches; i++)
        {
            //maxPointsForBranch = Mathf.Clamp(Mathf.CeilToInt(minDistance / segmentLength), 2, int.MaxValue);
            segmentLength = minDistance / maxPointsForBranch;
            Vector3 dir = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, hit.point - closestPoint, hit.normal) -90 + Random.Range(0,10/branches*i*1/maxPointsForBranch*10), hit.normal) * tangent;

            List<Vine> nodes = createBranch(maxPointsForBranch, closestPoint, hit.normal, dir, i);
            GameObject branch = new GameObject("Branch " + i);
            MeshGeneration b = branch.AddComponent<MeshGeneration>();
            VineGroth vg = branch.AddComponent<VineGroth>();
            b.init(nodes, minbranchRadius, branchRadiusPerSegment, maxSegmentRadius, branchMaterial);

            branch.transform.SetParent(Vine.transform);

            int pointOffset = nodes.Count - maxPointsForBranch; 

            for(int j= pointOffset+1; j< pointOffset + maxPointsForBranch; j++)
            {
                BezierKnot knot = new BezierKnot(nodes[j].getPosition());
                spline.Add(knot);
            }

            if (building !=null)
            if(Physics.OverlapSphere(nodes[nodes.Count - 1].getPosition(), 1f, lm) != null)
            {

                ResourceBuilding resourceBuilding;
                TowerBuilding towerBuilding;
                if(building.TryGetComponent<ResourceBuilding>(out resourceBuilding))
                {
                    resourceBuilding.EnableBuilding();
                }
                else if(building.TryGetComponent<TowerBuilding>(out towerBuilding))
                {
                    towerBuilding.EnableBuilding();
                }
            }
            Instantiate(vineEndings[Random.Range(0, vineEndings.Length)], nodes[nodes.Count - 1].getPosition() - Vector3.up * 0.08f, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Quaternion.AngleAxis(-90, Vector3.right));

            if (Vector3.Distance(hit.point, nodes[nodes.Count - 1].getPosition()) > 10)
            {
                for (int j = 0; j < branches; j++)
                {
                    minDistance = Vector3.Distance(hit.point, nodes[nodes.Count - 1].getPosition());
                    //maxPointsForBranch = Mathf.Clamp(Mathf.CeilToInt(minDistance / segmentLength), 2, int.MaxValue);
                    segmentLength = minDistance / maxPointsForBranch;
                    dir = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, hit.point - nodes[nodes.Count - 1].getPosition(), hit.normal) - 90 + Random.Range(0, 10 / branches * j * 1 / maxPointsForBranch * 10), hit.normal) * tangent;

                    nodes = createBranch(maxPointsForBranch, nodes[nodes.Count - 1].getPosition(), hit.normal, dir, j);
                    branch = new GameObject("Branch " + j);
                    b = branch.AddComponent<MeshGeneration>();
                    vg = branch.AddComponent<VineGroth>();
                    vg.secondryScale = 2;
                    b.init(nodes, minbranchRadius, branchRadiusPerSegment, maxSegmentRadius, branchMaterial);

                    branch.transform.SetParent(Vine.transform);

                    pointOffset = nodes.Count - maxPointsForBranch;

                    for (int k = pointOffset + 1; k < pointOffset + maxPointsForBranch; k++)
                    {
                        BezierKnot knot = new BezierKnot(nodes[k].getPosition());
                        spline.Add(knot);
                    }

                    if (building != null)


                    if (Physics.OverlapSphere(nodes[nodes.Count - 1].getPosition(),1f,lm) != null)
                    {
                        ResourceBuilding resourceBuilding;
                        TowerBuilding towerBuilding;
                        if (building.TryGetComponent<ResourceBuilding>(out resourceBuilding))
                        {
                            resourceBuilding.EnableBuilding();
                        }
                        else if (building.TryGetComponent<TowerBuilding>(out towerBuilding))
                        {
                            towerBuilding.EnableBuilding();
                        }
                    }
                }
                Instantiate(vineEndings[Random.Range(0, vineEndings.Length)], nodes[nodes.Count - 1].getPosition() - Vector3.up * 0.08f, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Quaternion.AngleAxis(-90, Vector3.right));
            }

            
            
        }

        vineCount++;
    }

    Vector3 calculateTangent(Vector3 p0, Vector3 p1, Vector3 normal)
    {
        var heading = p1 - p0;
        var distance = heading.magnitude;
        var direction = heading / distance;
        return Vector3.Cross(normal, direction).normalized;
    }

    Vector3 applyCorrection(Vector3 p, Vector3 normal)
    {
        return p + normal * 0.01f;
    }

    bool isOccluded(Vector3 from, Vector3 to)
    {
        Ray ray = new Ray(from, (to - from) / (to - from).magnitude);
        return Physics.Raycast(ray, (to - from).magnitude);
    }

    bool isOccluded(Vector3 from, Vector3 to, Vector3 normal)
    {
        return isOccluded(applyCorrection(from, normal), applyCorrection(to, normal));
    }

    Vector3 calculateMiddlePoint(Vector3 p0, Vector3 p1, Vector3 normal)
    {
        Vector3 middle = (p0 + p1) / 2;
        var h = p0 - p1;
        var distance = h.magnitude;
        var dir = h / distance;
        return middle + normal * distance;
    }

    List<Vine> createBranch(int count, Vector3 pos, Vector3 normal, Vector3 dir, int i)
    {
        int layer = 6;
        int layermask = ~(1 << layer);

        if (count == maxPointsForBranch)
        {
            Vine rootNode = new Vine(pos, normal);
            return new List<Vine> { rootNode }.join(createBranch(count - 1, pos, normal, dir, i));
        }
        else if (count < maxPointsForBranch && count > 0)
        {
            yOffsetWeight = (minbranchRadius+Mathf.Clamp(branchRadiusPerSegment*(count-i) * 4,0f, maxSegmentRadius));

            if (count % 2 == 0)
            {
                dir = Quaternion.AngleAxis(Random.Range(-5.0f / branches * i, 5.0f / branches * i), normal) * dir;
            }

            RaycastHit hit;
            Ray ray = new Ray(pos, normal);
            Vector3 p1 = pos + normal * segmentLength;

            if (Physics.Raycast(ray, out hit, segmentLength, layermask))
            {
                p1 = hit.point+new Vector3(0,Random.Range(0, yOffsetWeight),0);
            }
            ray = new Ray(p1, dir);

            if (Physics.Raycast(ray, out hit, segmentLength, layermask))
            {
                Vector3 p2 = hit.point + new Vector3(0, Random.Range(0, yOffsetWeight), 0);
                Vine p2Node = new Vine(p2, -dir);
                return new List<Vine> { p2Node }.join(createBranch(count - 1, p2, -dir, normal, i));
            }
            else
            {
                Vector3 p2 = p1 + dir * segmentLength;
                ray = new Ray(applyCorrection(p2, normal), -normal);
                if (Physics.Raycast(ray, out hit, segmentLength, layermask))
                {
                    Vector3 p3 = hit.point + new Vector3(0, Random.Range(0, yOffsetWeight), 0);
                    Vine p3Node = new Vine(p3, normal);

                    if (isOccluded(p3, pos, normal))
                    {
                        Vector3 middle = calculateMiddlePoint(p3, pos, (normal + dir) / 2);

                        Vector3 m0 = (pos + middle) / 2;
                        Vector3 m1 = (p3 + middle) / 2;

                        Vine m0Node = new Vine(m0, normal);
                        Vine m1Node = new Vine(m1, normal);

                        return new List<Vine> { m0Node, m1Node, p3Node }.join(createBranch(count - 3, p3, normal, dir, i));
                    }

                    return new List<Vine> { p3Node }.join(createBranch(count - 1, p3, normal, dir, i));
                }
                else
                {
                    Vector3 p3 = p2 - normal * segmentLength;
                    ray = new Ray(applyCorrection(p3, normal), -normal);

                    if (Physics.Raycast(ray, out hit, segmentLength, layermask))
                    {
                        Vector3 p4 = hit.point + new Vector3(0, Random.Range(0, yOffsetWeight), 0);
                        Vine p4Node = new Vine(p4, normal);

                        if (isOccluded(p4, pos, normal))
                        {
                            Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);
                            Vector3 m0 = (pos + middle) / 2;
                            Vector3 m1 = (p4 + middle) / 2;

                            Vine m0Node = new Vine(m0, normal);
                            Vine m1Node = new Vine(m1, normal);

                            return new List<Vine> { m0Node, m1Node, p4Node }.join(createBranch(count - 3, p4, normal, dir,i));
                        }

                        return new List<Vine> { p4Node }.join(createBranch(count - 1, p4, normal, dir,i));
                    }
                    else
                    {
                        Vector3 p4 = p3 - normal * segmentLength;
                        Vine p4Node = new Vine(p4, dir);

                        if (isOccluded(p4, pos, normal))
                        {
                            Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);

                            Vector3 m0 = (pos + middle) / 2;
                            Vector3 m1 = (p4 + middle) / 2;

                            Vine m0Node = new Vine(m0, dir);
                            Vine m1Node = new Vine(m1, dir);

                            return new List<Vine> { m0Node, m1Node, p4Node }.join(createBranch(count - 3, p4, dir, -normal,i));
                        }
                        return new List<Vine> { p4Node }.join(createBranch(count - 1, p4, dir, -normal,i));
                    }
                }
            }

        }
        return null;
    }

    //void combineAndClear()
    //{
    //    MeshManager.instance.combineAll();
    //    foreach (Transform t in transform)
    //    {
    //        Destroy(t.gameObject);
    //    }
    //}
}
