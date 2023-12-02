using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIButtonsController : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite validSprite;
    [SerializeField] private Sprite invalidSprite;
    
    [Header("Buttons")]
    [SerializeField] private Button[] buttons; // left right up down

    private Player player;

    private void Awake()
    {
        if (buttons.Length != 4)
        {
            Debug.LogError("There should be 4 buttons in the array.");
            return;
        }
        
        buttons[0].onClick.AddListener(() => SetPlayerDirection(MoveDirection.Left));
        buttons[1].onClick.AddListener(() => SetPlayerDirection(MoveDirection.Right));
        buttons[2].onClick.AddListener(() => SetPlayerDirection(MoveDirection.Up));
        buttons[3].onClick.AddListener(() => SetPlayerDirection(MoveDirection.Down));
    }
    private void Start()
    {
        player = GameManager.Instance.Player;
        player.DoUIButtonChange += DoUIButtonChange;
    }
    
    private void SetPlayerDirection(MoveDirection dir)
    {
        player.LastMoveDirection = dir;
    }

    private void DoUIButtonChange(object sender, Player.DoUIButtonChangeEventArgs e)
    {
        if (e.Direction == MoveDirection.None)
        {
            Debug.LogError("Direction cannot be none.");
            return;
        }

        buttons[(int)e.Direction - 1].image.sprite = e.InputValid ? validSprite : invalidSprite;

        if (e.InputValid)
        {
            Invoke(nameof(ResetButtonColors), 0.5f);
        }
    }

    private void ResetButtonColors()
    {
        foreach (var button in buttons)
        {
            button.image.sprite = normalSprite;
        }
    }

}
