using System;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{

   [SerializeField] private Button _chillButton;
   [SerializeField] private Button _proGamerButton;

   public void Start()
   {
      if (SaveManager.instance.Data.Difficulty == Difficulty.Chill)
      {
         _chillButton.Select();
      } else if (SaveManager.instance.Data.Difficulty == Difficulty.ProGamer)
      {
         _proGamerButton.Select();
      }
   }

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
