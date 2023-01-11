using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon
{
    [Header("Variable")]
    public float rocketLauncherDamage; //RocketLauncher 데미지
    public float fireDelay; //발사 지연시간

    [Header("Component")]
    public Transform firePosTransform; //발사 위치 Transform 정보
    public Transform effectPosTransform; //발사 효과 위치 Transform 정보
    public GameObject warHead; //WarHead 오브젝트

    [Header("Particle System")]
    public GameObject fireEffect; //발사 효과
    public GameObject hitEffect; //타격 효과

    [Header("Audio")]
    public AudioSource shootAudio; //발사 소리
    public AudioSource reloadAudio; //재장전 소리

    private void Start()
    {
        remainBullet = maxBullet; //장탄 수 초기화

        StartCoroutine(ShotGun());
        StartCoroutine(ReloadGun());
    }

    /* 총 발사하는 코루틴 함수 */
    private IEnumerator ShotGun()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(fireDelay);
        while (true)
        {
            if (weapon.activeSelf) //무기 오브젝트가 활성화되면
            {
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || Input.GetMouseButton(0)) //트리거를 당기면
                {
                    if (remainBullet > 0 && remainReloadTime <= 0f) //장탄 수가 남아있거나 장전중이 아니면
                    {
                        --remainBullet; //장탄 수 감소

                        GameObject fireEffectClone = Instantiate(fireEffect, effectPosTransform.position, effectPosTransform.rotation); //발사 효과 실행
                        fireEffectClone.transform.localScale = fireEffect.GetComponent<Transform>().lossyScale; //크기 지정
                        fireEffectClone.SetActive(true); //발사 효과 활성화

                        shootAudio.PlayOneShot(shootAudio.clip); //발사 소리 재생

                        RaycastHit hit;
                        bool isHit = Physics.Raycast(firePosTransform.position, firePosTransform.forward, out hit, 100f); //레이캐스트가 닿았는지 여부

                        Collider[] colliders = Physics.OverlapSphere(isHit ? hit.point : firePosTransform.position + firePosTransform.forward * 100f, 20f);
                        for (int i = 0; i < colliders.Length; ++i) //주위 오브젝트를 탐색하면서
                        {
                            if (colliders[i].tag.Equals("Enemy")) //적이면
                            {
                                colliders[i].GetComponent<Enemy>().Damaged(rocketLauncherDamage); //데미지 함수 실행
                            }
                        }

                        GameObject hitEffectClone = Instantiate(hitEffect, isHit ? hit.point : firePosTransform.position + firePosTransform.forward * 100f, firePosTransform.rotation); //타격 효과 실행
                        hitEffectClone.transform.localScale = hitEffect.GetComponent<Transform>().lossyScale; //크기 지정
                        hitEffectClone.SetActive(true); //발사 효과 활성화
                    }
                    else //장탄 수가 없거나 장전 중이면
                    {
                        reloadAudio.PlayOneShot(reloadAudio.clip); //재장전 소리 실행
                    }
                    yield return waitForSeconds; //발사 지연시간 만큼 대기
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

            warHead.SetActive(false); //WarHead 비활성화
            remainReloadTime = reloadTime; //재 장전 시간 초기화
            while (remainReloadTime > 0) //재장전 시간이 남아있는 동안
            {
                remainReloadTime -= Time.deltaTime; //재장전 시간 감소
                yield return null;
            }
            remainBullet = maxBullet; //총알 장전 완료
            warHead.SetActive(true); //WarHead 활성화
        }
    }
}