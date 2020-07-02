using Microsoft.IdentityServer.Web.Authentication.External;

namespace RekTec.Crm.AdfsCaptcha.MFAadapter
{
    /// <summary>
    /// 登录页面
    /// </summary>
    public class CustomPresentationForm : IAdapterPresentationForm
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        private string errorMssg { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        private string phoneMssg { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="phone">手机号码消息</param>
        /// <param name="errorMssg">错误信息</param>
        public CustomPresentationForm(string phoneMssg, string errorMssg = "")
        {
            this.errorMssg = errorMssg;
            this.phoneMssg = phoneMssg;
        }

        /// <summary>
        /// 验证页面 body 里的内容
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public string GetFormHtml(int lcid)
        {
            string htmlTemplate = Resource.CustomPage.Replace("$phoneMssg$", phoneMssg).Replace("$errorMssg$", errorMssg); //todo we will implement this
            return htmlTemplate;
        }

        /// <summary>
        /// 验证页面 head 里的内容
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public string GetFormPreRenderHtml(int lcid)
        {
            return null;
        }

        /// <summary>
        /// 验证页面 title 里的内容
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public string GetPageTitle(int lcid)
        {
            return "二次验证";
        }
    }
}
