using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Rigidbody2D playerRb;

    public float moveSpeed = 5f;

    private Vector2 moveInput, mouseInput;

    public float mouseSensitivity = 1f;

    public Camera playerCamera;

    public GameObject bulletImpact;
    public int currentAmmo;

    public Animator gunAnim;
    public Animator anim;

    [Header("Health")]
    [SerializeField] private int currentHealth;
    public int maxHealth = 100;
    private bool hasDied;
    
    [Header("UI")]
    public GameObject gameOver;
    public Text healthText, ammoText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthText.text = currentHealth.ToString() + "%";

        ammoText.text = currentAmmo.ToString();
    }

    void Update()
    {
        if (hasDied) { return; }

        #region PlayerMovement

        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector3 moveHorizontal = transform.up * -moveInput.x;

        Vector3 moveVertical = transform.right * moveInput.y;

        playerRb.velocity = (moveHorizontal + moveVertical) * moveSpeed;

        #endregion

        #region CameraMovement

        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - mouseInput.x);

        playerCamera.transform.localRotation = Quaternion.Euler(playerCamera.transform.localRotation.eulerAngles + new Vector3(0f, mouseInput.y, 0f));

        #endregion

        #region PlayerShooting

        if (Input.GetMouseButtonDown(0))
        {
            if (!(currentAmmo > 0)) { return; }
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
            RaycastHit hit;
            
            if (!Physics.Raycast(ray, out hit)) { return; }
            //Debug.Log("I'm looking at " + hit.transform.name);
            Instantiate(bulletImpact, hit.point, transform.rotation);

            if(hit.transform.tag == "Enemy")
            {
                hit.transform.GetComponentInParent<EnemyController>().TakeDamage();
            }

            AudioController.instance.PlayShotgunShot();

            currentAmmo--;
            gunAnim.SetTrigger("Shoot");
            UpdateAmmoUI();
            
            
            /*if (Physics.Raycast(ray, out hit))
            {
                 //Debug.Log("I'm looking at " + hit.transform.name);
                 Instantiate(bulletImpact, hit.point, transform.rotation);
            }

            else
            {
                 Debug.Log("I'm looking at null");
            }*/
        }

        #endregion

        anim.SetBool("isMoving", moveInput != Vector2.zero ? true : false);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0)
        {
            gameOver.SetActive(true);
            hasDied = true;
            currentHealth = 0;
        }

        healthText.text = currentHealth.ToString() + "%";

        AudioController.instance.PlayPlayerHurt();
    }

    public void AddHealth(int healAmount)
    {
        currentHealth += healAmount;

        currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
        healthText.text = currentHealth.ToString() + "%";
    }

    public void UpdateAmmoUI()
    {
        ammoText.text = currentAmmo.ToString();
    }
}

