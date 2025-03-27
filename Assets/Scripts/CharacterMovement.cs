using UnityEngine;
using UnityEngine.VFX;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    [SerializeField] GameObject meteorVFX;

    VisualEffect vfx;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        vfx = meteorVFX.GetComponent<VisualEffect>();
        vfx.pause = true;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // Rotate character to face movement direction
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        if(Input.GetMouseButtonDown(0))
        {
            vfx.pause = false;
            vfx.Play();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            vfx.Stop();
        }
    }
}
