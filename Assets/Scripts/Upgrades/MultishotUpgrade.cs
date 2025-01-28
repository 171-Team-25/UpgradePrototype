using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultishotUpgrade : UpgradeBase
{
    private PlayerAttack playerAttack;
    public int additionalProjectiles = 2; // Number of additional projectiles to shoot
    public float spreadAngle = 45f;       // Angle between each projectile

    // Start is called before the first frame update
    void Start()
    {
        upgradeType = UpgradeType.buff;
        playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.OnFire += ShootMultipleProjectiles;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ShootMultipleProjectiles()
    {
        if (playerAttack == null) return;

        // Get the original direction of the projectile
        Vector3 originalDirection = playerAttack.GetShootDirection();

        for (int i = 0; i < additionalProjectiles; i++)
        {
            // Calculate the spread angle for each additional projectile
            float angle = spreadAngle * (i - (additionalProjectiles - 1) / 2.0f);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 spreadDirection = rotation * originalDirection;

            // Instantiate and shoot the additional projectile
            playerAttack.ShootProjectile(spreadDirection);
        }
    }

    private void OnDestroy()
    {
        if (playerAttack != null)
        {
            playerAttack.OnFire -= ShootMultipleProjectiles;
        }
    }
}
