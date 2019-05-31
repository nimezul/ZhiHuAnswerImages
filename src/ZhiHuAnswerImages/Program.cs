using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace ZhiHuAnswerImages
{
    public class Program
    {
        private static int totalAnswers = 0;
        private static int currentPage = 0;

        //title = '拥有一副令人羡慕的好身材是怎样的体验？'
        //question_id = 297715922

        //# title = '身材好是一种怎样的体验？'
        //# question_id = 26037846

        //# title = '女孩子胸大是什么体验？'
        //# question_id = 291678281

        //# title = '女生什么样的腿是美腿？'
        //# question_id = 310786985

        //# title = '你的择偶标准是怎样的？'
        //# question_id = 275359100

        //# title = '什么样才叫好看的腿？'
        //# question_id = 63727821

        //# title = '身材对女生很重要吗？'
        //# question_id = 307403214

        //# title = '女生腿长是什么样的体验？'
        //# question_id = 273711203

        //# title = '女生腕线过裆是怎样一种体验？'
        //# question_id = 315236887

        //# title = '有着一双大长腿是什么感觉？'
        //# question_id = 292901966

        //# title = '拥有一双大长腿是怎样的体验？'
        //# question_id = 285321190

        //# title = '大胸女生如何穿衣搭配？'
        //# question_id = 26297181

        //# title = '胸大到底怎么穿衣服好看?'
        //# question_id = 293482116


        public static void Main(string[] args)
        {
            Console.WriteLine("请输入知乎问题ID......");
            string questionId = Console.ReadLine();
            string url = $"https://www.zhihu.com/api/v4/questions/{questionId}/answers?include=data[*].is_normal,admin_closed_comment,reward_info,is_collapsed,annotation_action,annotation_detail,collapse_reason,is_sticky,collapsed_by,suggest_edit,comment_count,can_comment,content,editable_content,voteup_count,reshipment_settings,comment_permission,created_time,updated_time,review_info,relevant_info,question,excerpt,relationship.is_authorized,is_author,voting,is_thanked,is_nothelp,is_labeled,is_recognized,paid_info;data[*].mark_infos[*].url;data[*].author.follower_count,badge[*].topics&limit=1&offset=0&platform=desktop&sort_by=default";
            GetAnswers(url);
        }

        public static void GetAnswers(string url)
        {
            //输出
            currentPage += 1;
            if (totalAnswers > 0)
            {
                Console.WriteLine("正在获取第" + currentPage + "个回答......");
            }

            //请求
            string jsonStr = string.Empty;
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                jsonStr = wc.DownloadString(new Uri(url));
            }
            JObject answer = JObject.Parse(jsonStr);

            //输出总回答数
            if (totalAnswers == 0)
            {
                var total = answer["paging"]["totals"].ToString();
                totalAnswers = int.Parse(total);
                Console.WriteLine("共" + total + "个回答\n");
                Console.WriteLine("正在获取第" + currentPage + "个回答......");
            }

            //下载图片
            string content = answer["data"][0]["content"].ToString();
            string userName = answer["data"][0]["author"]["name"].ToString();
            DownloadImage(content, userName);

            //获取下一个回答
            string isEnd = answer["paging"]["is_end"].ToString().ToLower();
            if (isEnd.Equals("false"))
            {
                GetAnswers(answer["paging"]["next"].ToString());
            }
        }

        public static void DownloadImage(string content, string floderName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var imageNodes = doc.DocumentNode.SelectNodes("//img");
            if (imageNodes == null)
            {
                return;
            }
            Console.WriteLine("\t共" + imageNodes.Count + "张图片");
            foreach (var node in imageNodes)
            {
                var original = (node.Attributes["data-original"] ?? node.Attributes["src"]).Value;
                if (!original.StartsWith("http"))
                {
                    continue;
                }
                var path = Environment.CurrentDirectory + @"\Pictures\" + floderName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string localFilename = Path.Combine(path, Path.GetFileName(original));
                Console.WriteLine("\t正在下载第" + (imageNodes.IndexOf(node) + 1) + "张图片......");
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(original, localFilename);
                }
            }
        }
    }
}
