using System.Linq;
using UnityEngine;

public class AiTank : Tank
{
    private AiManager aiManager;
    private int randomInvetoryIndex;

    protected override void Start()
    {
        base.Start();
        if (aiManager == null)
            aiManager = GetComponent<AiManager>();
    }

    public override void ManualUpdate()
    {
        if (hasFired) return;

        aiManager.ManualUpdate();
    }

    public void Move(Transform enemyTankPosition)
    {
        if (enemyTankPosition.position.x > gameObject.transform.position.x)
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (enemyTankPosition.position.x < gameObject.transform.position.x)
            gameObject.transform.rotation = Quaternion.Euler(0, -180, 0);

        Vector3 localDirection = gameObject.transform.InverseTransformDirection(Vector3.right);

        if (enemyTankPosition.position.x > gameObject.transform.position.x)
            gameObject.transform.position += Vector3.right * movementSpeed * Time.deltaTime;

        else if (enemyTankPosition.position.x < gameObject.transform.position.x)
            gameObject.transform.position -= Vector3.right * movementSpeed * Time.deltaTime;

        timeSinceLastEffect += Time.deltaTime;
        if (timeSinceLastEffect > timeBetweenEffectSpawn)
        {
            Instantiate(movementEffect, transform.position, Quaternion.identity);
            timeSinceLastEffect = 0;
        }

        if (!isSlowed)
            currentFuel -= 0.1f; // should be changed for the correct value;
        else
            currentFuel -= 0.1f * 2;
    }

    public void Aim(Transform enemyTarget)
    {
        float g = Physics.gravity.y;
        float x = transform.position.x - enemyTarget.position.x;
        float y = transform.position.y - enemyTarget.position.y;
        float v = maxShootForce;
        
        if (Vector3.Distance(enemyTarget.position, transform.position) <= 10)
        {
            v = maxShootForce / 3;
        }
   
        float v2 = v * v;
        float v4 = v * v * v * v;

        float gx2 = g * v2;
        float yv2 = 2 * y * v * v;
        float gx = g * x;

        float res = Mathf.Sqrt(v4 - g * (gx2 + yv2));
        float res1 = v2 + res;
        float res2 = res1 / gx;

        float trajectoryAngle = Mathf.Atan(res2) * 180 / Mathf.PI;

        if (float.IsNaN(trajectoryAngle))
        {
            trajectoryAngle = Random.Range(-90, 90);
        }

        currentShootForce = v;

        float randomBias = Random.Range(-5.0f, 5.0f);

        trajectoryAngle += randomBias;

        gun.botAim(-trajectoryAngle);

        ChooseAmmoType();

        if (CanFire())
            Fire();
    }

    private void ChooseAmmoType()
    {
        randomInvetoryIndex = Random.Range(0, player.Inventory.items.Count);
        
        if (player.Inventory.items[randomInvetoryIndex].amount <= 0)
        {
            ChooseAmmoType();
        }

        player.Inventory.selectedIndex = randomInvetoryIndex;
        return;
    }

}
