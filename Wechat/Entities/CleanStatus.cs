namespace Wechat.Entities
{
    public enum CleanStatus
    {
        /// <summary>
        /// 等待扫码
        /// </summary>
        WaitLogin=0,

        /// <summary>
        /// 已扫码
        /// </summary>
        Scaned=1,

        /// <summary>
        /// 已登录
        /// </summary>
        Logined=2,

        /// <summary>
        /// 已初始化(Cookie, 个人信息，通讯录等)
        /// </summary>
        Inited=3,

        /// <summary>
        /// 已群发消息
        /// </summary>
        Backcasted=4,

        /// <summary>
        /// 修改备注成功
        /// </summary>
        Marked=5,

        /// <summary>
        /// 取消
        /// </summary>
        Canceled=6,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed=7,

        /// <summary>
        /// 出错状态
        /// </summary>
        Error=8,
    }
}