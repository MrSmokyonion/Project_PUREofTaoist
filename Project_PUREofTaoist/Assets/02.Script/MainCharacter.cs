using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Purpose : 메인 캐릭터에 할당 되는 클래스.
 * Notice : 캐릭터와 관련된 변수들, 행동들을 담고 있다.
 */
public class MainCharacter : MonoBehaviour
{
    public float m_moveSpeed = 7f;
    public float m_jumpSpeed = 20f;

    public int m_maxJumpCount = 2;
    public float m_maxFallSpeed = -15f;
}
