using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Newtonsoft.Json;

namespace hw7.Model {
    class JWXT {
        static string rno = ""; // 具体作用不明，猜测是验证码的生成码
        static HttpClient client = new HttpClient();

        // 访问教务系统主页获取Cookie，并从返回html中解析rno
        static public async Task<bool> getCookie() {
            // 获取成绩列表需要添加render头，为了防止产生异常去除该头
            if (client.DefaultRequestHeaders.Contains("render")) {
                client.DefaultRequestHeaders.Remove("render");
            }
            var html = await client.GetStringAsync("http://uems.sysu.edu.cn/jwxt/");
            var regex = new Regex("<input type=\"hidden\" id=\"rno\" name=\"rno\" value=([\\d.]+)></input>"); // 用正则匹配获取rno的值
            var match = regex.Match(html);
            if (match.Success) {
                rno = match.Groups[1].Value;
                return true;
            } else {
                return false;
            }
        }

        // 从教务系统主页获取验证码图片
        static public async Task<BitmapImage> getCheckCode() {
            // 获取成绩列表需要添加render头，为了防止产生异常去除该头
            if (client.DefaultRequestHeaders.Contains("render")) {
                client.DefaultRequestHeaders.Remove("render");
            }
            var response = await client.GetStreamAsync("http://uems.sysu.edu.cn/jwxt/jcaptcha");
            var image = new BitmapImage();

            // 将读取的stream转换为BitmapImage
            var memStream = new MemoryStream();
            await response.CopyToAsync(memStream);
            memStream.Position = 0;
            await image.SetSourceAsync(memStream.AsRandomAccessStream());

            return image;
        }

        // md5加密
        static private string md5(string message) {
            var strAlgName = HashAlgorithmNames.Md5;
            var objAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);
            var objHash = objAlgProv.CreateHash();
            var buffMsg = CryptographicBuffer.ConvertStringToBinary(message, BinaryStringEncoding.Utf8);
            objHash.Append(buffMsg);
            var buffHash = objHash.GetValueAndReset();
            var strHash = CryptographicBuffer.EncodeToHexString(buffHash);
            return strHash.ToUpper();
        }

        // 向登陆接口发送POST数据完成登陆操作
        static public async Task<string> login(string username, string password, string checkcode) {
            // 获取成绩列表需要添加render头，为了防止产生异常去除该头
            if (client.DefaultRequestHeaders.Contains("render")) {
                client.DefaultRequestHeaders.Remove("render");
            }
            // 发送的数据需要将密码用md5加密，并且携带rno
            var content = new StringContent(string.Format("j_username={0}&j_password={1}&rno={2}&jcaptcha_response={3}", username, md5(password), rno, checkcode), Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync("http://uems.sysu.edu.cn/jwxt/j_unieap_security_check.do", content);
            var html = await response.Content.ReadAsStringAsync();
            // 如果返回的html中不包含以下两个错误信息则视为登陆成功
            var regex = new Regex("(错误的验证码！)|(用户名不存在或密码错误！)");
            var match = regex.Match(html);
            if (match.Success) { // 如果包含错误信息则返回对应错误信息
                return match.Value;
            } else {
                return "";
            }
        }

        // 向获取成绩接口获取成绩列表
        static public async Task<ObservableCollection<CourseScore>> getScore(string year, string term) {
            // 需要发送的数据，具体意义不明，可以通过改变xnd和xq的值来选择不同的学期和学年度
            var data = "{header:{\"code\": -100, \"message\": {\"title\": \"\", \"detail\": \"\"}},body:{dataStores:{kccjStore:{rowSet:{\"primary\":[],\"filter\":[],\"delete\":[]},name:\"kccjStore\",pageNumber:1,pageSize:100,recordCount:0,rowSetName:\"pojo_com.neusoft.education.sysu.xscj.xscjcx.model.KccjModel\",order:\"t.xn, t.xq, t.kch, t.bzw\"}},parameters:{\"kccjStore-params\": [{\"name\": \"Filter_t.pylbm_0.9296534471892823\", \"type\": \"String\", \"value\": \"\'01\'\", \"condition\": \" = \", \"property\": \"t.pylbm\"}, {\"name\": \"Filter_t.xn_0.034865942747631606\", \"type\": \"String\", \"value\": \"'" + year + "'\", \"condition\": \" = \", \"property\": \"t.xn\"}, {\"name\": \"Filter_t.xq_0.6471972329784646\", \"type\": \"String\", \"value\": \"'" + term + "'\", \"condition\": \" = \", \"property\": \"t.xq\"}], \"args\": [\"student\"]}}}";
            var content = new StringContent(data, Encoding.UTF8, "multipart/form-data");
            // 需要添加一个render头，否则将会返回404
            if (!client.DefaultRequestHeaders.Contains("render")) {
                client.DefaultRequestHeaders.Add("render", "unieap");
            }
            var response = await client.PostAsync("http://uems.sysu.edu.cn/jwxt/xscjcxAction/xscjcxAction.action?method=getKccjList", content);
            var msg = await response.Content.ReadAsStringAsync();
            // 返回的是一个不标准的json格式，而且其中有时会夹杂错误的符号，因此不能用现有的JSON解析器解析，只能用正则解析
            var primaryRegex = new Regex("primary:\\[([^\\]]*)\\]"); // 用于从返回信息中解析出成绩列表
            var courseRegex = new Regex("\\{([^\\}]*)\\}"); // 用于从成绩列表解析成单个课程
            var keyValueRegex = new Regex("\"([^\"]*)\":\"([^\"]*)\""); // 用于解析单个课程包含的键值对
            var primary = primaryRegex.Match(msg).Groups[1].Value;
            var matches = courseRegex.Matches(primary);
            var collection = new ObservableCollection<CourseScore>();
            for (int i = 0; i < matches.Count; ++i) {
                var score = matches[i].Groups[1].Value;
                var keyValues = keyValueRegex.Matches(score);
                var courseDic = new Dictionary<string, string>(); // 用于暂时保存单个课程的键值对
                for (int j = 0; j < keyValues.Count; ++j) {
                    var key = keyValues[j].Groups[1].Value;
                    var value = keyValues[j].Groups[2].Value;
                    // 由于返回的字符串中有不需要的反斜杠，将其去除
                    var index = value.IndexOf('\\');
                    if (index != -1) {
                        value = value.Remove(index, 1);
                    }
                    courseDic.Add(key, value);
                }
                var courseScore = new CourseScore();
                // 从解析好的键值对中获取需要的信息。由于不是每个课程都包含需要的键值对因此需要检测是否存在
                courseScore.Rank = courseDic.ContainsKey("jxbpm") ? courseDic["jxbpm"] : "";
                courseScore.EnglishCourseName = courseDic.ContainsKey("kcywmc") ? courseDic["kcywmc"] : "";
                courseScore.TeacherName = courseDic.ContainsKey("jsxm") ? courseDic["jsxm"] : "";
                courseScore.CourseName = courseDic.ContainsKey("kcmc") ? courseDic["kcmc"] : "";
                courseScore.Credit = courseDic.ContainsKey("xf") ? courseDic["xf"] : "";
                courseScore.Score = courseDic.ContainsKey("zzcj") ? courseDic["zzcj"] : "";
                courseScore.GPA = courseDic.ContainsKey("jd") ? courseDic["jd"] : "";
                collection.Add(courseScore);
            }
            return collection;
        }
    }
}
