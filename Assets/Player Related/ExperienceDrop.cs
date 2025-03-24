using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceDrop : MonoBehaviour
{

    public float expAmount = 50f;
    public GameObject Player;

    public void AddExperience (float expAmount)
    {

        Player.GetComponent<PlayerController3D>()._experience += expAmount;
        Debug.Log(Player.GetComponent<PlayerController3D>()._experience + " Exp Added!");



    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController3D _experience = other.GetComponent<PlayerController3D>();
            AddExperience(expAmount);
            Destroy(gameObject);
        }
    
    }


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
