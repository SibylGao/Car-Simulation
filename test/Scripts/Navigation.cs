using System.Collections;
using System.Collections.Generic;

//Navigation
//用来更新每个agent的导航点

namespace Simulate
{
    public class Navigation
    {
        /// <summary>
        /// 输入位置信息，输出所有导航点列表
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public List<Vector2> SetGuidPoins(Vector2 position)
        {
            return null;
        }
    }
}




//public class Navigation : MonoBehaviour {

//    public Transform goal;
//    public NavMeshAgent agent;
//    public NavMeshBuildSource nmd;

//    public GameObject PedestrianModel;
//    public List<GameObject> _obj = new List<GameObject>();

//    void Start()
//    {
//        //agent = GetComponent<NavMeshAgent>();
//        //agent.destination = goal.position;
//    }

//    void Update()
//    {

//        if (Input.GetMouseButton(0))
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit))
//            {
//                //print(hit.point);
//                goal.position = hit.point;
//                agent.destination = goal.position;
//                NavMeshPath path = agent.path;
//                for (int i = 0; i < _obj.Count; i++)
//                {
//                    DestroyObject(_obj[i]);
//                }
//                _obj.Clear();
//                for (int i = 0; i < agent.path.corners.Length; i++)
//                {
//                    _obj.Add(Instantiate(PedestrianModel, agent.path.corners[i], Quaternion.identity) as GameObject);
//                }
//            }
//        }

//    }
//}
