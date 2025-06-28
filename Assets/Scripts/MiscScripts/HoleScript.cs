using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScript : MonoBehaviour
{
    public float fallDuration = 1;
    private float fallTimer = 0;
    public Vector3 endScale = Vector3.one * 0.1f;
    private Vector3 returnPosition, startPosition;
    private Vector3 fallCentre;
    public Collider holeCollider;
    private enum scriptState
    {
        INACTIVE,
        PLAYER_FALLING,
        RETURN_PLAYER
    }
    private scriptState currState = scriptState.INACTIVE;
    void Update()
    {
        switch(currState)
        {
            case scriptState.INACTIVE:
                break;
            case scriptState.PLAYER_FALLING:
                UpdatePlayerFalling();
                break;
            case scriptState.RETURN_PLAYER:
                Vector3 offset = returnPosition - transform.position;
                offset.Normalize();
                PlayerController.instance.transform.position = returnPosition;
                PlayerController.instance.FreezeGameplayInput(false);
                PlayerController.instance.transform.localScale = Vector3.one;
                currState = scriptState.INACTIVE;
                break;
        }
    }
    void FixedUpdate()
    {
        if(currState != scriptState.INACTIVE){return;}
        holeCollider.enabled = !PlayerController.instance.IsInvulnerable();
    }
    private void UpdatePlayerFalling()
    {
        fallTimer += Time.deltaTime;
        if(fallTimer >= fallDuration)
        {
            PlayerController.instance.transform.position = fallCentre;
            currState = scriptState.RETURN_PLAYER;
            return;
        }
        PlayerController.instance.transform.position = Vector3.Lerp(startPosition,fallCentre,fallTimer / fallDuration);
        PlayerController.instance.transform.localScale = Vector3.Lerp(Vector3.one, endScale, fallTimer / fallDuration);
    }   
    private void OnTriggerEnter(Collider other)
    {
        if(currState == scriptState.PLAYER_FALLING){return;}
        if(other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if(PlayerController.instance.IsInvulnerable()){return;}
            fallTimer = 0;
            currState = scriptState.PLAYER_FALLING;
            PlayerController.instance.FreezeGameplayInput(true);
            returnPosition = GetNearestSafeNode(player.transform.position);
            startPosition = PlayerController.instance.transform.position;
            PlayerController.instance.Damage(10);
            PlayerController.instance.Protect(fallDuration);
            fallCentre = PlayerController.instance.transform.position + PlayerController.instance.GetPlayerMoveDirection() * 2;
            fallCentre.y -= 4;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(currState == scriptState.PLAYER_FALLING){return;}
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if(PlayerController.instance.IsInvulnerable()){return;}
            fallTimer = 0;
            currState = scriptState.PLAYER_FALLING;
            PlayerController.instance.FreezeGameplayInput(true);
            returnPosition = GetNearestSafeNode(player.transform.position);
            startPosition = PlayerController.instance.transform.position;
            PlayerController.instance.Damage(10);
            PlayerController.instance.Protect(fallDuration);
            fallCentre = PlayerController.instance.transform.position + PlayerController.instance.GetPlayerMoveDirection() * 2;
            fallCentre.y -= 4;
        }
    }
    private Vector3 GetNearestSafeNode(Vector3 playerPos)
    {
        EQSNode[,] nodes = EQS.instance.GetNodes();
        Vector3 offset = transform.position - playerPos;
        Vector3 closestNode = playerPos - offset; //Set it as this in case no valid spot can be found
        float distance = 0, closest = float.MaxValue;
        foreach(EQSNode node in nodes)
        {
            if(!node.GetTraversable()){continue;}
            distance = Vector3.Distance(playerPos, node.GetWorldPos());
            if(distance < closest)
            {
                closest = distance;
                closestNode = node.GetWorldPos();
            }
        }
        return closestNode;
    }
}
