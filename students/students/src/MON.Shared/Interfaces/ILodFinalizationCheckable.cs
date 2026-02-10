namespace MON.Shared.Interfaces
{
    /// <summary>
    /// Наличието на този интерфейс ще доведе до преверка за подписан(финализиран) ЛОД.
    /// </summary>
    public interface ILodFinalizationCheckable
    {
        public int PersonId { get; }
        public short SchoolYear { get; }
    }
}
