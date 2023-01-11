using UnityEngine;
using System.Collections;

public class Pistol : Weapon
{
    [Header("Variable")]
    public float pistolDamage; //Pistol 데미지

    [Header("Component")]
    public Transform firePosTransform; //발사 위치 Transform 정보
    public Animator pistolAnimator; //Pistol Animator 정보

    [Header("Line Renderer")]
    public LineRenderer pistolLineRenderer; //Pistol Renderer 정보

    [Header("Particle System")]
    public GameObject fireEffect; //발사 효과

    [Header("Audio")]
    public AudioSource shootAudio; //발사 소리
    public AudioSource bulletHitAudio; //맞는 소리
    public AudioSource reloadAudio; //재장전 소리

    private void Start()
    {
        remainBullet = maxBullet; //장탄 수 초기화

        StartCoroutine(DrawLine());
        StartCoroutine(ShotGun());
        StartCoroutine(ReloadGun());
    }

    /* 포인터를 그리는 코루틴 함수 */
    private IEnumerator DrawLine()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            pistolLineRenderer.SetPosition(0, firePosTransform.position); //시작 지점 갱신
            pistolLineRenderer.SetPosition(1, firePosTransform.position + firePosTransform.forward * 100); //끝 지점 갱신

            yield return waitForEndOfFrame;
        }
    }

    /* 총 발사하는 코루틴 함수 */
    private IEnumerator ShotGun()
    {
        while (true)
        {
            if (weapon.activeSelf) //무기 오브젝트가 활성화되면
            {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetMouseButtonDown(0)) //트리거를 당기면
                {
                    if (remainBullet > 0 && remainReloadTime <= 0f) //장탄 수가 남아있거나 장전중이 아니면
                    {
                        --remainBullet; //장탄 수 감소

                        GameObject fireEffectClone = Instantiate(fireEffect, firePosTransform.position, firePosTransform.rotation); //발사 효과 실행
                        fireEffectClone.transform.localScale = new Vector3(1f, 1f, 1f); //크기 지정
                        fireEffectClone.SetActive(true); //발사 효과 활성화

                        pistolAnimator.Play("Shoot"); //발사 애니메이션 실행

                        RaycastHit hit;
                        if (Physics.Raycast(firePosTransform.position, firePosTransform.forward, out hit, 100f)) //레이캐스트가 닿으면
                        {
                            if (hit.transform.tag.Equals("Enemy")) //닿은 물체가 적이면
                            {
                                hit.transform.GetComponent<Enemy>().Damaged(pistolDamage); //데미지 함수 실행
                                bulletHitAudio.PlayOneShot(bulletHitAudio.clip); //맞는 소리 실행
                            }
                            else //좀비가 아니면
                            {
                                shootAudio.PlayOneShot(shootAudio.clip); //발사 소리 실행
                            }
                        }
                        else //좀비가 아니면
                        {
                            shootAudio.PlayOneShot(shootAudio.clip); //발사 소리 실행
                        }
                    }
                    else //장탄 수가 없거나 장전 중이면
                    {
                        reloadAudio.PlayOneShot(reloadAudio.clip); //재장전 소리 실행
                    }
                }
            }
            yield return null;
        }
    }

    /* 재장전하는 함수 */
    private IEnumerator ReloadGun()
    {
        while (true)
        {
            while (remainBullet > 0 && OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y > -0.5f) yield return null; //총알이 남아있으면 대기

            remainReloadTime = reloadTime; //재 장전 시간 초기화
            while(remainReloadTime > 0) //재장전 시간이 남아있는 동안
            {
                remainReloadTime -= Time.deltaTime; //재장전 시간 감소
                yield return null;
            }
            remainBullet = maxBullet; //총알 장전 완료
        }
    }
}