using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.IdentityServer.Web.Authentication.External;
using Claim = System.Security.Claims.Claim;

namespace RekTec.Crm.AdfsCaptcha.MFAadapter
{
    /// <summary>
    /// 身份验证适配器
    /// </summary>
    public class CustomAuthenticationAdapter : IAuthenticationAdapter
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        private string phone;
        /// <summary>
        /// 手机号码消息
        /// </summary>
        private string phoneMssg;
        /// <summary>
        /// 接口信息
        /// </summary>
        private API_UrlInfo apiinfo = new API_UrlInfo();

        public IAuthenticationAdapterMetadata Metadata
        {
            get
            {
                return new CustomMetadata();
            }
        }

        public IAdapterPresentation BeginAuthentication(Claim identityClaim, HttpListenerRequest request, IAuthenticationContext authContext)
        {
            string acc = identityClaim.Value;
            // 调用发送手机验证码接口，并返回手机号码
            phone = CommonHelper.Request("get", apiinfo.API_SendVerifyCode + "?account=" + acc);
            phoneMssg = string.IsNullOrWhiteSpace(phone) ? "当前账户未维护手机号，请联系系统管理员！" : $"验证码将发送至-{phone}，如未收到，请联系系统管理员！";
            return new CustomPresentationForm(phoneMssg);
        }

        public bool IsAvailableForUser(Claim identityClaim, IAuthenticationContext authContext)
        {
            return true; //its all available for now
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="configData"></param>
        public void OnAuthenticationPipelineLoad(IAuthenticationMethodConfigData configData)
        {
            if (configData != null)
            {
                if (configData.Data != null)
                {
                    using (StreamReader reader = new StreamReader(configData.Data, Encoding.UTF8))
                    {
                        try
                        {
                            var config = reader.ReadToEnd();
                            apiinfo = CommonHelper.Deseralize<API_UrlInfo>(config);
                        }
                        catch
                        {
                            throw new ArgumentException();
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void OnAuthenticationPipelineUnload()
        {
        }

        public IAdapterPresentation OnError(HttpListenerRequest request, ExternalAuthenticationException ex)
        {
            //return new instance of IAdapterPresentationForm derived class
            return new CustomPresentationForm(phoneMssg, ex.Message);
        }

        public IAdapterPresentation TryEndAuthentication(IAuthenticationContext authContext, IProofData proofData, HttpListenerRequest request, out Claim[] outgoingClaims)
        {
            outgoingClaims = new Claim[0];
            string errorMssg = ValidateProofData(proofData, authContext);
            if (string.IsNullOrWhiteSpace(errorMssg))
            {
                outgoingClaims = new[]
                {
                    new Claim( "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod",
                    "http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/hardwaretoken" ) };

                return null;
            }
            else
            {
                return new CustomPresentationForm(phoneMssg, errorMssg);
            }
        }

        private string ValidateProofData(IProofData proofData, IAuthenticationContext authContext)
        {
            if (proofData == null || proofData.Properties == null || !proofData.Properties.ContainsKey("captcha"))
            {
                throw new ExternalAuthenticationException("请输入短信验证码！", authContext);
            }
            var code = (string)proofData.Properties["captcha"];
            return CommonHelper.Request("get", apiinfo.API_CheckVerifyCode + "?phone=" + phone + "&code=" + code);
        }
    }
}
