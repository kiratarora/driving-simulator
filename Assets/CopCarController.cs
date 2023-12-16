using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopCarController : MonoBehaviour
{
    public float speed = 10f;
    private List<RoadNode> path;
    private int currentPathIndex;

    public void SetPath(List<RoadNode> newPath)
    {
        path = newPath;
        currentPathIndex = 0;
        Debug.Log("New path set" + path.Count);
    }

    void Update()
    {
        if (path != null && currentPathIndex < path.Count)
        {
            Vector3 targetPosition = path[currentPathIndex].position;
            MoveTowards(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 1f) // 1 unit threshold
            {
                currentPathIndex++;
            }
        }
    }

    void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
