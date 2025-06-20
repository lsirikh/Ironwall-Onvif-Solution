using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Security
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 12/14/2023 11:46:49 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SoapSecurityHeader : MessageHeader
    {

        #region - Ctors -
        public SoapSecurityHeader(string username, string password, TimeSpan timeShift)
        {
            this.username = username;
            this.password = password;
            time_shift = timeShift;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            var nonce = GetNonce();
            var created = DateTime.UtcNow.Add(time_shift).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var nonce_str = Convert.ToBase64String(nonce);
            string password_hash = PasswordDigest(nonce, created, password);

            writer.WriteStartElement("UsernameToken");

            writer.WriteStartElement("Username");
            writer.WriteValue(username);
            writer.WriteEndElement();

            writer.WriteStartElement("Password");
            writer.WriteAttributeString("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest");
            writer.WriteValue(password_hash);
            writer.WriteEndElement();

            writer.WriteStartElement("Nonce");
            writer.WriteAttributeString("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
            writer.WriteValue(nonce_str);
            writer.WriteEndElement();

            writer.WriteStartElement("Created");
            writer.WriteXmlnsAttribute("", ns_wsu);
            writer.WriteValue(created);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("", Name, Namespace);
            writer.WriteAttributeString("s", "mustUnderstand", "http://www.w3.org/2003/05/soap-envelope", "1");
            writer.WriteXmlnsAttribute("", Namespace);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        string PasswordDigest(byte[] nonce, string created, string secret)
        {
            byte[] buffer = new byte[nonce.Length + Encoding.ASCII.GetByteCount(created + secret)];

            nonce.CopyTo(buffer, 0);
            Encoding.ASCII.GetBytes(created + password).CopyTo(buffer, nonce.Length);

            return Convert.ToBase64String(SHA1.Create().ComputeHash(buffer));
        }

        byte[] GetNonce()
        {
            byte[] nonce = new byte[0x10];
            var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(nonce);
            return nonce;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public override string Name { get; } = "Security";
        public override string Namespace { get; } = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        #endregion
        #region - Attributes -
        const string ns_wsu = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        readonly string username;
        readonly string password;
        readonly TimeSpan time_shift;
        #endregion
    }
}
