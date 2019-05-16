using Enumerations;

public class Module : Storable
{
    public float hp;
    public float armor;
    public MODULE type;

    public virtual void Update()
    {
        return;
    }

    public virtual bool IsUpdateable()
    {
        return false;
    }

}
