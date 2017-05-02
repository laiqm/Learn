﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.App.Json
{
    public class User
    {
        public long Uin { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string HeadImgUrl { get; set; }
        public string RemarkName { get; set; }
        public string PYInitial { get; set; }
        public string PYQuanPin { get; set; }
        public string RemarkPYInitial { get; set; }
        public string RemarkPYQuanPin { get; set; }
        public int HideInputBarFlag { get; set; }
        public int StarFriend { get; set; }
        public int Sex { get; set; }
        public string Signature { get; set; }
        public int AppAccountFlag { get; set; }
        public int VerifyFlag { get; set; }
        public int ContactFlag { get; set; }
        public int SnsFlag { get; set; }

        //me
        public int WebWxPluginSwitch { get; set; }
        public int HeadImgFlag { get; set; }

        //friend
        public int MemberCount { get; set; }
        public User[] MemberList { get; set; }
        public int OwnerUin { get; set; }
        public int Statues { get; set; }
        public long AttrStatus { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Alias { get; set; }
        public int UniFriend { get; set; }
        public int ChatRoomId { get; set; }

        //member
        //public int AttrStatus { get; set; }
        //public string DisplayName { get; set; }
        public int MemberStatus { get; set; }

        public string DisplayName => RemarkName == "" ? (NickName == "" ? UserName : NickName) : RemarkName;

        public bool IsGroup => UserName.StartsWith("@@");

        public bool IsPublic => VerifyFlag != 0;
    }
}
