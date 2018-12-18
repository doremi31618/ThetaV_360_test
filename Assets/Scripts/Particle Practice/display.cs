using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MallardDuck : Duck
{
    public MallardDuck()
    {
        quackBehavior = new MuteQuack();
        flyBehavior = new FlyWithWings();
        performFly();
        performQuack();
        setFlyBehavior(new FlyNoWay());
        performFly();
    }
}
public class display : MonoBehaviour
{
    MallardDuck mallardDuck;
    private void Start()
    {
        mallardDuck = new MallardDuck();
    }
}

