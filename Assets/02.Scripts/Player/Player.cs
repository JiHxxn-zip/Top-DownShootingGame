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

        // ������� moveVelocity�� PlayerController�� �����ؼ� �������� �κе��� ó���Ѵ�.
        controller.Move(moveVelocity);

        // ȭ��󿡼� ���콺�� ��ġ�� ��ȯ����
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        // �ڵ������ Plain�� ���� �� ���� ���͸� ����(�÷����� ������ �����)
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // ī�޶󿡼� ray�� �浹�� ���������� �Ÿ�
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
