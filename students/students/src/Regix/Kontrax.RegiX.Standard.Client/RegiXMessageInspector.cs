using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;

namespace Kontrax.RegiX.Standard.Client
{

    public class RegiXMessageInspector : IClientMessageInspector
    {
        private static readonly bool isLogEnabled;
        private static readonly string logDirectory;


        static RegiXMessageInspector()
        {
            bool.TryParse(ConfigurationManager.AppSettings["RegiXLogEnabled"], out isLogEnabled);
            logDirectory = ConfigurationManager.AppSettings["RegiXLogDirectory"];
        }

        #region Комуникация на message inspector-а с външния свят

        /// <summary>
        /// използваме AsyncLocal<T> вместо [ThreadStatic], защото ползваме стойността в async метод
        /// https://stackoverflow.com/questions/42507054/threadstatic-in-asynchronous-asp-net-web-api
        /// https://docs.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1?view=netcore-3.1#moniker-applies-to
        /// </summary>
        private static AsyncLocal<Guid> randomCallId;

        public static Guid BeforeCall()
        {
            randomCallId = new AsyncLocal<Guid>();
            randomCallId.Value = Guid.NewGuid();
            return randomCallId.Value;
        }

        /// <summary>
        /// Това е начин да се върне информация навън.
        /// </summary>
        //private static readonly Dictionary<Guid, string> rawResponses = new Dictionary<Guid, string>();
        private static readonly Dictionary<Guid, RawMessage> rawRequestMessages = new Dictionary<Guid, RawMessage>();
        private static readonly Dictionary<Guid, RawMessage> rawResponseMessages = new Dictionary<Guid, RawMessage>();


        //public static string AfterCall(Guid callId)
        //{
        //    rawResponses.TryGetValue(callId, out string rawResponse);
        //    rawResponses.Remove(callId);
        //    return rawResponse;
        //}

        public static void AfterCallAll(Guid callId, ref RawMessage request, ref RawMessage response)
        {
            rawRequestMessages.TryGetValue(callId, out RawMessage requestMessage);
            request = requestMessage;
            rawRequestMessages.Remove(callId);

            rawResponseMessages.TryGetValue(callId, out RawMessage responseMessage);
            response = responseMessage;
            rawResponseMessages.Remove(callId);

            return;
        }
        #endregion

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            #region log
            using (MessageBuffer buffer = request.CreateBufferedCopy(int.MaxValue))
            {
                Message msgToLog = buffer.CreateMessage();
                if (isLogEnabled && !string.IsNullOrEmpty(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                    string fileName = $"{DateTime.Now.ToString("yyMMdd_HHmmssfff")}_RegiXRawRequest.xml";
                    string filePath = Path.Combine(logDirectory, fileName);
                    //using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    XmlWriterSettings settings = new XmlWriterSettings();
                    //    settings.Encoding = Encoding.UTF8;
                    //    XmlWriter writer = XmlWriter.Create(stream, settings);
                    //    msgToLog.WriteMessage(writer);

                    //}

                    //fileName = $"{DateTime.Now.ToString("yyMMdd_HHmmssfff")}_RegiXRawRequest_UTF8.xml";
                    //filePath = Path.Combine(logDirectory, fileName);
                    File.WriteAllText(filePath, request.ToString(), Encoding.UTF8);

                }

                request = buffer.CreateMessage();
            }
            #endregion

            // Върнатият тук обект се подава като correlationState в AfterReceiveReply.
            Guid callId = randomCallId.Value;
            randomCallId = new AsyncLocal<Guid>();
            randomCallId.Value = Guid.Empty;

            rawRequestMessages.Add(callId, new RawMessage(request.Headers.MessageId, request.Headers.RelatesTo, request.ToString()));


            return callId;
        }


        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            Guid callId = (Guid)correlationState;

            using (MessageBuffer buffer = reply.CreateBufferedCopy(int.MaxValue))
            {
                // Debug log на суровите байтове, още преди отговорът да е конвертиран към string и кеширан.
                if (isLogEnabled && !string.IsNullOrEmpty(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                    string fileName = $"{DateTime.Now.ToString("yyMMdd_HHmmssfff")}_RegiXRawResponse.xml";
                    string filePath = Path.Combine(logDirectory, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        buffer.WriteMessage(stream);
                    }
                }

                // Кеширане на суровия резултат като string.
                //string rawResponse;
                //using (MemoryStream stream = new MemoryStream())
                //{
                //    buffer.WriteMessage(stream);
                //    stream.Position = 0;
                //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                //    {
                //        rawResponse = reader.ReadToEnd();
                //    }
                //}
                //rawResponses.Add(callId, rawResponse);
                //rawResponseMessages.Add(callId, new RawMessage(reply.Headers.RelatesTo, rawResponse));
                rawResponseMessages.Add(callId, new RawMessage(reply.Headers.MessageId, reply.Headers.RelatesTo, reply.ToString()));

                // Тъй като CreateBufferedCopy по-горе прочита съобщението, а то може да бъде прочетено само веднъж,
                // тук се създава и връща копие на съобщението, за да може WCF да продължи работа с копието.
                reply = buffer.CreateMessage();
            }
        }
    }
}
