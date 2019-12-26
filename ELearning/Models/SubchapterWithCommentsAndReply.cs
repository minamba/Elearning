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
    }



}