﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    [SyncVar]
    public int teamIndex;

    public Character character;
    public NetworkAnimator animator;
    public string PlayerName => character.PlayerName;
    public Team Team => TeamManager.Instance.teams[teamIndex];

    [Header("Inputs")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode dodgeKey = KeyCode.Mouse1;
    private bool inputEnabled => TeamManager.Instance.InputEnabled;

    Vector3 spawnPoint;

    private void Start()
    {
        character.Initialize(this);
        if (!hasAuthority)
        {
            character.enabled = false;
            character.UpdateTexture();

            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            spawnPoint = transform.position;

            UIManager.Instance.AssignPlayer(this);

            //character.animator.onAttackEndAnim += () => animator.SetTrigger("Attack");
            character.animator.onDoubleJumpAnim += () => animator.SetTrigger("DoubleJump");
            character.animator.onJumpAnim += () => animator.SetTrigger("Jump");
            character.animator.onLandAnim += () => animator.SetTrigger("Land");
            character.animator.onDeathAnim += () => animator.SetTrigger("Death");
            character.animator.onDodgeAnim += () => animator.SetTrigger("Dodge");
        }

        TeamManager.Instance.JoinTeam(teamIndex, this);
    }

    private void Update()
    {
        if (!hasAuthority) return;

        Inputs();
    }
    

    private void Inputs()
    {
        if (!inputEnabled) return;

        if (Input.GetKeyDown(jumpKey))
        {
            character.ReceiveJumpInput();
        }

        if (Input.GetKeyDown(attackKey))
        {
            character.StartAttack();
        }

        if (Input.GetKeyDown(dodgeKey))
        {
            character.Dodge();
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        character.InputHorizontal(horizontal);
        character.InputVertical(vertical);
    }

    private void OnDestroy()
    {
        Team.Leave(this);
    }

    [Command]
    public void CmdKillPlayer(NetworkIdentity killerID, NetworkIdentity victimID)
    {
        Debug.Log(victimID);
        RpcKillPlayer(killerID, victimID);
    }

    [ClientRpc]
    public void RpcKillPlayer(NetworkIdentity killerID, NetworkIdentity victimID)
    {
        character.Kill(victimID.gameObject.GetComponent<Character>());
    }
}
