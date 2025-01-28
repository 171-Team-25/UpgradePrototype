using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum teamName{
    red,
    blue, 
    neutral,
}

public class PlayerClass : Damageable
{
    public GameObject modelPrefab;   // Reference to the higher quality model prefab

    void Start()
    {
        health = maxHealth;
        GameManager.Instance.AddPlayer(gameObject);
        team = GameManager.Instance.AssignTeam(gameObject);
        if(team == teamName.red)
        {
            this.GetComponent<Renderer>().material.color = Color.red;
        }
        if (team == teamName.blue)
        {
            this.GetComponent<Renderer>().material.color = Color.blue;
        }
        gameObject.transform.position = GameManager.Instance.lobbySpawnPoint;


        // Visual Stuff Below
        // Disable the capsule's MeshRenderer to make it invisible
        MeshRenderer capsuleRenderer = GetComponent<MeshRenderer>();
        if (capsuleRenderer != null)
        {
            capsuleRenderer.enabled = false;
        }
        // Instantiate the model and parent it to the capsule
        GameObject modelInstance = Instantiate(modelPrefab, gameObject.transform);
        modelInstance.transform.localPosition = new Vector3(0, -1.12f, 0);
        modelInstance.transform.localRotation = Quaternion.identity;
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            playerMovement = this.AddComponent<PlayerMovement>();
        }
        playerMovement.SetModel(modelInstance);
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack == null)
        {
            playerAttack = this.AddComponent<PlayerAttack>();
        }
        playerAttack.SetModel(modelInstance);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gameObject.AddComponent<AsolUpgrade>();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gameObject.AddComponent<ShieldUpgrade>();
        }
    }

    public override void Die()
    {
        GameManager.Instance.PlayerDie(gameObject);
        Debug.Log(this.name.ToString() + " DIED");
    }

    public teamName GetTeam()
    {
        return team;
    }

    public void Respawn(Vector3 spawnPoint, float time)
    {
        StartCoroutine(RespawnCoroutine(spawnPoint, time));
    }

    private IEnumerator RespawnCoroutine(Vector3 spawnPoint, float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<CharacterController>().enabled = false;
        gameObject.transform.position = spawnPoint;
        GetComponent<CharacterController>().enabled = true;
        health = maxHealth;
        Debug.Log("RESPAWNED!");
        gameObject.SetActive(true);
    }

}
