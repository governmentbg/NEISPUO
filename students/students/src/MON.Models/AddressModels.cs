namespace MON.Models
{
    public class AddressModel
    {
        public int? Id { get; set; }
        public int CityId { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Neighborhood { get; set; }
        public string Block { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string Appartment { get; set; }
    }

    public class AddressViewModel : AddressModel
    {
        public string City { get; set; }
        public string Municipality { get; set; }
        public string District { get; set; }
        public DropdownViewModel DropdownModel { get; set; }
    }
}
