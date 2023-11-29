using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonsController : MonoBehaviour
{
    [SerializeField] private Button buttonLeft;
    [SerializeField] private Button buttonRight;
    [SerializeField] private Button buttonUp;
    [SerializeField] private Button buttonDown;

    private Player player;
    
    private void Awake()
    {
        buttonLeft.onClick.AddListener(() => SetPlayerDirection(Player.Direction.Left));
        buttonRight.onClick.AddListener(() => SetPlayerDirection(Player.Direction.Right));
        buttonUp.onClick.AddListener(() => SetPlayerDirection(Player.Direction.Up));
        buttonDown.onClick.AddListener(() => SetPlayerDirection(Player.Direction.Down));
    }
    private void Start()
    {
        player = GameManager.Instance.Player;
        player.OnPlayerMovingChanged += OnPlayerMovingChanged;
    }
    
    private void OnPlayerMovingChanged(object sender, EventArgs e)
    {
        //Enable/Disable Buttons
        ToggleButtons(!player.IsMoving);
    }

    private void ToggleButtons(bool val)
    {
        buttonLeft.enabled = val;
        buttonRight.enabled = val;
        buttonUp.enabled = val;
        buttonDown.enabled = val;
    }

    private void SetPlayerDirection(Player.Direction dir)
    {
        if (!player.IsMoving)
        {
            player.LastMoveDirection = dir;
        }
    }
    


    }
}