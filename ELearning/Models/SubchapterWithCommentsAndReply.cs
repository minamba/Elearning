using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class SubchapterWithCommentsAndReply
    {
        public List<Comment_> ListComment { get; set; }
        public Subchapter_ CurrentSubchapter { get; set; }
        public List<Reply_> ListReply { get; set; }
        public List<User_> ListUser { get; set; }
        public List<string> TagComment{ get; set; }
        public List<string> TagComment2 { get; set; }
        public List<string> TagReply { get; set; }
        public List<string> TagReply2 { get; set; }
        public List<string> ListDate { get; set; }
        public List<Video_> ListVideo { get; set; }
        public User_ CurrrentUsr { get; set; }
        public int SubchapterId { get; set; }
        public List<string> tag { get; set; }
        public List<Chapter_> ListChapter { get; set; }
        public List<Subchapter_> ListSubchapter { get; set; }
        public Video_ video { get; set; }
        public Dictionary<string,TimeSpan>TimeOfVideo { get; set; }
        public List<Seen_> ListSeen { get; set; }
        public int UserPurcentage { get; set; }
        public int TotalVideoSection { get; set; }
        public int VideoSeenUser { get; set; }
        public string UID { get; set; }

        elearningEntities db = new elearningEntities();

        public string GetUserName(int? userID)
        {
            var us = db.User_.FirstOrDefault(u => u.id == userID);

            if (us == null)
            {
                return null;
            }
            else
                return us.first_name;

        }

        public string DisplayVideo(Subchapter_ CurrentSubchapter)
        {
            string displayVideo;

            if (CurrentSubchapter.url_video != null || CurrentSubchapter.url_video != "")
                displayVideo = "style = display:block;";
            else
                displayVideo = "style=display:none;";

            return displayVideo;
        }

        public int CountSubChapterOfChapter(int chapter_id)
        {
            var countSubChapter = (from s in db.Subchapter_
                                   where s.chapter_id == chapter_id
                                   select s).Count();

            return countSubChapter;
        }


        public int CountVideoViewByUser(int chapter_id, string uid)
        {
            var countUserVideoView = (from cuv in db.Seen_
                                      where cuv.user_asp_net_id == uid
                                      && cuv.chapter_id == chapter_id
                                      && cuv.seen == true
                                      select cuv).Count();

            return countUserVideoView;
        }


        public string TotalTimeChapter(int chapter_id)
        {
            var totalTime = (from tt in db.Subchapter_
                             where tt.chapter_id == chapter_id
                             select tt.time_video).Sum();

            //double secondes = Convert.ToDouble(totalTime);
            //int minutes = (int)totalTime/60;
            double secondes = Convert.ToDouble(totalTime * 60);
            if(totalTime > 60)
            {         
                    TimeSpan t = TimeSpan.FromSeconds(secondes);
                    string answer = string.Format("{0:D2}h:{1:D2}min",
                    t.Hours,
                    t.Minutes);

                    return answer;              
            }
            else
            {
                return totalTime.ToString() +" "+ "min";
            }
           

            return "";
        }


    }



}