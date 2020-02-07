namespace WrapAround.Logic.Interfaces
{
    /// <summary>
    /// Represents an entity that can be destroyed (ie blocks)
    /// </summary>
    public interface IDestructable
    {
        int health { get; set; }
        void Damage();

    }


}