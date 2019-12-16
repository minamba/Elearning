using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

namespace ELearning.Utilities
{
    public class SendMail
    {
        public void SenEmail(string mailfrom,string mailto,string title, string message)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(mailfrom);
                msg.To.Add(new MailAddress(mailto));
                msg.Subject = "Malik Ibn Anas - PlateForm E-learning";

                msg.Body = message;
                var client = new SmtpClient
                {
                    //Host = "smtp.gmail.com",
                    //Port = 587,
                    //EnableSsl = true,
                    //DeliveryMethod = SmtpDeliveryMethod.Network,
                    //UseDefaultCredentials = false,
                    //Timeout = 30 * 1000,
                    //Credentials = new NetworkCredential("minamba.c@gmail.com", "minamba19882011")
                };
                client.Send(msg);
                //ScriptManager.RegisterStartupScript(this,this.GetType(), "popup", "alert('Votre message a bien été envoyé');",true);
            }
            catch(Exception e)
            {
              
            }

        }
    }
}