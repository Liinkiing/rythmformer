using System;
using UnityEngine;

public class DifficultySelector : MonoBehaviour
{

   public void ChangeDifficulty(string difficulty)
   {
      if (Enum.IsDefined(typeof(Difficulty), difficulty))
      {
         foreach (Difficulty DIFFICULTY in Enum.GetValues(typeof(Difficulty)))
         {
            if (difficulty == DIFFICULTY.ToString())
            {
               GameManager.instance.ChangeDifficulty(DIFFICULTY);
            }
         }
      }
      else
      {
         Debug.Log($"You want to change difficulty to '{difficulty}' which is not defined in Difficulty enum");
      }
      
      
      
   }
}
