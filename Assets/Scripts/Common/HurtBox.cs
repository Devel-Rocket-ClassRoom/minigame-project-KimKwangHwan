using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField] private IDamageable owner;

    public void ReceiveHit()
    {
        Debug.Log("Hit!!");
        //owner.TakeDamage();
    }
}
