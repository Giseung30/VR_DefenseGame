using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : Enemy
{
    [Header("Component")]
    public NavMeshAgent zombieNavMeshAgent; //좀비 네비게이션 정보
    public Transform zombieTransform; //좀비 Transform 정보
    public Animator zombieAnimator; //좀비 Animator 정보
    public Rigidbody zombieRigidbody; //좀비 Rigidbody 정보
    public Collider zombieCollider;  //좀비 Collider 정보

    [Header("Variable")]
    public float HP; //체력
    public float attackDamage; //공격 데미지
    private float moveSpeed; //이동 속도
    private Vector3 initDestination; //목표 지점
    private Vector3 prePosition; //이전 위치

    [Header("Audio")]
    public AudioSource zombieDeadAudio; //죽는 소리
    public AudioSource zombieDead2Audio; //죽는 소리2
    public AudioSource zombieAttackAudio; //공격 소리
    public AudioSource zombieFootAudio; //발소리

    /* 초기화 하는 함수 */
    public override void Init(float HP, float moveSpeed, float attackDamage, Vector3 initDestination)
    {
        this.HP *= HP; //체력 초기화
        zombieNavMeshAgent.speed *= moveSpeed; //이동 속도 초기화
        this.attackDamage *= attackDamage; //공격 데미지 초기화
        this.initDestination = initDestination; //목표 지점 초기화
    }

    private void Start()
    {
        zombieNavMeshAgent.SetDestination(initDestination); //최초 목적지 지정
        prePosition = zombieTransform.position; //이전 위치 초기화

        StartCoroutine(CheckDead());
        StartCoroutine(SetAnimatorParameter());
        StartCoroutine(UpdateMoveSpeed());
    }

    /* Animator 파라미터 갱신하는 코루틴 함수 */
    private IEnumerator SetAnimatorParameter()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            zombieAnimator.SetFloat("moveSpeed", moveSpeed); //이동속도 설정
            zombieAnimator.SetBool("attack", HP > 0 && Vector3.Distance(initDestination, zombieTransform.position) < 2f); //공격 여부 설정

            yield return waitForEndOfFrame;
        }
    }

    /* 이동속도 갱신하는 코루틴 함수 */
    private IEnumerator UpdateMoveSpeed()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            float changedSpeed = Vector3.Distance(zombieTransform.position, prePosition) * 100f; //변경된 속도를 구함
            moveSpeed = Mathf.Lerp(moveSpeed, changedSpeed, Time.deltaTime); //보간 처리
            prePosition = zombieTransform.position; //이전 위치 갱신

            yield return waitForEndOfFrame;
        }
    }

    /* 데미지 입는 함수 */
    public override void Damaged(float damage)
    {
        HP -= damage; //체력 감소
    }

    /* 죽음을 확인하는 코루틴 함수 */
    private IEnumerator CheckDead()
    {
        while (HP > 0) yield return new WaitForEndOfFrame(); //죽음 확인
        zombieAnimator.Play("Dead"); //죽는 애니메이션 실행
        zombieNavMeshAgent.enabled = false; //네비게이션 비활성화
        Destroy(zombieRigidbody); //Rigidbody 제거
        Destroy(zombieCollider); //Collider 제거
        OutputAudio(Random.Range(0, 2)); //랜덤으로 죽는 소리 재생
        ++Static.commanderScript.enemyKill; //킬 수 증가
        Destroy(gameObject, 5f); //잠시 후 좀비 제거
    }

    /* 공격하는 함수 */
    public void Attack()
    {
        OutputAudio(2);
        Static.commanderScript.playerHP -= attackDamage; //체력 감소
    }

    /* Audio 재생하는 함수 */
    private void OutputAudio(int i)
    {
        switch (i)
        {
            case 0:
                zombieDeadAudio.PlayOneShot(zombieDeadAudio.clip); //죽는 소리1
                break;
            case 1:
                zombieDead2Audio.PlayOneShot(zombieDead2Audio.clip); //죽는 소리2
                break;
            case 2:
                zombieAttackAudio.PlayOneShot(zombieAttackAudio.clip); //공격 소리 실행
                break;
            case 3:
                zombieFootAudio.PlayOneShot(zombieFootAudio.clip); //발소리 실행
                break;
        }
    }
}