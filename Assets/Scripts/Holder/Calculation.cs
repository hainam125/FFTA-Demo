using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calculation {
  public static int Dam(Player attacker, Player target){
    return 5;//attacker.Atk - target.Def;
  }


  public static int Hit(Player attacker, Player target){
    return 100;//attacker.Speed - target.Evade;
  }
}
