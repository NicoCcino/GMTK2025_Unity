using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    int Idle;
    int Jump;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        Idle = Animator.StringToHash("Idle");
        Jump = Animator.StringToHash("Jump");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetJumpTrigger()
    {
        anim.SetBool(Jump, true);
        anim.SetBool(Idle, false);
    }

    public void SetIdleTrigger()
    {
        anim.SetBool(Idle, true);
        anim.SetBool(Jump, false);
    }
}
