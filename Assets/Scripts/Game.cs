using UnityEngine;
using System.Collections;
using Enumerations;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Linq;

public class Game : MonoBehaviour
{
#if UNITY_EDITOR
    public static string pathToDatabase = @"\Assets\Resources\Database";
#endif
#if !UNITY_EDITOR
    public static string pathToDatabase = @"\\";
#endif

    public static GAME_STATE gameState = GAME_STATE.MAIN_MENU;
    public static GameObject player;
    public static Dictionary<int, Storable> loadedPresets;

    public List<GameObject> units;

    public Game()
    {
        units = new List<GameObject>();
        loadedPresets = new Dictionary<int, Storable>();
    }

    void Start()
    {
        Time.timeScale = 0;
        if (player == null)
        {
            player = units.Where(e => e.name == "Player").FirstOrDefault();
        }
    }
}
