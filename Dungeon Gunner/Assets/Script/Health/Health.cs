using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int sstartingHealth;
    private int currentHealth;

    /// <summary>
    /// Set starting health
    /// </summary>
    /// <param name="startingHealth"></param>
    public void SetStartingHealth(int startingHealth)
    {
        this.sstartingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    /// <summary>
    ///  Get the starting health
    /// </summary>
    /// <returns></returns>
    public int GetStartingHealth()
    {
        return sstartingHealth;
    }

}
