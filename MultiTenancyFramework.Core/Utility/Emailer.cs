using MultiTenancyFramework.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MultiTenancyFramework
{
    public static class Emailer
    {
        private static EmailAndSmtpSetting EmailAndSmtpSetting;
        private static string ApplicationName;
        private static bool EmailLogMessages;
        private static ILogger Logger;
        static Emailer()
        {
            var settings = Utilities.SystemSettings;
            EmailAndSmtpSetting = settings.EmailAndSmtpSetting;
            ApplicationName = settings.ApplicationName;
            EmailLogMessages = settings.EmailLogMessages;

            Logger = Utilities.Logger;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This is the client as setup in the Logging section of the config file
        /// </summary>
        /// <returns></returns>
        public static SmtpClient GetDefaultClient()
        {
            SmtpClient client = new SmtpClient
            {
                EnableSsl = EmailAndSmtpSetting.EnableSSL,
                UseDefaultCredentials = false,
                Host = EmailAndSmtpSetting.SmtpHost,
                Port = EmailAndSmtpSetting.SmtpPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(EmailAndSmtpSetting.SmtpUsername, EmailAndSmtpSetting.SmtpPassword),
            };
            return client;
        }

        public static bool EmailLogMessage(string logMessage, bool isInfo)
        {
            if (!EmailLogMessages || string.IsNullOrWhiteSpace(logMessage)) return true;

            string subject;
            if (isInfo)
            {
                subject = string.Format("INFORMATION FROM {0} - {1}", ApplicationName, DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"));
            }
            else
            {
                subject = string.Format("ERROR ON {0} - {1}", ApplicationName, DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"));
            }
            return SendMailWithDefaultCredentials(logMessage, subject);
        }

        public static bool SendMailWithDefaultCredentials(string body, string subject = null)
        {
            var client = GetDefaultClient();
            bool success;
            using (client)
            {
                success = SendEmail(EmailAndSmtpSetting.DefaultEmailReceiver, body, subject ?? EmailAndSmtpSetting.DefaultEmailSubject, client, EmailAndSmtpSetting.SmtpUsername, false, EmailAndSmtpSetting.DefaultSenderDisplayName);
            }
            return success;
        }

        public static bool SendEmail(MailMessage msg, SmtpClient client)
        {
            try
            {
                if (msg != null)
                {
                    if (client == null) client = GetDefaultClient();
                    ServicePointManager.ServerCertificateValidationCallback =
                        delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                        { return true; };

                    client.Send(msg);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(new ApplicationException("[Swallowed] error sending mail: ", ex));
            }
            return false;
        }

        public static bool SendEmail(string toemail, string body, string subject, SmtpClient client, string mailFrom, bool isBodyHtml = true, string displayName = null)
        {
            return SendEmail(mailFrom, body, subject, client, new[] { toemail }, null, null, isBodyHtml, displayName);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="senderEmail">The sender email. If null, we use the default.</param>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject. If null, we use the default.</param>
        /// <param name="client">The client. If null, we use the default.</param>
        /// <param name="toEmails">To emails.</param>
        /// <param name="ccEmails">The cc emails.</param>
        /// <param name="bccEmails">The BCC emails.</param>
        /// <param name="isBodyHtml">if set to <c>true</c> [is body HTML].</param>
        /// <param name="displayName">The sender display name. If null, we use the default.</param>
        /// <returns></returns>
        public static bool SendEmail(string senderEmail, string body, string subject, SmtpClient client, string[] toEmails, string[] ccEmails, string[] bccEmails, bool isBodyHtml = true, string displayName = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(body)) return false;
                if (string.IsNullOrWhiteSpace(senderEmail))
                    senderEmail = EmailAndSmtpSetting.DefaultEmailSender ?? EmailAndSmtpSetting.SmtpUsername;

                if (string.IsNullOrWhiteSpace(senderEmail)) return false;
                if (toEmails == null || toEmails.Length == 0) return false;

                //ServicePointManager.ServerCertificateValidationCallback =
                //    delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                //    System.Security.Cryptography.X509Certificates.X509Chain chain,
                //    System.Net.Security.SslPolicyErrors sslPolicyErrors)
                //    { return true; };

                ServicePointManager.ServerCertificateValidationCallback = (obj, cert, chain, policy) => true;

                var msg = new MailMessage()
                {
                    IsBodyHtml = isBodyHtml,

                };
                msg.Sender = msg.From = string.IsNullOrWhiteSpace(displayName) ? new MailAddress(senderEmail) : new MailAddress(senderEmail, displayName);
                if (!string.IsNullOrWhiteSpace(body) && !string.IsNullOrWhiteSpace(body.Trim()))
                {
                    msg.Body = body;
                }
                foreach (var toemail in toEmails)
                {
                    msg.To.Add(toemail);
                }
                if (ccEmails != null && ccEmails.Length > 0)
                {
                    foreach (var email in ccEmails)
                    {
                        msg.CC.Add(new MailAddress(email));
                    }
                }
                if (bccEmails != null && bccEmails.Length > 0)
                {
                    foreach (var email in bccEmails)
                    {
                        msg.Bcc.Add(new MailAddress(email));
                    }
                }
                if (!string.IsNullOrWhiteSpace(subject))
                {
                    msg.Subject = subject;
                }

                using (msg)
                {
                    return SendEmail(msg, client);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
        }

        public static bool SendEmail(string templateUrl, SmtpClient client, Dictionary<string, object> entities, string fromEmailAddress, string toEmailAddresses, string displayName = null, string ccEmailAddresses = null, string bccEmailAddresses = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmailAddresses)) return false;

                var mailValues = GetEmailSubjectAndBodyFromTemplate(templateUrl, entities);

                var to = toEmailAddresses.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] cc = null, bcc = null;
                if (!string.IsNullOrWhiteSpace(ccEmailAddresses))
                {
                    cc = ccEmailAddresses.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (!string.IsNullOrWhiteSpace(bccEmailAddresses))
                {
                    bcc = bccEmailAddresses.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                }

                return SendEmail(fromEmailAddress, mailValues[1], mailValues[0], client, to, cc, bcc, true, displayName);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
        }

        public static string ConstructMailBody(string path, Dictionary<string, string> keyValuePairs)
        {
            string mailBody = File.ReadAllText(path);

            foreach (KeyValuePair<string, string> key in keyValuePairs)
            {
                mailBody.Replace(string.Format("<%", key.Key, "%>"), key.Value);
            }
            return mailBody;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">The url of the mail template</param>
        /// <param name="entities"></param>
        /// <returns>A list of two items: the first is the Subject and the second is the Body</returns>
        private static string[] GetEmailSubjectAndBodyFromTemplate(string url, Dictionary<string, object> entities)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            string mailBody = reader.ReadToEnd();
            foreach (KeyValuePair<string, object> entity in entities)
            {
                mailBody = TransformMessage(mailBody, entity.Value, entity.Key);
            }

            Match subjectMatch = Regex.Match(mailBody, @"<title>[0-9a-zA-Z -]*</title>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string subject = "";
            if (subjectMatch.Success)
            {
                subject = Regex.Replace(subjectMatch.Value, "(<title>|</title>)", "",
                                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            return new string[] { subject, mailBody };
        }

        public static List<string> GetEmailValues(string path, object entity, string toEmail, SmtpClient client, string mailFrom)
        {
            List<string> toReturn = new List<string>();

            WebRequest request = WebRequest.Create(path);
            WebResponse response = request.GetResponse();
            StreamReader reader;
            try
            {
                reader = new StreamReader(response.GetResponseStream());
            }
            catch
            {
                return toReturn;
            }

            var mailBody = reader.ReadToEnd();

            mailBody = Regex.Replace(mailBody, @"#\S*#", x =>
            {
                PropertyInfo pInfo = entity.GetType().GetProperty(x.Value.Trim('#'), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                if (pInfo != null && pInfo.CanRead)
                {
                    if (pInfo.PropertyType.IsPrimitive || pInfo.PropertyType == typeof(DateTime) || pInfo.PropertyType == typeof(String))
                    {
                        return Convert.ToString(pInfo.GetValue(entity, null));
                    }
                    else
                    {
                        var subEntity = pInfo.GetValue(entity, null) as IEntity;
                        if (subEntity != null)
                            return subEntity.Name;
                        return string.Empty;
                    }
                }
                return string.Empty;
            });


            Match subjectMatch = Regex.Match(mailBody, @"<title>\D*</title>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string subject = string.Empty;
            if (subjectMatch.Success) subject = Regex.Replace(subjectMatch.Value, "(<title>|</title>)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            //SendEmail(Toemail, mailBody, subject, client, mailFrom);

            toReturn.Add(toEmail);
            toReturn.Add(mailBody);
            toReturn.Add(subject);

            return toReturn;
        }

        private static string TransformMessage(string mailBody, object entity, string prefix)
        {
            mailBody = Regex.Replace(mailBody.ToString(), string.Format(@"#{0}\.\S*#", prefix), x =>
            {
                PropertyInfo pInfo = entity.GetType().GetProperty(x.Value.Trim('#').Replace(prefix + ".", ""), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                if (pInfo != null && pInfo.CanRead)
                {
                    if (pInfo.PropertyType.IsPrimitive || pInfo.PropertyType == typeof(DateTime) || pInfo.PropertyType == typeof(String))
                    {
                        return Convert.ToString(pInfo.GetValue(entity, null));
                    }
                    else
                    {
                        var subEntity = pInfo.GetValue(entity, null) as IEntity;
                        if (subEntity != null)
                            return subEntity.Name;
                        return string.Empty;
                    }
                }
                return string.Empty;
            });

            return mailBody;
        }

    }

}
