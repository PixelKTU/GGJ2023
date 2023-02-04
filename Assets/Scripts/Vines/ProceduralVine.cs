using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ProceduralVine : MonoBehaviour
{
    public Camera cam;
    public Transform vineOrigin;
    Vector3 closestPoint;
    [Space]
    public float recycleInterval = 30;
    [Space]
    public int branches = 3;
    public int maxPointsForBranch = 20;
    public float segmentLength = .002f;
    public float branchRadius = 0.02f;
    [Space]
    public Material branchMaterial;


    float originDistance = 0;
    float minDistance = float.MaxValue;
    public Spline spline;

    int vineCount = 0;

    private void Start()
    {
        BezierKnot knot = new BezierKnot(vineOrigin.position);
        spline.Add(knot);
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
        //closestPoint = vineOrigin.position;
        //minDistance = Vector3.Distance(closestPoint, vineOrigin.position);
        minDistance = float.MaxValue;
        foreach (BezierKnot knots in spline)
        {
            if(minDistance > Vector3.Distance(knots.Position, hit.point))
            {
                minDistance = Vector3.Distance(new Vector3(knots.Position.x, knots.Position.y, knots.Position.z)+transform.position, hit.point);
                closestPoint = new Vector3(knots.Position.x, knots.Position.y, knots.Position.z) + transform.position;
            }
        }

        Vector3 tangent = findTangentFromArbitraryNormal(hit.normal);
        GameObject Vine = new GameObject("Vine " + vineCount);
        Vine.transform.SetParent(transform);
        for (int i = 0; i < branches; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, hit.point - closestPoint, hit.normal) -90 + Random.Range(0,10), hit.normal) * tangent;

            maxPointsForBranch = Mathf.Clamp(Mathf.CeilToInt(minDistance / segmentLength),2,int.MaxValue);
            List<Vine> nodes = createBranch(maxPointsForBranch, closestPoint, hit.normal, dir);
            GameObject branch = new GameObject("Branch " + i);
            MeshGeneration b = branch.AddComponent<MeshGeneration>();
            b.init(nodes, branchRadius, branchMaterial);

            branch.transform.SetParent(Vine.transform);

            BezierKnot knot = new BezierKnot(nodes[nodes.Count-1].getPosition());
            spline.Add(knot);
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

    List<Vine> createBranch(int count, Vector3 pos, Vector3 normal, Vector3 dir)
    {

        if (count == maxPointsForBranch)
        {
            Vine rootNode = new Vine(pos, normal);
            return new List<Vine> { rootNode }.join(createBranch(count - 1, pos, normal, dir));
        }
        else if (count < maxPointsForBranch && count > 0)
        {

            if (count % 2 == 0)
            {
                dir = Quaternion.AngleAxis(Random.Range(-20.0f, 20.0f), normal) * dir;
            }

            RaycastHit hit;
            Ray ray = new Ray(pos, normal);
            Vector3 p1 = pos + normal * segmentLength;

            if (Physics.Raycast(ray, out hit, segmentLength))
            {
                p1 = hit.point;
            }
            ray = new Ray(p1, dir);

            if (Physics.Raycast(ray, out hit, segmentLength))
            {
                Vector3 p2 = hit.point;
                Vine p2Node = new Vine(p2, -dir);
                return new List<Vine> { p2Node }.join(createBranch(count - 1, p2, -dir, normal));
            }
            else
            {
                Vector3 p2 = p1 + dir * segmentLength;
                ray = new Ray(applyCorrection(p2, normal), -normal);
                if (Physics.Raycast(ray, out hit, segmentLength))
                {
                    Vector3 p3 = hit.point;
                    Vine p3Node = new Vine(p3, normal);

                    if (isOccluded(p3, pos, normal))
                    {
                        Vector3 middle = calculateMiddlePoint(p3, pos, (normal + dir) / 2);

                        Vector3 m0 = (pos + middle) / 2;
                        Vector3 m1 = (p3 + middle) / 2;

                        Vine m0Node = new Vine(m0, normal);
                        Vine m1Node = new Vine(m1, normal);

                        return new List<Vine> { m0Node, m1Node, p3Node }.join(createBranch(count - 3, p3, normal, dir));
                    }

                    return new List<Vine> { p3Node }.join(createBranch(count - 1, p3, normal, dir));
                }
                else
                {
                    Vector3 p3 = p2 - normal * segmentLength;
                    ray = new Ray(applyCorrection(p3, normal), -normal);

                    if (Physics.Raycast(ray, out hit, segmentLength))
                    {
                        Vector3 p4 = hit.point;
                        Vine p4Node = new Vine(p4, normal);

                        if (isOccluded(p4, pos, normal))
                        {
                            Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);
                            Vector3 m0 = (pos + middle) / 2;
                            Vector3 m1 = (p4 + middle) / 2;

                            Vine m0Node = new Vine(m0, normal);
                            Vine m1Node = new Vine(m1, normal);

                            return new List<Vine> { m0Node, m1Node, p4Node }.join(createBranch(count - 3, p4, normal, dir));
                        }

                        return new List<Vine> { p4Node }.join(createBranch(count - 1, p4, normal, dir));
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

                            return new List<Vine> { m0Node, m1Node, p4Node }.join(createBranch(count - 3, p4, dir, -normal));
                        }
                        return new List<Vine> { p4Node }.join(createBranch(count - 1, p4, dir, -normal));
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
