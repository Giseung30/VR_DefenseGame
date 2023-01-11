using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Tracking")]
    public Transform canvasTransform; //Canvas Transform 정보
    public Transform centerEyeAnchorTransform; //CenterEyeAnchor Transform 정보

    [Header("Start/Exit")]
    public GameObject startButton; //Start 버튼
    public GameObject exitButton; //Exit 버튼

    [Header("HP Bar")]
    public Image barImage; //체력 바 Image 정보
    public Text hpStateText; //체력 상태 Text 정보

    [Header("Kill")]
    public Text killText; //Kill Text 정보

    [Header("Weapon")]
    public Text weaponName; //무기 이름 Text 정보
    public Text weaponBullet; //무기 장탄 수 Text 정보

    [Header("LookAt")]
    public Transform[] lookAtTransforms; //카메라를 바라볼 Transform 정보들

    private void Awake()
    {
        Static.uiControllerScript = this;
    }

    private void Start()
    {
        StartCoroutine(TrackingSight());
        StartCoroutine(LookAtCamera());
        StartCoroutine(UpdateState());
    }

    /* Canvas가 카메라를 쫓아다니는 코루틴 함수 */
    private IEnumerator TrackingSight()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        Vector3 distance = centerEyeAnchorTransform.InverseTransformPoint(canvasTransform.position); //Canvas와 CenterEyeAnchor의 거리
        while (true)
        {
            canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - centerEyeAnchorTransform.position); //카메라 주시
            canvasTransform.position = centerEyeAnchorTransform.TransformPoint(distance); //Canvas 위치 이동
            yield return waitForEndOfFrame;
        }
    }

    /* UI의 주요한 수치들을 갱신하는 코루틴 함수 */
    private IEnumerator UpdateState()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            barImage.fillAmount = Static.commanderScript.playerHP / 1000f; //체력바 이미지 갱신
            hpStateText.text = Static.commanderScript.gameState ? string.Format("{0:0}", Static.commanderScript.playerHP) + "<color=#56A3D1> / 1000</color>" : "대기 중"; //체력 상태 텍스트 갱신

            killText.text = "<color=#C8C8C8>처치  </color>" + Static.commanderScript.enemyKill; //처치 Text 갱신

            weaponName.text = Static.weaponControllerScript.currentWeapon.weaponName; //무기 이름 갱신
            if (Static.weaponControllerScript.currentWeapon.remainReloadTime > 0f) //장전 중이면
            {
                weaponBullet.text = "재장전 중 ... " + string.Format("{0:0.0}", Static.weaponControllerScript.currentWeapon.remainReloadTime); //장전 시간 출력
            }
            else
            {
                weaponBullet.text = Static.weaponControllerScript.currentWeapon.remainBullet + " / " + Static.weaponControllerScript.currentWeapon.maxBullet; //장탄 수 출력
            }
            
            yield return waitForEndOfFrame;
        }
    }

    /* Transoform들이 카메라를 바라보는 코루틴 함수 */
    private IEnumerator LookAtCamera()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            for (int i = 0; i < lookAtTransforms.Length; ++i) //Transform들을 탐색하면서
            {
                lookAtTransforms[i].rotation = Quaternion.LookRotation(lookAtTransforms[i].position - centerEyeAnchorTransform.position); //카메라 주시
            }
            yield return waitForEndOfFrame;
        }
    }
}