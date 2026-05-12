using UnityEngine;

public class BlackSheep : Sheep
{
    private MyTimer timer;
    [SerializeField] private int secondsDeduced = 10;

    protected override void Start()
    {
        base.Start();
        timer = GameObject.Find("Timer").GetComponent<MyTimer>();
    }

    protected override void ReachedGoal()
    {
        timer.SubtractFromTimer(secondsDeduced);
        base.ReachedGoal();
    }
}
