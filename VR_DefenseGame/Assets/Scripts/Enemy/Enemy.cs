using UnityEngine;

public class Enemy : MonoBehaviour
{
    public virtual void Init(float HP, float moveSpeed, float attackDamage, Vector3 initDestination) { } //적 초기화 함수
    public virtual void Damaged(float damage) { } //적 데미지 입는 함수
}