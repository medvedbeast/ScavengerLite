namespace Enumerations
{
    public enum INTERACTIVE_TYPE
    {
        NONE = 0,
        LOOT_CONTAINER,
        DOOR
    }
  
    public enum GAME_STATE
    {
        NONE = 0,
        MAIN_MENU,
        LOADING,
        GAME,
        PAUSE
    }

    public enum MODULE
    {
        NONE = 0,
        BODY,
        WEAPON,
        SHIELD,
        UPGRADE
    }

    public enum UNIT_TYPE
    {
        NONE = 0,
        SHIP,
        ROBOT,
        LOOTABLE
    }

    public enum WEAPON_TYPE
    {
        NONE = 0,
        MACHINE_GUN,
        ROCKET_LAUNCHER
    }

    public enum RARITY
    {
        NONE = 0,
        POOR,
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY,
        HEIRLOOM
    }

}
