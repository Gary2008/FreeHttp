﻿using Fiddler;
using FreeHttp.FiddlerHelper;
using FreeHttp.HttpHelper;
using FreeHttp.AutoTest;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeHttp
{
    public class FiddlerSessionTamper
    {
        /// <summary>
        /// Modific the http request in oSession with your rule
        /// </summary>
        /// <param name="oSession">oSession</param>
        /// <param name="nowFiddlerRequsetChange">FiddlerRequsetChange</param>
        public static void ModificSessionRequest(Session oSession, FiddlerRequsetChange nowFiddlerRequsetChange, Action<string> ShowError)
        {
            if (nowFiddlerRequsetChange.IsRawReplace)
            {
                ReplaceSessionRequest(oSession, nowFiddlerRequsetChange, ShowError);
            }
            else
            {
                if (nowFiddlerRequsetChange.UriModific != null && nowFiddlerRequsetChange.UriModific.ModificMode != ContentModificMode.NoChange)
                {
                    oSession.fullUrl = nowFiddlerRequsetChange.UriModific.GetFinalContent(oSession.fullUrl);
                }
                if (nowFiddlerRequsetChange.HeadDelList != null && nowFiddlerRequsetChange.HeadDelList.Count > 0)
                {
                    foreach (var tempDelHead in nowFiddlerRequsetChange.HeadDelList)
                    {
                        oSession.RequestHeaders.Remove(tempDelHead);
                    }
                }
                if (nowFiddlerRequsetChange.HeadAddList != null && nowFiddlerRequsetChange.HeadAddList.Count > 0)
                {
                    foreach (var tempAddHead in nowFiddlerRequsetChange.HeadAddList)
                    {
                        if (tempAddHead.Contains(": "))
                        {
                            oSession.RequestHeaders.Add(tempAddHead.Remove(tempAddHead.IndexOf(": ")), tempAddHead.Substring(tempAddHead.IndexOf(": ") + 2));
                        }
                        else
                        {
                            ShowError(string.Format("error to deal add head string with [{0}]", tempAddHead));
                        }
                    }
                }
                if (nowFiddlerRequsetChange.BodyModific != null && nowFiddlerRequsetChange.BodyModific.ModificMode != ContentModificMode.NoChange)
                {
                    string sourceRequestBody = null;
                    try  //GetRequestBodyAsString may throw exception
                    {
                        //oSession.utilDecodeRequest();
                        sourceRequestBody = oSession.GetRequestBodyAsString();
                    }
                    catch (Exception ex)
                    {
                        ShowError(string.Format("error in GetRequestBodyAsString [{0}]", ex.Message));
                        oSession.utilDecodeRequest();
                        sourceRequestBody = oSession.GetRequestBodyEncoding().GetString(oSession.requestBodyBytes);
                    }
                    finally
                    {
                        //oSession.requestBodyBytes = oSession.GetRequestBodyEncoding().GetBytes(nowFiddlerRequsetChange.BodyModific.GetFinalContent(sourceRequestBody)); // 直接修改内部成员
                        //oSession.ResponseBody = oSession.GetRequestBodyEncoding().GetBytes(nowFiddlerRequsetChange.BodyModific.GetFinalContent(sourceRequestBody)); //修改内部成员 更新Content-Length ，适用于hex数据
                        oSession.utilSetRequestBody(nowFiddlerRequsetChange.BodyModific.GetFinalContent(sourceRequestBody));  //utilSetRequestBody 虽然会自动更新Content-Length 但是会强制使用utf8 ，适用于字符串
                    }
                }
            }
        }

        /// <summary>
        /// Replace the http request in oSession with your rule (it may call by ModificSessionRequest)
        /// </summary>
        /// <param name="oSession">oSession</param>
        /// <param name="nowFiddlerRequsetChange">FiddlerRequsetChange</param>
        public static void ReplaceSessionRequest(Session oSession, FiddlerRequsetChange nowFiddlerRequsetChange, Action<string> ShowError)
        {
            oSession.oRequest.headers = new HTTPRequestHeaders();
            oSession.RequestMethod = nowFiddlerRequsetChange.HttpRawRequest.RequestMethod;
            oSession.fullUrl = nowFiddlerRequsetChange.HttpRawRequest.RequestUri;
            ((Fiddler.HTTPHeaders)(oSession.RequestHeaders)).HTTPVersion = nowFiddlerRequsetChange.HttpRawRequest.RequestVersions;
            if (nowFiddlerRequsetChange.HttpRawRequest.RequestHeads != null)
            {
                
                foreach (var tempHead in nowFiddlerRequsetChange.HttpRawRequest.RequestHeads)
                {
                    if (tempHead.Key == "Host")
                    {
                        oSession.oRequest.headers.Remove("Host");
                    }
                    oSession.oRequest.headers.Add(tempHead.Key, tempHead.Value);
                }
            }
            oSession.requestBodyBytes = nowFiddlerRequsetChange.HttpRawRequest.RequestEntity;
        }

        /// <summary>
        /// Modific the http response in oSession with your rule (if IsRawReplace and  IsIsDirectRespons it will not call ReplaceSessionResponse)
        /// </summary>
        /// <param name="oSession">oSession</param>
        /// <param name="nowFiddlerResponseChange">FiddlerResponseChange</param>
        public static void ModificSessionResponse(Session oSession, FiddlerResponseChange nowFiddlerResponseChange, Action<string> ShowError, Action<string> ShowMes)
        {
            if (nowFiddlerResponseChange.IsRawReplace)
            {
                //if IsIsDirectRespons do nothing
                if (!nowFiddlerResponseChange.IsIsDirectRespons)
                {
                    ReplaceSessionResponse(oSession, nowFiddlerResponseChange, ShowError, ShowMes);
                }
            }
            else
            {
                if (nowFiddlerResponseChange.HeadDelList != null && nowFiddlerResponseChange.HeadDelList.Count > 0)
                {
                    foreach (var tempDelHead in nowFiddlerResponseChange.HeadDelList)
                    {
                        oSession.ResponseHeaders.Remove(tempDelHead);
                    }
                }
                if (nowFiddlerResponseChange.HeadAddList != null && nowFiddlerResponseChange.HeadAddList.Count > 0)
                {
                    foreach (var tempAddHead in nowFiddlerResponseChange.HeadAddList)
                    {
                        if (tempAddHead.Contains(": "))
                        {
                            oSession.ResponseHeaders.Add(tempAddHead.Remove(tempAddHead.IndexOf(": ")), tempAddHead.Substring(tempAddHead.IndexOf(": ") + 2));
                        }
                        else
                        {
                            ShowError(string.Format("error to deal add head string with [{0}]", tempAddHead));
                        }
                    }
                }
                if (nowFiddlerResponseChange.BodyModific != null && nowFiddlerResponseChange.BodyModific.ModificMode != ContentModificMode.NoChange)
                {
                    //you should not change the media data as string
                    string sourceResponseBody = null;
                    try
                    {
                        sourceResponseBody = oSession.GetResponseBodyAsString();
                    }
                    catch (Exception ex)
                    {
                        ShowError(string.Format("error in GetResponseBodyAsString [{0}]", ex.Message));
                        oSession.utilDecodeResponse();
                        sourceResponseBody = oSession.GetResponseBodyEncoding().GetString(oSession.ResponseBody);
                    }
                    finally
                    {
                        oSession.utilSetResponseBody(nowFiddlerResponseChange.BodyModific.GetFinalContent(sourceResponseBody));
                    }

                    //you can use below code to modific the body
                    //oSession.utilDecodeResponse();
                    //oSession.utilReplaceInResponse("","");
                    //oSession.utilDeflateResponse();
                }
            }
        }

        /// <summary>
        /// Replace the http response in oSession with your rule
        /// </summary>
        /// <param name="oSession">oSession</param>
        /// <param name="nowFiddlerResponseChange">FiddlerResponseChange</param>
        public static void ReplaceSessionResponse(Session oSession, FiddlerResponseChange nowFiddlerResponseChange, Action<string> ShowError, Action<string> ShowMes)
        {
            byte[] tempResponseBytes;
            string errMes;
            NameValueCollection nameValueCollection;
            try
            {
                tempResponseBytes = nowFiddlerResponseChange.HttpRawResponse.GetRawHttpResponse(out errMes, out nameValueCollection);
            }
            catch(Exception ex)
            {
                ShowError(string.Format("Fail to ReplaceSessionResponse :{0}", ex.Message));
                return;
            }
            if (errMes!=null)
            {
                ShowError(errMes);
            }
            if (nameValueCollection != null && nameValueCollection.Count>0)
            {
                ShowMes(string.Format("[ParameterizationContent]:{0}", nameValueCollection.MyToFormatString()));
            }
            using (MemoryStream ms = new MemoryStream(tempResponseBytes))
            {
                if (!oSession.LoadResponseFromStream(ms, null))
                {
                    ShowError("error to oSession.LoadResponseFromStream");
                    ShowError("try to modific the response");
                    //modific the response
                    oSession.oResponse.headers = new HTTPResponseHeaders();
                    oSession.oResponse.headers.HTTPResponseCode = nowFiddlerResponseChange.HttpRawResponse.ResponseCode;
                    oSession.ResponseHeaders.StatusDescription = nowFiddlerResponseChange.HttpRawResponse.ResponseStatusDescription;
                    ((Fiddler.HTTPHeaders)(oSession.ResponseHeaders)).HTTPVersion = nowFiddlerResponseChange.HttpRawResponse.ResponseVersion;
                    if (nowFiddlerResponseChange.HttpRawResponse.ResponseHeads != null && nowFiddlerResponseChange.HttpRawResponse.ResponseHeads.Count > 0)
                    {
                        foreach (var tempHead in nowFiddlerResponseChange.HttpRawResponse.ResponseHeads)
                        {
                            oSession.oResponse.headers.Add(tempHead.Key, tempHead.Value);
                        }
                    }
                    oSession.responseBodyBytes = nowFiddlerResponseChange.HttpRawResponse.ResponseEntity;
                }
            }
        }
        
        public static string GetSessionRawData(Session oSession,bool isHaveResponse)
        {
            if(oSession==null)
            {
                return null;
            }
            StringBuilder sbRawData = new StringBuilder();
            MemoryStream ms = new MemoryStream();
            //tempSession.WriteToStream(SmartAssembly, false);
            oSession.WriteRequestToStream(true, true, ms);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms, Encoding.UTF8);
            sbRawData.Append(sr.ReadToEnd());
            sr.Close();
            ms.Close();

            if (oSession.requestBodyBytes != null && oSession.requestBodyBytes.Length > 0)
            {
                sbRawData.AppendLine(oSession.GetRequestBodyAsString());
                sbRawData.Append("\r\n");
            }
            if (isHaveResponse && oSession.bHasResponse)
            {
                sbRawData.AppendLine(oSession.ResponseHeaders.ToString());
                if (oSession.responseBodyBytes != null && oSession.responseBodyBytes.Length > 0)
                {
                    sbRawData.AppendLine(oSession.GetResponseBodyAsString());
                }
            }
            return sbRawData.ToString();
        }
    }
}
