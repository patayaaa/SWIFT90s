﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    public Character character;
    public string PlayerName => character.PlayerName;
    public int TeamIndex { get; private set; }
    public Team Team => TeamManager.Instance.teams[TeamIndex];

    [Header("Inputs")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode dodgeKey = KeyCode.Mouse1;
    private bool inputEnabled = true;

    private void Start()
    {
        character.Initialize(this);
        if (!hasAuthority)
        {
            character.enabled = false;

            GetComponent<Rigidbody>().isKinematic = true;
        }

        else
        {
            UIManager.Instance.AssignPlayer(this);
        }

        TeamIndex = TeamManager.Instance.JoinSmallestTeam(this);
    }

    private void Update()
    {
        Inputs();
    }

    public void ToggleInputs(bool on)
    {
        inputEnabled = on;
        if(!on)
        {
            character.InputHorizontal(0);
            character.InputVertical(0);
        }
    }

    private void Inputs()
    {
        if (!inputEnabled) return;

        if(Input.GetKeyDown(jumpKey))
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
}
