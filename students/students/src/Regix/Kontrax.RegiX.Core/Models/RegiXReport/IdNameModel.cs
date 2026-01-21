namespace Kontrax.RegiX.Core.TestStandard.Models.RegiXReport
{
    public class IdNameModel
    {
        private readonly int _id;
        private readonly string _name;

        public IdNameModel(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
