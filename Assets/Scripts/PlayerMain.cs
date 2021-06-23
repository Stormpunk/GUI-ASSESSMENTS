using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMain : MonoBehaviour
{
    public Transform player;
    #region Movement Variables
    float baseSpeed = 20f;
    float moveSpeed = 20f;
    float sprintSpeed = 30f;
    float crouchSpeed = 10f;
    public Vector3 jump;
    float jumpSpeed = 3f;
    bool isGrounded;
    public Text speedText;
    public Rigidbody rb;
    #endregion
    #region Stat Variables
    public float maxStamina;
    public float stamina;
    public Image staminaImage;
    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private WaitForSeconds manaTick = new WaitForSeconds(1f);
    private WaitForSeconds healthTick = new WaitForSeconds(1f);
    public float maxHealth;
    public float health;
    public Image healthImage;
    public float mana;
    public float maxMana;
    public Image manaImage;
    private Coroutine regen;
    private Coroutine manaRegen;
    private Coroutine healthRegen;
    bool canSprint;
    bool isSprinting;
    public Text MagicText;
    #endregion
    #region Checkpointing Variables
    public Transform currentCheckpoint;
    public GameObject RestPoint;
    #endregion
    #region Death and Respawn Variables
    public GameObject damagePanel;
    public GameObject deathPanel;
    public Text deathText;
    private bool isDamaged;
    bool isDead;
    public float flashCooldown;
    public float maxFlashCooldown;
    public float respawnDelay;
    public float maxRespawnDelay;
    public bool canRespawn;
    public AudioSource deathSound;
    public AudioSource respawnSound;
    #endregion
    public GameObject menu;
    bool isPaused;
    #region Keybinds
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    public Text forward, backwards, left, right, jumping, cast;
    public GameObject currentKey;
    #endregion

    void Start()
    {
        #region Setting Values
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        maxHealth = 100;
        maxStamina = 100;
        maxMana = 100;
        maxFlashCooldown = 0.2f;
        flashCooldown = 0.2f;
        health = maxHealth;
        mana = maxMana;
        stamina = maxStamina;
        isSprinting = false;
        canSprint = true;
        isDamaged = false;
        canRespawn = false;
        deathPanel.SetActive(false);
        isDead = false;
        menu.SetActive(false);
        isPaused = false;
        #endregion

        #region Keybind stuff
        keys.Add("Forward", KeyCode.W);
        keys.Add("Backwards", KeyCode.S);
        keys.Add("Left", KeyCode.A);
        keys.Add("Right", KeyCode.D);
        keys.Add("Jump", KeyCode.Space);
        keys.Add("Cast", KeyCode.Mouse1);

        forward.text = keys["Forward"].ToString();
        backwards.text = keys["Backwards"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();
        jumping.text = keys["Jump"].ToString();
        cast.text = keys["Cast"].ToString();
        #endregion
        //there's probably a better way to do this but I'm honestly so far beyond the point of caring, if I can figure it out before deadlines... yay
    }
    private void FixedUpdate()
    {
        healthImage.fillAmount = health / 100;
        staminaImage.fillAmount = stamina / 100;
        manaImage.fillAmount = mana / 100;
        if (Input.GetKey(keys["Forward"]))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(keys["Backwards"]))
        {
            transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(keys["Right"]))
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(keys["Left"])){
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
        }

    }
    // Update is called once per frame
    void Update()
    {
      
      
        #region Movement
        if (stamina < 0)
        {
            stamina = 0;
        }
        if (stamina <= 0)
        {
            canSprint = false;
        }
        if (isSprinting == true)
        {
            UseStamina(10 * Time.deltaTime);
        }
        if (!menu.activeInHierarchy)
        {
            isPaused = false;
        }
       // Vector3 moveBy = transform.right * x + transform.forward * z;
    //    rb.MovePosition(transform.position + moveBy.normalized * moveSpeed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.LeftShift) && canSprint)
        {
            moveSpeed = sprintSpeed;
            speedText.text = "Sprinting!";
            isSprinting = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !canSprint)
        {
            Debug.Log("No Stamina!");
            speedText.text = "No Stamina!";
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = baseSpeed;
            speedText.text = "Walking";
            isSprinting = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            moveSpeed = crouchSpeed;
            speedText.text = "Sneaking";
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            moveSpeed = baseSpeed;
            speedText.text = "Walking";
        }
        if (Input.GetKeyDown(keys["Jump"]) && isGrounded)
        {
            rb.AddForce(jump * jumpSpeed, ForceMode.Impulse);
            isGrounded = false;
            UseStamina(3);
        }
        #endregion
        #region Health
        if (health > maxHealth)
        {
            health = 100;
        }
        if (health < 0)
        {
            health = 0;
        }
        #endregion
        #region Magic Casting
        if (Input.GetKey(keys["Cast"]))
        {
            UseMana(30f);
            MagicText.text = "Fireball!";
        }
        #endregion
        #region Damage Taking
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(5);
        }
        if (isDamaged == true)
        {
            damagePanel.SetActive(true);
            flashCooldown -= Time.deltaTime;
        }
        if (flashCooldown <= 0)
        {
            damagePanel.SetActive(false);
            flashCooldown = maxFlashCooldown;
            isDamaged = false;
        }
        #endregion
        #region Respawn
        if (isDead == true)
        {
            respawnDelay -= Time.deltaTime;
        }
        if (respawnDelay <= 0)
        {
            canRespawn = true;
            respawnDelay = maxRespawnDelay;
        }
        if (canRespawn == true)
        {
            deathText.text = "Respawn?";
        }
        if (Input.anyKeyDown && canRespawn)
        {
            Respawn();
        }
        #endregion
        #region Enable Menu
        if (Input.GetKeyDown(KeyCode.P) && (isPaused = false))
        {
            isPaused = true;
            menu.SetActive(true);
            Debug.Log("Paused baybeeeee");
        }
        else if (Input.GetKeyDown(KeyCode.P) && (isPaused))
        {
            isPaused = !isPaused;
            PauseGame();
        }
        #endregion
    }

    private void OnCollisionStay()
    {
        isGrounded = true;
    }
    #region "Use" Functions
    void UseMana(float amount)
    {
        if (mana - amount >= 0)
        {
            mana -= amount;
            if (manaRegen != null)
            {
                StopCoroutine(manaRegen);
            }
            manaRegen = StartCoroutine(RegenMana());
        }
    }
    void UseStamina(float amount)
    {
        if (stamina - amount >= 0)
        {
            stamina -= amount;
            if (regen != null)
            {
                StopCoroutine(regen);
            }
            regen = StartCoroutine(RegenStamina());
        }
        else
        {
            Debug.Log("Not Enough Stamina");
        }
    }
    #endregion
    #region Damage and Death 
    void TakeDamage(float amount)
    {
        if (health - amount >= 0)
        {
            health -= amount;
            isDamaged = true;
            if (healthRegen != null)
            {
                StopCoroutine(healthRegen);
            }
            healthRegen = StartCoroutine(RegenHealth());
        }
        else if (health - amount <= 0)
        {
            Death();
        }
    }
    void Death()
    {
        deathPanel.SetActive(true);
        deathSound.Play(0);
        isDead = true;
    }
    void Respawn()
    {
        player.transform.position = new Vector3(5, 0, 0) + currentCheckpoint.transform.position;
        deathPanel.SetActive(false);
        isDead = false;
        canRespawn = false;
        health = maxHealth;
        stamina = maxStamina;
        mana = maxMana;
        respawnSound.Play(0);
    }
    #endregion
    #region Regeneration
    private IEnumerator RegenMana()
    {
        yield return new WaitForSeconds(5);
        while (mana < maxMana)
        {
            mana += (maxMana / 200);
            yield return manaTick;
        }
        manaRegen = null;
    }
    private IEnumerator RegenHealth()
    {
        yield return new WaitForSeconds(3);
        while (health < maxHealth)
        {
            health += (maxHealth / 100);
            yield return healthTick;
        }
        healthRegen = null;
    }
    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(3);
        while (stamina < maxStamina)
        {
            stamina += (maxStamina / 100);
            yield return regenTick;

        }
        regen = null;
    }
    #endregion
    #region Checkpoint
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "RestPoint")
        {
            currentCheckpoint.transform.position = collision.transform.position;
            Debug.Log("Respawn Point Set");
        }

    }
    #endregion
    #region Menus and Pausing
    public void PauseGame()
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Time.timeScale = 1f;
            menu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion
    private void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                currentKey = null;
            }
        }
    }
    public void ChangedKey(GameObject clicked)
    {
        currentKey = clicked;
    }
}
