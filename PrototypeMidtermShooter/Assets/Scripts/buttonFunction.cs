using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonFunction : MonoBehaviour
{
   
    public void resume()
    {
        gameManager.instance.resume();
    }

    public void quit()
    {
        Application.Quit();
    }

    public void givePlayerHP(int amount)
    {
        gameManager.instance.playerScript.giveHP(amount);
    }
    
    public void respawn()
    {
        gameManager.instance.playerScript.respawn();
        gameManager.instance.resume();
    }
}
