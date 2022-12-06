using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    //Друга лабораторна (нова система воду)
    private Vector2 movementVector;
    private CapsuleCollider2D capsuleCollider2D;
    private Rigidbody2D rb;
    public float movementSpeed = 0f;  
    public float jumpHeight = 0f;
    //Двері (Додаткова лабораторна)
    public BoxCollider2D doorBoxCollider2D;
    public SpriteRenderer doorSpriteRenderer;
    public Sprite[] statusDoorSpriteArray;
    public float timerToOpenDoor = 0.3f;
    public float timerToCloseDoor = 1.5f;
    private float timerDoor = 0.0f;
    private bool timeStartDoor;
    //Анімаціїї (третя лабораторна)
    private float HorizontalMove = 0f;
    private Animator animator;
    private bool FacingRight = true;
    // ривок (лаб3)
    public float spurtSpeed = 0.0f;
    private bool spurtActive;
    private bool spurtTimeStart;
    private float spurtTimer;
    public float spurtTimeToDeactivate = 0.0f;
    // атака (СР2)
    private bool attackActive;
    private bool attackTimeStart;
    private float attackTimer;
    private float impactTimeAttack = 0.45f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        //Двері (Додаткова лабораторна)
        if (capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Door")))
        {
            timeStartDoor = true;                             
        }
        if (timeStartDoor)
        {
            timerDoor += Time.deltaTime;
        }
        if (timerDoor > timerToOpenDoor && capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Door")))
        {
            doorBoxCollider2D.enabled = false;
            doorSpriteRenderer.sprite = statusDoorSpriteArray[1];
        }
        if (timerDoor > timerToCloseDoor && !capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Door")))
        {
            doorBoxCollider2D.enabled = true;
            doorSpriteRenderer.sprite = statusDoorSpriteArray[0];
            timerDoor = timerDoor - timerToCloseDoor;
            timeStartDoor = false;
        }
        //Анімаціїї (третя лабораторна)
        HorizontalMove = Input.GetAxisRaw("Horizontal") * movementSpeed;
        animator.SetFloat("HorizontalMove", Mathf.Abs(HorizontalMove));
        //повороти
        if (HorizontalMove < 0 && FacingRight)
        {
            Flip();
        }
        else if (HorizontalMove > 0 && !FacingRight)
        {
            Flip();
        }
        //анімація стрибка
        if (capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {           
            animator.SetBool("Jumping", false);
        }
        else 
        {
            animator.SetBool("Jumping", true);
        }
        //ривок таймер
        if (spurtTimeStart)
        {           
            spurtTimer += Time.deltaTime;
        }
        if (spurtTimer > spurtTimeToDeactivate)
        {
            spurtActive = false;
            spurtTimer = spurtTimer - spurtTimeToDeactivate;
            spurtTimeStart = false;           
        }
        //таймер дії удару
        if (attackTimeStart)
        {
            attackTimer += Time.deltaTime;
            animator.SetBool("Attack", true);
            gameObject.tag = "PlayerAttack";
        }
        if (attackTimer > impactTimeAttack)
        {
            attackActive = false;
            attackTimer = attackTimer - impactTimeAttack;
            attackTimeStart = false;
            animator.SetBool("Attack", false);
            gameObject.tag = "Player";
        }       
    }
    //Друга лабораторна (нова система воду)
    private void FixedUpdate()
    {
        Vector2 playerVelocity = new Vector2(movementVector.x *  (spurtActive ? spurtSpeed : movementSpeed), rb.velocity.y);
        rb.velocity = playerVelocity;
    }
    private void OnMove(InputValue value)
    {
        movementVector = value.Get<Vector2>();
        Debug.Log(movementVector);
    }
    private void OnJump(InputValue value)
    {
        if (!capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return; 
        }
        if (value.isPressed)
        {
            rb.velocity += new Vector2(0f, jumpHeight);
        }
    }

    private void OnSpurt(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Ривок active");
            spurtActive = true;
            spurtTimeStart = true;
        }
    }
    private void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Атака active");
            attackActive = true;
            attackTimeStart = true;
        }
    }
    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    //перезагрузка сцени при дотику до ворога
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && attackActive == false)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}