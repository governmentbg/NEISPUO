namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public class XsdModel
    {
        // Не се ползва във view, а само за пренос на пътя до схемата към метода за валидиране на XML.
        public string Path { get; set; }

        public MultiNodeModel Root { get; set; }

        #region Debug информация преди submit-ването на заявката

        public string RootWarning { get; set; }

        public string Description { get; set; }

        #endregion
    }
}
