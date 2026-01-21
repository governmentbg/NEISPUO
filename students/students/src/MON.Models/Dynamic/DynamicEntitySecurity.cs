namespace MON.Models.Dynamic
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class DynamicEntitySecurity
    {
        [Required]
        public RequredPermission RequiredRermissions { get; set; }
    }

    public class RequredPermission
    {
        public HashSet<string> Read { get; set; }
        public HashSet<string> Create { get; set; }
        public HashSet<string> Update { get; set; }
        public HashSet<string> Delete { get; set; }
    }
}
