using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    public enum Stat
    {
        GamesPlayed,
        ProjectilesFired,
        TanksDestroyed,
        MetersTraveled,
        DeathsByWater,
        BullseyeKills,
        DamageDealt
    }

    private Dictionary<Stat, float> stats;

    void Start()
    {
        // Load from file
        stats = new Dictionary<Stat, float>();
    }

    public void Add(Stat stat, float amount)
    {
        stats[stat] += amount;
    }

    private void LoadFromFile()
    {
        
    }
    
    public void SaveToFile()
    {
        
    }
    
}
