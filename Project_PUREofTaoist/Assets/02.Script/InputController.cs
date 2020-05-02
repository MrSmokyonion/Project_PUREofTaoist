using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Purpose : 플레이어의 키 입력을 감지하고 처리하는 클래스
 * Notice : 캐릭터의 기본적인 움직임은 그냥 여기서 처리함. (ex-움직이기)
 */
public class InputController : MonoBehaviour
{
    public MainCharacter m_mainCharacter;   //메인 캐릭터
    public Transform m_groundCheckPos;      //캐릭터에 땅위에 서있는지 확인용 오브젝트

    public LayerMask m_whatIsGround;        //땅 레이어마스크

    private Rigidbody2D m_rigid;            //메인 캐릭터 리지드바디

    private int m_curJumpCount;             //현재 캐릭터가 점프한 횟수
    private float m_jumpTimeMax;            //캐릭터가 점프할 수 있는 최대시간.
    private float m_jumpTimeCounter;        //캐릭터가 점프하고 있는 시간을 샘.
    private bool m_isJumping;               //현재 캐릭터가 점프하고 있는가.

    private void Start()
    {
        Init_DefaultVariable();
    }

    private void Update()
    {
        Do_CharacterJump();
    }

    private void FixedUpdate()
    {
        Do_CharacterMove();
    }

    #region Init
    /* Purpose : 변수 초기화 함수.
     * Argument: X
     * Notive  : MainCharacter 가 존재해야만 가능.
     */
    public void Init_DefaultVariable()
    {
        m_rigid = m_mainCharacter.GetComponent<Rigidbody2D>();

        m_curJumpCount = 0;
        m_jumpTimeMax = 0.20f;
        m_jumpTimeCounter = 0f;
        m_isJumping = false;
    }
    #endregion

    #region Do
    /* Purpose : 캐릭터가 좌우로 움직이는 함수.
     * Argument: X
     * Notive  : 키 입력 감지.
     */
    private void Do_CharacterMove()
    {
        float moveValue = Input.GetAxisRaw("Horizontal");
        m_rigid.velocity = new Vector2(moveValue * m_mainCharacter.m_moveSpeed, m_rigid.velocity.y);
    }

    /* Purpose : 캐릭터가 점프하는 함수.
    * Argument: X
    * Notive  : 키 입력 감지.
    */
    private void Do_CharacterJump()
    {
        if (m_rigid.velocity.y <= 0 && Physics2D.OverlapCircle(m_groundCheckPos.position, 0.3f, m_whatIsGround))
            m_curJumpCount = 0;

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (m_curJumpCount < m_mainCharacter.m_maxJumpCount)
            {
                m_rigid.velocity = new Vector2(m_rigid.velocity.x, m_mainCharacter.m_jumpSpeed);
                m_jumpTimeCounter = m_jumpTimeMax;
                m_isJumping = true;
                m_curJumpCount++;
            }
        }
        if(Input.GetKey(KeyCode.K) && m_isJumping == true)
        {
            if(m_jumpTimeCounter > 0)
            {
                m_rigid.velocity = new Vector2(m_rigid.velocity.x, m_mainCharacter.m_jumpSpeed);
                m_jumpTimeCounter -= Time.deltaTime;
            } else
            {
                m_isJumping = false;
            }
        }
        if(Input.GetKeyUp(KeyCode.K))
        {
            m_isJumping = false;
        }

        //떨어지는 속도 제한두기
        if (m_rigid.velocity.y < -Mathf.Abs(m_mainCharacter.m_maxFallSpeed))
            m_rigid.velocity = new Vector2(m_rigid.velocity.x, Mathf.Clamp(m_rigid.velocity.y, -Mathf.Abs(m_mainCharacter.m_maxFallSpeed), Mathf.Infinity));
    }

    /* Purpose : 캐릭터가 요술을 교체하는 함수.
     * Argument: X
     * Notice  : 시간이 멈춤. 요술 교체 UI가 튀어나옴.
     */
    private void Do_CharacterSkillSelect()
    {
        //if(Input.GetKey(KeyCode.U))
        //{
        //    Time.timeScale = 0.05f;
        //    Time.fixedDeltaTime = Time.timeScale * 0.02f;
        //}
        //else
        //{
        //    Time.timeScale = 1f;
        //    //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        //}
    }

    #endregion
}//end class