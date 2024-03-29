﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Outcome
{
    Defeat,
    Victory,
    Draw
}

[System.Serializable]
public class Team
{
    public int index = 0;
    public List<NetworkedPlayer> players = new List<NetworkedPlayer>();
    public int PlayerCount => players.Count;
    public Color Color { get => color; }
    public int Score { get => score; set => score = value; }

    public Outcome outcome;
    //public bool HasWon { get; private set; }
    public int score;
    private Color color;

    public void EarnPoint(int point = 1)
    {
        score += point;
        if (score >= CTFManager.Instance.goalPoints)
        {
            outcome = Outcome.Victory;
            CTFManager.Instance.TeamWins(this);
        }
    }

    public Team(int index, Color color)
    {
        this.index = index;
        this.color = color;
    }

    public void Join(NetworkedPlayer player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            UIManager.Instance.RefreshPortraits();
            //Debug.Log(player.PlayerName + " joined team " + index);
        }
    }

    public void Leave(NetworkedPlayer player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            UIManager.Instance.RefreshPortraits();
            Debug.Log(player.Username + " left team " + index);
        }
    }
}

public class TeamManager : NetworkBehaviour
{
    private static TeamManager instance;
    public static TeamManager Instance
    {
        get { return instance; }
    }


    public static int TeamCount = 2;
    public List<Team> teams = new List<Team>();
    public List<Color> colors = new List<Color>();

    public bool IsDraw
    {
        get
        {
            if(teams.Count > 0)
            {
                int team0Score = teams[0].score;
                for (int i = 0; i < teams.Count; i++)
                {
                    if(teams[i].score != team0Score)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }

    public bool InputEnabled { get; private set; }


    public int GetIndex(NetworkedPlayer player)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].players.Contains(player))
            {
                return i;
            }
        }

        return 0;
    }

    public void Awake()
    {
        instance = this;
        InitializeTeams();
    }

    private void OnEnable()
    {
        ToggleInputs(false);
    }

    private void InitializeTeams()
    {
        teams.Clear();
        for (int i = 0; i < TeamCount; i++)
        {
            teams.Add(new Team(i, colors[i]));
        }
    }

    public int JoinSmallestTeam(NetworkedPlayer player)
    {
        int index = SmallestTeamIndex();
        teams[index]?.Join(player);
        return index;
    }

    public void JoinTeam(int i, NetworkedPlayer player)
    {
        teams[i].Join(player);
    }

    public int GetTeamScore(int i)
    {
        if (teams.Count > i)
            return teams[i].Score;
        return 0;
    }

    public void ToggleInputs(bool on)
    {
        InputEnabled = on;
    }

    public void Score(int i)
    {
        teams[i].EarnPoint();
    }

    public Color GetTeamColor(int i)
    {
        if (teams.Count <= i)
        {
            InitializeTeams();
        }

        if (teams.Count > 0)
        {
            if (i < teams.Count && i >= 0)
                return teams[i].Color;
        }

        return Color.white;
    }

    public Color GetOppositeTeamColor(int i)
    {
        for (int t = 0; t < teams.Count; t++)
        {
            if (t != i)
            {
                return teams[t].Color;
            }
        }
        return Color.white;
    }

    private int SmallestTeamIndex()
    {
        int smallest = 0;
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].PlayerCount < teams[smallest].PlayerCount)
            {
                smallest = i;
            }
        }

        return smallest;
    }
}
