using UnityEngine;

public class EnemyContext
{
    public Transform self;
    public Transform target;
    public Rigidbody2D rb;
    public Animator anim;
    public MonoBehaviour runner;
    public Hitbox hitbox;
    public Transform muzzle;
    public float Facing => Mathf.Sign(target.position.x - self.position.x);
}