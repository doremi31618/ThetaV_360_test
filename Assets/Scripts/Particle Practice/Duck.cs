using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Duck <see langword="abstract"/> .
/// </summary>
public abstract class Duck {
    protected FlyBehavior flyBehavior;
    protected QuackBehavior quackBehavior;
    public void performQuack()
    {
        quackBehavior.Quack();
    }

    public void performFly()
    {
        flyBehavior.fly();
    }
    public void swim()
    {
        Debug.Log("All Duck Can Swim");
    }
    public void setQuackBehavior(QuackBehavior qb)
    {
        quackBehavior = qb;
    }
    public void setFlyBehavior(FlyBehavior fb)
    {
        flyBehavior = fb;
    }

    
}
public abstract class FlyBehavior{
   public abstract void fly();
}
public abstract class  QuackBehavior{
    public abstract void Quack();
}
public class FlyWithWings : FlyBehavior
{
    public override void fly()
    {
        Debug.Log("Im flying!!");
    }
}
public class FlyNoWay : FlyBehavior
{
    public override void fly()
    {
        Debug.Log("I Cant fly");
    }
}
public class MuteQuack : QuackBehavior 
{
    public override void Quack()
    {
        Debug.Log("<Silence>");
    }
}
public class Squeak : QuackBehavior
{
    public override void Quack()
    {
        Debug.Log("Squack");
    }
}
