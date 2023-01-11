using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : MonoBehaviour
{
    [Header("Variable")]
    public bool gameState; //게임 시작 여부
    public int enemyKill; //적 킬 수
    public float spawnDelay; //소환 딜레이

    [Header("Ability")]
    public float enemyHP; //적 체력 배수
    public float enemyMoveSpeed; //적 이동속도 배수
    public float enemyAttackDamage; //적 공격속도 배수
    public float playerHP; //플레이어 체력
    

    [Header("Enemy")]
    public GameObject[] enemiesObject; //적 오브젝트들
    public Transform[] spawnPositions; //적 생성 위치
    public Transform[] destinations; //목표 지점
    private List<GameObject> spawnEnemies = new List<GameObject>(); //소환된 적 오브젝트들

    [Header("Coroutine")]
    private Coroutine increaseEnemyPowerCoroutine;
    private Coroutine spawnEnemiesCoroutine;
    private Coroutine calSpawnDelayCoroutine;
    private Coroutine checkPlayerHPCoroutine;

    private void Awake()
    {
        Static.commanderScript = this;
    }

    private void Update()
    {
        spawnEnemies.RemoveAll(GameObject => GameObject == null); //제거된 적은 리스트에서 삭제
    }

    /* 적 능력 증가하는 코루틴 함수 */
    private IEnumerator IncreaseEnemyPower()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(10f);
        while (true)
        {
            yield return waitForSeconds;
            if (enemyHP < 3f) enemyHP += 1f / 6f; //체력 증가
            if (enemyMoveSpeed < 2f) enemyMoveSpeed += 1f / 30f; //이동속도 증가
            if (enemyAttackDamage < 5f) enemyAttackDamage += 2f / 9f; //공격 데미지 증가
        }
    }

    /* 적 생성하는 코루틴 함수 */
    private IEnumerator SpawnEnemies()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            while (spawnEnemies.Count >= 10) yield return waitForEndOfFrame; //최대 적 수 제한

            int enemyNum = Random.Range(0, enemiesObject.Length); //적 번호
            int spawnNum = Random.Range(0, spawnPositions.Length); //소환 위치

            GameObject enemyClone = Instantiate(enemiesObject[enemyNum], spawnPositions[spawnNum].position, spawnPositions[spawnNum].rotation); //적 생성
            enemyClone.name = enemiesObject[enemyNum].name; //복제 이름 변경
            enemyClone.GetComponent<Enemy>().Init(enemyHP, enemyMoveSpeed, enemyAttackDamage, destinations[Random.Range(0, destinations.Length)].position); //초기화
            enemyClone.SetActive(true); //적 활성화
            spawnEnemies.Add(enemyClone); //적 리스트에 삽입

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    /* 소환 딜레이 계산하는 코루틴 함수 */
    private IEnumerator CalSpawnDelay()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (spawnDelay > 1f)
        {
            yield return waitForEndOfFrame;
            spawnDelay -= (4f / 120f) * Time.deltaTime; //소환 시간 감소
        }
        spawnDelay = 1f;
    }

    /* 플레이어 체력 검사하는 코루틴 함수 */
    private IEnumerator CheckPlayerHP()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (playerHP > 0) yield return waitForEndOfFrame; //죽음 확인
        OnClickExitButton(); //종료 함수 실행
    }

    /* 시작버튼 클릭하면 실행되는 함수 */
    public void OnClickStartButton()
    {
        gameState = true; //게임 시작

        enemyHP = 1f; //적 체력 초기화
        enemyMoveSpeed = 1f; //적 이동속도 초기화
        enemyAttackDamage = 1f; //적 공격 데미지 초기화
        playerHP = 1000f; //플레이어 체력 초기화

        enemyKill = 0; //적 킬 수 초기화
        spawnDelay = 5f; //소환 딜레이 초기화

        spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
        increaseEnemyPowerCoroutine = StartCoroutine(IncreaseEnemyPower());
        calSpawnDelayCoroutine = StartCoroutine(CalSpawnDelay());
        checkPlayerHPCoroutine = StartCoroutine(CheckPlayerHP());

        Static.uiControllerScript.startButton.SetActive(false); //시작 버튼 비활성화
        Static.uiControllerScript.exitButton.SetActive(true); //종료 버튼 활성화
    }

    /* 종료버튼 클릭하면 실행되는 함수 */
    public void OnClickExitButton()
    {
        gameState = false; //게임 종료

        foreach (GameObject enemy in spawnEnemies) //적을 탐색하면서
        {
            enemy.GetComponent<Enemy>().Damaged(10000f); //적 모두 사망
        }

        if (spawnEnemiesCoroutine != null) StopCoroutine(spawnEnemiesCoroutine);
        if (increaseEnemyPowerCoroutine != null) StopCoroutine(increaseEnemyPowerCoroutine);
        if (calSpawnDelayCoroutine != null) StopCoroutine(calSpawnDelayCoroutine);
        if (checkPlayerHPCoroutine != null) StopCoroutine(checkPlayerHPCoroutine);

        playerHP = 0f; //체력 제거

        Static.uiControllerScript.startButton.SetActive(true); //시작 버튼 활성화
        Static.uiControllerScript.exitButton.SetActive(false); //종료 버튼 비활성화
    }
}