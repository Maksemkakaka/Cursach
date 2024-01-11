using UnityEngine;

public class Sword : MonoBehaviour
{
    public int maxUses = 3;
    private int currentUses;

    private void Start()
    {
        currentUses = maxUses;
    }

    public void OnHitEnemy()
    {
        currentUses--;
        if (currentUses <= 0)
        {
            DestroyWeapon();
        }
    }

    private void DestroyWeapon()
    {
        Destroy(gameObject);
    }
}
