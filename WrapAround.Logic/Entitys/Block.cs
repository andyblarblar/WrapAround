namespace WrapAround.Logic.Entitys
{
    public class Block : IDestructable
    {
        public int health { get; set; }


        public void Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}