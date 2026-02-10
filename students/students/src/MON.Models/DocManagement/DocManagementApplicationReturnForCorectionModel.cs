namespace MON.Models.DocManagement
{
    public class DocManagementApplicationReturnForCorectionModel
    {
        public int ApplicationId { get; set; }

        public string Description { get; set; }
    }

    public class DocManagementApplicationResponseModel : DocManagementApplicationReturnForCorectionModel
    {
        public int ParentId { get; set; }
    }

    public class DocManagementExchangeRequestApproveModel : DocManagementApplicationReturnForCorectionModel
    {

    }

    public class DocManagementExchangeRequestRejectModel : DocManagementExchangeRequestApproveModel
    {

    }
}
