namespace Diplomas.Public.API.Infrastructure.ErrorHandling
{
    /// <summary>
	/// Object that holds a single Validation Error for the business object
	/// </summary>
	public class ValidationError
    {

        /// <summary>
        /// The error message for this validation error.
        /// </summary>
        public string Message
        {
            get
            {
                return cMessage;
            }
            set
            {
                cMessage = value;
            }
        }
        string cMessage = "";

        /// <summary>
        /// The name of the field that this error relates to.
        /// </summary>
        public string ControlID
        {
            get { return this.cFieldName; }
            set { cFieldName = value; }
        }
        string cFieldName = "";

        /// <summary>
        /// An ID set for the Error. This ID can be used as a correlation between bus object and UI code.
        /// </summary>
        public string ID
        {
            get { return cID; }
            set { cID = value; }
        }
        string cID = "";

        public ValidationError() : base() { }
        public ValidationError(string message)
        {
            Message = message;
        }
        public ValidationError(string message, string fieldName)
        {
            Message = message;
            ControlID = fieldName;
        }
        public ValidationError(string message, string fieldName, string id)
        {
            Message = message;
            ControlID = fieldName;
            ID = id;
        }

    }
}
