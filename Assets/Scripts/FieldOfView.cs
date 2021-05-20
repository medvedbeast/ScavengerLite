using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public float meshResolution;
    public int edgeResloveIterations;
    public float edgeDistanceThreshold;
    public float edgeCutaway;
    public MeshFilter viewMeshFilter;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();


    Mesh viewMesh;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "ViewMesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            if (target == this.transform)
            {
                continue;
            }
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void DrawFieldOfView()
    {

        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldInfo = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepSize * i;
            ViewCastInfo info = ViewCast(angle);

            if (i > 0)
            {
                bool edgeThresholdFlag = Mathf.Abs(oldInfo.distance - info.distance) > edgeDistanceThreshold;
                if (oldInfo.hit != info.hit || (oldInfo.hit && info.hit && edgeThresholdFlag))
                {
                    EdgeInfo edge = FindEdge(oldInfo, info);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
                
            }
            viewPoints.Add(info.point);
            oldInfo = info;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] verticies = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        verticies[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            verticies[i + 1] = transform.InverseTransformPoint(viewPoints[i] + Vector3.forward * edgeCutaway);
            
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = verticies;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

    }

    EdgeInfo FindEdge(ViewCastInfo minInfo, ViewCastInfo maxInfo)
    {
        float minAngle = minInfo.angle;
        float maxAngle = maxInfo.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResloveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newInfo = ViewCast(angle);

            bool edgeThresholdFlag = Mathf.Abs(minInfo.distance - newInfo.distance) > edgeDistanceThreshold;
            if (newInfo.hit == minInfo.hit && !edgeThresholdFlag)
            {
                minAngle = angle;
                minPoint = newInfo.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newInfo.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirectionFromAngle(float angle, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.distance = distance;
            this.angle = angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }
}
