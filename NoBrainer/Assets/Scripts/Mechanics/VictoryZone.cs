using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Marks a trigger as a VictoryZone, usually used to end the current game level.
    /// </summary>
    public class VictoryZone : MonoBehaviour
    {

        void OnTriggerEnter2D(Collider2D collider)
        {

        SceneLoading.instance.LoadNextScene();
            
        }
    }
}