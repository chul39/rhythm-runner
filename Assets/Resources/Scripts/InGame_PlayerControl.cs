using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator))]
public class InGame_PlayerControl : MonoBehaviour {

    public enum State {
        isIdle,
        isRunning,
        isSliding,
        isJumping,
        isFalling,
        isDead
    }

    public State playerState;
    private Rigidbody2D rb;
    private Vector3 zeroVector;
    private float jumpVelocity;

    private Animator anim;
    private int currentHealth;
    private float bpm, timer, percent;
    private Renderer characterRenderer;

    public bool isGrounded, isAbleToDoubleJump, isHit = false;

    private InGame_LevelManager i_lm;
    private InGame_ObjectPool i_op;

    private BoxCollider2D bc;
    private CapsuleCollider2D cc;

    public Slider healthBar;

    private void Start() {
        playerState = State.isIdle;
        rb = transform.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        i_lm = GameObject.Find("LevelManager").GetComponent<InGame_LevelManager>();
        i_op = GameObject.Find("LevelGenerator").GetComponent<InGame_ObjectPool>();
        bc = GetComponent<BoxCollider2D>();
        cc = GetComponent<CapsuleCollider2D>();
        characterRenderer = GetComponent<SpriteRenderer>();
        zeroVector = new Vector3(0f, 0f, 0f);
        bc.enabled = false;
        bpm = 0f;
        currentHealth = 100;
    }

    private void Update() {
        switch (i_lm.levelState) {
            case InGame_LevelManager.State.Awake:
                // Not allow any input to the character
                break;
            case InGame_LevelManager.State.Init:
                if (anim.enabled == false) {
                    anim.enabled = true;
                }
                if (bpm == 0f) {
                    bpm = i_lm.GetBpm();
                    if (bpm > 200) {
                        bpm /= 2;
                    }
                    UpdateJumpVelocity();
                }
                break;
            case InGame_LevelManager.State.Play:
                if (anim.enabled == false) {
                    anim.enabled = true;
                }
                ControlInput();
                UpdateDeadStatus();
                if (rb.velocity.y > 0) {
                    playerState = State.isJumping;
                } else if(rb.velocity.y < 0)  {
                    playerState = State.isFalling;
                }
                break;
            case InGame_LevelManager.State.Pause:
                anim.enabled = false;
                break;
            case InGame_LevelManager.State.NearEnd:
                if (anim.enabled == false) {
                    anim.enabled = true;
                }
                ControlInput();
                UpdateDeadStatus();
                break;
            case InGame_LevelManager.State.End:
                if (playerState != State.isDead) {
                    playerState = State.isIdle;
                }
                break;
        }
    }

    private IEnumerator SetInvul(int blinkCount, float blinkDelay) {
        for (int i = 0; i < blinkCount; i++) {
            characterRenderer.enabled = !characterRenderer.enabled;
            yield return new WaitForSeconds(blinkDelay);
        }
        characterRenderer.enabled = true;
        isHit = false;
    }

    private void ControlInput(){
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) {
            if (isGrounded) {
                playerState = State.isJumping;
                rb.velocity = new Vector3(0f, jumpVelocity);
                isAbleToDoubleJump = true;
            } else {
                if (isAbleToDoubleJump) {
                    rb.velocity = zeroVector;
                    rb.velocity = new Vector3(0f, jumpVelocity/1.5f);
                    isAbleToDoubleJump = false;
                }
            }
        } else {
            playerState = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.DownArrow)) ? State.isSliding : State.isRunning;
		}
    }

    private void FixedUpdate(){
        switch (playerState) {
            case State.isIdle:
                anim.Play("Idle");
                break;
            case State.isRunning:
                anim.Play("Run");
                cc.enabled = true;
                bc.enabled = false;
                break;
            case State.isSliding:
                anim.Play("Slide");
                cc.enabled = false;
                bc.enabled = true;
                break;
            case State.isJumping:
                anim.Play("Jump_up");
                cc.enabled = true;
                bc.enabled = false;
                break;
            case State.isFalling:
                anim.Play("Jump_down");
                break;
            case State.isDead:
                anim.Play("Dead");
                break;
        }
    }

    private void UpdateJumpVelocity(){
        float newGravity = 4f / Mathf.Pow((60f / bpm), 2);
        Physics2D.gravity = new Vector2(0f, -newGravity);
        jumpVelocity = Mathf.Sqrt(2f * newGravity * 1.8f);
    }

    private void UpdatePlayerHealth(){
        healthBar.value = currentHealth;
    }

    private void UpdateDeadStatus(){
        if (currentHealth <= 0) {
            anim.Play("Dead");
            playerState = State.isDead;
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.tag == "Item") {
            switch (other.gameObject.GetComponent<InGame_ObjectControl>().GetItemId()) {
                case (0):
                    if (playerState != State.isSliding ) {
                        i_op.DeactiveObject(other.gameObject, 99);
                        i_lm.UpdateItemObtained();
                        if (currentHealth < 100) {
                            currentHealth += 1;
                            UpdatePlayerHealth();
                        }
                    }
                    break;
                case (1):
                    if (playerState == State.isJumping || playerState == State.isFalling) {
                        i_op.DeactiveObject(other.gameObject, 99);
                        i_lm.UpdateItemObtained();
                        if (currentHealth < 100) {
                            currentHealth += 1;
                            UpdatePlayerHealth();
                        }
                    }
                    break;
                case (2):
                    if (playerState == State.isSliding) {
	                    i_op.DeactiveObject(other.gameObject, 99);
                        i_lm.UpdateItemObtained();
                        if (currentHealth < 100) {
                            currentHealth += 1;
                            UpdatePlayerHealth();
                        }
                    }
                    break;
            }
        } else {
            if (!isHit) {
                StartCoroutine(SetInvul (8, 0.2f));
                isHit = true;
                switch (other.gameObject.tag) {
                    case ("Saw"):
                        currentHealth -= 30;
                        UpdatePlayerHealth();
                        break;
                    case ("Spike"):
                        currentHealth -= 25;
                        UpdatePlayerHealth();
                        break;
                    case ("TopBlock"):
                        currentHealth -= 20;
                        UpdatePlayerHealth();
                        break;
                    case ("Bullet"):
                        currentHealth -= 25;
                        UpdatePlayerHealth();
                        break;
                    case ("Acid"):
                        currentHealth -= 30;
                        UpdatePlayerHealth();
                        if (playerState != State.isDead) {
                            transform.position = new Vector3(0f, 3f, 0f);
                        } else {
                            this.gameObject.SetActive(false);
                        }
                        break;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other){
        if (other.collider.tag == "Ground") isGrounded = true;
    }

    private void OnCollisionStay2D(Collision2D other){
        if (other.collider.tag == "Ground") isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D other){
        if (other.collider.tag == "Ground") isGrounded = false;
    }

}
