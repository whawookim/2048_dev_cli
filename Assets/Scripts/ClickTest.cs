using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class ClickTest : MonoBehaviour
    {
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                foreach (var r in results)
                    Debug.Log("Hit: " + r.gameObject.name);
            }
        }
    }
}
