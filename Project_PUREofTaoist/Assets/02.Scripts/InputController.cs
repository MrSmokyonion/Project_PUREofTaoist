using UnityEngine;

/* Purpose : 플레이어의 키 입력을 감지하고 처리하는 클래스
 * Notice : 캐릭터의 기본적인 움직임은 그냥 여기서 처리함. (ex-움직이기)
 */
public class InputController : MonoBehaviour
{
    public MainCharacter m_mainCharacter;   //메인 캐릭터
    public Transform m_groundCheckPos;      //캐릭터에 땅위에 서있는지 확인용 오브젝트

    public LayerMask m_whatIsGround;        //땅 레이어마스크

    public PLAYERSTATE m_playerState = PLAYERSTATE.Idle;       //플레이어 상태 변수
    public Animator m_playerAnimator;       //플레이어 애니메이터

    public GameObject m_AttackParticle;     //공격시 나타날 이펙트

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
        Do_CharacterAttack();
        Do_CharacterJump();
        Do_PlayerState();
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

        SpriteRenderer sr = m_mainCharacter.GetComponent<SpriteRenderer>();
        if (moveValue > 0 && sr.flipX == false)
        {
            sr.flipX = true;
        }
        else if (moveValue < 0 && sr.flipX == true)
        {
            sr.flipX = false;
        }

        if (m_playerState != PLAYERSTATE.Jump)
        {
            if (moveValue < 1 && moveValue > -1)
            {
                m_playerState = PLAYERSTATE.Idle;
            }
            else
            {
                m_playerState = PLAYERSTATE.Run;
            }
        }
        else
        {
        }
    }

    /* Purpose : 캐릭터가 점프하는 함수.
    * Argument: X
    * Notive  : 키 입력 감지.
    */
    private void Do_CharacterJump()
    {
        //Physics2D.OverlapCircle(m_groundCheckPos.position, 0.5f, m_whatIsGround)
        if (m_rigid.velocity.y <= 0 && Is_Ground() && m_playerState == PLAYERSTATE.Jump)
        {
            m_curJumpCount = 0;
            m_playerState = PLAYERSTATE.Idle;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (m_curJumpCount < m_mainCharacter.m_maxJumpCount)
            {
                m_rigid.velocity = new Vector2(m_rigid.velocity.x, m_mainCharacter.m_jumpSpeed);
                m_jumpTimeCounter = m_jumpTimeMax;
                m_isJumping = true;
                m_curJumpCount++;
                m_playerState = PLAYERSTATE.Jump;
            }
        }
        if (Input.GetKey(KeyCode.K) && m_isJumping == true)
        {
            if (m_jumpTimeCounter > 0)
            {
                m_rigid.velocity = new Vector2(m_rigid.velocity.x, m_mainCharacter.m_jumpSpeed);
                m_jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                m_isJumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            m_isJumping = false;
        }

        //떨어지는 속도 제한두기
        if (m_rigid.velocity.y < -Mathf.Abs(m_mainCharacter.m_maxFallSpeed))
            m_rigid.velocity = new Vector2(m_rigid.velocity.x, Mathf.Clamp(m_rigid.velocity.y, -Mathf.Abs(m_mainCharacter.m_maxFallSpeed), Mathf.Infinity));

    }

    /* Purpose : 캐릭터가 공격하는 함수.
     * Argument: X
     * Notice  : 
     */
    private void Do_CharacterAttack()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            m_playerAnimator.SetTrigger("AttackTrg");
            if(m_mainCharacter.GetComponent<SpriteRenderer>().flipX == true)
            {
                Instantiate(m_AttackParticle, m_mainCharacter.transform.position + new Vector3(1, -0.2f, 0), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                Instantiate(m_AttackParticle, m_mainCharacter.transform.position + new Vector3(-1, -0.2f, 0), Quaternion.Euler(0, 0, 0));
            }
        }
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

    /* Purpose : 캐릭터의 상태를 체크하고 처리하는 함수.
     * Argument: X
     * Notice  : 애니메이션 처리.
     */
    private void Do_PlayerState()
    {
        switch (m_playerState)
        {
            case PLAYERSTATE.Idle:
                {
                    m_playerAnimator.SetBool("isRun", false);
                    m_playerAnimator.SetBool("isJump", false);
                    break;
                }
            case PLAYERSTATE.Run:
                {
                    m_playerAnimator.SetBool("isRun", true);
                    m_playerAnimator.SetBool("isJump", false);
                    break;
                }
            case PLAYERSTATE.Jump:
                {
                    m_playerAnimator.SetBool("isRun", false);
                    m_playerAnimator.SetBool("isJump", true);
                    float jumpVelocity = m_mainCharacter.GetComponent<Rigidbody2D>().velocity.y;
                    if (jumpVelocity > 0.2)
                    {
                        m_playerAnimator.SetFloat("JumpVelocity", 0f);
                    }
                    if (jumpVelocity < 0.3 && jumpVelocity > -0.3)
                    {
                        m_playerAnimator.SetFloat("JumpVelocity", 0.5f);
                    }
                    if (jumpVelocity < -0.2)
                    {
                        m_playerAnimator.SetFloat("JumpVelocity", 1f);
                    }
                    break;
                }
            case PLAYERSTATE.Attack:
                {
                    m_playerAnimator.SetTrigger("AttackTrg");
                    break;
                }
            case PLAYERSTATE.Dead:
                {
                    break;
                }
        }
    }

    #endregion

    /*
     * Purpose: 캐릭터가 바닥에 붙어있는지 확인하는 함수
     * Variable: X
     * Notice: 붙어있으면 true, 아니면 false
     */
    protected bool Is_Ground()
    {
        Debug.DrawRay(m_mainCharacter.transform.position, Vector2.down * 1.2f, Color.red);

        if (Physics2D.Raycast(m_mainCharacter.transform.position, Vector2.down, 1.2f, m_whatIsGround.value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public enum PLAYERSTATE
    {
        Idle = 0,
        Run,
        Jump,
        Attack,
        Dead
    }
}//end class