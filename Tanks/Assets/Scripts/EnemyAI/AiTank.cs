using System.Linq;
using UnityEngine;


public class AiTank : Tank
{
    private AiManager aiManager;

    protected override void Start()
    {
        base.Start();
        if(aiManager == null)
            aiManager = GetComponent<AiManager>();
    }

    public void ManualAIUpdate()
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

        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position + new Vector3(0, 1, 0) * 0.66f + localDirection, Vector3.down, out hit, 2, groundLayerMask))
        {
            if (hit.normal.y < 0.85f)
                return;
        }

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
            currentFuel -= 0.05f; // should be changed for the correct value;
        else
            currentFuel -= 0.05f * 2;
        fuelSlider.value = currentFuel / maxFuel;
    }

    public void Aim(Transform enemyTarget)
    {
        float g = Physics.gravity.y;
        float x = transform.position.x - enemyTarget.position.x;
        float y = transform.position.y - enemyTarget.position.y;
        float v = maxShootForce / 2;//Mathf.Sqrt(g * (y + Mathf.Sqrt(x * x + y * y)));

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
            trajectoryAngle = 0;
        }

        //return trajectoryAngle;
        currentShootForce = v;
        rotatePoint.transform.rotation = Quaternion.Euler(0, 0, trajectoryAngle);

        if (CanFire())
            Fire();


    }

    public override void Ready()
    {
        base.Ready();
    }

    public override void LinkPlayer(Player player)
    {
        base.LinkPlayer(player);
    }
}
