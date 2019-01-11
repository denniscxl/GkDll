using System;

namespace GKToy
{
    /// <summary>
    /// 客户端导出结点属性Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExportClientAttribute : Attribute { }

    /// <summary>
    /// 服务器导出结点属性Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExportServerAttribute : Attribute { }
}
