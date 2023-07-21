using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;

    private PlayerController controller;

    private Camera viewCamera;

    private GunController gunController; 


    void Start()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;

        // 만들어진 moveVelocity를 PlayerController에 전달해서 물리적인 부분들을 처리한다.
        controller.Move(moveVelocity);

        // 화면상에서 마우스의 위치를 반환해줌
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        // 코드상으로 Plain을 생성 후 법선 벡터를 넣음(플레인의 수직으 통과함)
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // 카메라에서 ray가 충돌한 지점까지의 거리
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);

            controller.LookAt(point);
        }

        //
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
    }
}
