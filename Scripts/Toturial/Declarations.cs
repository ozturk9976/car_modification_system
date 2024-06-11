using System;

public class HeroHelper<T> where T : Hero
{
    public T Data;

    public HeroHelper(T data) {    
        Data = data;
    }

    public void Print() {
        Console.WriteLine();
    }

    public void ForceHeroToAttack() {
        Data.Attack();
    }
}

public abstract class Hero
{
    public int damage;
    public string name;

    public void Attack() 
    {

    }
}

public class Mage : Hero 
{

}

public class Warrior : Hero 
{

}

public class Archer : Hero 
{

}

public interface IPing 
{
    void PingMap();
}

public class Anvil : IPing
{
    public void PingMap()
    {
        throw new NotImplementedException();
    }
}

public class Mailbox : IPing
{
    public void PingMap() 
    {
        throw new NotImplementedException();
    }
}
