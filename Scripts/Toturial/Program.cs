#region GenericsAndInterfaces
/*
using System.Collections.Generic;


var warrior = new Warrior()
{
    name = "Hamza",
    damage = 0,
};

  
var helper = new HeroHelper<Warrior>(warrior);

helper.Print();
helper.ForceHeroToAttack();

T HeroFactory<T>(string heroName) where T : Hero, new() // new() constraint means whatever we send this method it will have constructor with no parameter
{
    T newHero = new T();
    newHero.name = heroName;
    return newHero;
}

var archer = HeroFactory<Archer>("dasdsa");


/// one function to perform different logic all the classes
void PingMap<T>(T inputObject)  where T : IPing 
{
    inputObject.PingMap();
}
*/
#endregion