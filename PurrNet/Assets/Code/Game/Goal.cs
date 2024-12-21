using PurrNet;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    [SerializeField] Team team;

    public UnityEvent<Team, PlayerID> OnGoalScored = new UnityEvent<Team, PlayerID>();

    public void Score(Ball ball)
    {
        OnGoalScored?.Invoke(team, ball.GetLastOwner());
    }
}
