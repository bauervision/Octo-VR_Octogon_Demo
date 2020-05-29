using UnityEngine;

public class Carousel : MonoBehaviour
{

public float rotationSpeed = 100f;
  private Vector3 prevPos = Vector3.zero;
   private Vector3 curPos = Vector3.zero;


private void Update() {
      
    //if(Input.GetMouseButtonDown(0)){
      

        float x = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        
        GetComponent<Rigidbody>().AddTorque(Vector3.up * x);
   // }
   
}

//   private Vector3 prevPos = Vector3.zero;
//    private Vector3 curPos = Vector3.zero;
//    private bool isDragging = false;

//     private void OnMouseDrag() {
//         isDragging = true;
//     }

//    private void Update() {
//        if(Input.GetMouseButton(0)){
//            curPos = Input.mousePosition - prevPos;
//            transform.Rotate(transform.up, Vector3.Dot(curPos, Camera.main.transform.right) * 5f, Space.World);
//        }
//        prevPos = Input.mousePosition;
//        GetComponent<Rigidbody>().AddTorque(Vector3.down * 3);
//    }

}

