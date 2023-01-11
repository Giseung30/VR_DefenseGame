using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon")]
    public Weapon currentWeapon; //현재 무기
    public int currentIndex; //현재 인덱스
    public Weapon[] weaponScripts; //Weapon 스크립트들
    public Camera[] eventCameras; //각 무기에 부착된 EventCamera들
    public Canvas canvas; //Canvas

    [Header("Tracking")]
    public Transform rightHandAnchorTransform; //오른쪽 손 트래커 Transform 정보
    public Transform centerEyeAnchorTransform; //HMD 시야 오브젝트 Transform 정보

    private void Awake()
    {
        Static.weaponControllerScript = this;
    }

    private void Start()
    {
        if (Static.isPC) //PC 환경이면
        {
            Cursor.lockState = CursorLockMode.Locked; //마우스 잠금
            StartCoroutine(UpdataPositionPC());
            StartCoroutine(ChangeWeaponPC());
        }
        else //모바일 환경이면
        {
            StartCoroutine(UpdatePositionMobile());
            StartCoroutine(ChangeWeaponMobile());
        }
    }

    /* 컨트롤러 위치 갱신하는 코루틴 함수 - PC */
    private IEnumerator UpdataPositionPC()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        float rotateSpeed = 1f; //회전 속도
        float degreeX = 0f; //총의 x축 각도
        float degreeY = 0f; //총의 y축 각도
        while (true)
        {
            currentWeapon.weaponModelTransform.position = centerEyeAnchorTransform.position + centerEyeAnchorTransform.forward * 0.5f; //시야의 중간으로 건 위치 옮기기
            currentWeapon.weaponModelTransform.position -= (currentWeapon.weaponModelTransform.up * 0.25f); //아래로 살짝 내림

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            degreeX -= mouseY * rotateSpeed;
            degreeX = Mathf.Clamp(degreeX, -89f, 89f); //x축 각도 제한
            degreeY += mouseX * rotateSpeed;
            degreeY = Mathf.Clamp(degreeY, -89f, 89f); //y축 각도 제한

            currentWeapon.weaponModelTransform.localEulerAngles = new Vector3(Static.playerControllerScript.playerTransform.eulerAngles.x + degreeX, Static.playerControllerScript.playerTransform.eulerAngles.y + degreeY);//각도 지정

            yield return waitForEndOfFrame;
        }
    }

    /* 컨트롤러 위치 갱신하는 코루틴 함수 - Mobile */
    private IEnumerator UpdatePositionMobile()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            currentWeapon.weaponModelTransform.position = centerEyeAnchorTransform.position + centerEyeAnchorTransform.forward * 0.5f; //시야의 중간으로 건 위치 옮기기
            currentWeapon.weaponModelTransform.position -= (currentWeapon.weaponModelTransform.up * 0.25f); //아래로 살짝 내림

            Vector3 degree = centerEyeAnchorTransform.rotation.eulerAngles - rightHandAnchorTransform.rotation.eulerAngles; //시야와 트래커 사이의 각도를 구함
            currentWeapon.weaponModelTransform.eulerAngles = Quaternion.LookRotation(centerEyeAnchorTransform.forward).eulerAngles - degree; //시야를 바라보는 방향에서 각도를 뺌

            yield return waitForEndOfFrame;
        }
    }

    /* 무기 변경하는 함수 - PC */
    private IEnumerator ChangeWeaponPC()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) //이전 무기
            {
                currentWeapon.weapon.SetActive(false);

                currentIndex = currentIndex == 0 ? weaponScripts.Length - 1 : currentIndex - 1; //인덱스 감소
                currentWeapon = weaponScripts[currentIndex]; //무기 스크립트 적용
                canvas.worldCamera = Static.graphicRaycastScript.target = eventCameras[currentIndex]; //이벤트 카메라 변경

                currentWeapon.weapon.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)) //다음 무기
            {
                currentWeapon.weapon.SetActive(false);

                currentIndex = (currentIndex + 1) % weaponScripts.Length; //인덱스 증가
                currentWeapon = weaponScripts[currentIndex]; //무기 스크립트 적용
                canvas.worldCamera = Static.graphicRaycastScript.target = eventCameras[currentIndex]; //이벤트 카메라 변경

                currentWeapon.weapon.SetActive(true);
            }
            yield return null;
        }
    }

    /* 무기 변경하는 함수 - Mobile */
    private IEnumerator ChangeWeaponMobile()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        while (true)
        {
            while (Mathf.Abs(OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).x) <= 0.5f) yield return null;
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).x < -0.5f) //이전 무기
            {
                currentWeapon.weapon.SetActive(false);

                currentIndex = currentIndex == 0 ? weaponScripts.Length - 1 : currentIndex - 1; //인덱스 감소
                currentWeapon = weaponScripts[currentIndex]; //무기 스크립트 적용
                canvas.worldCamera = Static.graphicRaycastScript.target = eventCameras[currentIndex]; //이벤트 카메라 변경

                currentWeapon.weapon.SetActive(true);
            }
            else if (OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).x > 0.5f) //다음 무기
            {
                currentWeapon.weapon.SetActive(false);

                currentIndex = (currentIndex + 1) % weaponScripts.Length; //인덱스 증가
                currentWeapon = weaponScripts[currentIndex]; //무기 스크립트 적용
                canvas.worldCamera = Static.graphicRaycastScript.target = eventCameras[currentIndex]; //이벤트 카메라 변경

                currentWeapon.weapon.SetActive(true);
            }
            yield return waitForSeconds;
        }
    }
}