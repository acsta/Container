namespace TaoTie
{
    public class Host: Unit,IEntity<int>
    {
        public override EntityType Type => EntityType.Host;

        #region IEntity

        public void Init(int id)
        {
            ConfigId = id;
            AddComponent<GameObjectHolderComponent>();
            var cac = AddComponent<CasualActionComponent>();
            cac.SetEnable(true);
        }

        public void Destroy()
        {
            
        }

        #endregion
    }
}