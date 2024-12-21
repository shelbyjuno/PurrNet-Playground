using PurrNet;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    [SerializeField] TeamManager.Team team;

    public UnityEvent<TeamManager.Team, PlayerID> OnGoalScored = new UnityEvent<TeamManager.Team, PlayerID>();

    public void Score(Ball ball)
    {
        OnGoalScored?.Invoke(team, ball.GetLastOwner());
    }
}
