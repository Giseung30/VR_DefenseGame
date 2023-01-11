using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Control")]
    public Transform playerTransform; //Player Transform 정보

    private void Awake()
    {
        Static.playerControllerScript = this;
    }

    private void Start()
    {
        if (Static.isPC) //PC 환경이면
        {
            StartCoroutine(RotateSight());
        }
    }

    /* 시야를 회전하는 코루틴 함수 */
    private IEnumerator RotateSight()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        float rotateSpeed = 1f; //회전 속도
        float xDegree = 0f; //시야 x축 각도 제한 변수
        while (true)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            xDegree -= vertical * rotateSpeed;
            xDegree = Mathf.Clamp(xDegree, -89f, 89f);
            playerTransform.eulerAngles = new Vector3(xDegree, playerTransform.eulerAngles.y + horizontal * rotateSpeed, 0f); //시야 각도 지정

            yield return waitForEndOfFrame;
        }
    }
}