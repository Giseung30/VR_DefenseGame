using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Parent")]
    public GameObject weapon; //Weapon 오브젝트
    public Transform weaponModelTransform; //무기 모델링 Transform 정보
    public string weaponName; //무기 이름
    public int maxBullet; //최대 장탄 수
    public int remainBullet; //남은 장탄 수
    public float reloadTime; //재장전 시간
    public float remainReloadTime; //남은 재장전 시간
}