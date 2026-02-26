namespace TaoTie
{
    public class Animal: Unit,IEntity<int>
    {
        public override EntityType Type => EntityType.Animal;

        #region IEntity

        public void Init(int id)
        {
            ConfigId = id;
            AddComponent<GameObjectHolderComponent>();
        }

        public void Destroy()
        {
            
        }

        #endregion
    }
}