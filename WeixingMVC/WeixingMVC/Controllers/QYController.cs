using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Elmah;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP.Sample.CommonService.QyMessageHandlers;
using Senparc.Weixin.QY.Entities;

namespace WeixingMVC.Controllers
{
    /// <summary>
    /// 企业号对接测试
    /// </summary>
    public class QYController : Controller
    {
        public static readonly string Token = "weixin";//与微信企业账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = "mXDemvu2UhlMgIldGhoiAW6epCQM1sb8CkRSWINWsvE";//与微信企业账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string CorpId = "wx5e0f73032fc9109b";//与微信企业账号后台的EncodingAESKey设置保持一致，区分大小写。


        public QYController()
        {

        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信企业后台应用的“修改配置”的Url填写如：http://weixin.senparc.com/qy
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string msg_signature = "", string timestamp = "", string nonce = "", string echostr = "")
        {
            //return Content(echostr); //返回随机字符串则表示验证通过
            var verifyUrl = Senparc.Weixin.QY.Signature.VerifyURL(Token, EncodingAESKey, CorpId, msg_signature, timestamp, nonce,
                echostr);
            if (verifyUrl != null)
            {
                string querystr = "\r\n";
                Error error = new Error();
                error.Message = "微信企业版号验证成功";
                error.HostName = Request.Url.Host;
                error.StatusCode = 100;
                error.Time = DateTime.Now;
                error.User = Environment.UserName;
                error.Type = "微信企业版号";
                querystr += "Token:" + Token + ",msg_signature:" + msg_signature + ",echostr:" + echostr+",verifyUrl:"+verifyUrl;
                error.Detail = "" + querystr;
                error.Source = "QY_Controller";
                Elmah.ErrorLog.Default.Log(error);
                //创建一个异常并写入错误日志，无法自定义错误的类型
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("创建一个异常并写入错误日志" + querystr));


                return Content(verifyUrl); //返回解密后的随机字符串则表示验证通过
            }
            else
            {
                //var fileStream = System.IO.File.OpenWrite(Server.MapPath("~/App_Data/error.txt"));
                //fileStream.Write(Encoding.Default.GetBytes(verifyUrl), 0, Encoding.Default.GetByteCount(verifyUrl));
                //fileStream.Close();
                string querystr = "\r\n";
                Error error = new Error();
                error.Message = "THIS IS TEST  使用Elmah的Error对象";
                error.HostName = Request.Url.Host;
                error.StatusCode = 100;
                error.Time = DateTime.Now;
                error.User = Environment.UserName;
                error.Type = "Elmah 自定义类型";
                error.Detail = "THIS IS TEST  ，创建一个Elmah的Error对象并写错误日志 但没有Server Variables，cookie等信息" + querystr;
                error.Source = "Page_Load";
                Elmah.ErrorLog.Default.Log(error);

                //创建一个异常并写入错误日志，无法自定义错误的类型
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("创建一个异常并写入错误日志" + querystr));
                return Content("如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 微信后台验证地址（使用Post），微信企业后台应用的“修改配置”的Url填写如：http://weixin.senparc.com/qy
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            var maxRecordCount = 10;

            postModel.Token = Token;
            postModel.EncodingAESKey = EncodingAESKey;
            postModel.CorpId = CorpId;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new QyCustomMessageHandler(Request.InputStream, postModel, maxRecordCount);

            if (messageHandler.RequestMessage == null)
            {
                //验证不通过或接受信息有错误

                string querystr = "\r\n";
                Error error = new Error();
                error.Message = "post对象";
                error.HostName = Request.Url.Host;
                error.StatusCode = 100;
                error.Time = DateTime.Now;
                error.User = Environment.UserName;
                error.Type = "post ";

                querystr += "messageHandler.RequestMessage.FromUserName:" + messageHandler.RequestMessage.FromUserName + ",messageHandler.ResponseMessage.ToUserName:"
                    + messageHandler.ResponseMessage.ToUserName;

                error.Detail = "yanzheng ，创建一个Elmah的Error对象并写错误日志 但没有Server Variables，cookie等信息" + querystr;
                error.Source = "Page_Load";
                Elmah.ErrorLog.Default.Log(error);

                //创建一个异常并写入错误日志，无法自定义错误的类型
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("创建一个异常并写入错误日志" + querystr));

            }

            try
            {
                //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                messageHandler.RequestDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_Request_" + messageHandler.RequestMessage.FromUserName + ".txt"));
                //执行微信处理过程
                messageHandler.Execute();
                //测试时可开启，帮助跟踪数据
                messageHandler.ResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_Response_" + messageHandler.ResponseMessage.ToUserName + ".txt"));
                messageHandler.FinalResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_FinalResponse_" + messageHandler.ResponseMessage.ToUserName + ".txt"));

                string querystr = "\r\n";
                Error error = new Error();
                error.Message = "post对象";
                error.HostName = Request.Url.Host;
                error.StatusCode = 100;
                error.Time = DateTime.Now;
                error.User = Environment.UserName;
                error.Type = "post ";

                querystr += "messageHandler.RequestMessage.FromUserName:" + messageHandler.RequestMessage.FromUserName + ",messageHandler.ResponseMessage.ToUserName:"
                    + messageHandler.ResponseMessage.ToUserName;

                error.Detail = "THIS IS TEST  ，创建一个Elmah的Error对象并写错误日志 但没有Server Variables，cookie等信息" + querystr;
                error.Source = "Page_Load";
                Elmah.ErrorLog.Default.Log(error);

                //创建一个异常并写入错误日志，无法自定义错误的类型
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("创建一个异常并写入错误日志" + querystr));


                //自动返回加密后结果
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
            }
            catch (Exception ex)
            {
                using (TextWriter tw = new StreamWriter(Server.MapPath("~/App_Data/Qy_Error_" + DateTime.Now.Ticks + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.FinalResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.FinalResponseDocument.ToString());
                    }
                    tw.Flush();
                    tw.Close();
                }
                return Content("");
            }
        }

        /// <summary>
        /// 这是一个最简洁的过程演示
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MiniPost(PostModel postModel)
        {
            var maxRecordCount = 10;

            postModel.Token = Token;
            postModel.EncodingAESKey = EncodingAESKey;
            postModel.CorpId = CorpId;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new QyCustomMessageHandler(Request.InputStream, postModel, maxRecordCount);
            //执行微信处理过程
            messageHandler.Execute();
            //自动返回加密后结果
            return new FixWeixinBugWeixinResult(messageHandler);
        }
    }
}