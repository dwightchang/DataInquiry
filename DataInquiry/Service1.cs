using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using com;
using com.dsc.kernal.factory;
using com.dsc.kernal.global;
using com.dsc.kernal.utility;
using System.Net;
using System.IO;
using com.dsc.kernal.mail;

namespace JBossService
{
    public partial class Service1 : ServiceBase
    {
        private string ip = "";

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnShutdown()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //System.Threading.Thread.Sleep(20000);
                ip = getIP();
                string m_ServiceName = "JBossService";
                ServiceController service = new ServiceController(m_ServiceName);

                GlobalProperty.Filename = System.AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini";
                IOFactory factory = new IOFactory();
                string DBstr_LandBank = getEncodedProperty("DBstr", "LandBank");
                string DBstr_IntraAP = getEncodedProperty("DBstr", "IntraAP");   

                AbstractEngine LandBank = factory.getEngine("SQL", DBstr_LandBank);
                //string sql = "update BatchLog set status = 1,starttime = getdate(),endtime = null where progname = 'JBossService'";
                //LandBank.executeSQL(sql);
                //com.dsc.kernal.utility.Utility.writeFileLog("C:\\", "finish SQL");

                TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 300);
                

                m_ServiceName = "JBAS50SVC";
                service = new ServiceController(m_ServiceName);

                // 檢查相依性
                ServiceController www = new ServiceController("W3SVC");
                www.WaitForStatus(ServiceControllerStatus.Running, timeout);

                ServiceController[] depends = service.ServicesDependedOn;
                for (int i = 0; i < depends.Length; i++)
                {
                    //log(depends[i].ServiceName);                    
                    while (depends[i].Status.Equals(ServiceControllerStatus.Running) == false)
                    {
                        //log("wait");
                        System.Threading.Thread.Sleep(10000);
                    }
                }

                if (!service.Status.Equals(ServiceControllerStatus.Running))
                {
                    service.Start();                    
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                    // 可能前一次關閉時有問題，造成第一次啟動失敗，再一次關閉後再啟動
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }

                AbstractEngine IntraAP = factory.getEngine("SQL", DBstr_IntraAP);    

                if (checkNaNaWeb() == false)
                {
                    string subject = ip + "工作流程系統流程引擎JBoss服務執行失敗";
                    string msg = ip + "<Font color=\"#FF0000\">工作流程系統流程引擎JBoss服務執行失敗</Font>";
                    processNotice(LandBank, IntraAP, "1", subject, msg);
                   
  
                }
                else
                {
                    string msg = ip + "工作流程系統流程引擎JBoss服務執行成功";
                    processNotice(LandBank, IntraAP, "2", msg, msg);                   
                    
                }
                LandBank.close();                
            }
            catch (Exception ex)
            {
                //com.dsc.kernal.utility.Utility.writeFileLog("C:\\", ex.TargetSite + "服務無法啟動，請檢查相關設定!");
                log(ex.ToString());
                throw ex;
            }
        }

        protected override void OnStop()
        {
            try
            {
                ip = getIP();
                string m_ServiceName = "JBAS50SVC";
                ServiceController service = new ServiceController(m_ServiceName);

                GlobalProperty.Filename = System.AppDomain.CurrentDomain.BaseDirectory + "\\setting.ini";
                IOFactory factory = new IOFactory();
                string DBstr_LandBank = getEncodedProperty("DBstr", "LandBank");
                string DBstr_IntraAP = getEncodedProperty("DBstr", "IntraAP");

                AbstractEngine LandBank = factory.getEngine("SQL", DBstr_LandBank);
                AbstractEngine IntraAP = factory.getEngine("SQL", DBstr_IntraAP);

                // 設定一個 Timeout 時間，若超過 60 秒啟動不成功就宣告失敗!
                TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 60);

                // 若該服務不是「停用」的狀態，才將其停止運作，否則會引發 Exception
                if (service.Status != ServiceControllerStatus.Stopped &&
                    service.Status != ServiceControllerStatus.StopPending)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }

                //string msg = "";
                //if (checkNaNaWeb() == true)
                //{
                //    string subject = ip + "工作流程系統JBoss服務停止失敗";
                //    msg = ip + "<Font color=\"#FF0000\">工作流程系統JBoss服務停止失敗</Font>";
                //    processNotice(LandBank, IntraAP, "2", subject, msg);
                //}
                //else
                //{
                //    msg = ip + "工作流程系統JBoss服務已停止";
                //    processNotice(LandBank, IntraAP, "1", msg, msg);
                //}
            }
            catch (Exception ex)
            {
                // 如果無法停用服務會引發 Exception，也會讓反安裝自動中斷
                //throw new exception("服務無法停用，建議您可以先利用「工作管理員」將 INService.exe 程序結束，再進行解除安裝。");
                //com.dsc.kernal.utility.Utility.writeFileLog("C:\\", ex.TargetSite + "服務無法停用，建議您可以先利用「工作管理員」將 JBossService.exe 程序結束，再進行解除安裝。");
                log(ex.ToString());
                throw ex;
            }
        }

        private string objToStr(object str)
        {
            return (str == null) ? "" : str.ToString();
        }

        private void processNotice(AbstractEngine LandBank, AbstractEngine IntraAP, string pStatus, string pSubject, string pMailContent)
        {
            try
            {
                string sql = "";   
                //sql = string.Format("update BatchLog set status = {0},endtime = getdate() where progname = 'JBossService'", pStatus);
                //LandBank.executeSQL(sql);

                string DBstr_IntraAP = getEncodedProperty("DBstr", "IntraAP");

                //AbstractEngine IntraAP = factory.getEngine("SQL", DBstr_IntraAP);

                sql = "select AJMFA003 from AJMFA where AJMFA001='JBossServiceMail'";
                string sToMail = objToStr(IntraAP.executeScalar(sql));
                if (sToMail == "")
                {
                    throw new Exception("no data found: select AJMFA003 from AJMFA where AJMFA001='JBossServiceMail'");
                }

                sql = "select SMVHAAA004 from SMVHAAA where SMVHAAA002='SystemMail'";
                string sFromMail = objToStr(IntraAP.executeScalar(sql));
                if (sFromMail == "")
                {
                    throw new Exception("no data found: select SMVHAAA004 from SMVHAAA where SMVHAAA002='SystemMail'");
                }

                string[] emails = sToMail.Split(';');
                foreach (string singleEmail in emails)
                {
                    //com.dsc.kernal.utility.MailProcessor.sendMail("127.0.0.1", singleEmail, sFromMail, "差勤系統JBoss服務執行失敗", "差勤系統JBoss服務執行失敗");
                    sendEMail(singleEmail, sFromMail, pSubject, pMailContent);

                }
                //sql = "select sn from Landbank..BatchLog where progname='JBossService'";
                //string sSn = LandBank.executeScalar(sql).ToString();

                //sql = "select max(Seqno) from Landbank..SystemLog where SourceID = '" + sSn + "' and sdate = CONVERT(varchar(12) , getdate(), 111 )";
                //string sSeqno = LandBank.executeScalar(sql).ToString();
                string sSeqno = "";

                int iSeqno = 1;
                if (!String.IsNullOrEmpty(sSeqno))
                {
                    iSeqno = int.Parse(sSeqno);
                    iSeqno++;
                }

                //sql = "insert into Landbank..SystemLog (SourceID,Seqno,sdate,stime,ChgItem,Result) values('" + sSn + "','" + iSeqno.ToString() + "',CONVERT(varchar(12) , getdate(), 111 ),getdate(),'" + pSubject + "','" + pMailContent + "')";
                //LandBank.executeSQL(sql);
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                throw ex;
            }
        }

        private void sendEMail(string sendAdr, string pSenderAddress, string pSubject, string content)
        {
            try
            {
                MailFactory fac = new MailFactory();
                string mailclass = "INBillHunterUtility.INBillHunterUtility";
                AbstractMailUtility au = fac.getMailUtility(mailclass.Split(new char[] { '.' })[0], mailclass);

                au.sendMailHTML("", "127.0.0.1", sendAdr, "", pSenderAddress, pSubject, content);
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                //throw ex;
            }
        }

        private bool checkNaNaWeb()
        {
            System.Net.WebRequest wrGETURL = WebRequest.Create("http://127.0.0.1:8080/NaNaWeb/GP//Authentication?hdnMethod=switchLanguage");            
            //System.Net.WebRequest wrGETURL = WebRequest.Create("http://127.0.0.1:8080/NaNaWeb");            
            System.Threading.Thread.Sleep(40000);
            bool successURLResponse = true;
            //WorkflowServiceService

            try
            {
                Stream objStream = wrGETURL.GetResponse().GetResponseStream();
                
                StreamReader objReader = new StreamReader(objStream, System.Text.Encoding.Default);
                String sContent = objReader.ReadToEnd().Trim();

                //log(sContent);
                
                objReader.Close();
                if (String.IsNullOrEmpty(sContent))
                {
                    successURLResponse = false;                    
                }

                if (sContent.Contains("ErrorPage.jsp"))
                {
                    successURLResponse = false;
                }
            }
            catch (Exception ex)
            {
                //com.dsc.kernal.utility.Utility.writeFileLog("C:\\", ex.Message);
                successURLResponse = false;
            }

            return successURLResponse;
        }

        private void log(string msg)
        {
            string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            com.dsc.kernal.utility.Utility.writeFileLog(System.AppDomain.CurrentDomain.BaseDirectory + "\\", now + " " + msg);
        }

        private string getIP()
        {
            string local = Dns.GetHostName();
            IPHostEntry ipentry = Dns.GetHostByName(local);
            return string.Format("{0}({1})", local, ipentry.AddressList[0].ToString());                
        }

        private string getEncodedProperty(string type, string key)
        {
            return Base64.decodeToString(GlobalProperty.getProperty(type, key));
        }
    }
}
